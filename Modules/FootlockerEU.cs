using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Pluralsight.Crypto;
using UnleashedAIO.JSON;

namespace UnleashedAIO.Modules
{
    class FootlockerEU
    {

        private static string xcsrftoken;
        public static CookieContainer cookies = new CookieContainer();
        private static string productName = "product name here";
        private static X509Certificate2 cert;

        public static async Task<bool> Start(Tasks currentTask, string proxyString, string taskNumber, int delay)
        {

            //Proxy parsing
            string[] spearator = {":"};
            int count = 4;
            string[] proxyObj = proxyString.Split(spearator, count,
                StringSplitOptions.RemoveEmptyEntries);

            //Proxy login
            IWebProxy proxy = new WebProxy(proxyObj[0], Int32.Parse(proxyObj[1]))
            {
                Credentials = new NetworkCredential(proxyObj[2], proxyObj[3])
            };

            var handler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseCookies = true,
                ClientCertificateOptions = ClientCertificateOption.Automatic,

            };
            handler.CookieContainer = cookies;

            var client = new HttpClient(handler);

            //specify to use TLS 1.2 as default connection
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            client.BaseAddress = new Uri($"https://footlocker.{currentTask.Country}");
            var request = new HttpRequestMessage(HttpMethod.Get, "api/session");
            request.Headers.Add("authority", $"www.footlocker.{currentTask.Country}");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("upgrade-insecure-requests", "1");
            //request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36");
            request.Headers.Add("accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("sec-fetch-mode", "navigate");
            request.Headers.Add("sec-fetch-user", "?1");
            request.Headers.Add("sec-fetch-dest", "document");
            request.Headers.Add("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Connection", "keep-alive");

            Program.ChangeColor(ConsoleColor.Yellow);
            Console.WriteLine($"{Program.timestamp()}{taskNumber}Generating session");
            try
            {
                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Session session =
                        JsonConvert.DeserializeObject<Session>(await response.Content.ReadAsStringAsync());
                    client.DefaultRequestHeaders.Add("x-csrf-token", session.data.csrfToken);
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine(
                        $"{Program.timestamp()}{taskNumber}Failed generating session [{response.StatusCode}], retrying..");
                    Thread.Sleep(Program.random.Next(delay / 2, delay * 2));
                    await Start(currentTask, proxyString, taskNumber, delay);
                }
            }
            catch (Exception e) //sometimes the requests ends prematurely, retry
            {
                // Exception ignored - will send to webhook in a later process
                //Console.WriteLine(e);
                await Start(currentTask, proxyString, taskNumber, delay);
            }

            Program.ChangeColor(ConsoleColor.Green);
            Console.WriteLine($"{Program.timestamp()}{taskNumber}Successfully generated session, checking stock..");
            string SKU = await GetSKU(client, currentTask, taskNumber, delay);
            if (SKU != "")
            {
                if (!await ATC(client, currentTask, taskNumber, delay, SKU))
                {
                    return false;
                }
                else
                {
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine(
                        $"{Program.timestamp()}{taskNumber}{productName}Successfully added to cart, submitting shipping");
                    Thread.Sleep(delay);
                }
            }

            return true;
        }

