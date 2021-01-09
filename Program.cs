using AhmedBot.JunkyardSERaffle;
using DiscordRPC;
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
        public static string discordUsername; //value assigned when passed auth
        public static Random random = new Random(); //global random object
        public static int delayBetweenTasks = 3;

        // Task variables
        private static List<TinyCsvParser.Mapping.CsvMappingResult<Tasks>> Tasks = new List<CsvMappingResult<Tasks>>(); //local task object
        private static readonly string taskPath = "tasks.csv"; //path of our taskfile
        private static readonly int taskCap = 1000;

        //Proxy list variable
        private static string[] proxies; //local proxy object  
        private static readonly string proxyPath = "proxies.txt"; //path of our proxyfile


        //RPC variables
        private static readonly string discordAppId = "741032897368162344"; // discord app id, from dev settings
        public static DiscordRpcClient Client { get; private set; } //rpc client

        //Config variables
        private static configJson configObject = new configJson(); //our config object

        //timestamp function
        public static string timestamp()
        {
            return $"[{DateTime.Now}] ";
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

        //CLI logger color
        public static void ChangeColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        //Config loading function
        public static void LoadConfig()
        {
            var configFile = File.ReadAllText("config.txt");
            configObject = JsonConvert.DeserializeObject<configJson>(configFile);
            configObject.delayBetweenTasks = configObject.delayBetweenTasks * 1000; // seconds to milliseconds
            delayBetweenTasks = Convert.ToInt32(configObject.delayBetweenTasks);

        }

        //Tasks loading function
        public static void LoadTasks()
        {
            // Task parsing settings
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new QuotedStringTokenizer(','));
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvTasksMapping csvMapper = new CsvTasksMapping();
            CsvParser<Tasks> csvParser = new CsvParser<Tasks>(csvParserOptions, csvMapper);

            ChangeColor(ConsoleColor.Cyan);
            Console.WriteLine($"{timestamp()}Loading files: [{taskPath}] [{proxyPath}]");
            Tasks = csvParser
                .ReadFromFile(taskPath, Encoding.ASCII)
                .ToList();
        }
        public static void LoadProxies()
        {
            proxies = File.ReadAllLines("proxies.txt");
        }

        static async Task Main(string[] args)
        {
            //DisableConsoleQuickEdit.Go();
            Console.Title = $"UnleashedAIO - for more info visit UnleashedAIO.com";
            RichPresence();
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
                        LoadTasks();
                        LoadProxies();

                        ChangeColor(ConsoleColor.Yellow);
                        Console.WriteLine($"{timestamp()}Loaded [{Tasks.Count}] tasks & [{proxies.Length}] proxies");
                        if (Tasks.Count > taskCap)
                        {
                            ChangeColor(ConsoleColor.Red);
                            Console.WriteLine($"\n{timestamp()}Task limit crossed, please edit your task file so it only contains [{taskCap}] tasks");
                            Console.WriteLine($"{timestamp()}Please note that this task limit is only temporary until coming update!");
                            Environment.Exit(0);

                        }
                        ChangeColor(ConsoleColor.Cyan);
                        Console.WriteLine($"\n{timestamp()}Starting tasks..");


                        Parallel.ForEach(Tasks, async (currentTask) =>
                        {
                            await TaskStarterAsync(currentTask.Result, proxies, currentTask.RowIndex, delayBetweenTasks);
                        });

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


        private static async Task TaskStarterAsync(Tasks currentTask, string[] proxies, int taskNumber, int delay)
        {
            bool taskBool = false;
            switch (currentTask.Store.ToLower())
            {
                case "footlocker eu":
                    if (await FootlockerEU.StartTaskAsync(currentTask, proxies[taskNumber-1], $"Task [{taskNumber}] [{currentTask.Store.ToUpper()}] ", delay))
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
                case "zalando":
                    try
                    {
                         taskBool = await Zalando.Start(currentTask, proxies[taskNumber-1], $"Task [{taskNumber}] [{currentTask.Store.ToUpper()}] ", delay);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    if (taskBool)
                    {
                        checkoutCounter++;
                        Console.Title = $"[{discordUsername}'s UnleashedAIO] | [Version {Program.version}] | [Checkouts: {Program.checkoutCounter}]";
                        await TaskStarterAsync(currentTask, proxies, taskNumber, delay);
                    }
                    else
                    {
                        checkoutCounter++;
                        Console.Title = $"[{discordUsername}'s UnleashedAIO] | [Version {Program.version}] | [Checkouts: {Program.checkoutCounter}]";
                        Thread.Sleep(random.Next(50000,100000));
                        await TaskStarterAsync(currentTask, proxies, taskNumber, delay);
                    }
                    break;
                        

                default:
                    Console.WriteLine($"{timestamp()}[Task {taskNumber}] Invalid store!");
                    break;
            }
        }
    }
}
