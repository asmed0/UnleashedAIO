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
        public int retryOnFailAttempts = 10000;

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

        private tlsSolution tlsClient = new tlsSolution();
        public bool StartTaskAsync(Tasks currentTask, string taskNumber, int delay, int taskIndex)
        {

            //binding program vars
            _taskNumber = taskNumber;
            _delay = delay;

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
            _proxy = ProxyMaster.getProxy(taskIndex);

            if(_proxy == "NAP")
            {

                Console.WriteLine($"{Program.timestamp()}{_taskNumber} No proxies available, ending task.. ");
                Thread.Sleep(Timeout.Infinite);
            }
            Console.WriteLine($"{Program.timestamp()}{_taskNumber}Task starting..");
            //Program.ChangeColor(ConsoleColor.DarkGray);
            //Console.WriteLine($"{Program.timestamp()}{_taskNumber} Task Started! Marking Time");
            //Program.WriteLog("log",$"{Program.timestamp()}{_taskNumber} Task Started! Marking Time");
            switch (_region)
            {
                case "SE":
                    _country = "Sweden";
                    break;
                case "DE":
                    _country = "Germany";
                    break;
                default:
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}REGION NOT RECOGNIZED - UPDATE TASK(S)");
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
            for (int attempts = 0; attempts < 2; attempts++, _delay += 100)
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
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Carted, now going to checkout..");
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
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Initiated checkout session, now adding shipping..");
                    //Console.WriteLine($"cartId: {_cartId} and sessionid: {_sessionId}");

                    Thread.Sleep(_delay);
                    break;
                }
                //Thread.Sleep(_delay);
            }

            //adding shipping
            for (int attempts = 0; attempts < retryOnFailAttempts; attempts++, _delay += 100)
            {
                if (AddShipping())
                {
                    Program.ChangeColor(ConsoleColor.Cyan);
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Added shipping, now adding billing..");

                    Thread.Sleep(_delay);
                    break;
                }
                Thread.Sleep(_delay);
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

        private bool AddShipping()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", $"www.footlocker.{_region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", _csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("content-type", "application/json")
            .AddHeader("origin", $"https://www.footlocker.{_region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/checkout")
            .AddHeader("Cookie", $"{_sessionId}; {_cartId}");

            FootlockerJSON.Shipping.Root shippingObj = new FootlockerJSON.Shipping.Root();
            shippingObj.ShippingAddress = new FootlockerJSON.Shipping.ShippingAddress();
            shippingObj.ShippingAddress.Country = new FootlockerJSON.Shipping.Country();
            shippingObj.ShippingAddress.SetAsDefaultBilling = true;
            shippingObj.ShippingAddress.SetAsDefaultShipping = true;
            shippingObj.ShippingAddress.FirstName = _firstname;
            shippingObj.ShippingAddress.LastName = _lastname;
            shippingObj.ShippingAddress.Email = true;
            shippingObj.ShippingAddress.Phone = _phone;
            shippingObj.ShippingAddress.Country.Isocode = _region;
            shippingObj.ShippingAddress.Country.Name = _country;
            shippingObj.ShippingAddress.Id = null;
            shippingObj.ShippingAddress.SetAsBilling = true;
            shippingObj.ShippingAddress.Type = "default";
            shippingObj.ShippingAddress.Line1 = _addy;
            shippingObj.ShippingAddress.PostalCode = _zip;
            shippingObj.ShippingAddress.Town = _city;
            shippingObj.ShippingAddress.ShippingAddressInside = true;

            string body = JsonConvert.SerializeObject(shippingObj);

            string submitShipping = tlsClient.postRequest($"https://www.footlocker.{_region}/api/users/carts/current/addresses/shipping", chain.headers, body, _proxy);
            try
            {
                chain.collectCookies(submitShipping).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Failed setting shipping address, retrying..");
                return false;
            }
            if (submitShippingResponse.Status == 201 && submitShippingObj.Country.Isocode == _region)
            {
                _shippingId = submitShippingObj.Id;
                return true;
            }

            return false;
        }
        private bool InitiateCheckout()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();

            chain.AddHeader("authority", $"www.footlocker.{_region}")
            .AddHeader("content-length", "0")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-csrf-token", _csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("origin", $"https://www.footlocker.{_region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/checkout")
            .AddHeader("Cookie", $"{_sessionId}; {_cartId}");

            string initCheckout = tlsClient.putRequest($"https://www.footlocker.{_region}/api/users/carts/current/email/{_email}", chain.headers, _proxy);

            //try
            //{
            //    chain.collectCookies(initCheckout).headers.Clear();

            //}
            //catch (Exception)
            //{
            //    Program.ChangeColor(ConsoleColor.Red);
            //    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Failed setting headers, retrying..");
            //    return false;
            //}

            goTLSResponse initCheckoutObj = JsonConvert.DeserializeObject<goTLSResponse>(initCheckout);

            if (initCheckoutObj != null && initCheckoutObj.Status == 200)
            {
                return true;
            }
            Program.ChangeColor(ConsoleColor.Red);
            Console.WriteLine($"{Program.timestamp()}{_taskNumber}Failed initiating checkout, retrying..");
            return false;
        }
        private bool AddToCart()
        {
            tlsSolution.methodChain chain = new tlsSolution.methodChain();

            chain.AddHeader("authority", $"www.footlocker.{_region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("x-csrf-token", _csrf)
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("x-fl-productid", _sizeCode)
            .AddHeader("content-type", "application/json")
            .AddHeader("accept", "application/json")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("origin", $"https://www.footlocker.{_region}")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/product/-/{_product}.html")
            .AddHeader("Cookie", _sessionId);

            string body = $"{{\"productQuantity\":1,\"productId\":\"{_sizeCode}\"}}";

            string postCart = tlsClient.postRequest($"https://www.footlocker.{_region}/api/users/carts/current/entries", chain.headers, body, _proxy);
            try
            {
                chain.collectCookies(postCart).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Carting failed, retrying..");
                return false;
            }
            if (postCartObj != null && postCartObj.TotalItems > 0)
            {
                _cartId = $"cart-guid={postCartObj.Guid};";
                _cartCode = postCartObj.Code;
                return true;
            }
            return false;
        }
        private bool NewSession()
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

            string getSession = tlsClient.getRequest($"https://www.footlocker.{_region}/api/session", chain.headers, _proxy);
            try
            {
                chain.collectCookies(getSession).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Could not generate session, retrying..");
                return false;
            }
            if (sessionResponse != null && sessionResponseObj != null && sessionResponseObj.Success)
            {
                _csrf = sessionResponseObj.Data.CsrfToken;
                _sessionId = sessionResponse.Cookies[0];
                return true;
            }
            return false;
        }

        private bool GetProductInfos()
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

            string getProductInfos = tlsClient.getRequest($"https://www.footlocker.{_region}/api/products/pdp/{_product}", chain.headers, _proxy);
            try
            {
                chain.collectCookies(getProductInfos).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Failed setting headers, retrying..");
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
                Console.WriteLine($"{Program.timestamp()}{_taskNumber}Could not fetch product info, retrying..");
                return false;
            }

            if (productInfoObj != null)
            {
                _productName = productInfoObj.Name;
                _taskNumber += $"[{productInfoObj.Name}] [{_size}] ";

                //fetching sizecode
                Parallel.ForEach(productInfoObj.SellableUnits, (sku) =>
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
                    }
                });
                //checks if size hasn't been found
                if (_sizeCode == null)
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{Program.timestamp()}{_taskNumber}Product/size not loaded! Retrying after delay..");
                    Thread.Sleep(_delay);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

    }
}
