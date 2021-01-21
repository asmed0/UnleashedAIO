using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using golang;
using UnleashedAIO.JSON;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.Text.RegularExpressions;
using RestSharp;
using System.Web;

namespace UnleashedAIO.Modules
{
    class FootlockerNA
    {
        public int retryOnFailAttempts = 10000;

        //program vars
        private string taskNumber;
        private int delay;
        private string proxy;

        //will be used later on
        private string productName;
        private string sizeCode;
        private string releaseTimer;
        private string productImage;
        private double price;

        //profile vars
        private string mode;
        private string product;
        private string size;
        private string firstname;
        private string lastname;
        private string email;
        private string phone;
        private string addy;
        private string addy2;
        private string city;
        private string state;
        private string zip;
        private string region;
        private string country;
        private string cardNum;
        private string expMon;
        private string expYr;
        private string cvv;

        //session vars
        private string csrf;
        private string sessionId;
        private string cartId;
        private string cartCode;
        private string shippingId;
        private FootlockerJSON.EncryptedCardData.Root encryptedDataObj;
        private string paymentData;
        private string md;
        private string paReq;
        private string paRes;
        private string termURL;

        private tlsSolution tlsClient = new tlsSolution();

        private WebhookSend webhookSend = new WebhookSend();
        private bool is3DS = false;
        private string _authToken;
        private string _transToken;
        public bool StartTaskAsync(Tasks currentTask, string taskNumber, int delay, int taskIndex)
        {

            //binding program vars
            this.taskNumber = taskNumber;
            this.delay = delay;

            //binding profile vars
            mode = currentTask.Mode;
            product = currentTask.SKU;
            size = currentTask.Size;
            firstname = currentTask.FirstName;
            lastname = currentTask.LastName;
            email = currentTask.email;
            phone = currentTask.PhoneNumber;
            addy = currentTask.Adress;
            addy2 = currentTask.Adress2;
            city = currentTask.City;
            state = currentTask.State;
            zip = currentTask.ZipCode;
            region = currentTask.Country;
            cardNum = currentTask.CardNumber;
            expMon = currentTask.ExpiryMonth;
            expYr = currentTask.ExpiryYear;
            cvv = currentTask.CVV;
            proxy = ProxyMaster.getProxy(taskIndex);
            termURL = $"https://www.footlocker.{region}/adyen/checkout";

            if (size.Contains(".") && size.Length < 3 || size.Length > 2)
            {
                size = $"0{size}";
            }

            if (region == "US")
            {
                region = "com";
            }

            if (proxy == "NAP")
            {

                Console.WriteLine($"{timestamp()}{taskNumber} No proxies available, ending task.. ");
                Thread.Sleep(Timeout.Infinite);
            }
            Console.WriteLine($"{timestamp()}{taskNumber}Task starting..");
            //Program.ChangeColor(ConsoleColor.DarkGray);
            //Console.WriteLine($"{timestamp()}{_taskNumber} Task Started! Marking Time");
            //Program.WriteLog("log",$"{timestamp()}{_taskNumber} Task Started! Marking Time");
            switch (region)
            {
                case "CA":
                    country = "Canada";
                    break;
                case "US":
                    country = "United States";
                    break;
                default:
                    Console.WriteLine($"{timestamp()}{taskNumber}REGION NOT RECOGNIZED - UPDATE TASK(S)");
                    Thread.Sleep(Timeout.Infinite);
                    return false;
            }

            //example retry logic below
            //for (int attempts = 0; attempts < 5; attempts++)
            //// if you really want to keep going until it works, use   for(;;)
            //{
            //    try
            //    {
            //        DoWork();
            //        break;
            //    }
            //    catch { }
            //    Thread.Sleep(50); // Possibly a good idea to pause here, explanation below
            //}

            try
            {

                //fetching product details
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (GetProductInfos())
                    {
                        Program.ChangeColor(ConsoleColor.Yellow);
                        Console.WriteLine($"{timestamp()}{taskNumber}Fetched product info, now generating session..");

                        Thread.Sleep(delay);
                        break;
                    }
                    if (attempts > 3)
                    {
                        StartTaskAsync(currentTask, taskNumber, delay, taskIndex);
                        return false; //after 3 retries we assume that proxy is banned and switch to a new fresh task 
                    }
                    Thread.Sleep(delay);
                }

                //generating our session
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (NewSession())
                    {
                        Program.ChangeColor(ConsoleColor.Yellow);
                        Console.WriteLine($"{timestamp()}{taskNumber}Generated session, now carting..");
                        //Console.WriteLine($"csrf: {_csrf} and sessionid: {_sessionId}");

                        Thread.Sleep(delay);
                        break;
                    }
                    if (attempts > 3)
                    {
                        StartTaskAsync(currentTask, taskNumber, delay, taskIndex);
                        return false; //after 3 retries we assume that proxy is banned and switch to a new fresh task 
                    }

                    Thread.Sleep(delay);
                }

