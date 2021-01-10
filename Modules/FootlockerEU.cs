using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using golang;
using UnleashedAIO.JSON;
using Newtonsoft.Json;
using System.Threading;

namespace UnleashedAIO.Modules
{
    class FootlockerEU
    {
        public class Vars
        {
            public int retryOnFailAttempts = 100;

            //program vars
            public string _taskNumber;
            public int _delay;
            public string _proxy;

            //will be used later on
            public string _productName;
            public string _sizeCode;
            public string _releaseTimer;
            public string _productImage;
            public double _price;

            //profile vars
            public string _mode;
            public string _product;
            public string _size;
            public string _firstname;
            public string _lastname;
            public string _email;
            public string _phone;
            public string _addy;
            public string _addy2;
            public string _city;
            public string _state;
            public string _zip;
            public string _region;
            public string _country;
            public string _cardNum;
            public string _expMon;
            public string _expYr;
            public string _cvv;

            //session vars
            public string _csrf;
            public string _sessionId;
            public string _cartId;
            public string _cartCode;
            public string _shippingId;
        }

        private Vars vars = new Vars();
        private tlsSolution tlsClient = new tlsSolution();
        private tlsSolution.methodChain chain = new tlsSolution.methodChain();
        public bool StartTaskAsync(Tasks currentTask, string proxyString, string taskNumber, int delay)
        {

            //binding program vars
            vars._taskNumber = taskNumber;
            vars._delay = delay;
            vars._proxy = proxyString;

            //binding profile vars
            vars._mode = currentTask.Mode;
            vars._product = currentTask.SKU;
            vars._size = currentTask.Size;
            vars._firstname = currentTask.FirstName;
            vars._lastname = currentTask.LastName;
            vars._email = currentTask.email;
            vars._phone = currentTask.PhoneNumber;
            vars._addy = currentTask.Adress;
            vars._addy2 = currentTask.Adress2;
            vars._city = currentTask.City;
            vars._state = currentTask.State;
            vars._zip = currentTask.ZipCode;
            vars._region = currentTask.Country;
            vars._cardNum = currentTask.CardNumber;
            vars._expMon = currentTask.ExpiryMonth;
            vars._expYr = currentTask.ExpiryYear;
            vars._cvv = currentTask.CVV;
            vars._proxy = proxyString;

            switch (vars._region)
            {
                case "SE":
                    vars._country = "Sweden";
                    break;
                case "DE":
                    vars._country = "Germany";
                    break;
                default:
                    Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}REGION NOT RECOGNIZED - UPDATE TASK(S)");
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


            //fetching product details
            for (int attempts = 0; attempts < 2; attempts++, vars._delay += 100) 
            {
                if (GetProductInfos(vars))
                {
                    Program.ChangeColor(ConsoleColor.Yellow);
                    Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Fetched product info, now generating session..");

                    Thread.Sleep(vars._delay);
                    break;
                }
                Thread.Sleep(vars._delay);
            }

            //generating our session
            for (int attempts = 0; attempts < vars.retryOnFailAttempts; attempts++, vars._delay += 100)
            {
                if (NewSession(vars))
                {
                    Program.ChangeColor(ConsoleColor.Yellow);
                    Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Generated session, now carting..");
                    //Console.WriteLine($"csrf: {_csrf} and sessionid: {_sessionId}");

                    Thread.Sleep(vars._delay);
                    break;
                }
                Thread.Sleep(vars._delay);
            }

            //carting our product
            for (int attempts = 0; attempts < vars.retryOnFailAttempts; attempts++, vars._delay += 100)
            {
                if (AddToCart(vars))
                {
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Carted, now going to checkout..");
                    //Console.WriteLine($"cartId: {_cartId} and sessionid: {_sessionId}");

                    Thread.Sleep(vars._delay);
                    break;
                }
                Thread.Sleep(vars._delay);
            }

            //initating checkout
            for (int attempts = 0; attempts < vars.retryOnFailAttempts; attempts++, vars._delay += 100)
            {
                if (InitiateCheckout(vars))
                {
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Initiated checkout session, now adding shipping..");
                    //Console.WriteLine($"cartId: {_cartId} and sessionid: {_sessionId}");

                    Thread.Sleep(vars._delay);
                    break;
                }
                Thread.Sleep(vars._delay);
            }

            //adding shipping
            for (int attempts = 0; attempts < vars.retryOnFailAttempts; attempts++, vars._delay += 100)
            {
                if (AddShipping( vars))
                {
                    Program.ChangeColor(ConsoleColor.Cyan);
                    Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Added shipping, now adding billing..");

                    Thread.Sleep(vars._delay);
                    break;
                }
                Thread.Sleep(vars._delay);
            }
            ////adding billing -- will be implemented soon.. 
            //for (int attempts = 0; attempts < vars.retryOnFailAttempts; attempts++, vars._delay += 100)
            //{
            //    if (AddShipping(vars))
            //    {
            //        Program.ChangeColor(ConsoleColor.Cyan);
            //        Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Added shipping, now adding billing..");

            //        Thread.Sleep(vars._delay);
            //        break;
            //    }
            //    Thread.Sleep(vars._delay);
            //}

            //Console.ReadLine();

            return true;
        }

