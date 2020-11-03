using System.Collections.Generic;

namespace UnleashedAIO.JSON
{
    class authJson
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Discord
        {
            public string id { get; set; }
            public string username { get; set; }
            public string discriminator { get; set; }
            public string avatar { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string tag { get; set; }
        }

        public class License
        {
            public string email { get; set; }
            public string key { get; set; }
            public long created { get; set; }
            public string member { get; set; }
            public object customer { get; set; }
            public object subscription { get; set; }
            public object payment_method { get; set; }
            public string plan { get; set; }
            public string id { get; set; }
        }

        public class Member
        {
            public string email { get; set; }
            public Discord discord { get; set; }
            public long created { get; set; }
            public License license { get; set; }
            public string id { get; set; }
        }

        public class Plan
        {
            public bool active { get; set; }
            public string product { get; set; }
            public string price { get; set; }
            public string name { get; set; }
            public bool allow_unbinding { get; set; }
            public bool kick_on_unbind { get; set; }
            public int amount { get; set; }
            public long created { get; set; }
            public string currency { get; set; }
            public List<string> roles { get; set; }
            public object recurring { get; set; }
            public string type { get; set; }
            public string id { get; set; }
        }

        public class User
        {
            public string email { get; set; }
            public string key { get; set; }
            public long created { get; set; }
            public Member member { get; set; }
            public object customer { get; set; }
            public object subscription { get; set; }
            public object payment_method { get; set; }
            public Plan plan { get; set; }
            public string id { get; set; }
        }


    }
}
