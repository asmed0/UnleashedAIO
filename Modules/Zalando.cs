using Newtonsoft.Json;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnleashedAIO.JSON;

namespace UnleashedAIO.Modules
{
    class Zalando
    {
        //cookiestrings
        private static string _abck = "";
        private static string clientId = "";
        private static string frsx = "";
        private static string mpulseinject = "";
        private static string _fbp = "";
        private static string _ga = "";
        private static string _gcl_au = "";
        private static string _gid = "";
        private static string ak_bmsc = "";
        private static string bm_sz = "";
        private static string bm_sv = "";
        private static string ncx = "";
        private static string _gat_zalga = "";
        private static string sqt_cap = "";
        private static string butItAgainControl = "";
        private static string zac = "";
        private static string email = "";
        private static int shippingId = 0;
        private static string sessionId = "";
        private static string checkoutId = "";
        private static string eTag = "";

        private static Browser browser;
        private static CookieContainer cookies = new CookieContainer();
        public static async Task<bool> Start(Tasks currentTask, string proxyString, string taskNumber, int delay)
        {

            //Proxy parsing
            string[] spearator = { ":" };
            int count = 4;
            string[] proxyObj = proxyString.Split(spearator, count,
                StringSplitOptions.RemoveEmptyEntries);

            /*if (!File.Exists(ChromiumPath))
            {
                Program.ChangeColor(ConsoleColor.Yellow);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Downloading dependencies...");
                var bfetcher = await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
                ChromiumPath = bfetcher.ExecutablePath;
            }*/

            //local chromium
            var bfetcher = await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            var puppeteerExtra = new PuppeteerExtra();
            puppeteerExtra.Use(new StealthPlugin());
            browser = await Puppeteer.LaunchAsync(new LaunchOptions
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
            await page.GoToAsync($"https://www.zalando.{currentTask.Country}");
            await ABCK(page, currentTask, proxyString, taskNumber, delay);
            var cookieResponse = await page.GetCookiesAsync($"https://www.zalando.{currentTask.Country}");
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
                    case "bm_sv":
                        bm_sv = cookieval.Value;
                        continue;
                }
            }
            //await browser.DisposeAsync(); // we do not want the browser hanging in the bg


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
                AllowAutoRedirect = false,


            };

            handler.CookieContainer = cookies;
            var client = new HttpClient(handler);