        private bool AddShipping(Vars vars)
        {
            chain.AddHeader("authority", $"www.footlocker.{vars._region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", vars._csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("origin", $"https://www.footlocker.{vars._region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{vars._region}/en/checkout")
            .AddHeader("Cookie", $"{vars._sessionId}; {vars._cartId}");

            FootlockerJSON.Shipping.Root shippingObj = new FootlockerJSON.Shipping.Root();
            shippingObj.ShippingAddress = new FootlockerJSON.Shipping.ShippingAddress();
            shippingObj.ShippingAddress.Country = new FootlockerJSON.Shipping.Country();
            shippingObj.ShippingAddress.SetAsDefaultBilling = true;
            shippingObj.ShippingAddress.SetAsDefaultShipping = true;
            shippingObj.ShippingAddress.FirstName = vars._firstname;
            shippingObj.ShippingAddress.LastName = vars._lastname;
            shippingObj.ShippingAddress.Email = true;
            shippingObj.ShippingAddress.Phone = vars._phone;
            shippingObj.ShippingAddress.Country.Isocode = vars._region;
            shippingObj.ShippingAddress.Country.Name = "Sweden";
            shippingObj.ShippingAddress.Id = null;
            shippingObj.ShippingAddress.SetAsBilling = true;
            shippingObj.ShippingAddress.Type = "default";
            shippingObj.ShippingAddress.Line1 = vars._addy;
            shippingObj.ShippingAddress.PostalCode = vars._zip;
            shippingObj.ShippingAddress.Town = vars._city;
            shippingObj.ShippingAddress.ShippingAddressInside = true;

            string body = JsonConvert.SerializeObject(shippingObj);

            string submitShipping = tlsClient.postRequest($"https://www.footlocker.{vars._region}/api/users/carts/current/addresses/shipping",chain.headers, body, vars._proxy);
            try
            {
                chain.collectCookies(submitShipping).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine(e);
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Failed setting shipping address, retrying..");
                return false;
            }
            if (submitShippingResponse.Status == 201 && submitShippingObj.Country.Isocode == vars._region)
            {
                vars._shippingId = submitShippingObj.Id;
                return true;
            }
            return false;
        }
        private bool InitiateCheckout(Vars vars)
        {
            chain.AddHeader("authority", $"www.footlocker.{vars._region}")
            .AddHeader("content-length", "0")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", vars._csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("origin", $"https://www.footlocker.{vars._region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{vars._region}/en/checkout")
            .AddHeader("Cookie", $"{vars._sessionId}; {vars._cartId}");

            string initCheckout = tlsClient.putRequest($"https://www.footlocker.{vars._region}/api/users/carts/current/email/{vars._email}", chain.headers, vars._proxy);
            try
            {
                chain.collectCookies(initCheckout).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Failed setting headers, retrying..");
                return false;
            }

            goTLSResponse initCheckoutObj = null;
            try
            {
                initCheckoutObj = JsonConvert.DeserializeObject<goTLSResponse>(initCheckout);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Could not read response upon checkout initialization, retrying..");
                return false;
            }
            if (initCheckoutObj != null && initCheckoutObj.Status == 200)
            {
                return true;
            }
            return false;
        }
        private bool AddToCart(Vars vars)
        {
            chain.AddHeader("authority", $"www.footlocker.{vars._region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("x-csrf-token", vars._csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("x-fl-productid", vars._sizeCode)
            .AddHeader("content-type", "application/json")
            .AddHeader("accept", "application/json")
            .AddHeader("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("origin", $"https://www.footlocker.{vars._region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{vars._region}/en/product/-/{vars._product}.html")
            .AddHeader("Cookie", vars._sessionId);

            string body = $"{{\"productQuantity\":1,\"productId\":\"{vars._sizeCode}\"}}";

            string postCart = tlsClient.postRequest($"https://www.footlocker.{vars._region}/api/users/carts/current/entries", chain.headers, body, vars._proxy);
            try
            {
                chain.collectCookies(postCart).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Carting failed, retrying..");
                return false;
            }
            if (postCartObj != null && postCartObj.TotalItems > 0)
            {
                vars._cartId = $"cart-guid={postCartObj.Guid};";
                vars._cartCode = postCartObj.Code;
                return true;
            }
            return false;
        }
        private bool NewSession(Vars vars)
        {
            chain.AddHeader("authority", $"www.footlocker.{vars._region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{vars._region}/en/product/-/{vars._product}.html");

            string getSession = tlsClient.getRequest($"https://www.footlocker.{vars._region}/api/session",chain.headers, vars._proxy);
            try
            {
                chain.collectCookies(getSession).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Could not generate session, retrying..");
                return false;
            }
            if(sessionResponse != null && sessionResponseObj != null && sessionResponseObj.Success)
            {
                vars._csrf = sessionResponseObj.Data.CsrfToken;
                vars._sessionId = sessionResponse.Cookies[0];
                return true;
            }
            return false;
        }

        private bool GetProductInfos(Vars vars)
        {

            chain.AddHeader("authority", $"www.footlocker.{vars._region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{vars._region}/en/product/-/{vars._product}.html");

            string getProductInfos = tlsClient.getRequest($"https://www.footlocker.{vars._region}/api/products/pdp/{vars._product}", chain.headers, vars._proxy);
            try
            {
                chain.collectCookies(getProductInfos).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Could not fetch product info, retrying..");
                return false;
            }

            if (productInfoObj != null)
            {
                vars._productName = productInfoObj.Name;
                vars._taskNumber += $"[{productInfoObj.Name}] [{vars._size}] ";

                //fetching sizecode
                foreach (var sku in productInfoObj.SellableUnits)
                {
                    if (sku.Attributes[0].Value == vars._size)
                    {
                        vars._sizeCode = sku.Attributes[0].Id;

                        //fetching timer
                        if (productInfoObj.VariantAttributes[0].DisplayCountDownTimer)
                        {
                            vars._releaseTimer = productInfoObj.VariantAttributes[0].CstSkuLaunchDate;
                        };

                        vars._price = productInfoObj.VariantAttributes[0].Price.OriginalPrice;
                        return true;
                    }
                }
                //checks if size hasn't been found
                if (vars._sizeCode == null)
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{Program.timestamp()}{vars._taskNumber}Product/size not loaded! Retrying after delay..");
                    Thread.Sleep(vars._delay);
                    return false;
                }
            }
            return false;
        }
    }
}
