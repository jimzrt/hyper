using hyper.commands;
using hyper.config;
using hyper.Database.DAO;
using hyper.Helper;
using hyper.Input;
using hyper.Inputs;
using hyper.Output;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using ZWave;
using ZWave.BasicApplication.Devices;
using ZWave.CommandClasses;
using ZWave.Enums;

namespace hyper
{
    internal class Program
    {

        private static void SetupInputs()
        {
            var configuration = new LoggingConfiguration();

            var tcpTarget = new TCPInput(5432)
            {
                Layout = @"${longdate} ${uppercase:${level}} ${message}"
            };
            var consoleTarget = new ConsoleInput()
            {
                Layout = @"${longdate} ${uppercase:${level}} ${message}"
            };

            var fileTarget = new FileTarget()
            {
                Name = "FileTarget",
                Layout = @"${longdate} ${uppercase:${level}} ${message}",
                AutoFlush = true,
                FileName = "${basedir}/logs/log.${shortdate}.txt",
                ArchiveFileName = "${basedir}/logs/archives/log.{#####}.zip",
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ConcurrentWrites = true,
                ArchiveEvery = FileArchivePeriod.Day,
                EnableArchiveFileCompression = true,
                MaxArchiveFiles = 14,
                OptimizeBufferReuse = true,
                CreateDirs = true
            };



            configuration.AddTarget(fileTarget);
            configuration.AddTarget(consoleTarget);
            configuration.AddTarget(tcpTarget);

            configuration.AddRuleForAllLevels(fileTarget);
            configuration.AddRuleForAllLevels(consoleTarget);
            configuration.AddRuleForAllLevels(tcpTarget);

            LogManager.Configuration = configuration;


            InputManager.AddInput(consoleTarget);
            InputManager.AddInput(tcpTarget);
            var udpInput = new UDPInput(54322);
            InputManager.AddInput(udpInput);
        }

        private static void SetupOutputs()
        {

            var udpOutput = new UDPOutput("127.0.0.1", 54321);
            var databaseOutput = new DatabaseOutput("events.db");
            OutputManager.AddOutput(udpOutput);
            OutputManager.AddOutput(databaseOutput);
        }



        private static void Main(string[] args)
        {


            SetupInputs();
            SetupOutputs();



            //  Target.Register("MyFirst", typeof(MyNamespace.MyFirstTarget)); //OR, dynamic










            ICommand currentCommand = null;






            Common.logger.Debug("==== ZWave Hyper Hyper 5000 ====");
            Common.logger.Debug("-----------------------------------");
            Common.logger.Debug("Loading device configuration database...");
            if (!File.Exists("config.yaml"))
            {
                Common.logger.Error("configuration file config.yaml does not exist!");
                return;
            }
            var config = Common.ParseConfig("config.yaml");
            if (config == null)
            {
                Common.logger.Error("Could not parse configuration file config.yaml!");
                return;
            }
            Common.logger.Info("Got configuration for " + config.Count + " devices.");
            Common.logger.Debug("-----------------------------------");

            if (args.Length < 2)
            {
                Common.logger.Info("usage:");
                Common.logger.Info("./hyper [serialPort] [command]");
                Common.logger.Info("valid commands:");
                Common.logger.Info("r/replace, c/config, i/include, e/exclude, l/listen, p/ping");
                return;
            }

            var port = args[0];
            Common.logger.Info("Initialize Serialport: {0}", port);
            var initController = Common.InitController(port, out Controller controller, out string errorMessage);
            if (!initController)
            {
                Common.logger.Error("Error connecting with port {0}! Error Mesage:", port);
                Common.logger.Error(errorMessage);
                return;

            }
            Common.logger.Info("Version: {0}", controller.Version);
            Common.logger.Info("Included nodes: {0}", controller.IncludedNodes.Length);
            Common.logger.Info("-----------------------------------");







            if (args[1] == "r" || args[1] == "replace" || args[1] == "c" || args[1] == "config")
            {


                if (args.Length != 3)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");
                    if (args[1] == "r" || args[1] == "replace")
                    {
                        Common.logger.Info("./hyper [serialPort] r [nodeid]");
                    }
                    else
                    {
                        Common.logger.Info("./hyper [serialPort] c [nodeid]");
                    }

                    return;
                }

                if (!byte.TryParse(args[2], out byte nodeId))
                {
                    Common.logger.Info("argument 1 should be node id! " + args[2] + " is not a number!");
                    return;

                }







                if (!controller.IncludedNodes.Contains(nodeId))
                {
                    Common.logger.Info("NodeID " + nodeId + " not included in network!");
                    Common.logger.Info(string.Join(", ", controller.IncludedNodes));
                    //    return;
                }

                if (args[1] == "r" || args[1] == "replace")
                {
                    new ReplaceCommand(controller, nodeId, config).Start();

                }
                else
                {
                    new ConfigCommand(controller, nodeId, config).Start();
                }



            }
            else if (args[1] == "i" || args[1] == "include")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] i");



