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
        static string _proxy;
        static string _csrf;
        static string _sessionId;
        static string _cartId;
        public static async Task<bool> Start(Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
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


            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", $"www.footlocker.{_region}")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            //.AddHeader("x-fl-request-id", "f2842a10-510b-11eb-af96-39eaf89e80b4")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/product/-/{_product}.html");

            //string postCart = tlsSolution.postRequest("https://www.footlocker.se/api/users/carts/current/entries", chain.headers, "{\"productQuantity\":1,\"productId\":\"SIZE_992410\"}", "resi.proxies.aycd.io:7777:customer-aycd107549plan1t1607150756635-cc-gb-sessid-00gwemtdx1zolaayadn5a:WCYDzRXMGcJ5qip");


            string getProductInfos = tlsSolution.getRequest($"https://www.footlocker.{_region}/api/products/pdp/{_product}", chain.headers, _proxy);
            //chain.collectCookies(getProductInfos).headers.Clear();

            FootlockerJSON.Root productInfoObj = JsonConvert.DeserializeObject<FootlockerJSON.Root>(JsonConvert.DeserializeObject<goTLSResponse>(getProductInfos).Body);

            if (productInfoObj != null)
            {
                _productName = productInfoObj.Name;

                //fetching sizecode
                foreach (var sku in productInfoObj.SellableUnits)
                {
                    if (sku.Attributes[0].Value == _size)
                    {
                        _sizeCode = sku.Attributes[0].Value;
                        taskNumber = taskNumber + $"[{productInfoObj.Name}] [{sku.Attributes[0].Value}] ";
                        //fetching timer
                        if (productInfoObj.VariantAttributes[0].DisplayCountDownTimer)
                        {
                            _releaseTimer = productInfoObj.VariantAttributes[0].CstSkuLaunchDate;
                        };

                        _price = productInfoObj.VariantAttributes[0].Price.OriginalPrice;

                        Console.WriteLine($"{Program.timestamp()}{taskNumber}Fetched product info, generating session..");
                    }
                }
                //checks if size hasn't been found
                if(_sizeCode == null)
                {
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Product/size not loaded! Retrying after delay..");
                    Thread.Sleep(delay);
                    await Start(currentTask, proxyString, taskNumber, delay);
                }

            }
            NewSession();
            Console.ReadLine();

            return true;
        }

        private static string[] NewSession()
        {
            string[] sessionVars = new string[10];

            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("authority", "www.footlocker.se")
            .AddHeader("pragma", "no-cache")
            .AddHeader("cache-control", "no-cache")
            .AddHeader("accept", "application/json")
            .AddHeader("x-api-lang", "en-GB")
            .AddHeader("accept-language", "en-GB,en;q=0.9")
            .AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
            .AddHeader("x-fl-request-id", "b0989990-5126-11eb-ade8-e3831dcc04a3")
            .AddHeader("sec-fetch-site", "same-origin")
            .AddHeader("sec-fetch-mode", "cors")
            .AddHeader("sec-fetch-dest", "empty")
            .AddHeader("referer", $"https://www.footlocker.{_region}/en/product/-/{_product}.html");

            string getSession = tlsSolution.getRequest($"https://api.ipify.org?format=json",chain.headers, _proxy);
            goTLSResponse sessionResponse = JsonConvert.DeserializeObject<goTLSResponse>(getSession);
            Console.WriteLine(sessionResponse.Body);

            return sessionVars;
        }
    }
}
