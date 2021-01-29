using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using static UnleashedAIO.JSON.CookieJsonHandler;

namespace UnleashedAIO.Unleased.Utilities
{
    class CookieMaster
    {

        
        private static object lockObject = new Object();

        private static List<Dictionary<string, ListCookies>> cookieLists = new List<Dictionary<string, ListCookies>>();
        private static List<Dictionary<string, ListCookies>> usedCookies = new List<Dictionary<string, ListCookies>>();

        public static bool saveCookiesToJson(string store, UniqueCookie obj)
        {
            ListCookies json = new ListCookies();
            string directory = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}/UnleashedAIO/Cookies/";
            string path = $"{directory}{store}.json";


            lock (lockObject)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(directory);  
                }

                if (File.Exists(path))
                {
                    try
                    {
                        string jsonFile;
                        using (var reader = new StreamReader(path))
                        {
                            jsonFile = reader.ReadToEnd();
                        }

                        json = JsonConvert.DeserializeObject<ListCookies>(jsonFile);

                    }catch(Exception e)
                    {
                        json = new ListCookies();
                    }
                }
                else
                {
                    File.Create(path);
                    json = new ListCookies();
                }
                if (json.CookieList == null)
                {
                    json.CookieList = new List<UniqueCookie>();
                }

                json.CookieList.Add(obj);
                try
                {
                    string jsons = JsonConvert.SerializeObject(json, Formatting.Indented);

                    using (var writer = new StreamWriter(path))
                    {
                        writer.Write(jsons);
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
           
            return true;
        }
        
        public static UniqueCookie getCookieFromStore(string store)
        {
 



        }

        public static void loadCookies()
        {

            try
            {

            }catch(Exception e)
            {

            }

        }


    }
}
