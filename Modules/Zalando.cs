using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using RestSharp;
using UnleashedAIO;
using UnleashedAIO.JSON;

namespace UnleashedAIO.Modules
{
    class Zalando
    {
        //cookiestrings
        private static string _abck = "";
        private static string clientId = "";
        private static string frsx = "";

        public static CookieContainer cookies = new CookieContainer();

        public static async Task<bool> Start(Tasks currentTask, string proxyString, string taskNumber, int delay)
        {

            //Proxy parsing
            string[] spearator = {":"};
            int count = 4;
            string[] proxyObj = proxyString.Split(spearator, count,
                StringSplitOptions.RemoveEmptyEntries);

            //local chromium
            var bfetcher = await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            
            var puppeteerExtra = new PuppeteerExtra();
            puppeteerExtra.Use(new StealthPlugin());
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = false,
                    ExecutablePath = bfetcher.ExecutablePath,
                    Args = new[] { $"--proxy-server={proxyObj[0]}:{proxyObj[1]}" },
                    DefaultViewport = new ViewPortOptions
                    {
                        Width = 1920,
                        Height = 1080
                    }
                });

                Program.ChangeColor(ConsoleColor.Yellow);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Generating session..");

                var page = await browser.NewPageAsync();
                if (proxyObj[2] != "")
                {
                    await page.AuthenticateAsync(new Credentials { Username = proxyObj[2], Password = proxyObj[3] });
                }

                await page.EvaluateFunctionOnNewDocumentAsync("delete navigator.__proto__.webdriver");
                await page.SetUserAgentAsync(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
                page.DefaultTimeout = -1;
                await page.GoToAsync($"https://www.zalando.{currentTask.Country}/faq");
                await ABCK(page, currentTask, proxyString, taskNumber, delay);
                var cookieResponse = await page.GetCookiesAsync($"https://www.zalando.{currentTask.Country}/faq");
                foreach (var cookieval in cookieResponse)
                {
                    switch (cookieval.Name) //Sorting out relevant cookies
                    {
                        case "_abck":
                            _abck = cookieval.Value;
                            continue;
                        case "Zalando-Client-Id":
                            clientId = cookieval.Value;
                            continue;
                        case "frsx":
                            frsx = cookieval.Value;
                            continue;
                    }
                }
                await browser.DisposeAsync(); // we do not want the browser hanging in the bg


                if (_abck == "" && clientId == "" && frsx == "")
            {
                Program.ChangeColor(ConsoleColor.Green);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Failed generating session, retrying..");
                //await Start(currentTask, proxyString, taskNumber, delay);
                return false; //handle retries through taskstarter instead..
            }

            //Initializing the client that leads us through the rest.
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
            //from here everything is intialized and happens inside functions instead
            //we nest switch cases
            Thread.Sleep(delay);
            switch (await Login(client, currentTask, proxyString, taskNumber, delay))
            {
                case true:
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Logged into [{currentTask.email}]..");
                    Thread.Sleep(delay);
                    //after login comes add to cart
                    switch (await ATC(client, currentTask,proxyString,taskNumber,delay))
                    {
                        case true:
                            Program.ChangeColor(ConsoleColor.Green);
                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Added [{currentTask.SKU}] to cart..");
                            switch (await SetShipping(client, currentTask, proxyString, taskNumber, delay))
                            {
                                case true:
                                    Program.ChangeColor(ConsoleColor.Green);
                                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Set shipping");
                                    return true;
                                case false:
                                    return false;
                            }
                        case false:
                            return false;
                    }
                case false:
                    return false;

            }
        }

