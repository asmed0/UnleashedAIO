using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace UnleashedAIO.Modules
{
    class Captcha
    {
        public static string GeetestInitialize(string currentTaskInfo,string apiKey, string gt, string challenge, string pageurl)
        {
            string requestId = null;
            Uri url = new Uri($"https://2captcha.com/in.php?key={apiKey}&method=geetest&gt={gt}&challenge={challenge}&api_server=api-na.geetest.com&pageurl={pageurl}");

            var client = new RestClient();
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);

            var parseResponse = JsonConvert.DeserializeObject<captchaJson>(response.Content);

            switch (parseResponse.request)
            {
                case "ERROR_KEY_DOES_NOT_EXIST":
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine(
                        $"{Program.timestamp()}{currentTaskInfo}2Captcha returned error [{parseResponse.request}] - please make sure your apikey is valid!");
                    break;
                default:
                    Program.ChangeColor(ConsoleColor.Yellow);
                    Console.WriteLine(
                        $"{Program.timestamp()}{currentTaskInfo}Successfully requested captcha token from 2captcha - ID: {parseResponse.request}");
                    requestId = parseResponse.request;
                    break;
            }

            return requestId;
        }

        public static string GeetestGet(string currentTaskInfo, string apiKey, string requestId)
        {
            string[] challengeSolved = null;
            Uri url = new Uri($"https://2captcha.com/res.php?key={apiKey}&action=get&id={requestId}");

            var client = new RestClient();
            client.Timeout = -1;

            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var parseResponse = JsonConvert.DeserializeObject<captchaJson>(response.Content);

            switch (parseResponse.status)
            {
                case 1:
                    Program.ChangeColor(ConsoleColor.Cyan);
                    Console.WriteLine($"{Program.timestamp()}{currentTaskInfo}Captcha ready, solving datadome..");
                    break;
                default:
                    Program.ChangeColor(ConsoleColor.DarkYellow);
                    Console.WriteLine($"{Program.timestamp()}{currentTaskInfo}Captcha not ready, retrying..");
                    Thread.Sleep(5000);
                    GeetestGet(currentTaskInfo, apiKey, requestId);
                    break;
            }

            return "";

        }
    }
}
