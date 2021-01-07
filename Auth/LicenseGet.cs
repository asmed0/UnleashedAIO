using Discord.Webhook;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnleashedAIO.JSON;
using UnleashedAIO.Modules;

namespace UnleashedAIO.Auth
{
    class LicenseGet
    {
        private static readonly string apiKey = "BCT9DYS-ZXR4A9M-JKNEDTN-CCVR5GE";
        private static readonly DiscordWebhookClient webhook = new DiscordWebhookClient("https://discordapp.com/api/webhooks/775837711609233419/mnL18O4wzuD7EBl-Biq8zuW2a-AzCeNFM1NSwadlpkNfCfZX4pVDojXuAxPMJSJZecTT");
        private static readonly DiscordWebhookClient loginHook = new DiscordWebhookClient("https://discordapp.com/api/webhooks/775844576808271893/UBR9NHzwjhFRedCWk8N90Nqa8-eHVazIqFp9z4pa_yJxaXbbMd9nOvEvpnA-Tk0fIFzc");
        //[Obfuscation(Feature = "virtualization", Exclude = false)]
        public static async Task<bool>

            GetLicense(string license) //mode = 0 for first auth, mode = 1 for continous auth to detect duplicate instances
        {
            using (var web = new WebClient())
            {
                web.Proxy = new WebProxy();
                try
                {
                    var ip = web.DownloadString("http://ipv4.icanhazip.com/");
                    var fiddler = web.DownloadString("http://localhost:8888");
                    if (fiddler.Contains("Fiddler"))
                    {
                        await webhook.SendMessageAsync($"**⚠️User has opened Fiddler\n⚠️Key: {license}\n⚠️IP: {ip}**");
                        Program.ChangeColor(ConsoleColor.Red);
                        Console.WriteLine(@"Security module has been activated. Your key and IP have been logged.");
                        Thread.Sleep(2000);
                        Environment.Exit(1);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            using (var webClient = new WebClient())
            {
                webClient.Proxy = new WebProxy();
                var fiddler = Process.GetProcessesByName("Fiddler");
                var ip = webClient.DownloadString("http://ipv4.icanhazip.com/");
                if (fiddler.Length > 0)
                {
                    await webhook.SendMessageAsync($"**⚠️User has opened Fiddler\n⚠️Key: {license}\n⚠️IP: {ip}**");
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Security module has been activated. Your key and IP have been logged.");
                    Thread.Sleep(2000);
                    Environment.Exit(1);
                }

                var dnspy = Process.GetProcessesByName("dnSpy");
                if (dnspy.Length > 0)
                {
                    await webhook.SendMessageAsync($"**⚠️User has opened dnSpy\n⚠️Key: {license}\n⚠️IP: {ip}**");
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Security module has been activated. Your key and IP have been logged.");
                    Thread.Sleep(2000);
                    Environment.Exit(1);
                }
                var procmon = Process.GetProcessesByName("procmon");
                if (procmon.Length > 0)
                {
                    await webhook.SendMessageAsync($"**⚠️User has opened ProcMon\n⚠️Key: {license}\n⚠️IP: {ip}**");
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"Security module has been activated. Your key and IP have been logged.");
                    Thread.Sleep(2000);
                    Environment.Exit(1);
                }

                var dnspy86 = Process.GetProcessesByName("dnSpy-x86");
                if (dnspy86.Length > 0)
                {
                    await webhook.SendMessageAsync($"**⚠️User has opened dnSpy\n⚠️Key: {license}\n⚠️IP: {ip}**");
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine("Security module has been activated. Your key and IP have been logged.");
                    Thread.Sleep(2000);
                    Environment.Exit(1);
                }
            }
            var mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            var mbsList = mbs.Get();
            var id = "";
            foreach (var o in mbsList)
            {
                var mo = (ManagementObject)o;
                id = mo["ProcessorID"].ToString();
            }
            var mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            var moc = mos.Get();
            var motherBoard = "";
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                motherBoard = (string)mo["SerialNumber"];
            }
            var hwId = id + motherBoard;

            var client = new RestClient($"https://api.metalabs.io/v2/licenses/{license}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Basic {apiKey}");
            IRestResponse response = client.Execute(request);


            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    authJson.User parsedResponse =
                        JsonConvert.DeserializeObject<authJson.User>(response.Content.ToString());
                    Program.ChangeColor(ConsoleColor.Green);
                    Program.discordUsername = parsedResponse.member.discord.username;
                    await loginHook.SendMessageAsync($"✅User logged in\n✅Key: {license}\n✅HwId: {hwId}");
                    Console.WriteLine($"{Program.timestamp()}License valid! Welcome back, {parsedResponse.member.discord.username}");
                    Program.ChangeColor(ConsoleColor.Magenta);
                    Console.WriteLine($"{Program.timestamp()}License type: {parsedResponse.plan.name}");
                }
                catch (Exception e)
                {
                    await GetLicense(license);
                }
            }
            else
            {
                Program.ChangeColor(ConsoleColor.Red);
                await loginHook.SendMessageAsync($"❌User failed login\n❌Key: {license}\n❌HwId: {hwId}");
                Console.WriteLine(
                    $"{Program.timestamp()}[{license}] is not a valid license, or it is already active on another machine!!");
                Program.ChangeColor(ConsoleColor.Cyan);
                Console.Write($"{Program.timestamp()}Please input a valid license: ");
                license = Console.ReadLine();
                await GetLicense(license);
            }
            return true;
        }
    }
}
