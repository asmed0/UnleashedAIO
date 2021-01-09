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
        static int retryOnFailAttempts = 100;

        //program vars
        static string _taskNumber;
        static int _delay;
        static string _proxy;

        //will be used later on
        static string _productName;
        static string _sizeCode;
        static string _releaseTimer;
        static string _productImage;
        static double _price;

        //profile vars
        static string _mode;
        static string _product;
        static string _size;
        static string _firstname;
        static string _lastname;
        static string _email;
        static string _phone;
        static string _addy;
        static string _addy2;
        static string _city;
        static string _state;
        static string _zip;
        static string _region;
        static string _cardNum;
        static string _expMon;
        static string _expYr;
        static string _cvv;

        //session vars
        static string _csrf;
        static string _sessionId;
        static string _cartId;
        static string _cartCode;
        public static async Task<bool> StartTaskAsync(Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            //binding program vars
            _taskNumber = taskNumber;
            _delay = delay;
            _proxy = proxyString;

            //binding profile vars
            _mode = currentTask.Mode;
            _product = currentTask.SKU;
            _size = currentTask.Size;
            _firstname = currentTask.FirstName;
            _lastname = currentTask.LastName;
            _email = currentTask.email;
            _phone = currentTask.PhoneNumber;
            _addy = currentTask.Adress;
            _addy2 = currentTask.Adress2;
            _city = currentTask.City;
            _state = currentTask.State;
            _zip = currentTask.ZipCode;
            _region = currentTask.Country;
            _cardNum = currentTask.CardNumber;
            _expMon = currentTask.ExpiryMonth;
            _expYr = currentTask.ExpiryYear;
            _cvv = currentTask.CVV;
            _proxy = proxyString;

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
            for (int attempts = 0; attempts < retryOnFailAttempts; attempts++, _delay+=100) //incrementing delay by 100ms to simulate staggering human behaviour
            {
                if (GetProductInfos())
                {
                    Program.ChangeColor(ConsoleColor.Yellow);
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Fetched product info, now generating session..");

                    Thread.Sleep(_delay);
                    break;
                }
                Thread.Sleep(_delay);
            }

            //generating our session
            for (int attempts = 0; attempts < retryOnFailAttempts; attempts++, _delay += 100)
            {
                if (NewSession())
                {
                    Program.ChangeColor(ConsoleColor.Yellow);
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Generated session, now carting..");
                    //Console.WriteLine($"csrf: {_csrf} and sessionid: {_sessionId}");

                    Thread.Sleep(_delay);
                    break;
                }
                Thread.Sleep(_delay);
            }

            //carting our product
            for (int attempts = 0; attempts < retryOnFailAttempts; attempts++, _delay += 100)
            {
                if (AddToCart())
                {
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Carted, now adding shipping..");
                    //Console.WriteLine($"cartId: {_cartId} and sessionid: {_sessionId}");

                    Thread.Sleep(_delay);
                    break;
                }
                Thread.Sleep(_delay);
            }

            //initating checkout
            for (int attempts = 0; attempts < retryOnFailAttempts; attempts++, _delay += 100)
            {
                if (InitiateCheckout())
                {
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Carted, now adding shipping..");
                    //Console.WriteLine($"cartId: {_cartId} and sessionid: {_sessionId}");

                    Thread.Sleep(_delay);
                    break;
                }
                Thread.Sleep(_delay);
            }

            //Console.ReadLine();

            return true;
        }
        private static bool AddShipping()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            return false;
        }
        private static bool InitiateCheckout()
        {
            return false;

        }
        private static bool AddToCart()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", "www.footlocker.se")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("x-csrf-token", _csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("x-fl-productid", _sizeCode)
            .AddHeader("content-type", "application/json")
            .AddHeader("accept", "application/json")
            .AddHeader("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("origin", $"https://www.footlocker.{_region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/product/-/{_product}.html")
            .AddHeader("Cookie", _sessionId);

            string body = $"{{\"productQuantity\":1,\"productId\":\"{_sizeCode}\"}}";

            string postCart = tlsSolution.postRequest($"https://www.footlocker.{_region}/api/users/carts/current/entries", chain.headers, body, _proxy);
            chain.collectCookies(postCart).headers.Clear();

            FootlockerJSON.Cart.Root postCartObj = null;
            try
            {
                postCartObj = JsonConvert.DeserializeObject<FootlockerJSON.Cart.Root>(JsonConvert.DeserializeObject<goTLSResponse>(postCart).Body);
            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Carting failed, retrying..");
                return false;
            }
            if (postCartObj != null && postCartObj.TotalItems > 0)
            {
                _cartId = postCartObj.Guid;
                _cartCode = postCartObj.Code;
                return true;
            }
            return false;
        }
        private static bool NewSession()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", "www.footlocker.se")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/product/-/{_product}.html");

            string getSession = tlsSolution.getRequest($"https://www.footlocker.{_region}/api/session",chain.headers, _proxy);
            chain.collectCookies(getSession).headers.Clear();
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
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Could not generate session, retrying..");
                return false;
            }
            if(sessionResponse != null && sessionResponseObj != null && sessionResponseObj.Success)
            {
                _csrf = sessionResponseObj.Data.CsrfToken;
                _sessionId = sessionResponse.Cookies[0];
                return true;
            }
            return false;
        }

        private static bool GetProductInfos()
        {

            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", $"www.footlocker.{_region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/product/-/{_product}.html");

            string getProductInfos = tlsSolution.getRequest($"https://www.footlocker.{_region}/api/products/pdp/{_product}", chain.headers, _proxy);
            chain.collectCookies(getProductInfos).headers.Clear();

            FootlockerJSON.Product.Root productInfoObj = null;
            try
            {
                productInfoObj = JsonConvert.DeserializeObject<FootlockerJSON.Product.Root>(JsonConvert.DeserializeObject<goTLSResponse>(getProductInfos).Body);
            }
            catch (Exception e) //forwarding error e to our db soon - will setup later!
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Could not fetch product info, retrying..");
                return false;
            }

            if (productInfoObj != null)
            {
                _productName = productInfoObj.Name;
                _taskNumber += $"[{productInfoObj.Name}] [{_size}] ";

                //fetching sizecode
                foreach (var sku in productInfoObj.SellableUnits)
                {
                    if (sku.Attributes[0].Value == _size)
                    {
                       _sizeCode = sku.Attributes[0].Id;

                        //fetching timer
                        if (productInfoObj.VariantAttributes[0].DisplayCountDownTimer)
                        {
                            _releaseTimer = productInfoObj.VariantAttributes[0].CstSkuLaunchDate;
                        };

                        _price = productInfoObj.VariantAttributes[0].Price.OriginalPrice;
                        return true;
                    }
                }
                //checks if size hasn't been found
                if (_sizeCode == null)
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Product/size not loaded! Retrying after delay..");
                    Thread.Sleep(_delay);
                    return false;
                }
            }
            return false;
        }
    }
}
