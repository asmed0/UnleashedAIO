using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnleashedAIO.Modules
{
    class HawkCF
    {
        public async Task<CookieContainer> ChallengeInitialize(string productlink, string site, string original, string script, bool isCaptcha)
        {
            var cookies = new CookieContainer();
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://cf-v2.hwkapi.com");

            var payload = new Dictionary<string, dynamic>()
            {
                {"body",original},
                {"url", script},
                {"domain",site},
                {"captcha",isCaptcha},
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/cf-a/ov1/p1");
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("user-agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
            request.Headers.Add("content-type", "application/x-www-form-urlencoded");
            request.Headers.Add("accept", "*/*");
            request.Headers.Add("origin", site);
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("referer", productlink);
            request.Headers.Add("accept-language", "cs,en-US;q=0.9,en;q=0.8,en-GB;q=0.7,de;q=0.6");
            request.Headers.Add("cf-challenge", "");


            return cookies;
        }
    }
}
