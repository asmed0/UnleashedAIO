using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UnleashedAIO.Modules
{
    class OffSpring
    {
        public static async Task<bool> Start()
        {
            HttpClient client = new HttpClient();

            await client.GetAsync("https://www.offspring.co.uk/");
            return true;
        }
    }
}
