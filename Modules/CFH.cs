using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using golang;
using Newtonsoft.Json;
using RestSharp;
using UnleashedAIO.JSON;

namespace UnleashedAIO.Modules
{
    class CFH
    {

        //TaskVariables
        private string consoleStart;
        private int taskNumber;
        private int delay;

        private string website;

        private string keyStrUriSafe;

        //Determines which mode to run; -- "Safe","Fast","Restock" each system will run differently!
        private string mode;

        private string cp;
        private string[] proxySplit;


        private tlsSolution tlsClient = new tlsSolution();

        public CFH(int taskNumber,string website)
        {
            this.taskNumber = taskNumber;
            this.cp = ProxyMaster.getProxy(taskNumber);
            this.proxySplit = cp.Split(":");
            this.website = website;


            
           

        }

        public async void AreWeWinners()
        {
            string c = await GetABC();
            Program.ChangeColor(ConsoleColor.Green);
            Console.WriteLine($"{Program.timestamp()}" + c);



        }

        public async Task<string> GetABC()
        {
           Program.ChangeColor(ConsoleColor.Yellow);
           Console.WriteLine($"{Program.timestamp()} generating Cookie");
            // string proxysend = $"{proxySplit[2]}:{proxySplit[3]}@{proxySplit[0]}:{proxySplit[1]}";
            // var client = new RestClient($"http://localhost:5000/getcfc/{website}/{proxysend}");
            //  client.Timeout = -1;
            //  var request = new RestRequest(Method.GET);
            //  IRestResponse response = client.Execute(request);z

            tlsSolution.methodChain chain = new tlsSolution.methodChain();
            chain.AddHeader("sec-ch-ua-mobile", "?0")
                .AddHeader("upgrade-insecure-requests", "1")
                .AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36")
                .AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
                .AddHeader("sec-fetch-site", "none")
                .AddHeader("sec-fetch-mode", "navigate")
                .AddHeader("sec-fetch-user", "?1")
                .AddHeader("sec-fetch-dest", "document")
                .AddHeader("accept-encoding", "gzip, deflate, br")
                .AddHeader("accept-language", "en-CA,en-GB;q=0.9,en-US;q=0.8,en;q=0.7");

            string getProductInfos = tlsClient.getRequest($"https://www.off---white.com", chain.headers, cp);
             try
            {
                chain.collectCookies(getProductInfos).headers.Clear();

            }
            catch (Exception)
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}{taskNumber}Failed setting headers, retrying..");
            }
            Program.ChangeColor(ConsoleColor.Yellow);
            Console.WriteLine($"{Program.timestamp()}" + JsonConvert.DeserializeObject<goTLSResponse>(getProductInfos).Body);

            return "Holy shit i did it";
        }

        private HttpClient createHttpClient()
        {
            IWebProxy proxy = new WebProxy(proxySplit[0], Int32.Parse(proxySplit[1]))
            {
                Credentials = new NetworkCredential(proxySplit[2], proxySplit[3])
            };
            return new HttpClient(createClientHandler(proxy));
        }

        private HttpClientHandler createClientHandler(IWebProxy proxy)
        {
            return new HttpClientHandler()
            {
                Proxy = proxy,
            };
        }

        private Boolean checkResponseForCaptcha()
        {
            return false;
        }



    }
}
