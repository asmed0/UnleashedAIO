using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using UnleashedAIO.JSON;

namespace UnleashedAIO.Unleased.Utilities
{
    class AkamaiCookieGen
    {

        private CookieJsonHandler.UniqueCookie cookieHandler;
        
        private Random rnd;

        private RevisionInfo bfetcher;

        private string cookiesFilePath;

        private string region;
        private string store;

        private string targetWebsite;

        private int taskNumber;

        private string currentProxy;
        private string[] proxySplit;

        private Browser browser;

        public AkamaiCookieGen(int taskNumber, string store, string region, Random random)
        {
            this.rnd = random;
            this.taskNumber = taskNumber;
            this.region = region;
            this.store = store;

            if(store == "nike")
            {
                targetWebsite = $"https://www.nike.com/{region}/launch";
            }
            else if (store == "zalando")
            {
                if (isZalandoRegion(region))
                {
                    if (regionHoldsExtraMod(region))
                    {
                        targetWebsite = $"www.zalando.co.{region}";
                    }
                    else
                    {
                        targetWebsite = $"www.zalando.{region}";
                    }
                }
            }
            
            currentProxy = ProxyMaster.getProxy(taskNumber);

            if (currentProxy != "LocalHost")
            {
                proxySplit = currentProxy.Split(":");
            }

           
        }

        public async Task<string> GetCookies()
        {
            this.cookieHandler = new CookieJsonHandler.UniqueCookie();
            cookieHandler.OtherCookies = new List<string>();

            Program.ChangeColor(ConsoleColor.Yellow);
            Console.WriteLine($"{Program.timestamp()}{taskNumber} {store} Gathering Cookies! ");
            bfetcher = await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            var puppeteerExtra = new PuppeteerExtra();
            puppeteerExtra.Use(new StealthPlugin());        

            browser = await Puppeteer.LaunchAsync(getLaunchOptions());

            

            var page = await browser.NewPageAsync();
            if (proxySplit != null && proxySplit[2] != "")
            {
                await page.AuthenticateAsync(new Credentials { Username = proxySplit[2], Password = proxySplit[3] });
            }
            await page.EvaluateFunctionOnNewDocumentAsync("delete navigator.__proto__.webdriver");
            await page.SetUserAgentAsync(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
            page.DefaultTimeout = -1;
            await page.GoToAsync(targetWebsite);
            await handleBP(page);
            var cookieResponse = await page.GetCookiesAsync(targetWebsite);

            foreach (var cookieval in cookieResponse)
            {
                if(cookieval.Name.Equals("_abck"))
                {
                    cookieHandler.ABCK = cookieval.Name + "=" + cookieval.Value;
                }
                else if(cookieval.Name.Equals("bm_sz"))
                {
                    addCookieToHandler(cookieval);
                }
                else if (store.Equals("nike"))
                {
                    if (cookieval.Name.Equals("ak_bmsc") || cookieval.Name.Equals("AKA_A2") || cookieval.Name.Equals("geoloc") || cookieval.Name.Equals("anonymousId"))
                    {
                        addCookieToHandler(cookieval);
                    }
                }
                else if (store.Equals("zalando"))
                {
                    if (cookieval.Name.Equals("Zalando-Client-Id") || cookieval.Name.Equals("frsx"))
                    {
                        addCookieToHandler(cookieval);
                    }
                }
            }

            cookieHandler.Website = targetWebsite;

            CookieMaster.saveCookiesToJson(store, cookieHandler);

            await page.DeleteCookieAsync();

            await page.CloseAsync();

            await browser.CloseAsync();

            GetCookies();

            return "GOT EM";
        }

        public void addCookieToHandler(CookieParam cookieval)
        {
            cookieHandler.OtherCookies.Add(cookieval.Name + "=" + cookieval.Value);
        }

        public LaunchOptions getLaunchOptions()
        {
            if (currentProxy == "LocalHost")
            {
                return new LaunchOptions
                {
                    Headless = false,
                    ExecutablePath = bfetcher.ExecutablePath,
                    Args = new[] { $"--proxy-server={proxySplit[0]}:{proxySplit[1]}" },
                    DefaultViewport = new ViewPortOptions
                    {
                        Width = 1920,
                        Height = 1080
                    }
                };
            }
            else
            {
                return new LaunchOptions
                {
                    Headless = false,
                    ExecutablePath = bfetcher.ExecutablePath,
                    DefaultViewport = new ViewPortOptions
                    {
                        Width = 1920,
                        Height = 1080
                    }
                };
            }
        }

        public async Task handleBP(Page page)
        {
            Program.ChangeColor(ConsoleColor.DarkCyan);
            Console.WriteLine($"{Program.timestamp()}{taskNumber} Solving Bot Protection!");

            List<KeyValuePair<decimal, decimal>> points = new List<KeyValuePair<decimal, decimal>>();
            
            for(int i = 1; i <= 6; ++i)
            {
                points.Add(KeyValuePair.Create((decimal)rnd.Next(1920), (decimal)rnd.Next(1080)));
            }
            try
            {
                foreach (KeyValuePair<decimal, decimal> pairs in points)
                {
                    await page.Mouse.ClickAsync(pairs.Key, pairs.Value);
                    await page.Mouse.MoveAsync(pairs.Key, pairs.Value);

                    Thread.Sleep(rnd.Next(300, 1000));
                }
            }
            catch (Exception e)
            {
                //Send Exception to Data Collection
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Failed Solving Bot Protection, Retrying..");
                await handleBP(page);

            }
         }


        public bool regionHoldsExtraMod(string region)
        {
            return region == "uk";
        }

        public bool isZalandoRegion(string region)
        {
            return region == "de" || region == "it" || region == "at" 
                || region == "ch" || region == "nl" || region == "es" || region == "se" 
                || region == "no" || region == "uk" || region == "dk" || region == "pl" 
                || region == "be" || region == "fi" || region == "cz" || region == "ie";
        }

    }
}
