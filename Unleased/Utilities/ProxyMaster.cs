


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UnleashedAIO
{
    public static class ProxyMaster
    {

        public static Dictionary<string, List<string>> proxyLists = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> usedLists = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> badLists = new Dictionary<string, List<string>>();

        public static Random rand = new Random();


        public static void setupProxyListsFromFolder(string filePath)
        {
           
            string[] fileNames = Directory.GetFiles(filePath);
            if (fileNames.Length > 0) {
                Parallel.ForEach(fileNames, (path) =>
                {
                    if (path.Contains(".txt"))
                    {
                        string[] stringarray = File.ReadAllLines(path);
                        List<string> proxylist = new List<string>(stringarray);
                        string name = Path.GetFileName(path).Replace(".txt", "");
                        setProxyList(name, proxylist);
                    }
                });
            }
            else
            {
                Program.ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{Program.timestamp()} YIKES! Looks like you Have no Proxies! You should Add Proxies to make your tasks start! Folder is set in Config! Idk, Watch the Video Please? If we made one...");
            }
        }

        public static void setProxyList(string name,List<string> proxyList)
        {
            proxyLists.Add(name, proxyList);
            usedLists.Add(name, new List<string>());
            badLists.Add(name, new List<string>());
        }

        public static string getProxy(string currentProxy, string listName,bool isBad)
        {
            List<string> proxyList = proxyLists[listName];
            List<string> usedProxies = usedLists[listName];
            List<string> badList = badLists[listName];

            if (currentProxy != null)
            {
                if (isBad)
                {
                    badLists[listName].Add(currentProxy);
                }
            }
            if ((proxyList.Count - badList.Count) > usedProxies.Count)
            {

                while (true)
                {
                    int proxyListLength = proxyList.Count;
                    int randomProxy = rand.Next(proxyListLength);
                    string theChosenOne = proxyLists[listName][randomProxy];
                    if (!badList.Contains(theChosenOne))
                    {
                        if (!usedLists[listName].Contains(theChosenOne))
                        {
                            usedLists[listName].Add(theChosenOne);
                            return theChosenOne;
                        }
                    }
                }
            }
            else
            {
                // Super Witty Means NO AVAILABLE PROXIES JOKES! NAP THREAD NAP!
                return "NAP";
            }
        }



    }

}