        private static async Task<string> GetSKU(HttpClient client, Tasks currentTask,
            string taskNumber, int delay)
        {
            string SKU = "";
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/products/pdp/{currentTask.SKU}");

            var response = await client.SendAsync(request);
            FootlockerJson myDeserializedClass = null;
            try
            {
                myDeserializedClass = JsonConvert.DeserializeObject<FootlockerJson>(await response.Content.ReadAsStringAsync());

            }
            catch (Exception )
            {
                //ignore
            }

            if (myDeserializedClass != null)
            {
                foreach (var sku in myDeserializedClass.sellableUnits)
                {
                    if (sku.attributes[0].value == currentTask.Size)
                    {
                        if (sku.stockLevelStatus.ToLower() != "instock")
                        {
                            Program.ChangeColor(ConsoleColor.Yellow);
                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Size oos! Retrying in {delay}");
                            Thread.Sleep(delay);
                            await GetSKU(client, currentTask, taskNumber, delay);
                        }
                        else
                        {
                            SKU = sku.attributes[0].id;
                            productName = $"[{myDeserializedClass.name}] [{sku.attributes[0].value}] ";
                            Program.ChangeColor(ConsoleColor.Yellow);
                            Console.WriteLine($"{Program.timestamp()}{taskNumber}{productName}Size in stock, adding to cart..");
                            return SKU;
                        }
                    }
                    /*else //this doesn't let you add higher than size[0]
                    {
                        Program.ChangeColor(ConsoleColor.Yellow);
                        Console.WriteLine(sku.stockLevelStatus);
                        Console.WriteLine(sku.attributes[0].value);
                        Console.WriteLine($"{Program.timestamp()}{taskNumber}Product loaded without desired size! Retrying in {delay}");
                        Thread.Sleep(delay);
                        await GetSKU(client, currentTask, taskNumber, delay);
                    }*/
                }

            }
            else
            {
                Program.ChangeColor(ConsoleColor.Yellow);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Product is not loaded or all sizes oos! Retrying in {delay}");
                Thread.Sleep(delay);
                await GetSKU(client, currentTask, taskNumber, delay);
            }

            return SKU;
        }

        private static async Task<bool> ATC(HttpClient client,
            Tasks currentTask, string taskNumber, int delay, string SKU)
        {
            ;
            Uri rqUri = new Uri("https://www.footlocker.se/api/users/carts/current/entries");
            // body
            ATC content = new ATC();
            content.productId = SKU;
            content.productQuantity = 1;
            var json = JsonConvert.SerializeObject(content);
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            //request init
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri,
                Content = jsonContent
            };

            //request headers
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("accept-language", "en-US,en;q=0.9,");
            request.Headers.Add("x-fl-productid", SKU);

            //request execution
            var response = await client.SendAsync(request);
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(rqUri);
            foreach (var cookie in responseCookies)
            {
                if (cookie.Name == "datadome")
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}{productName}Failed adding to cart, proxy banned..");
                    Console.WriteLine(cookie.Name);
                    return false; ; //if we get hit with DD captcha - we return false
                }

                if ((await response.Content.ReadAsStringAsync()).ToString().Contains("The product you are trying to view is no longer available."))
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}{productName}Failed adding to cart, product is not live yet! Retrying in {delay}..");
                    Thread.Sleep(delay);
                    await ATC(client, currentTask, taskNumber, delay, SKU);
                }

            }
            return true;
        }

        private static async Task<bool> SubmitShipping(HttpClient client,
            Tasks currentTask, string taskNumber, int delay, string SKU)
        {
            Uri rqUri = new Uri("https://www.footlocker.se/api/users/carts/current/set-billing");

            //countryname used in body
            string countryName = "Sweden";
            switch (currentTask.Country.ToLower())
            {
                case "se":
                    countryName = "Sweden";
                    break;
                case "no":
                    countryName = "Norway";
                    break;
                case "at":
                    countryName = "Austria";
                    break;
            }

            //body
            SubmitShipping content = new SubmitShipping();
            content.setAsDefaultBilling = true;
            content.setAsDefaultShipping = true;
            content.firstName = currentTask.FirstName;
            content.lastName = currentTask.LastName;
            content.email = false;
            content.phone = currentTask.PhoneNumber;
            content.country.isocode = currentTask.Country;
            content.country.name = countryName;
            content.id = null;
            content.setAsBilling = true;
            content.type = default;
            content.line1 = currentTask.Adress;
            content.postalCode = currentTask.ZipCode;
            content.town = currentTask.City;
            content.shippingAddress = true;

            //content serialization
            var json = JsonConvert.SerializeObject(content);
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            //request init
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri,
                Content = jsonContent
            };

            //request execution
            var response = await client.SendAsync(request);
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(rqUri);

            return true;
        }
    }
}
