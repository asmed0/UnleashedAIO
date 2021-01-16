using AhmedBot.JunkyardSERaffle;
using DiscordRPC;
using golang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;
using UnleashedAIO.Assets;
using UnleashedAIO.Auth;
using UnleashedAIO.JSON;
using UnleashedAIO.Modules;

//  ██╗   ██╗███╗   ██╗██╗     ███████╗ █████╗ ███████╗██╗  ██╗███████╗██████╗ 
//  ██║   ██║████╗  ██║██║     ██╔════╝██╔══██╗██╔════╝██║  ██║██╔════╝██╔══██╗
//  ██║   ██║██╔██╗ ██║██║     █████╗  ███████║███████╗███████║█████╗  ██║  ██║
//  ██║   ██║██║╚██╗██║██║     ██╔══╝  ██╔══██║╚════██║██╔══██║██╔══╝  ██║  ██║
//  ╚██████╔╝██║ ╚████║███████╗███████╗██║  ██║███████║██║  ██║███████╗██████╔╝
//   ╚═════╝ ╚═╝  ╚═══╝╚══════╝╚══════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚══════╝╚═════╝ 
//                                                                             
//Developer: Ahmed Soliman
//If you've deobuscated this - please contact me and we can settle a deal! Discord:Ahmed#6969

namespace UnleashedAIO
{
    // ReSharper disable once InconsistentNaming
    class Program
    {
        //Diverse
        public static readonly string version = "0.1.0"; //version
        public static int checkoutCounter = 0; //successful checkouts
        public static int failedCounter = 0; //failed checkouts
        public static int delayBetweenTasks = 3;
        public static Random random = new Random(); //global random object

        // Task variables
        private static List<TinyCsvParser.Mapping.CsvMappingResult<Tasks>> Tasks = new List<CsvMappingResult<Tasks>>(); //local task object
        private static readonly string taskFolderPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\UnleashedAIO\\Tasks";
        private static readonly int taskCap = 1000;

        //Proxy list variable
        private static readonly string proxyFolderPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\UnleashedAIO\\Proxies";



        //RPC variables
        private static readonly string discordAppId = "741032897368162344"; // discord app id, from dev settings
        public static DiscordRpcClient Client { get; private set; } //rpc client

        public static string discordUsername; //value assigned when passed auth

        //Config variables
        public static configJson configObject = new configJson(); //our config object

        //timestamp function
        public static string timestamp()
        {
            return $"[{DateTime.Now}] ";
        }
        //CLI logger color
        public static void ChangeColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }


        //Start Rich Presence (discord)
        public static void RichPresence()
        {
            Client = new DiscordRpcClient(discordAppId); //Creates the client
            Client.Initialize();
            Client.SetPresence(new RichPresence()
            {
                Details = $"Version {version}",
                //State = "@Unleashed",
                Timestamps = DiscordRPC.Timestamps.Now,
                Assets = new DiscordRPC.Assets()
                {
                    LargeImageKey = "unleashed",
                    SmallImageKey = "smiley",
                    SmallImageText = "what's up"

                }
            });
        }


        //Config loading function
        public static void LoadConfig()
        {
            var configFile = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\UnleashedAIO\\config.txt");
            configObject = JsonConvert.DeserializeObject<configJson>(configFile);
            configObject.delayBetweenTasks *= 1000; // seconds to milliseconds
            delayBetweenTasks = Convert.ToInt32(configObject.delayBetweenTasks);
        }