                //carting our product
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (AddToCart())
                    {
                        Program.ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"{timestamp()}{taskNumber}Carted, now going to checkout..");
                        //Console.WriteLine($"cartId: {_cartId} and sessionid: {_sessionId}");

                        Thread.Sleep(delay);
                        break;
                    }

                    Thread.Sleep(delay);
                }

                //initating checkout
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (InitiateCheckout())
                    {
                        Program.ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"{timestamp()}{taskNumber}Initiated checkout session, now adding shipping..");
                        //Console.WriteLine($"cartId: {_cartId} and sessionid: {_sessionId}");

                        Thread.Sleep(delay);
                        break;
                    }
                    //Thread.Sleep(_delay);
                }

                //adding shipping
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (AddShipping())
                    {
                        Program.ChangeColor(ConsoleColor.Cyan);
                        Console.WriteLine($"{timestamp()}{taskNumber}Added shipping, now adding billing..");

                        Thread.Sleep(delay);
                        break;
                    }
                    Thread.Sleep(delay);
                }
                //adding billing
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (AddBilling())
                    {
                        Program.ChangeColor(ConsoleColor.Cyan);
                        Console.WriteLine($"{timestamp()}{taskNumber}Added billing, now preparing checkout data..");

                        Thread.Sleep(delay);
                        break;
                    }
                    Thread.Sleep(delay);
                }
                //encrypt card data
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (EncryptCardData())
                    {
                        Program.ChangeColor(ConsoleColor.Cyan);
                        Console.WriteLine($"{timestamp()}{taskNumber}Prepared checkout data, now submitting..");

                        Thread.Sleep(delay);
                        break;
                    }
                    Thread.Sleep(delay);
                }
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (AdyenSubmit())
                    {
                        Program.ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"{timestamp()}{taskNumber}Submitted checkout data, now handling 3DS Security..");
                        Thread.Sleep(delay);
                        break;
                    }
                    Thread.Sleep(delay);
                }
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (PayerAuthenticate())
                    {
                        Program.ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"{timestamp()}{taskNumber}Fetched 3DS challenge, now solving it..");

                        if (is3DS)
                        {
                            for (int threeDSattempts = 0; threeDSattempts < retryOnFailAttempts; threeDSattempts++)
                            {
                                if (Poll3DS())
                                {
                                    ConfirmTransaction();
                                    Thread.Sleep(delay);
                                    break;
                                }
                                Thread.Sleep(delay);
                            }
                        }

                        Thread.Sleep(delay);
                        break;
                    }
                    Thread.Sleep(delay);
                }

                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (PayerSubmit())
                    {
                        Program.ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"{timestamp()}{taskNumber}Solved 3DS challenge, now finalizing checkout..");

                        Thread.Sleep(delay);
                        break;
                    }
                    Thread.Sleep(delay);
                }
                for (int attempts = 0; attempts < retryOnFailAttempts; attempts++)
                {
                    if (CompletePayment())
                    {
                        Program.ChangeColor(ConsoleColor.Green);
                        Console.WriteLine($"{timestamp()}{taskNumber}Order completed, check webhook..");

                        return true;
                    }
                    Thread.Sleep(delay);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                StartTaskAsync(currentTask, taskNumber, delay, taskIndex);
                return false;
            }
            return true;
        }
        private bool CompletePayment()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("origin", $"www.footlocker.{region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"www.footlocker.{region}/en/checkout")
            .AddHeader("Cookie", $"cart-guid={cartId}; {sessionId}");

            FootlockerJSON.CompletePayment.Root bodyObj = new FootlockerJSON.CompletePayment.Root();
            bodyObj.Md = md;
            bodyObj.PaRes = paRes;
            bodyObj.CartId = cartId;
            bodyObj.PaymentData = paymentData;
            bodyObj.PaymentMethod = "CREDITCARD";
            string body = JsonConvert.SerializeObject(bodyObj);

            string completePayment = tlsClient.postRequest($"https://www.footlocker.{region}/api/users/orders/completePayment", chain.headers, body, proxy);
            goTLSResponse completePaymentResponse = null;
            FootlockerJSON.OrderComplete.Root completePaymentObj = null;
            try
            {
                completePaymentResponse = JsonConvert.DeserializeObject<goTLSResponse>(completePayment);
                completePaymentObj = JsonConvert.DeserializeObject<FootlockerJSON.OrderComplete.Root>(completePaymentResponse.Body);
            }
            catch (Exception e)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed submitting order, retrying..");
                return false;
            }
            if (completePaymentObj != null)
            {
                try
                {
                    webhookSend.Send(new Discord.Webhook.DiscordWebhookClient($"https://discordapp.com{Program.configObject.webhook}"), $"Footlocker {region.ToUpper()}", completePaymentObj.Order.Entries[0].Product.BaseOptions[0].Selected.Name, size,"",default, completePaymentObj.Order.Code,product);
                    webhookSend.Send(new Discord.Webhook.DiscordWebhookClient($"https://discordapp.com{WebhookSend.publicWebhook}"), $"Footlocker {region.ToUpper()}", completePaymentObj.Order.Entries[0].Product.BaseOptions[0].Selected.Name, size, "", default, default, product);
                }
                catch (Exception)
                {
                }
                return true;
            }
            return false;
        }
        private bool PayerSubmit()
        {
            var client = new RestClient("https://www.footlocker.se/adyen/checkout");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("authority", "www.footlocker.se");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("origin", "https://idcheck.acs.touchtechpayments.com");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.AddHeader("sec-fetch-site", "cross-site");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-dest", "iframe");
            request.AddHeader("referer", "https://idcheck.acs.touchtechpayments.com/");
            request.AddHeader("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.AddHeader("cookie", $"{sessionId} cart-guid={cartId}");
            request.AddParameter("PaRes", paRes);
            request.AddParameter("MD", md);
            IRestResponse response = client.Execute(request);

            string pattern = @"(?<=type: ')(.*)(?=')";
            RegexOptions options = RegexOptions.Multiline;
            if (Regex.Matches(response.Content, pattern, options)[0].Value == "checkoutChallengeComplete" && response.IsSuccessful)
            {
                return true;
            }
            return false;
        }
        private bool PayerAuthenticate()
        {
            var client = new RestClient("https://idcheck.acs.touchtechpayments.com/v1/payerAuthentication");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("authority", "idcheck.acs.touchtechpayments.com");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("origin", $"https://www.footlocker.{region}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
            request.AddHeader("accept", "application/json");
            request.AddHeader("sec-fetch-site", "cross-site");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-user", "?1");
            request.AddHeader("sec-fetch-dest", "iframe");
            request.AddHeader("referer", $"https://www.footlocker.{region}");
            request.AddHeader("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.AddParameter("MD", md);
            request.AddParameter("PaReq", paReq);
            request.AddParameter("TermUrl", termURL);
            IRestResponse response = client.Execute(request);

            try
            {
                RegexOptions options = RegexOptions.Multiline;
                if (is3DS)
                {
                    string pattern = @"(?<=pares = "")(.*)(?=;)";
                    paRes = Regex.Matches(response.Content, pattern, options)[0].Value;
                    string tokenPattern = @"(?<=token: "")(.*)(?="")";
                    _transToken = Regex.Matches(response.Content, tokenPattern, options)[0].Value;
                    return true;
                }
                else
                {
                    string pattern = @"(?<=pares = "")(.*)(?=;)";
                    paRes = Regex.Matches(response.Content, pattern, options)[0].Value;
                    return true;
                }
            }
            catch (Exception e)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Encountered error while fetching 3DS data , retrying..");
                return false;
            }
        }

        private bool AdyenSubmit()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("origin", $"www.footlocker.{region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"www.footlocker.{region}/en/checkout")
            .AddHeader("Cookie", $"cart-guid={cartId}; {sessionId}");

            FootlockerJSON.AdyenSubmit.Root adyenObj = new FootlockerJSON.AdyenSubmit.Root();
            adyenObj.PreferredLanguage = "en";
            adyenObj.TermsAndCondition = true;
            adyenObj.DeviceId = "0400CIfeAe15Cx8Nf94lis1ztlZtWBcopyaUnrp/XmcfWoVVgr+Rt2dAZAOoliNLIgrxaHjoci9mfzLNc/p/1y9PHkhCTHg+Ms0Qhlz5wlRFwWJi0FRulruXQQGCQaJkXU7G2epMgE22AlfKb5RAt5yDPAwgX6kMAgS5MXERlI7OnWiArM+ekFoA/991Z0ZkZ5kERBj4KWNrO6R/PjsbhNmiTfHBQm916StC15tFwbb5NkVtrNHzt95ePgXs+oQQc60trrGto44dFZ/k0B48ux+V4SxwhwBmNydI6S6la0lc7CNSwejOX5dhKDBAKBIrRLbRVKNQ0ZFLTxibRIXhZBoHcUWyskll+DoOeFUoW2PhFQQ+rqTLMfHvuGtGMuSZx2KTGP+2o6gTw3NQWjU6dDeaO/eJBm7pS/piJVR6Eq97a4GXPH40yCGiGelapidgItc7d8QXzT4BxAfV+VTxIJM3GyZ3ZJvaGz6yOj5zV1CK90DA4YKYRpZ4lqYiJ/+IO4wJwtQHr2DgT3vFL0ZMZD89cWte+8Pcqas/CQf7ODMKeW56Jn7YNCrmjgVViccHOEn2jcJoWWMCo6ViDwK4w63kwk+N0eY/qPMOYwHKk4MdqvbAG8bDG/3KhUsys+sxW8mW8sTzEDL+2GfZsbSyl9kH53ErNjRzWcwCm0SF4WQaB3ETLAxTSmCbUkBHbtl2L6+OonKBFCAZfXSVwAGEd3H4L1p1B4VyJESbaHzOXzuP+AtDgm50c6V/UpKetslI3IixYH1H5YVrp93GJ/KtOFGi8RKePA1UZdKAZDwic+y5/r+SkyAbziDM7k8xAXTS4l7D1erHMnjL6rg571Aobl8VJeJYJ4IT6kdNdMcew6I0e4WWMCVbBcdxP+fMxqv4WdGaMrHg9Btf04cRCL7CUahnxfbKRXey06cY4a7euFqiMUivlcetxenoDd7uW4fKrTjGYSqEk9HYTgjgklNKGT1jRHxfYV3WeA0UZK211nKAo9yNc557XJoPFyIq6DT5+rjE/a8We6nhBVlA0wHqjWT+OFRGSxTV28IDgVH79obFAl3YFLjm3SKvJrZ/+LS/hxnjVum3rOjk5C8fQIJpxzj6su/OL0PUeS7qLD4mV3TQqnxX7F2RS8mZK9e/TuJj3cu8BqVHq2r3awuh/3nRbiSbHgfJ/zck+QEC91vDzq9GxxVn+WvCwoViHULgwe4ejOQ9IF9wOHABPWNq9K72wUVXedwTidneZ5WeqDQRJDWdY1eS0ikV7ewoMthejcFMJfaGRwTY+OgBvs6eun9eZx9ahVWCv5G3Z0BkkT0+Q9R3XYb5xJxN//0biuCxRjE0I+SNuPBMLblzNZiullg8phlAO9epk22+Kb2H2izPBPwXH8zM4R26KRSUzWB9eJYJaYa6aO1DNSo4A7FptZxvgSTrBo9IAd82iQnT8VWo4o1/6JStbT2iknzW7KkIhzNyUcHJwhZwyFyl0Sty7FhQ4lDl481dnBUU06v7Z64UZs7ZSCPJg0rtMMIVhA==";
            adyenObj.CartId = cartId;
            adyenObj.EncryptedCardNumber = encryptedDataObj.Num;
            adyenObj.EncryptedSecurityCode = encryptedDataObj.Cvv;
            adyenObj.EncryptedExpiryMonth = encryptedDataObj.Month;
            adyenObj.EncryptedExpiryYear = encryptedDataObj.Year;
            adyenObj.PaymentMethod = "CREDITCARD";
            adyenObj.ReturnUrl = $"https://www.footlocker.{region.ToLower()}/adyen/checkout";
            adyenObj.BrowserInfo = new FootlockerJSON.AdyenSubmit.BrowserInfo();
            adyenObj.BrowserInfo.ScreenWidth = 2048;
            adyenObj.BrowserInfo.ScreenHeight = 864;
            adyenObj.BrowserInfo.ColorDepth = 24;
            adyenObj.BrowserInfo.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36";
            adyenObj.BrowserInfo.TimeZoneOffset = -60;
            adyenObj.BrowserInfo.Language = "en-US";
            adyenObj.BrowserInfo.JavaEnabled = false;

            string body = JsonConvert.SerializeObject(adyenObj);
            string submitAdyen = tlsClient.postRequest($"https://www.footlocker.{region}/api/users/orders/adyen", chain.headers, body, proxy);
            goTLSResponse submitAdyenResponse = null;
            FootlockerJSON.AdyenSubmitResponse.Root submitAdyenObj = null;
            try
            {
                submitAdyenResponse = JsonConvert.DeserializeObject<goTLSResponse>(submitAdyen);
                submitAdyenObj = JsonConvert.DeserializeObject<FootlockerJSON.AdyenSubmitResponse.Root>(submitAdyenResponse.Body);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed submitting payment infos, retrying..");
                return false;
            }
            if (submitAdyenObj.Action.Method != "GET")
            {
                paymentData = submitAdyenObj.Action.PaymentData;
                md = submitAdyenObj.Md;
                paReq = submitAdyenObj.PaReq;
                return true;
            }
            else
            {
                is3DS = true;
                if (SolveRevolut3DS(submitAdyenObj.Action.Url))
                {
                    return true;
                }
                return false;
            }

        }
        private bool EncryptCardData()
        {
            var fullName = firstname + " " + lastname;
            var client = new RestClient($"https://adyenencrypt.herokuapp.com/ftl_encryption/{cardNum}/{cvv}/{expMon}/{expYr}/{fullName}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);


            //tlsSolution.methodChain chain = new tlsSolution.methodChain();
            //chain.AddHeader("accept", "application/json");
            //string encryptedData = tlsClient.postRequest($"http://localhost:4050/ftl_encryption/{cardNum}/{cvv}/{expMon}/{expYr}/{firstname}+\" \"+{_lastname}", chain.headers, _proxy);
            //goTLSResponse encryptedDataResponse = null;

            try
            {
                //encryptedDataResponse = JsonConvert.DeserializeObject<goTLSResponse>(encryptedData);
                encryptedDataObj = JsonConvert.DeserializeObject<FootlockerJSON.EncryptedCardData.Root>(response.Content);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed preparing checkout data, retrying..");
                return false;
            }
            return true;
        }
        private bool AddBilling()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("origin", $"https://www.footlocker.{region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{region}/en/checkout")
            .AddHeader("Cookie", $"{sessionId}; cart-guid={cartId}");

            FootlockerJSON.Billing.Root billingObj = new FootlockerJSON.Billing.Root();
            billingObj.Country = new FootlockerJSON.Billing.Country();
            billingObj.SetAsDefaultBilling = true;
            billingObj.SetAsDefaultShipping = true;
            billingObj.FirstName = firstname;
            billingObj.LastName = lastname;
            billingObj.Email = true;
            billingObj.Phone = phone;
            billingObj.Country.Isocode = region;
            billingObj.Country.Name = country;
            billingObj.Id = null;
            billingObj.SetAsBilling = true;
            billingObj.Type = "default";
            billingObj.Line1 = addy;
            billingObj.Line2 = addy2;
            billingObj.PostalCode = zip;
            billingObj.Town = city;
            billingObj.ShippingAddress = true;

            string body = JsonConvert.SerializeObject(billingObj);

            string submitBilling = tlsClient.postRequest($"https://www.footlocker.{region}/api/users/carts/current/set-billing", chain.headers, body, proxy);
            try
            {
                chain.collectCookies(submitBilling).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed setting headers, retrying..");
                return false;
            }

            goTLSResponse submitBillingResponse = null;
            try
            {
                submitBillingResponse = JsonConvert.DeserializeObject<goTLSResponse>(submitBilling);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed setting billing address, retrying..");
                return false;
            }
            if (submitBillingResponse.Status == 200)
            {
                return true;
            }
            return false;
        }

        private bool AddShipping()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("origin", $"https://www.footlocker.{region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{region}/en/checkout")
            .AddHeader("Cookie", $"{sessionId}; cart-guid={cartId}");

            FootlockerJSON.Shipping.Root shippingObj = new FootlockerJSON.Shipping.Root();
            shippingObj.ShippingAddress = new FootlockerJSON.Shipping.ShippingAddress();
            shippingObj.ShippingAddress.Country = new FootlockerJSON.Shipping.Country();
            shippingObj.ShippingAddress.SetAsDefaultBilling = true;
            shippingObj.ShippingAddress.SetAsDefaultShipping = true;
            shippingObj.ShippingAddress.FirstName = firstname;
            shippingObj.ShippingAddress.LastName = lastname;
            shippingObj.ShippingAddress.Email = true;
            shippingObj.ShippingAddress.Phone = phone;
            shippingObj.ShippingAddress.Country.Isocode = region;
            shippingObj.ShippingAddress.Country.Name = country;
            shippingObj.ShippingAddress.Id = null;
            shippingObj.ShippingAddress.SetAsBilling = true;
            shippingObj.ShippingAddress.Type = "default";
            shippingObj.ShippingAddress.Line1 = addy;
            shippingObj.ShippingAddress.Line2 = addy2;
            shippingObj.ShippingAddress.PostalCode = zip;
            shippingObj.ShippingAddress.Town = city;
            shippingObj.ShippingAddress.ShippingAddressInside = true;

            string body = JsonConvert.SerializeObject(shippingObj);

            string submitShipping = tlsClient.postRequest($"https://www.footlocker.{region}/api/users/carts/current/addresses/shipping", chain.headers, body, proxy);
            try
            {
                chain.collectCookies(submitShipping).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed setting headers, retrying..");
                return false;
            }

            goTLSResponse submitShippingResponse = null;
            FootlockerJSON.ShippingResponse.Root submitShippingObj = null;
            try
            {
                submitShippingResponse = JsonConvert.DeserializeObject<goTLSResponse>(submitShipping);
                submitShippingObj = JsonConvert.DeserializeObject<FootlockerJSON.ShippingResponse.Root>(submitShippingResponse.Body);
            }
            catch (Exception e)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed setting shipping address, retrying..");
                return false;
            }
            if (submitShippingResponse.Status == 201 && submitShippingObj.Country.Isocode == region)
            {
                shippingId = submitShippingObj.Id;
                return true;
            }

            return false;
        }
        private bool InitiateCheckout()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();

            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("content-length", "0")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("origin", $"https://www.footlocker.{region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{region}/en/checkout")
            .AddHeader("Cookie", $"{sessionId}; cart-guid={cartId}");

            string initCheckout = tlsClient.putRequest($"https://www.footlocker.{region}/api/users/carts/current/email/{email}", chain.headers, proxy);

            //try
            //{
            //    chain.collectCookies(initCheckout).headers.Clear();

            //}
            //catch (Exception)
            //{
            //    Program.ChangeColor(ConsoleColor.Red);
            //    Console.WriteLine($"{timestamp()}{_taskNumber}Failed setting headers, retrying..");
            //    return false;
            //}

            goTLSResponse initCheckoutObj = JsonConvert.DeserializeObject<goTLSResponse>(initCheckout);

            if (initCheckoutObj != null && initCheckoutObj.Status == 200)
            {
                return true;
            }
            Program.ChangeColor(ConsoleColor.Red);
            Console.WriteLine($"{timestamp()}{taskNumber}Failed initiating checkout, retrying..");
            return false;
        }
        private bool AddToCart()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();

            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("sec-ch-ua", "'Google Chrome', 'v = '87'',  'Not;A Brand', 'v = '99'', 'Chromium', v = '87''")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("x-csrf-token", csrf)
            .AddHeader("x-api-lang", "en-CA")
            .AddHeader("accept-language", "en-CA,en;q=0.9")
            .AddHeader("x-fl-productid", sizeCode)
            .AddHeader("content-type", "application/json")
            .AddHeader("accept", "application/json")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("origin", $"https://www.footlocker.{region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{region}/en/product/-/{product}.html")
            .AddHeader("Cookie", sessionId);

            string body = $"{{\"productQuantity\":1,\"productId\":\"{sizeCode}\"}}";

            string postCart = tlsClient.postRequest($"https://www.footlocker.{region}/api/users/carts/current/entries", chain.headers, body, proxy);
            try
            {
                chain.collectCookies(postCart).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed setting headers, retrying... ");
                return false;
            }

            FootlockerJSON.Cart.Root postCartObj = null;
            try
            {
                postCartObj = JsonConvert.DeserializeObject<FootlockerJSON.Cart.Root>(JsonConvert.DeserializeObject<goTLSResponse>(postCart).Body);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Carting failed, retrying.." + postCart);
                return false;
            }
            if (postCartObj != null && postCartObj.TotalItems > 0)
            {
                cartId = postCartObj.Guid;
                cartCode = postCartObj.Code;
                return true;
            }
            Program.ChangeColor(ConsoleColor.Red);
            Console.WriteLine($"{timestamp()}{taskNumber}Carting failed, retrying..");



            return false;
        }
        private bool NewSession()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();

            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{region}/en/product/-/{product}.html");

            string getSession = tlsClient.getRequest($"https://www.footlocker.{region}/api/session", chain.headers, proxy);
            try
            {
                chain.collectCookies(getSession).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed setting headers, retrying..");
                return false;
            }

            goTLSResponse sessionResponse = null;
            FootlockerJSON.Session.Root sessionResponseObj = null;

            try
            {
                sessionResponse = JsonConvert.DeserializeObject<goTLSResponse>(getSession);
                sessionResponseObj = JsonConvert.DeserializeObject<FootlockerJSON.Session.Root>(sessionResponse.Body);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Could not generate session, retrying..");
                return false;
            }
            if (sessionResponse != null && sessionResponseObj != null && sessionResponseObj.Success)
            {
                csrf = sessionResponseObj.Data.CsrfToken;
                sessionId = sessionResponse.Cookies[0];
                return true;
            }
            return false;
        }

        private bool GetProductInfos()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();

            chain.AddHeader("authority", $"www.footlocker.{region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-CA")
            .AddHeader("accept-language", "en-CA,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{region}/en/product/-/{product}.html");

            string getProductInfos = tlsClient.getRequest($"https://www.footlocker.{region}/api/products/pdp/{product}", chain.headers, proxy);
            try
            {
                chain.collectCookies(getProductInfos).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed setting headers, retrying..");
                return false;
            }

            FootlockerJSON.Product.Root productInfoObj = null;
            try
            {
                productInfoObj = JsonConvert.DeserializeObject<FootlockerJSON.Product.Root>(JsonConvert.DeserializeObject<goTLSResponse>(getProductInfos).Body);
            }
            catch (Exception e) //forwarding error e to our db soon - will setup later!
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Could not fetch product info, retrying..");
                return false;
            }

            if (productInfoObj != null)
            {
                productName = productInfoObj.Name;
                taskNumber += $"[{productInfoObj.Name}] [{size}] ";

                //fetching sizecode
                Parallel.ForEach(productInfoObj.SellableUnits, (sku) =>
                {
                    string skuname = Regex.Match(sku.Attributes[1].Value, $"[0-9]+(-[0-9]+)+$").Value;
                    string[] splitSku = skuname.Split("-");
                    string pTM = splitSku[0] + splitSku[1];

                    if (pTM == product && sku.Attributes[0].Value == size)
                    {
                        sizeCode = sku.Attributes[0].Id;

                        if (productInfoObj.VariantAttributes[0].DisplayCountDownTimer)
                        {
                            releaseTimer = productInfoObj.VariantAttributes[0].CstSkuLaunchDate;
                        };
                        price = productInfoObj.VariantAttributes[0].Price.OriginalPrice;
                    }
                });
                //checks if size hasn't been found
                if (sizeCode == null)
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{timestamp()}{taskNumber}Product/size not loaded! Retrying after delay..");
                    Thread.Sleep(delay);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool SolveRevolut3DS(string url)
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("Connection", "keep-alive")
            .AddHeader("Upgrade-Insecure-Requests", "1")
            .AddHeader("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36")
            .AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
            .AddHeader("Sec-Fetch-Site", "none")
            .AddHeader("Sec-Fetch-Mode", "navigate")
            .AddHeader("Sec-Fetch-Dest", "document")
            .AddHeader("Accept-Language", "en-US,en;q=0.9,sv;q=0.8");

            string initiateChallenge = tlsClient.getRequest(url, chain.headers, proxy);
            goTLSResponse initiateChallengeResponse = null;
            try
            {
                initiateChallengeResponse = JsonConvert.DeserializeObject<goTLSResponse>(initiateChallenge);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed solving 3DS {{ERRCODE: 0}}..");
                return false;
            }
            if (initiateChallengeResponse.Status == 200 && initiateChallengeResponse.Body != null)
            {
                //binding MD VALUE
                string mdPattern = @"(?<=""MD"" value="")(.*)(?="")";
                RegexOptions options = RegexOptions.Multiline;
                foreach (Match m in Regex.Matches(initiateChallengeResponse.Body, mdPattern, options))
                {
                    md = m.Value;
                }

                //bindin paReq VALUE
                string paReqPattern = @"(?<=""PaReq"" value="")(.*)(?="")";
                foreach (Match m in Regex.Matches(initiateChallengeResponse.Body, paReqPattern, options))
                {
                    paReq = m.Value;
                }

                string termURLPattern = @"(?<=TermUrl"" value="")(.*)(?="")";
                foreach (Match m in Regex.Matches(initiateChallengeResponse.Body, termURLPattern, options))
                {
                    termURL = m.Value;
                }
            }
            if (md != null && paReq != null)
            {
                return true;
            }
            return false;
        }

        private bool Poll3DS()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", "poll.touchtechpayments.com")
            .AddHeader("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("accept", "*/*")
            .AddHeader("origin", "https://idcheck.acs.touchtechpayments.com")
            .AddHeader("sec-fetch-site", "same-site")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", "https://idcheck.acs.touchtechpayments.com/")
            .AddHeader("accept-language", "en-US,en;q=0.9,sv;q=0.8");

            FootlockerJSON.Poll.Root pollBodyObj = new FootlockerJSON.Poll.Root();
            pollBodyObj.TransToken = _transToken;
            string body = JsonConvert.SerializeObject(pollBodyObj);

            string poll = tlsClient.postRequest("https://poll.touchtechpayments.com/poll", chain.headers, body, proxy);
            goTLSResponse pollResponse = null;
            FootlockerJSON.Poll.Root pollObj = null;
            try
            {
                pollResponse = JsonConvert.DeserializeObject<goTLSResponse>(poll);
                pollObj = JsonConvert.DeserializeObject<FootlockerJSON.Poll.Root>(pollResponse.Body);
                if (pollObj.Status == "success")
                {
                    _authToken = pollObj.AuthToken;
                    return true;
                }
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed getting 3DS state, retrying..");
                return false;

            }

            return false;
        }

        private bool ConfirmTransaction()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", "macs.touchtechpayments.com")
            .AddHeader("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("accept", "*/*")
            .AddHeader("origin", "https://idcheck.acs.touchtechpayments.com")
            .AddHeader("sec-fetch-site", "same-site")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", "https://idcheck.acs.touchtechpayments.com/")
            .AddHeader("accept-language", "en-US,en;q=0.9,sv;q=0.8");

            FootlockerJSON.Poll.Root pollBodyObj = new FootlockerJSON.Poll.Root();
            pollBodyObj.TransToken = _transToken;
            pollBodyObj.AuthToken = _authToken;

            string body = JsonConvert.SerializeObject(pollBodyObj);

            string confirmTransaction = tlsClient.postRequest("https://macs.touchtechpayments.com/v1/confirmTransaction", chain.headers,body, proxy);
            goTLSResponse confirmTransactionResponse = null;
            FootlockerJSON.Poll.Root confirmTransactionObj = null;
            try
            {
                confirmTransactionResponse = JsonConvert.DeserializeObject<goTLSResponse>(confirmTransaction);
                confirmTransactionObj = JsonConvert.DeserializeObject<FootlockerJSON.Poll.Root>(confirmTransactionResponse.Body);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}{taskNumber}Failed confirming 3DS state, retrying..");
                return false;
            }
            if (confirmTransactionObj.ResponseCode == "3")
            {
                return true;
            }
            return false;
        }

        private string timestamp()
        {
            return $"[{DateTime.Now}] ";
        }

    }
}
