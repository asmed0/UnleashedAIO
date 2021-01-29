using System;
using System.Collections.Generic;
using System.Text;

namespace UnleashedAIO.JSON
{
    class CookieJsonHandler
    {

        public class UniqueCookie
        {
            public string ABCK { get; set; }
            public string Website { get; set; }
            public int Timestamp { get; set; }
            public List<string> OtherCookies  { get; set; }

        }

        public class ListCookies
        {
           public List<UniqueCookie> CookieList { get; set; }
        }

    }
}