        public static void LoadProxies()
        {
            string[] fileNames = Directory.GetFiles(proxyFolderPath);
            if (fileNames.Length > 0)
            {
                Program.ChangeColor(ConsoleColor.Cyan);
                Console.WriteLine($"\n{timestamp()}Found {fileNames.Length} proxy files");
                Program.ChangeColor(ConsoleColor.White);
                Console.WriteLine("------------------------------------------------");
                for (int i = 0; i < fileNames.Length; i++)
                {
                    Program.ChangeColor(ConsoleColor.Cyan);
                    Console.WriteLine($"{i}. [{fileNames[i].Split('\\')[6]}]");
                }
                Program.ChangeColor(ConsoleColor.White);
                Console.WriteLine("------------------------------------------------");
                Program.ChangeColor(ConsoleColor.Cyan);
                Console.Write($"Input selection: ");
                var selection = Convert.ToInt32(Console.ReadLine());
                if (selection < fileNames.Length)
                {
                    ProxyMaster.setProxyList(Tasks.Count, new List<string>(File.ReadAllLines(fileNames[selection])));
                }
                else
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{timestamp()}Oops, your selection was invalid! Please try again..");
                    LoadProxies();
                }
            }
        }

        public static void LoadTasks()
        {
            string[] fileNames = Directory.GetFiles(taskFolderPath);
            if (fileNames.Length > 0)
            {
                Program.ChangeColor(ConsoleColor.Cyan);
                Console.WriteLine($"\n{timestamp()}Found {fileNames.Length} task files");
                Program.ChangeColor(ConsoleColor.White);
                Console.WriteLine("------------------------------------------------");
                for (int i = 0; i < fileNames.Length; i++)
                {
                    Program.ChangeColor(ConsoleColor.Cyan);
                    Console.WriteLine($"{i}. [{fileNames[i].Split('\\')[6]}]");
                }
                Program.ChangeColor(ConsoleColor.White);
                Console.WriteLine("------------------------------------------------");
                Program.ChangeColor(ConsoleColor.Cyan);
                Console.Write($"Input selection: ");
                var selection = Convert.ToInt32(Console.ReadLine());
                if (selection < fileNames.Length)
                {
                    CsvParserOptions csvParserOptions = new CsvParserOptions(true, new QuotedStringTokenizer(','));
                    CsvTasksMapping csvMapper = new CsvTasksMapping();
                    CsvParser<Tasks> csvParser = new CsvParser<Tasks>(csvParserOptions, csvMapper);

                    ChangeColor(ConsoleColor.Cyan);
                    Tasks = csvParser
                        .ReadFromFile(fileNames[selection], Encoding.ASCII)
                        .ToList();
                }
                else
                {
                    Program.ChangeColor(ConsoleColor.Red);
                    Console.WriteLine($"{timestamp()}Oops, your selection was invalid! Please try again..");
                    LoadTasks();
                }
            }
        }

        static async Task Main(string[] args)
        {
            //DisableConsoleQuickEdit.Go();
            Console.Title = $"UnleashedAIO - for more info visit UnleashedAIO.com";
            LoadConfig();


            ChangeColor(ConsoleColor.Cyan);
            Console.WriteLine(art.logoart);
            Console.WriteLine($@"{timestamp()}Checking license...");

            if (configObject.license_key != "")
            {
                if (await LicenseGet.GetLicense(configObject.license_key))
                {
                    try
                    {
                        Console.Title = $"[{discordUsername}'s UnleashedAIO] | [Version {Program.version}] | [Checkouts: {Program.checkoutCounter} Success / {Program.failedCounter} failed]";

                        RichPresence();
                        LoadTasks();
                        LoadProxies();


                        ChangeColor(ConsoleColor.Yellow);
                        if (Tasks.Count > taskCap)
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine($"\n{timestamp()}Task limit crossed, please edit your task file so it only contains [{taskCap}] tasks");
                            Console.WriteLine($"{timestamp()}Please note that this task limit is only temporary until coming update!");
                            Environment.Exit(0);

                        }
                        ChangeColor(ConsoleColor.Cyan);
                        Console.WriteLine($"\n{timestamp()}Starting tasks..");


                        Parallel.ForEach(Tasks, (currentTask) =>
                        {
                            new Thread(() =>
                            {
                                TaskStarterAsync(currentTask.Result, currentTask.RowIndex, delayBetweenTasks);
                            }).Start();
                        });


                        //slow 1 by 1
                        // foreach(var currentTask in Tasks)
                        // {
                        //     new Thread(() =>
                        //    {
                        //         TaskStarterAsync(currentTask.Result, proxies, currentTask.RowIndex, delayBetweenTasks);
                        //     }).Start();
                        //  }

                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(e);
                    }
                }
            }
            else
            {
                ChangeColor(ConsoleColor.Red);
                Console.WriteLine($"{timestamp()}Please fill in your key in the config file and restart..");
            }

            Console.Read();

        }


        private static void TaskStarterAsync(Tasks currentTask, int taskNumber, int delay)
        {
            bool taskBool = false;
            switch (currentTask.Store.ToLower())
            {
                case "footlocker eu":
                    FootlockerEU FootlockerEUObj = new FootlockerEU();
                    if (FootlockerEUObj.StartTaskAsync(currentTask, $"Task [{taskNumber}] [{currentTask.Store.ToUpper()}] ", delay, taskNumber))
                    {
                        checkoutCounter++;
                        Console.Title = $"[{discordUsername}'s UnleashedAIO] | [Version {Program.version}] | [Checkouts: {Program.checkoutCounter}]";
                    }
                    else
                    {
                        failedCounter++;
                        Console.Title = $"[{discordUsername}'s UnleashedAIO] | [Version {Program.version}] | [Checkouts: {Program.checkoutCounter}]";
                    }

                    break;
                //case "zalando":
                //    try
                //    {
                //         taskBool = await Zalando.Start(currentTask, proxies[taskNumber-1], $"Task [{taskNumber}] [{currentTask.Store.ToUpper()}] ", delay);

                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e);
                //    }

                //    if (taskBool)
                //    {
                //        checkoutCounter++;
                //        Console.Title = $"[{discordUsername}'s UnleashedAIO] | [Version {Program.version}] | [Checkouts: {Program.checkoutCounter}]";
                //        await TaskStarterAsync(currentTask, proxies, taskNumber, delay);
                //    }
                //    else
                //    {
                //        checkoutCounter++;
                //        Console.Title = $"[{discordUsername}'s UnleashedAIO] | [Version {Program.version}] | [Checkouts: {Program.checkoutCounter}]";
                //        Thread.Sleep(random.Next(50000,100000));
                //        await TaskStarterAsync(currentTask, proxies, taskNumber, delay);
                //    }
                //    break;


                default:
                    Console.WriteLine($"{timestamp()}[Task {taskNumber}] Invalid store!");
                    break;
            }
        }

        public static async void WriteLog(string strFileName, string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(), strFileName), FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                objStreamWriter.WriteLine(strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }


}