                    return;
                }


                new IncludeCommand(controller, config).Start();


            }
            else if (args[1] == "e" || args[1] == "exclude")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] e");



                    return;
                }





                new ExcludeCommand(controller).Start();




            }
            else if (args[1] == "l" || args[1] == "listen")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] l");



                    return;
                }
                currentCommand = new ListenCommand(controller, config);
                currentCommand.Start();
            }
            else if (args[1] == "p" || args[1] == "ping")
            {

                if (args.Length != 3)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] p [nodeid]");


                    return;
                }

                if (!byte.TryParse(args[2], out byte nodeId))
                {
                    Common.logger.Info("argument should be node id! " + args[2] + " is not a number!");
                    return;

                }







                if (!controller.IncludedNodes.Contains(nodeId))
                {
                    Common.logger.Info("NodeID " + nodeId + " not included in network!");
                    Common.logger.Info(string.Join(", ", controller.IncludedNodes));
                    return;
                }


                currentCommand = new PingCommand(controller, nodeId);
                currentCommand.Start();
            }
            else if (args[1] == "rc" || args[1] == "reconfigure")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] rc");



                    return;
                }
                currentCommand = new ReconfigureCommand(controller, config);
                currentCommand.Start();
            }
            else if (args[1] == "it" || args[1] == "interactive")
            {
                currentCommand = new InteractiveCommand(controller, config);
                currentCommand.Start();
            }
            else if (args[1] == "fr" || args[1] == "forceRemove")
            {
                if (args.Length != 3)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] fr [nodeid]");


                    return;
                }

                if (!byte.TryParse(args[2], out byte nodeId))
                {
                    Common.logger.Info("argument should be node id! " + args[2] + " is not a number!");
                    return;

                }







                if (!controller.IncludedNodes.Contains(nodeId))
                {
                    Common.logger.Info("NodeID " + nodeId + " not included in network!");
                    Common.logger.Info(string.Join(", ", controller.IncludedNodes));
                    return;
                }


                currentCommand = new ForceRemoveCommand(controller, nodeId);
                currentCommand.Start();
            }
            else
            {
                Common.logger.Info("unknown command: {0}", args[1]);
                Common.logger.Info("valid commands:");
                Common.logger.Info("r/replace, c/config, i/include, e/exclude, l/listen, p/ping");
            }







            Common.logger.Info("----------");
            Common.logger.Info("Press any key to exit...");
            Console.ReadKey();

        }





        internal class InteractiveCommand : ICommand
        {
            private readonly Controller controller;
            private List<ConfigItem> configList;
            private ICommand currentCommand = null;

            private bool blockExit = false;

            public InteractiveCommand(Controller controller, List<ConfigItem> configList)
            {
                this.controller = controller;
                this.configList = configList;
            }

            public bool Active { get; private set; } = false;


            private void CancelHandler(object evtSender, ConsoleCancelEventArgs evtArgs)
            {

                if (evtArgs != null)
                {
                    evtArgs.Cancel = true;

                }
                if (blockExit)
                {
                    Common.logger.Info("Cannot abort application now!\nPlease wait for operation to finish.");
                    return;
                }

                if (currentCommand == null)
                {
                    Common.logger.Info("No current command, stopping application");
                    if (evtArgs != null)
                    {
                        evtArgs.Cancel = false;
                    }
                    Environment.Exit(0);
                    return;
                }




                Common.logger.Info("Stopping current command!");
                currentCommand.Stop();


            }

            public bool Start()
            {

                Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);
                InputManager.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);
                // InputManager.AddCancelEventHandler(CancelHandler);

                var oneTo255Regex = @"((?<!\d)(?:1\d{2}|2[0-4]\d|[1-9]?\d|25[0-5])(?!\d))";

                 
                var pingRegex = new Regex(@$"^ping\s*{oneTo255Regex}");
                var configRegex = new Regex(@"^config\s*" + oneTo255Regex);
                var replaceRegex = new Regex(@"^replace\s*" + oneTo255Regex);
                var basicRegex = new Regex(@"^basic\s*" + oneTo255Regex + @"\s*(false|true)");
                var listenRegex = new Regex(@"^listen\s*(stop|start|filter\s*"+ oneTo255Regex + ")");
                //var testRegex = new Regex(@"^firmware\s*" + oneTo255Regex);
                var forceRemoveRegex = new Regex(@"^remove\s*" + oneTo255Regex);
                var debugRegex = new Regex(@"^debug\s*(false|true)");


                Active = true;
                Common.logger.Info("-----------");
                Common.logger.Info("Interaction mode");
                Common.logger.Info("-----------");




                ListenCommand listenComand = new ListenCommand(controller, configList);

                Thread InstanceCaller = new Thread(
                new ThreadStart(() => listenComand.Start()));

                InstanceCaller.Start();

                while (Active)
                {
                    //   while (Console.KeyAvailable)
                    //       Console.ReadKey(true);

                    Common.logger.Info("choose your destiny girl!");
                    var input = InputManager.ReadAny();
                    if (input == null)
                    {
                        return false;
                    }
                    Common.logger.Info("Command: {0}", input);
                    switch (input.Trim().ToLower())
                    {
                       
                        case "reset!":
                            {
                                blockExit = true;
                                Common.logger.Info("Resetting controller...");
                                controller.SetDefault();
                                controller.SerialApiGetInitData();
                                Common.logger.Info("Done!");

                                blockExit = false;
                                break;
                            }
                       
                        case "include":
                            {
                                currentCommand = new IncludeCommand(controller, configList);
                                break;
                            }
                        case "exclude":
                            {
                                currentCommand = new ExcludeCommand(controller);
                                break;
                            }
                        case "backup":
                            {
                                blockExit = true;
                                var result = Common.ReadNVRam(controller, out byte[] eeprom);
                                if (result)
                                {
                                    File.WriteAllBytes("eeprom.bin", eeprom);
                                    Common.logger.Info("Result is {0}", result);

                                }
                                blockExit = false;
                                break;
                            }
                        case "restore!":
                            {
                                blockExit = true;
                                byte[] read = File.ReadAllBytes("eeprom.bin");
                                var result = Common.WriteNVRam(controller, read);
                                Common.logger.Info("Result is {0}", result);
                                controller.SerialApiGetInitData();

                                blockExit = false;
                                break;
                            }
                        case "reload":
                            {
                                Common.logger.Info("Reloading conifg!");
                                configList = Common.ParseConfig("config.yaml");
                                listenComand.UpdateConfig(configList);
                                break;
                            }
                        case var listenVal when listenRegex.IsMatch(listenVal):
                            {
                                var val = listenRegex.Match(listenVal).Groups[1].Value;
                                if (val == "start" || val == "stop")
                                {
                                    listenComand.Active = val == "start";

                                }
                                else if (val.StartsWith("filter"))
                                {

                                    var nodeId = Byte.Parse(listenRegex.Match(listenVal).Groups[2].Value);
                                    listenComand.Filter = nodeId;
                                    //   Console.WriteLine("FILTER {0}", nodeId);
                                }
                                break;
                            }
                        case var debugVal when debugRegex.IsMatch(debugVal):
                            {
                                var val = debugRegex.Match(debugVal).Groups[1].Value;
                                var debug = bool.Parse(val);
                                listenComand.Debug = debug;
                                break;
                            }
                        case var removeVal when forceRemoveRegex.IsMatch(removeVal):
                            {
                                var val = forceRemoveRegex.Match(removeVal).Groups[1].Value;
                                var nodeId = byte.Parse(val);
                                currentCommand = new ForceRemoveCommand(controller, nodeId);
                                break;
                            }
/*                        case var testVal when testRegex.IsMatch(testVal):
                            {

                                var val = testRegex.Match(testVal).Groups[1].Value;
                                var nodeId = Byte.Parse(val);

                                byte[] bytes = new byte[256];
                                byte[] numArray = File.ReadAllBytes(@"C:\Users\james\Desktop\tmp\MultiSensor 6_OTA_EU_A_V1_13.exe");
                                int length = (int)numArray[numArray.Length - 4] << 24 | (int)numArray[numArray.Length - 3] << 16 | (int)numArray[numArray.Length - 2] << 8 | (int)numArray[numArray.Length - 1];
                                byte[] flashData = new byte[length];
                                Array.Copy((Array)numArray, numArray.Length - length - 4 - 4 - 256, (Array)flashData, 0, length);
                                Array.Copy((Array)numArray, numArray.Length - 256 - 4 - 4, (Array)bytes, 0, 256);



                                var cmd = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REQUEST_GET();
                                cmd.manufacturerId = new byte[] { 0, 0x86 };
                                cmd.firmwareId = new byte[] { 0, 0 };
                                cmd.checksum = Tools.CalculateCrc16Array(flashData);
                                controller.SendData(nodeId, cmd, Common.txOptions);



                                break;
                            }*/
                        case var pingVal when pingRegex.IsMatch(pingVal):
                            {


                                var val = pingRegex.Match(pingVal).Groups[1].Value;
                                var nodeId = byte.Parse(val);
                                currentCommand = new PingCommand(controller, nodeId);
                                break;
                            }
                        case var basicSetVal when basicRegex.IsMatch(basicSetVal):
                            {

                                blockExit = true;
                                var val = basicRegex.Match(basicSetVal).Groups[1].Value;
                                var nodeId = byte.Parse(val);
                                val = basicRegex.Match(basicSetVal).Groups[2].Value;
                                var value = bool.Parse(val);
                                var trys = 5;
                                Common.SetBinary(controller, nodeId, value);
                               /* Thread.Sleep(500);
                                while(trys >= 0 && (!Common.GetBinary(controller, nodeId, out bool retValue) || retValue != value))
                                {
                                    Common.SetBinary(controller, nodeId, value);
                                    Thread.Sleep(500);
                                    trys--;
                                }*/
                                blockExit = false;
                                break;
                            }
                        case var configVal when configRegex.IsMatch(configVal):
                            {
                                var val = configRegex.Match(configVal).Groups[1].Value;
                                var nodeId = byte.Parse(val);
                                currentCommand = new ConfigCommand(controller, nodeId, configList);
                                break;
                            }
                        case var replaceVal when replaceRegex.IsMatch(replaceVal):
                            {
                                var val = replaceRegex.Match(replaceVal).Groups[1].Value;
                                var nodeId = byte.Parse(val);
                                currentCommand = new ReplaceCommand(controller, nodeId, configList);
                                break;
                            }

                       

                        default:
                            Common.logger.Warn("unknown command");
                            break;
                    }
                    if (currentCommand == null)
                    {
                        continue;
                    }
                    listenComand.Active = false;
                    currentCommand.Start();
                    currentCommand = null;
                    listenComand.Active = true;


                }


                Common.logger.Info("goodby master,,,");
                return true;
            }

            public void Stop()
            {
                if (currentCommand != null)
                {
                    Common.logger.Info("stoppping current command!");
                    currentCommand.Stop();
                }
                else
                {
                    Common.logger.Info("stopping interactive mode");
                    Common.logger.Info("press any key to exit");
                    Active = false;
                }
            }
        }

        internal class ExcludeCommand : ICommand
        {




            private readonly Controller controller;


            private bool abort = false;

            public ExcludeCommand(Controller controller)
            {
                this.controller = controller;
            }

            public bool Start()
            {

                Common.logger.Info("-----------");
                Common.logger.Info("Exclusion mode");
                Common.logger.Info("-----------");

                Common.logger.Info("Starting exclusion, please wake up device...");

                var nodeExcluded = Common.ExcludeNode(controller, out byte nodeId);
                while (!nodeExcluded && !abort)
                {
                    Common.logger.Info("Could not exclude any node, trying again...");
                    nodeExcluded = Common.ExcludeNode(controller, out nodeId);
                }

                if (abort)
                {
                    Common.logger.Info("Aborted!");
                    return false;
                }

                Common.logger.Info("Success! node with id: {0} excluded.", nodeId);


                Common.logger.Info("Exclusion done!");
                return true;
            }

            public void Stop()
            {
                Common.logger.Info("aborting... please wait.");
                abort = true;
            }
        }

        public class IncludeCommand : ICommand
        {




            private readonly Controller controller;
            private readonly List<ConfigItem> configList;

            private bool abort = false;

            public bool Active { get; private set; } = false;


            ICommand currentCommand = null;


            public IncludeCommand(Controller controller, List<ConfigItem> configList)
            {
                this.controller = controller;
                this.configList = configList;
            }

            public bool Start()
            {
                Active = true;
                Common.logger.Info("-----------");
                Common.logger.Info("Inclusion mode");
                Common.logger.Info("-----------");

                Common.logger.Info("Starting inclusion, please wake up device...");


                var nodeIncluded = Common.IncludeNode(controller, out byte nodeId);
                while (!nodeIncluded && !abort)
                {
                    Common.logger.Info("Could not include any node, trying again...");
                    nodeIncluded = Common.IncludeNode(controller, out nodeId);
                }

                if (abort)
                {
                    Common.logger.Info("aborted!");
                    return false;
                }
                Common.logger.Info("Success! New node id: {0}", nodeId);


                Common.logger.Info("Inclusion done!");

                currentCommand = new ConfigCommand(controller, nodeId, configList);
                return currentCommand.Start();


            }

            public void Stop()
            {
                Common.logger.Info("aborting... Please wait.");
                abort = true;
                currentCommand?.Stop();
            }
        }

        internal class ConfigCommand : ICommand
        {




            private readonly Controller controller;
            private readonly byte nodeId;
            private readonly List<ConfigItem> configList;

            private bool abort = false;

            public bool Active { get; private set; } = false;


            public ConfigCommand(Controller controller, byte nodeId, List<ConfigItem> configList)
            {
                this.controller = controller;
                this.nodeId = nodeId;
                this.configList = configList;
            }

            public bool Start()
            {

                Active = true;
                Common.logger.Info("-----------");
                Common.logger.Info("Configuration mode");
                Common.logger.Info("node to configure: " + nodeId);
                Common.logger.Info("-----------");




                Common.logger.Info("Getting configuration for device...");
                ConfigItem config = Common.GetConfigurationForDevice(controller, nodeId, configList, ref abort);
                if (config == null)
                {
                    Common.logger.Info("could not find configuration!");
                    Common.logger.Info("Either there is no configuration or device did not reply!");
                    Active = false;
                    return false;
                }

                Common.logger.Info("configuration found for {0}!", config.deviceName);
                Common.logger.Info("Setting values.");
                if (Common.SetConfiguration(controller, nodeId, config))
                {
                    Common.logger.Info("Configuration successful!");
                    Common.logger.Info("-------------------");
                    Active = false;
                    return true;
                }

                return false;




            }

            public void Stop()
            {
                Common.logger.Info("aborting... Please wait.");
                abort = true;
                // Common.logger.Warn("Cannot abort!");
            }
        }


        internal class ForceRemoveCommand : ICommand
        {
            public bool Active { get; private set; } = false;


            private readonly Controller controller;
            private readonly byte nodeId;
            public ForceRemoveCommand(Controller controller, byte nodeId)
            {
                this.controller = controller;
                this.nodeId = nodeId;
            }

            public bool Start()
            {
                Active = true;
                Common.logger.Info("-----------");
                Common.logger.Info("Force Remove mode");
                Common.logger.Info("node to remove: " + nodeId);
                Common.logger.Info("-----------");
                Common.logger.Info("Check if node is reachable...");
                var reachable = Common.CheckReachable(controller, nodeId);
                if (reachable)
                {
                    Common.logger.Info("Node is reachable!");
                    Common.logger.Info("If node is reachable, you should exclude it!");
                    return false;
                }
                else
                {
                    Common.logger.Info("OK, node is not reachable");
                }
                Common.logger.Info("Mark node as failed...");
                var markedAsFailed = Common.MarkNodeFailed(controller, nodeId);
                if (!markedAsFailed)
                {
                    Common.logger.Info("Node could not be marked as failed!");
                    Common.logger.Info("Try again and ensure that node is not reachable.");
                    return false;
                }
                else
                {
                    Common.logger.Info("OK, node is marked as failed");
                }

                Common.logger.Info("Removing Node...");
                var result = controller.RemoveFailedNodeId(nodeId);
                Common.logger.Info(result.State);
                Common.logger.Info("Removing Done!");


                Active = false;
                return true;
            }

            public void Stop()
            {
                Common.logger.Info("Not stoppable");
            }
        }

        internal class ReplaceCommand : ICommand
        {



            private readonly Controller controller;
            private readonly byte nodeId;
            private readonly List<ConfigItem> configList;

            private ICommand currentCommand = null;

            public bool Active { get; private set; } = false;

            private bool abort = false;

            public ReplaceCommand(Controller controller, byte nodeId, List<ConfigItem> configList)
            {
                this.controller = controller;
                this.nodeId = nodeId;
                this.configList = configList;
            }

            public bool Start()
            {
                Active = true;
                Common.logger.Info("-----------");
                Common.logger.Info("Replacement mode");
                Common.logger.Info("node to replace: " + nodeId);
                Common.logger.Info("-----------");
                Common.logger.Info("Check if node is reachable...");
                var reachable = Common.CheckReachable(controller, nodeId);
                if (reachable)
                {
                    Common.logger.Info("Node is reachable!");
                    Common.logger.Info("If node is reachable, we cannot replace it!");
                    return false;
                }
                else
                {
                    Common.logger.Info("OK, node is not reachable");
                }
                if (abort)
                {
                    return false;
                }
                Common.logger.Info("Mark node as failed...");
                var markedAsFailed = Common.MarkNodeFailed(controller, nodeId);
                if (!markedAsFailed)
                {
                    Common.logger.Info("Node could not be marked as failed!");
                    Common.logger.Info("Try again and ensure that node is not reachable.");
                    return false;
                }
                else
                {
                    Common.logger.Info("OK, node is marked as failed");
                }
                Common.logger.Info("Replacing Node... Set new device to inclusion mode!");
                bool nodeReplaced = Common.ReplaceNode(controller, nodeId);
                while (!nodeReplaced && !abort)
                {
                    Common.logger.Info("Could not replace device! Trying again.");
                    nodeReplaced = Common.ReplaceNode(controller, nodeId);
                }

                if (abort)
                {
                    Common.logger.Info("aborted!");
                    return false;
                }

                Common.logger.Info("Node sucessfully replaced!");


                //     Common.logger.Info("Write new Configuration...");
                //     bool configurationSet = SetConfiguration();

                //   Common.logger.Info("Get Association");
                //  GetAssociation();
                //GetConfig();

                //   GetManufactor();
                //GetWakeUp();
                //Console.ReadLine();
                Common.logger.Info("Replacement done!");

                currentCommand = new ConfigCommand(controller, nodeId, configList);
                return currentCommand.Start();


            }

            public void Stop()
            {
                Common.logger.Info("aborting... Please wait.");
                abort = true;
                currentCommand?.Stop();
            }




            //private void GetWakeUp()
            //{
            //    var cmd = new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_GET();
            //    var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT(), 20000);
            //    if (result)
            //    {
            //        var rpt = (COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT)result.Command;
            //        Common.logger.Info("wake up interval: " + Tools.GetInt32(rpt.seconds));

            //    }
            //    else
            //    {
            //        Common.logger.Info("Could Not get wake up!!");
            //    }
            //}












        }



        internal class ListenCommand : ICommand
        {
            private readonly Controller controller;
            private List<ConfigItem> configList;

            private EventDAO eventDao = new EventDAO();

            public ListenCommand(Controller controller, List<ConfigItem> configList)
            {
                this.controller = controller;
                this.configList = configList;

            }

            private bool _active = true;
            public bool Active
            {
                get { return _active; }
                set
                {
                   // Common.logger.Info("listening {0}", value == true ? "active" : "inactive");
                  //  _active = value;
                }
            }

            private byte _filterNodeId = 0;
            public byte Filter {
                get { return _filterNodeId; } 
                set
                {
                    Common.logger.Info("Setting filter {0}", value == 0 ? "inactive" : $"active to node id {value}");
                    _filterNodeId = value;
                }
            }
            private bool _debug = false;
            public bool Debug
            {
                get { return _debug; }
                set
                {
                    Common.logger.Info("Setting debug mode {0}", value ? "on" : "off");
                    _debug = value;
                }
            }


            private readonly BlockingCollection<Action> queueItems = new BlockingCollection<Action>();
            private ActionToken dataListener;
            private ActionToken controllerListener;

            public void AddToQueue(Action action)
            {
                queueItems.Add(action);
            }





            public bool Start()
            {
                

                Common.logger.Info("-----------");
                Common.logger.Info("Listening mode");
                Common.logger.Info("-----------");

                Common.logger.Info("Loading available command classes...");
                var assembly = typeof(COMMAND_CLASS_BASIC).GetTypeInfo().Assembly;
                var commandClasses = Common.GetAllCommandClasses(assembly, "CommandClasses");
                Common.logger.Info("Got {0} command classes", commandClasses.Count);
                var nestedCommandClasses = Common.GetAllNestedCommandClasses(commandClasses.Values);
                Common.logger.Info("Got all inner command classes for {0} command classes", commandClasses.Count);
                Common.logger.Info("Listening...");



            //    byte[] bytes = new byte[256];
            //    byte[] numArray = File.ReadAllBytes(@"C:\Users\james\Desktop\tmp\MultiSensor 6_OTA_EU_A_V1_13.exe");
            //    int length = (int)numArray[numArray.Length - 4] << 24 | (int)numArray[numArray.Length - 3] << 16 | (int)numArray[numArray.Length - 2] << 8 | (int)numArray[numArray.Length - 1];
            //    byte[] flashDataB = new byte[length];
            //    Array.Copy((Array)numArray, numArray.Length - length - 4 - 4 - 256, (Array)flashDataB, 0, length);
            //    //Array.Copy((Array)numArray, numArray.Length - 256 - 4 - 4, (Array)bytes, 0, 256);
            //    List<byte> flashData = new List<byte>(flashDataB);
            ////   var filename = Encoding.UTF8.GetString(bytes);
            // //   flashData = flashData.Take(5146).ToArray();
            //   // Console.WriteLine(filename);
            //  //  Console.WriteLine(flashData.Length);


                dataListener = controller.ListenData((x) =>
                {
                    if (!Active)
                    {
                        return;
                    }

                    var filterActive = Filter != 0 && Filter != x.SrcNodeId;


                    var _commandClass = commandClasses.TryGetValue(x.Command[0], out var commandClass);
                    if (!_commandClass)
                    {
                        Common.logger.Error("command class {0} not found!", x.Command[0]);
                        return;
                    }
                    var _nestedDict = nestedCommandClasses.TryGetValue(commandClass, out var nestedDict);
                    if (!_nestedDict)
                    {
                        Common.logger.Error("nested command classes for command class {0} not found!", commandClass.Name);
                        return;
                    }
                    var _nestedType = nestedDict.TryGetValue(x.Command[1], out Type nestedType);
                    if (!_nestedType)
                    {
                        Common.logger.Error("nested command class {0} for command class {1} not found!", x.Command[1], commandClass.Name);
                        return;
                    }


                    if(!filterActive)
                        Common.logger.Info("{0}: {2}:{3} from node {1}", x.TimeStamp, x.SrcNodeId, _commandClass ? commandClass.Name : string.Format("unknown(id:{0})", x.Command[0]), _nestedType ? nestedType.Name : string.Format("unknown(id:{0})", x.Command[1]));

                    if (commandClass == null)
                    {
                        Common.logger.Error("command class is null!");
                        return;
                    }
                    if (nestedType == null)
                    {
                        Common.logger.Error("nested type is null!");
                        return;
                    }

                    //     var dummyInstance = Activator.CreateInstance(nestedType);

                    var implicitCastMethod =
                  nestedType.GetMethod("op_Implicit",
                                       new[] { x.Command.GetType() });

                    if (implicitCastMethod == null)
                    {
                        Common.logger.Warn("byteArray to {0} not possible!", nestedType.Name);
                        return;
                    }
                    var report = implicitCastMethod.Invoke(null, new[] { x.Command });

                    if (!filterActive && Debug)
                        Common.logger.Info(Util.ObjToJson(report));

                   
                    if(!(report is COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_GET))
                    {
                        OutputManager.HandleCommand(report, x.SrcNodeId, x.DestNodeId);

                    }

                    //    Handle(dummyInstance, x.SrcNodeId, x.Command);

                    switch (report)
                    {
                        case COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_NOTIFICATION _:
                            // TODO EVENT, last Battery
                            var lastDate = eventDao.GetLastEvent(typeof(COMMAND_CLASS_BATTERY.BATTERY_REPORT).Name, x.SrcNodeId);
                            Console.WriteLine("difference in hours: {0}", (DateTime.Now - lastDate).Hours);
                            Console.WriteLine("difference in minutes: {0}", (DateTime.Now - lastDate).Minutes);
                            if ((DateTime.Now - lastDate).Hours >= 6)
                            {
                                queueItems.Add(() => Common.RequestBatteryReport(controller, x.SrcNodeId));
                            }
                            
                            break;
                        case COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_GET fupdateReport:

                            //var rep1 = fupdateReport.properties1.reportNumber1;
                            //var rep2 = fupdateReport.reportNumber2;
                            //var count = fupdateReport.numberOfReports;

                            //queueItems.Add(() => {

                            //    var take = 40;
                            //    var cmd = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REPORT();
                            //    cmd.properties1.last = 0;
                            //    cmd.properties1.reportNumber1 = rep1;
                            //    cmd.reportNumber2 = rep2;
                            //    short repNumber = BitConverter.ToInt16(new byte[] { rep2, rep1 });
                            //    //Console.WriteLine($"Repnumber: {repNumber}");
                            //    var offset = (repNumber - 1) * 40;
                            //    Common.logger.Info("Progress: {0}", (offset / (float)flashData.Count).ToString("0.00%"));

                            //    if ((flashData.Count - offset) < 40)
                            //    {
                            //       // Console.WriteLine("ima full!");
                            //        Console.WriteLine(offset);
                            //        Console.WriteLine(flashData.Count);
                            //        take = flashData.Count - offset;
                            //        Console.WriteLine(take);
                            //        cmd.properties1.last = 1;
                            //        Common.logger.Info("Progress: {0}", (1).ToString("0.00%"));
                            //    }
                            //    var data = flashData.Skip(offset).Take(take).ToArray();
                            //    cmd.data = data;
                            //    //7A 97
                            //   // Common.logger.Info(Util.ObjToJson((byte[])cmd));
                            //   // Common.logger.Info(Util.ObjToJson(new byte[] { COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.ID, COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REPORT.ID }.Concat(new byte[] { cmd.properties1, cmd.reportNumber2 }).Concat(data).ToArray()));
                            //    cmd.checksum = Tools.CalculateCrc16Array((byte[])cmd,0,((byte[])cmd).Length -2);
                            //        //cmd.checksum =  Tools.CalculateCrc16Array(new byte[] { COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.ID, COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REPORT .ID}.Concat(new byte[] { cmd.properties1, cmd.reportNumber2 }).Concat(data));
                            //    controller.SendData(x.SrcNodeId, cmd, Common.txOptions);
                                
                            //});
                           
                            break;
                        default:
                            break;
                    }



                });

                controllerListener = controller.HandleControllerUpdate((r) =>
                {
                    if (!Active)
                    {
                        return;
                    }
                    Common.logger.Info("{0}: Got {2} for node {1}", DateTime.Now, r.NodeId, r.Status);
                    var lastDate = default(DateTime);// eventDao.GetLastEvent(typeof(COMMAND_CLASS_BATTERY.BATTERY_REPORT).Name, r.NodeId);
                    Console.WriteLine("difference in hours: {0}", (DateTime.Now - lastDate).TotalHours);
                    Console.WriteLine("difference in minutes: {0}", (DateTime.Now - lastDate).TotalMinutes);
                    if ((DateTime.Now - lastDate).TotalHours >= 6)
                    {
                        queueItems.Add(() => Common.RequestBatteryReport(controller, r.NodeId));
                    }
                    //   Common.logger.Info(JObject.FromObject(r).ToString());

                    //  queueItems.Add(() => Common.RequestBatteryReport(controller, r.NodeId));

                });





                while (!queueItems.IsCompleted)
                {
                    try
                    {
                        var action = queueItems.Take();
                        if (Active)
                            action();

                    }
                    catch (InvalidOperationException) { }


                }

                Active = false;
                Common.logger.Info("Listening done!");
                return true;
            }

            //private void OnCommand(object sender, string e)
            //{
            //    Common.logger.Info("recevied from da console: " + e);
            //    if(e == "configure 14")
            //    {
            //        queueItems.Add(() => {

            //            // Common.RequestBatteryReport(controller, x.SrcNodeId)
            //                  new ConfigCommand(controller, 14, configList).Start();

            //        });
            //    }
            //}

            public void Stop()
            {
                Common.logger.Info("stop listening!");
                dataListener?.SetCompleted();
                controllerListener?.SetCompleted();
                queueItems?.CompleteAdding();
            }

            internal void UpdateConfig(List<ConfigItem> configList)
            {
                this.configList = configList;
            }
        }


        internal class ReconfigureCommand : ICommand
        {
            private readonly Controller controller;
            private readonly List<ConfigItem> configList;
            readonly BlockingCollection<byte> queueItems = new BlockingCollection<byte>(255);


            public ReconfigureCommand(Controller controller, List<ConfigItem> configList)
            {
                this.controller = controller;
                this.configList = configList;
            }


            public readonly object lockObject = new object();
            private ActionToken token;
            private bool abort = false;

            private HashSet<byte> nodesToCheck = new HashSet<byte>(255);

            public bool Active { get; private set; } = false;

            public bool Start()
            {
                Active = true;
                Common.logger.Info("-----------");
                Common.logger.Info("Listening mode");
                Common.logger.Info("Press Enter to exit");
                Common.logger.Info("-----------");

                Common.logger.Info("Loading available command classes...");
                var assembly = typeof(COMMAND_CLASS_BASIC).GetTypeInfo().Assembly;
                var commandClasses = Common.GetAllCommandClasses(assembly, "CommandClasses");
                Common.logger.Info("Got {0} command classes", commandClasses.Count);


                nodesToCheck.AddRange(controller.IncludedNodes);
                nodesToCheck.Remove(1);
                //nodesToCheck.AddRange(new List<byte>{ 9, 25, 33 });


                // ping all nodes
                // all nodes that respond are probably not battery powered and we can overwrite the configuration without delay
                Common.logger.Info("There are {0} to check/reconfigure. Pinging each node...", nodesToCheck.Count);
                //foreach (var nodeId in nodesToCheck.ToList())
                //{
                //    Common.logger.Info("Pinging node {0}...", nodeId);
                //    bool notReachable;
                //    int retryCount = 0;
                //    while ((notReachable = !Common.CheckReachable(controller, nodeId)) && retryCount < 3 && !abort)
                //    {
                //        Common.logger.Info("node {0} is not reachable, trying again.", nodeId);
                //        retryCount++;
                //    }
                //    if (abort)
                //    {
                //        Common.logger.Info("Aborting!");
                //        return;
                //    }
                //    if (notReachable)
                //    {
                //        Common.logger.Info("node {0} is not reachable, wait for wakeup!", nodeId);
                //        continue;
                //    }
                //    Common.logger.Info("node {0} is reachable! Configure directly.", nodeId);

                //    //ZWave.Enums.RequestNeighborUpdateStatuses nodeNeighborUpdateResult = ZWave.Enums.RequestNeighborUpdateStatuses.Started;

                //    //int retryCounter = 0;
                //    //while (true){
                //    //    nodeNeighborUpdateResult = controller.RequestNodeNeighborUpdate(nodeId, 60000 * 5).NeighborUpdateStatus;
                //    //    if(nodeNeighborUpdateResult != ZWave.Enums.RequestNeighborUpdateStatuses.Done && retryCounter < 10)
                //    //    {
                //    //        Common.logger.Info("not done, again!");
                //    //        retryCounter++;
                //    //        continue;
                //    //    }
                //    //    Common.logger.Info("done or too many retries!");
                //    //    break;
                //    //}

                //    //controller.DeleteReturnRoute(nodeId, out ActionToken deleteToken);
                //    //var resDelete = (ZWave.BasicApplication.Operations.DeleteReturnRouteResult)deleteToken.Result;
                //    //Common.logger.Info("delete return:");
                //    //Common.logger.Info(deleteToken.State);
                //    //Common.logger.Info(resDelete.RetStatus);

                //    //controller.AssignReturnRoute(1, nodeId, out ActionToken assignToken);
                //    //var resAssign = (ZWave.BasicApplication.Operations.AssignReturnRouteResult)assignToken.Result;
                //    //Common.logger.Info("assign return:");
                //    //Common.logger.Info(resAssign.State);
                //    //Common.logger.Info(resAssign.RetStatus);

                //    //Common.logger.Info("Neighbour update successful!");

                //    if (new ConfigCommand(controller, nodeId, configList).Start())
                //    {
                //        nodesToCheck.Remove(nodeId);
                //    }
                //    Common.logger.Info("{0} nodes left to check.", nodesToCheck.Count);
                //}

                Common.logger.Info("There are {0} nodes left to check/reconfigure. Waiting for wake up notifications...", nodesToCheck.Count);
                Common.logger.Info("Exit by pressing CTRL-C");



                bool inProgress = false;


                token = controller.ListenData((x) =>
                {
                    if (inProgress)
                    {
                        return;
                    }
                    lock (lockObject)
                    {


                        var getValue = commandClasses.TryGetValue(x.Command[0], out Type commandClass);
                        Common.logger.Info("{0}: Got {2} notification from node {1}", x.TimeStamp, x.SrcNodeId, getValue ? commandClass.Name : string.Format("unknown(id:{0})", x.Command[0]));

                        if (!getValue)
                        {
                            Common.logger.Info("Unknown command class");
                            return;
                        }
                        if (!nodesToCheck.Contains(x.SrcNodeId))
                        {
                            Common.logger.Info("node {0} already processed", x.SrcNodeId);
                            return;
                        }
                        if (x.Command[0] != COMMAND_CLASS_WAKE_UP.ID)
                        {
                            Common.logger.Info("Not a wakeup notification");
                            return;
                        }
                        Common.logger.Info("Got a wakeup! Reconfigure node {0}", x.SrcNodeId);
                        nodesToCheck.Remove(x.SrcNodeId);

                    }
                    //  new ConfigCommand(controller, x.SrcNodeId, configList).Start();

                    queueItems.Add(x.SrcNodeId);

                });


                //controller.HandleControllerUpdate((r) =>
                //{
                //    Common.logger.Info("{0}: Got {2} for node {1}", DateTime.Now, r.NodeId, r.Status);
                //});


                if (nodesToCheck.Count == 0)
                {
                    token.SetCompleted();
                    queueItems.CompleteAdding();
                }


                while (!queueItems.IsCompleted)
                {
                    try
                    {
                        var nodeId = queueItems.Take();
                        //   Common.logger.Info("Got queue element for node {0}", nodeId);
                        //token.SetCancelled();
                        inProgress = true;
                        if (!new ConfigCommand(controller, nodeId, configList).Start())
                        {
                            Common.logger.Info("Adding node back to queue");
                            nodesToCheck.Add(nodeId);
                        }
                        Common.logger.Info("{0} nodes left to check.", nodesToCheck.Count);
                        if (nodesToCheck.Count == 0)
                        {
                            token.SetCompleted();
                            queueItems.CompleteAdding();
                        }
                        inProgress = false;

                    }
                    catch (InvalidOperationException) { }


                }




                Common.logger.Info("Reconfiguration done!");
                Common.logger.Info("---------------------");
                Active = false;
                return true;
            }

            public void Stop()
            {
                Common.logger.Info("Stopping Reconfiguration");
                abort = true;
                token?.SetCompleted();
                queueItems?.CompleteAdding();

                var nodesToList = new List<byte>(nodesToCheck);
                nodesToList.Sort();
                Common.logger.Info("Nodes left:");
                Common.logger.Info(string.Join(",", nodesToList));
            }
        }


        internal class PingCommand : ICommand
        {
            private readonly Controller controller;
            private readonly byte nodeId;

            private bool running = true;


            public PingCommand(Controller controller, byte nodeId)
            {
                this.controller = controller;
                this.nodeId = nodeId;
            }

            public bool Active { get; private set; } = false;

            public bool Start()
            {
                Active = true;
                Common.logger.Info("-----------");
                Common.logger.Info("Ping mode");
                Common.logger.Info("ctrl-c to exit");
                Common.logger.Info("-----------");

                Common.logger.Info("Pinging node {0}...", nodeId);



                while (running)
                {
                    var reachable = Common.CheckReachable(controller, nodeId);
                    Common.logger.Info("node {0} is{1}reachable! Pinging again...", nodeId, reachable ? " " : " NOT ");
                    if (reachable)
                    {
                        Task.Delay(2000).Wait();
                    }
                }

                Common.logger.Info("Goodbye...", nodeId);
                Active = false;
                return true;

            }

            public void Stop()
            {
                Common.logger.Info("aborting... Please wait.");
                running = false;
            }
        }
    }
}