            //we nest switch cases
            Thread.Sleep(delay);
            switch (await Register(client, currentTask, proxyString, taskNumber, delay))
            {
                case true:
                    Program.ChangeColor(ConsoleColor.Green);
                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Signed into [{email}]..");
                    Thread.Sleep(delay);
                    //after login comes add to cart
                    switch (await ATC(client, currentTask, proxyString, taskNumber, delay))
                    {
                        case true:
                            Program.ChangeColor(ConsoleColor.Green);
                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Added [{currentTask.SKU}] to cart..");
                            switch (await SetShipping(client, currentTask, proxyString, taskNumber, delay))
                            {
                                case true:
                                    Program.ChangeColor(ConsoleColor.Green);
                                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Set shipping");
                                    Thread.Sleep(delay);
                                    //after ATC comes checkout session initialization

                                    switch (await SetDefaultShipping(client, currentTask, proxyString, taskNumber, delay))
                                    {
                                        case true:
                                            Program.ChangeColor(ConsoleColor.Green);
                                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Checkout session initialized");
                                            Thread.Sleep(delay);
                                            switch (await GetNextStep(client, currentTask, proxyString, taskNumber, delay))
                                            {
                                                case true:
                                                    switch (await SetCreditCard(client, currentTask, proxyString, taskNumber, delay))
                                                    {
                                                        case true:
                                                            Program.ChangeColor(ConsoleColor.Green);
                                                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Set CreditCard payment method");
                                                            Thread.Sleep(delay);
                                                            switch (await GetCheckoutId(client, currentTask, proxyString, taskNumber, delay))
                                                            {
                                                                case true:
                                                                    Program.ChangeColor(ConsoleColor.Green);
                                                                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Checkout session ready");
                                                                    Thread.Sleep(delay);
                                                                    switch (await Checkout(client, currentTask, proxyString, taskNumber, delay))
                                                                    {
                                                                        case true:
                                                                            Program.ChangeColor(ConsoleColor.Green);
                                                                            Console.WriteLine($"{Program.timestamp()}{taskNumber}Succesfully checked out, sent to webhook!");
                                                                            return true;
                                                                        case false:
                                                                            return false;
                                                                    }
                                                                case false:
                                                                    Program.ChangeColor(ConsoleColor.Red);
                                                                    Console.WriteLine($"{Program.timestamp()}{taskNumber}Failed getting checkout tokens, restarting task..");
                                                                    return false;
                                                            }
                                                        case false:
                                                            return false;
                                                    }
                                                case false:
                                                    return false;
                                            }
                                        case false:
                                            return false;
                                    }
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

        public static async Task<bool> Checkout(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/api/checkout/buy-now");
            //content builder
            ZalandoJson.CheckoutInfo content = new ZalandoJson.CheckoutInfo();
            content.checkoutId = checkoutId;
            content.eTag = $"\"{eTag}\"";
            var json = JsonConvert.SerializeObject(content);
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            //request init
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri,
                Content = jsonContent
            };

            request.Headers.Add("authority", "www.zalando.se");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("x-zalando-footer-mode", "desktop");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("x-zalando-header-mode", "desktop");
            request.Headers.Add("x-xsrf-token", frsx);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            request.Headers.Add("x-zalando-checkout-app", "web");
            request.Headers.Add("origin", $"https://www.zalando.{currentTask.Country}");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("referer", $"https://www.zalando.{currentTask.Country}/checkout/confirm");
            request.Headers.Add("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.Headers.Add("cookie", $"Zalando-Client-Id={clientId}; zac={zac}; _abck={_abck}; bm_sv={bm_sv}; frsx={frsx};");

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (responseContent.Contains("error") || response.StatusCode == HttpStatusCode.Forbidden)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.RequestMessage);
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Checkout failed, restarting task..");
                return false;
            }
            return true;
        }
        public static async Task<bool> GetCheckoutId(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {

            Console.WriteLine("getting tokens");
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/checkout/confirm");
            //request init
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = rqUri,
            };

            //request headers
            request.Headers.Add("authority", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            request.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.Headers.Add("sec-fetch-site", "cross-site");
            request.Headers.Add("origin", $"www.zalando.{currentTask.Country}");
            request.Headers.Add("sec-fetch-mode", "navigate");
            request.Headers.Add("sec-fetch-dest", "document");
            request.Headers.Add("referer", $"https://checkout.payment.zalando.com/");
            request.Headers.Add("accept-language", "en-US,en;q=0.9,sv;q=0.8");
            request.Headers.Add("cookie", $"mpulseinject={mpulseinject}; _fbp={_fbp};" +
                                          $" _ga={_ga}; _gcl_au={_gcl_au}; _gid={_gid}; ak_bmsc={ak_bmsc};" +
                                          $"bm_sz={bm_sz}; ncx={ncx}; use-delivery-location-service=true;" +
                                          $" Zalando-Client-Id={clientId}; _ga={_ga}; sqt_cap={sqt_cap}; _gat_zalga={_gat_zalga}; " +
                                          $"zac={zac}; frsx={frsx}; {butItAgainControl}=buy-it-again-control; bm_sv={bm_sv}; _abck={_abck}; ");

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (responseContent != "")
                {
                    checkoutId = await getBetween(responseContent, "checkoutId&quot;:&quot;", "&quot;,&quot;");
                }

                if (responseContent != "")
                {
                    eTag = await getBetween(responseContent, ";eTag&quot;:&quot;\\&quot;", "\\&quot;&quot;,");

                }

                //rather be sure xD
                if (checkoutId != "" && eTag != "")
                {
                    IEnumerable<Cookie> cooks = cookies.GetCookies(rqUri);
                    foreach (var cookie in cooks)
                    {
                        switch (cookie.Name)
                        {
                            case "zac":
                                zac = cookie.Value;
                                continue;
                            case "_abck":
                                _abck = cookie.Value;
                                continue;
                            case "bm_sv":
                                bm_sv = cookie.Value;
                                continue;
                            case "Zalando-Client-Id":
                                clientId = cookie.Value;
                                continue;
                            case "frsx":
                                frsx = cookie.Value;
                                continue;
                            case "mpulseinject":
                                mpulseinject = cookie.Value;
                                continue;
                            case "_fbp":
                                _fbp = cookie.Value;
                                continue;
                            case "_ga":
                                _ga = cookie.Value;
                                continue;
                            case "_gcl_au":
                                _gcl_au = cookie.Value;
                                continue;
                            case "_gid":
                                _gid = cookie.Value;
                                continue;
                            case "ak_bmsc":
                                ak_bmsc = cookie.Value;
                                continue;
                            case "bm_sz":
                                bm_sz = cookie.Value;
                                continue;
                            case "ncx":
                                ncx = cookie.Value;
                                continue;
                            case "_gat_zalga":
                                _gat_zalga = cookie.Value;
                                continue;
                            case "sqt_cap":
                                sqt_cap = cookie.Value;
                                continue;
                        }

                        if (cookie.Value == "buy-it-again-control")
                        {
                            butItAgainControl = cookie.Name;
                        }
                    }

                    Console.WriteLine("etag:" + eTag);
                    Console.WriteLine("checkoutid:" + eTag);

                    return true;
                }
                Console.WriteLine(responseContent);

                return false;
            }
            Console.WriteLine(responseContent);

            return false;
        }

        public static async Task<bool> SetCreditCard(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Thread.Sleep(4000);
            //content builder
            var json = "{\"card_holder\":\"Ahmed  Soliman\",\"pan\":\"5273 4695 2949 9036 \",\"cvv\":\"671\",\"expiry_month\":\"12\",\"expiry_year\":\"2024\",\"options\":{\"selected\":[\"store_for_reuse\"],\"not_selected\":[]}}";
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            Uri rqUri = new Uri($"https://card-entry-service.zalando-payments.com/contexts/checkout/cards");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri,
                Content = jsonContent
            };
            request.Headers.Add("authority", "card-entry-service.zalando-payments.com");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("authorization", "Bearer CiAgICAgIHsidHlwIjoiSldUIiwiYWxnIjoiRVMzODQiLCJraWQiOiIyMDE5MDczMCJ9CiAgICA.ewogICJpc3MiIDogInRva2VuLmFwaS56cGF5LmNvbSIsCiAgInN1YiIgOiAiZjllZmExOTA5OTBiNmYxNzE2NGQ0MTVhM2EzMmNiOWQ0NmQzNDhiMzg4NDcwYmI2OGQ5ZjNhNzJlZWViZTU2ZSIsCiAgImF1ZCIgOiAiaHR0cHM6Ly9wdXJjaGFzZS1zZXJ2aWNlLnBheW1lbnRzLWNvcmUuemFsYW4uZG8vc2Vzc2lvbnMvMDVhYTkxY2ItMDNjZC00YWQ3LTlmNDAtNTQzNTk5MTI3MmVlIiwKICAiYXpwIiA6ICIwNWFhOTFjYi0wM2NkLTRhZDctOWY0MC01NDM1OTkxMjcyZWUiLAogICJleHAiIDogMTYwNTU3Mzk3MywKICAiaWF0IiA6IDE2MDU0ODc1NzMKfQ.iUs2zO4dDWyOHjdCI1lSb91O2y6bbHvxJvRH8w0If5q8q5eH6hnykJXSfSdUbSB8g6aanLt6WFI8Rh0I88qHhJTf2X4kLSAH3uEPW2eda-4vE3Lb6cqiQ54_22UdYTrl");
            request.Headers.Add("accept-language", "sv_SE");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            request.Headers.Add("accept", "*/*");
            request.Headers.Add("origin", "https://card-entry-service.zalando-payments.com");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("referer", "https://card-entry-service.zalando-payments.com/static/pci-sdk-js/0.2.11/hosted-store.html");
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK) //&& response2.StatusCode != HttpStatusCode.OK && response3.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.RequestMessage);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                Console.WriteLine(response.StatusCode);
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Error while setting payment, restarting task..");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return false;
            }

            var iframeSourceId = JsonConvert.DeserializeObject<JSON.ZalandoJson.CreditCardJson>(await response.Content.ReadAsStringAsync()).id;
            //content builder
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("payz_selected_payment_method", "CREDIT_CARD");
            content.Add("payz_credit_card_former_payment_method_id", "-1");
            content.Add("iframe_funding_source_id", iframeSourceId);
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(content);

            Uri rqUri2 = new Uri($"https://checkout.payment.zalando.com/payment-method-selection-session/{sessionId}/selection");
            var request2 = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri2,
                Content = formUrlEncodedContent
            };
            request2.Headers.Add("Connection", "keep-alive");
            request2.Headers.Add("Pragma", "no-cache");
            request2.Headers.Add("Cache-Control", "no-cache");
            request2.Headers.Add("Upgrade-Insecure-Requests", "1");
            request2.Headers.Add("Origin", "https://checkout.payment.zalando.com");
            request2.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            request2.Headers.Add("Sec-Fetch-Site", "same-origin");
            request2.Headers.Add("Sec-Fetch-Mode", "navigate");
            request2.Headers.Add("Sec-Fetch-User", "?1");
            request2.Headers.Add("Sec-Fetch-Dest", "document");
            request2.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request2.Headers.Add("Referer", "https://checkout.payment.zalando.com/selection");
            request2.Headers.Add("Accept-Language", "en-US,en;q=0.9,sv;q=0.8");
            request2.Headers.Add("cookie", $"Session-ID={sessionId}; mpulseinject={mpulseinject}; _fbp={_fbp};" +
                                           $" _ga={_ga}; _gcl_au={_gcl_au}; _gid={_gid}; ak_bmsc={ak_bmsc};" +
                                           $"bm_sz={bm_sz}; ncx={ncx}; use-delivery-location-service=true;" +
                                           $" Zalando-Client-Id={clientId}; _ga={_ga}; sqt_cap={sqt_cap}; _gat_zalga={_gat_zalga}; " +
                                           $"zac={zac}; frsx={frsx}; {butItAgainControl}=buy-it-again-control; bm_sv={bm_sv}; _abck={_abck}; ");

            var response2 = await client.SendAsync(request2);

            var page = await browser.NewPageAsync();
            IEnumerable<Cookie> cooks = cookies.GetCookies(new Uri($"https://www.zalando.se"));
            await page.GoToAsync($"https://www.zalando.{currentTask.Country}/checkout/confirm");
            foreach (var cookie in cooks)
            {
                await page.SetCookieAsync(new CookieParam
                {
                    Name = cookie.Name,
                    Value = cookie.Value

                });
            }
                await page.GoToAsync($"https://www.zalando.{currentTask.Country}/checkout/confirm");
