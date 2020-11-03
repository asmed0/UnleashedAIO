using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using UnleashedAIO.JSON;

namespace UnleashedAIO.Auth
{
    class LicenseGet
    {
        private static readonly string apiKey = "BCT9DYS-ZXR4A9M-JKNEDTN-CCVR5GE";
        public static bool GetLicense(string license) //mode = 0 for first auth, mode = 1 for continous auth to detect duplicate instances
        {

            var client = new RestClient($"https://api.metalabs.io/v2/licenses/{license}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Basic {apiKey}");
            IRestResponse response = client.Execute(request);


            if (response.StatusCode == HttpStatusCode.OK)
            {
                authJson.User parsedResponse = JsonConvert.DeserializeObject<authJson.User>(response.Content.ToString());
                Program.ChangeColor(ConsoleColor.Green);
                Program.discordUsername = parsedResponse.member.discord.username;
                Console.WriteLine($"{Program.timestamp()}License valid! Welcome back, {parsedResponse.member.discord.username}");
                Program.ChangeColor(ConsoleColor.Magenta);
                Console.WriteLine($"{Program.timestamp()}License type: {parsedResponse.plan.name}");
                return true;
            }
            else
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()}[{license}] is not a valid license, or it is already active on another machine!!");
                Program.ChangeColor(ConsoleColor.Cyan);
                Console.Write($"{Program.timestamp()}Please input a valid license:");
                license = Console.ReadLine();
                GetLicense(license);
            }

            return true;
        }
    }
}