        public static async Task<bool> SetShipping(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/api/user-account-address/addresses");
            //content builder
            ZalandoJson.Shipping content = new ZalandoJson.Shipping();
            content.type = "HomeAddress";
            content.city = currentTask.City;
            content.countryCode = currentTask.Country;
            content.firstname = currentTask.FirstName;
            content.lastname = currentTask.LastName;
            content.street = currentTask.Adress;
            content.additional = currentTask.Adress2;
            content.gender = "MALE";
            content.defaultBilling = true;
            content.defaultShipping = true;
            content.zip = currentTask.ZipCode;
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
            //request headers
            request.Headers.Add("cookie", $"frsx={frsx}; Zalando-Client-Id={clientId};_abck={_abck}");
            request.Headers.Add("authority", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("x-xsrf-token", frsx);
            request.Headers.Add("Zalando-Client-Id", clientId);
            request.Headers.Add("x-zalando-client-id", clientId);
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("origin", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.Headers.Add("viewport-width", "1107");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("referer", $"https://m.zalando.{currentTask.Country}/myaccount/addresses/");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            
            //request execution
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!responseContent.Contains("HomeAddress") && !responseContent.Contains(currentTask.FirstName))
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Error while setting shipping, restarting task..");
                return false;
            }
            return true;
        }

        public static async Task<bool> ATC(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/api/graphql/");
            //content builder
            ZalandoJson.ATC.AddToCartInput addTocartInput = new ZalandoJson.ATC.AddToCartInput();
            addTocartInput.productId = currentTask.SKU;
            addTocartInput.clientMutationId = "addToCartMutation"; // graphql action
            ZalandoJson.ATC.Variables variables = new ZalandoJson.ATC.Variables();
            variables.addToCartInput = addTocartInput; 
            ZalandoJson.ATC.Array array = new ZalandoJson.ATC.Array();
            array.id = "e7f9dfd05f6b992d05ec8d79803ce6a6bcfb0a10972d4d9731c6b94f6ec75033"; //graphql id
            array.variables = variables;

            var json = JsonConvert.SerializeObject(array);
            json = $"[{json}]"; //instead of really making an array i wrap in brackets
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            //request init
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri,
                Content = jsonContent
            };
            //request headers
            request.Headers.Add("cookie", $"frsx={frsx}; Zalando-Client-Id={clientId};_abck={_abck}");
            request.Headers.Add("authority", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("x-xsrf-token", frsx);
            request.Headers.Add("Zalando-Client-Id", clientId);
            request.Headers.Add("x-zalando-client-id", clientId);
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("origin", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.Headers.Add("viewport-width", "1107");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            //request execution
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!responseContent.Contains("AddToCartPayload"))
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}ATC error, restarting task..");
                return false;
            }

            return true;
        }

        public static async Task<bool> Login(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/api/reef/login");
            //content builder
            ZalandoJson.Login content = new ZalandoJson.Login();
            content.username = currentTask.email;
            content.password = currentTask.PhoneNumber;
            content.wnaMode = "modal";
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
            request.Headers.Add("cookie", $"frsx={frsx}; Zalando-Client-Id={clientId};_abck={_abck}");
            request.Headers.Add("authority", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("x-xsrf-token", frsx);
            request.Headers.Add("x-zalando-client-id", clientId);
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("origin", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.Headers.Add("viewport-width", "1107");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("x-zalando-toggle-label", "THE_LABEL_IS_ENABLED");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");

            //request execution
            var response = await client.SendAsync(request);
            IEnumerable<Cookie> cooks = cookies.GetCookies(rqUri);
            var responsecontent = await response.Content.ReadAsStringAsync();
            if (!responsecontent.Contains(currentTask.email))
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Login error, restarting task..");
                return false;
            }

            return true;
        }

        public static async Task ABCK(Page page, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Program.ChangeColor(ConsoleColor.DarkCyan);
            Console.WriteLine($"{Program.timestamp()}{taskNumber}Solving akamai..");
            var rnd = new Random();
            var startingX = rnd.Next(0, 1920);
            var startingY = rnd.Next(0, 1080);
            var finalX = rnd.Next(0, 1920);
            var finalY = rnd.Next(0, 1080);

            try
            {
                await page.Mouse.MoveAsync(startingX, startingY);
                await page.Mouse.MoveAsync(finalX, finalY);
                await page.Mouse.ClickAsync(finalX, finalY);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Failed solving akamai, retrying..");
                await ABCK(page, currentTask, proxyString, taskNumber, delay);
            }
        }
    }
}