/*
            if (response2.StatusCode != HttpStatusCode.OK) //&& response2.StatusCode != HttpStatusCode.OK && response3.StatusCode != HttpStatusCode.OK)
            {

                Console.WriteLine(response2.StatusCode);
                Console.WriteLine(response2.RequestMessage);
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Error while setting payment, restarting task..");
                return false;
            }

            Console.WriteLine(response2.StatusCode);
            Console.WriteLine(response2.RequestMessage);*/
                return true;
        }

        public static async Task<bool> SetInvoice(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            //content builder
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("payz_selected_payment_method", "INVOICE");
            content.Add("payz_invoice_social_security_number", currentTask.PhoneNumber);
            content.Add("iframe_funding_source_id", "");
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(content);

            Uri rqUri = new Uri($"https://checkout.payment.zalando.com/payment-method-selection-session/{sessionId}/selection");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri,
                Content = formUrlEncodedContent
            };
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Pragma", "no-cache");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("Origin", "https://checkout.payment.zalando.com");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-User", "?1");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.Headers.Add("Referer", "https://checkout.payment.zalando.com/selection");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9,sv;q=0.8");
            request.Headers.Add("cookie", $"Session-ID={sessionId}");
            /*
            Uri rqUri2 = new Uri($"https://checkout.payment.zalando.com/selection");
            var request2 = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = rqUri2,
                Content = formUrlEncodedContent
            };
            request2.Headers.Add("Connection", "keep-alive");
            request2.Headers.Add("Pragma", "no-cache");
            request2.Headers.Add("Cache-Control", "no-cache");
            request2.Headers.Add("Upgrade-Insecure-Requests", "1");
            request2.Headers.Add("Origin", "https://checkout.payment.zalando.com");
            request2.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            request2.Headers.Add("Sec-Fetch-Site", "same-origin");
            request2.Headers.Add("Sec-Fetch-Mode", "navigate");
            request2.Headers.Add("Sec-Fetch-User", "?1");
            request2.Headers.Add("Sec-Fetch-Dest", "document");
            request2.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request2.Headers.Add("Referer", "https://checkout.payment.zalando.com/selection");
            request2.Headers.Add("Accept-Language", "en-US,en;q=0.9,sv;q=0.8");
            request2.Headers.Add("cookie",$"Session-ID={sessionId}");

            Uri rqUri3 = new Uri($"https://www.zalando.se/checkout/payment-complete");
            var request3 = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = rqUri3,
                Content = formUrlEncodedContent
            };
            request3.Headers.Add("Connection", "keep-alive");
            request3.Headers.Add("Pragma", "no-cache");
            request3.Headers.Add("Cache-Control", "no-cache");
            request3.Headers.Add("Upgrade-Insecure-Requests", "1");
            request3.Headers.Add("Origin", "https://checkout.payment.zalando.com");
            request3.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            request3.Headers.Add("Sec-Fetch-Site", "same-origin");
            request3.Headers.Add("Sec-Fetch-Mode", "navigate");
            request3.Headers.Add("Sec-Fetch-User", "?1");
            request3.Headers.Add("Sec-Fetch-Dest", "document");
            request3.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request3.Headers.Add("Referer", "https://checkout.payment.zalando.com/selection");
            request3.Headers.Add("Accept-Language", "en-US,en;q=0.9,sv;q=0.8");
            request3.Headers.Add("cookie", $"Session-ID={sessionId}");
            var response = await client.SendAsync(request);
            var response2 = await client.SendAsync(request2);
            var response3 = await client.SendAsync(request3);
            */
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK) //&& response2.StatusCode != HttpStatusCode.OK && response3.StatusCode != HttpStatusCode.OK)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Error while setting payment, restarting task..");
                return false;
            }

            return true;
        }

        public static async Task<bool> GetNextStep(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/api/checkout/next-step");
            //request init
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = rqUri,
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
            request.Headers.Add("referer", $"https://m.zalando.{currentTask.Country}/myaccount/addresses/");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!responseContent.Contains("url"))
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Error while setting payment, restarting task..");
                return false;
            }
            IEnumerable<Cookie> cooks = cookies.GetCookies(rqUri);
            foreach (var cookie in cooks)
            {
                if (cookie.Name == "zac")
                {
                    zac = cookie.Value;
                }
                if (cookie.Name == "_abck")
                {
                    _abck = cookie.Value;
                }
                if (cookie.Name == "Zalando-Client-Id")
                {
                    clientId = cookie.Value;
                }
                if (cookie.Name == "frsx")
                {
                    frsx = cookie.Value;
                }
            }
            sessionId = await getBetween(responseContent, "selection-session/", "/selection");

            return true;
        }
        public static async Task<bool> SetDefaultShipping(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/api/checkout/address/{shippingId}/default");
            //content build
            ZalandoJson.SetDefaultShip content = new ZalandoJson.SetDefaultShip();
            content.isDefaultShipping = true;
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
            if (!responseContent.Contains(shippingId.ToString()))
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Error while initiating checkout, restarting task..");
                return false;
            }

            return true;
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
            string cleanContent = Regex.Replace(responseContent, "[\\[\\]]", "");
            ZalandoJson.ShippingResponse deserializeObject = JsonConvert.DeserializeObject<ZalandoJson.ShippingResponse>(cleanContent);

            if (deserializeObject.firstname == null || !deserializeObject.firstname.Contains(currentTask.FirstName))
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Error while setting shipping, restarting task..");
                return false;
            }

            shippingId = deserializeObject.id;
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

        public static async Task<bool> Register(HttpClient client, Tasks currentTask, string proxyString, string taskNumber, int delay)
        {
            Uri rqUri = new Uri($"https://www.zalando.{currentTask.Country}/api/reef/register");
            //content builder
            ZalandoJson.NewCustomerData newCustomerData = new ZalandoJson.NewCustomerData();
            newCustomerData.firstname = currentTask.FirstName;
            newCustomerData.lastname = currentTask.LastName;
            email = $"{currentTask.FirstName}{RandomString(5)}{currentTask.LastName}@{currentTask.email}"; // we want to save the email in a variable
            newCustomerData.email = email;
            newCustomerData.password = "P@ssword123haha";
            newCustomerData.fashion_preference = new List<string>();
            newCustomerData.fashion_preference.Add("mens");
            newCustomerData.subscribe_to_news_letter = true;
            newCustomerData.accepts_terms_and_conditions = true;
            ZalandoJson.Register content = new ZalandoJson.Register();
            content.newCustomerData = newCustomerData;
            content.wnaMode = "shop";
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
            request.Headers.Add("referer", $"https://www.zalando.{currentTask.Country}/login?target=/myaccount/");
            request.Headers.Add("x-zalando-request-uri", "/login?target=/myaccount/");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");

            //request execution
            var response = await client.SendAsync(request);
            var responsecontent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.Created)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Register error, restarting task..");
                Thread.Sleep(5000);
                return false;
            }
            IEnumerable<Cookie> cooks = cookies.GetCookies(rqUri);
            foreach (var cookie in cooks)
            {
                if (cookie.Name == "zac")
                {
                    zac = cookie.Value;
                }
                if (cookie.Name == "_abck")
                {
                    _abck = cookie.Value;
                }
                if (cookie.Name == "Zalando-Client-Id")
                {
                    clientId = cookie.Value;
                }
                if (cookie.Name == "frsx")
                {
                    frsx = cookie.Value;
                }
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
            var middleX = rnd.Next(0, 1920);
            var middleY = rnd.Next(0, 1080);
            var finalX = rnd.Next(0, 1920);
            var finalY = rnd.Next(0, 1080);

            try
            {
                await page.Mouse.ClickAsync(startingX, startingY);
                await page.Mouse.MoveAsync(startingX, startingY);
                await page.Mouse.MoveAsync(middleX, middleY);
                await page.Mouse.ClickAsync(middleX, middleY);
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
        //A helper function that we use to get the checkout tokens
        public static async Task<string> getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Program.random.Next(s.Length)]).ToArray());
        }
    }
}
