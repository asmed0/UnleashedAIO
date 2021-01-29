


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

        public static int split = 1;
        public static double taskDivider = 1;
       
        public static string getProxy(int taskNumber)
        {
            return getProxy(null, taskNumber, false);
        }

        public static void setProxyList(int totalTasks,List<string> proxyList)
        {
                    
            if (totalTasks > 10 && proxyList.Count > 10)
            {
                if (totalTasks > 40)
                {
                    if (totalTasks > 100)
                    {
                        split = 10;
                    }
                    else
                    {
                        split = 4;
                    }
                }
                else
                {
                    split = 2;
                }
                double div = totalTasks / split;
                taskDivider = Math.Floor(div) ;
            }
            else
            {
                split = 1;
            }

            if(split == 1)
            {
                addProxyList("1", proxyList);
            }
            else
            {

                int count = proxyList.Count / split;
                for (int x = 1; x <= split; ++x)
                {

                    List<string> newList;
                    if(x == split)
                    {
                        newList = new List<string>(proxyList);
                    }
                    else
                    {
                        newList = new List<string>();
                      
                        for (int r = 0; r < count; ++r)
                        {
                            newList.Add(proxyList[0]);
                            proxyList.RemoveAt(0);
                        }
                    }
                    addProxyList(x.ToString(), newList);
                }
            }

        }

        public static string getProxy(string currentProxy, int taskNumber,bool isBad)
        {
            string listName = "1";
            if(split > 1)
            {
                double floatAnswer = taskNumber / taskDivider;
                double listNameInt = Math.Ceiling(floatAnswer);
                if(listNameInt > split)
                {
                    listNameInt = split;
                }
                listName = listNameInt.ToString();

            }
            List<string> proxyList = proxyLists[listName];
            List<string> usedProxies = usedLists[listName];
            List<string> badList = badLists[listName];

            if(proxyList.Count.Equals(1) && taskNumber.Equals(1))
            {
                return proxyList[0];
            }
            else if(proxyList.Count == 1)
            {
                return "NAP";
            }

            if (currentProxy != null)
            {
                if (isBad)
                {
                    badLists[listName].Add(currentProxy);
                }
                usedLists[listName].Remove(currentProxy);
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

        private static void addProxyList(string name, List<string> list)
        {
            proxyLists.Add(name, list);
            usedLists.Add(name, new List<string>());
            badLists.Add(name, new List<string>());
        }

    }
}