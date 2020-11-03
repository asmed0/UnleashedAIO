using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnleashedAIO.JSON;

namespace UnleashedAIO.Modules
{
    class FootlockerEU
    {

        WebhookSend webhookSend = new WebhookSend();
        private static string xcsrftoken;
        public static CookieContainer cookies = new CookieContainer();


        public static async Task<bool> Start(Tasks currentTask, string proxyString, string taskNumber, int delay)
        {

            //Proxy parsing
            string[] spearator = { ":" };
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
            request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36");
            request.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
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
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Failed generating session [{response.StatusCode}], retrying..");
                    Thread.Sleep(Program.random.Next(delay / 2, delay * 2));
                    await Start(currentTask, proxyString, taskNumber, delay);
                }
            }
            catch (Exception) //sometimes the requests ends prematurely, retry
            {
                // we just catch exception but ignore it - will send to webhook in a later process
                await Start(currentTask, proxyString, taskNumber, delay);
            }
            Program.ChangeColor(ConsoleColor.Green);
            Console.WriteLine($"{Program.timestamp()}{taskNumber}Successfully generated session, checking stock..");
            string SKU = await GetSKU(client, currentTask, taskNumber, delay);
            if (SKU != "")
            {
                if (await ATC(client, currentTask, taskNumber, delay, SKU))
                {
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Successfully added to cart, submitting shipping");
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
            FootlockerJson myDeserializedClass = JsonConvert.DeserializeObject<FootlockerJson>(await response.Content.ReadAsStringAsync());
            if (myDeserializedClass != null)
            {
                foreach (var sku in myDeserializedClass.sellableUnits)
                {
                    if (sku.attributes[0].value == currentTask.Size)
                    {
                        if (sku.stockLevelStatus != "inStock")
                        {
                            Program.ChangeColor(ConsoleColor.Yellow);
                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Size oos! Retrying in {delay}");
                            Thread.Sleep(delay);
                            await GetSKU(client, currentTask, taskNumber, delay);
                        }
                        else
                        {
                            Program.ChangeColor(ConsoleColor.Yellow);
                            SKU = sku.attributes[0].id;
                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Size [{sku.attributes[0].value}] in stock, adding to cart..");
                        }
                    }
                    else
                    {
                        Program.ChangeColor(ConsoleColor.Yellow);
                        Console.WriteLine($"{Program.timestamp()}{taskNumber}Product loaded without desired size! Retrying in {delay}");
                        Thread.Sleep(delay);
                        await GetSKU(client, currentTask, taskNumber, delay);
                    }
                }

            }
            else
            {
                Program.ChangeColor(ConsoleColor.Yellow);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Product is not live or all sizes oos! Retrying in {delay}");
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
            request.Headers.Add("x-fl-productid", SKU);

            //request execution
            await client.SendAsync(request);
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(rqUri);
            foreach (var cookie in responseCookies)
            {
                if (cookie.Name == "datadome")
                {
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Failed adding to cart, proxy banned..");
                    return false; ; //if we get hit with DD captcha - we return false
                }

            }
            return true;
        }

        private static async Task<bool> SubmitShipping(HttpClient client,
            Tasks currentTask, string taskNumber, int delay, string SKU)
        {
            Uri rqUri = new Uri("https://www.footlocker.se/api/users/carts/current/set-billing");

            return true;
        }
    }
}
