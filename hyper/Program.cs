using hyper.commands;
using hyper.config;
using hyper.Database;
using hyper.Database.DAO;
using hyper.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ZWave;
using ZWave.BasicApplication.Devices;
using ZWave.CommandClasses;

namespace hyper
{
    internal class Program
    {
        private static void Main(string[] args)
        {






            if (!File.Exists("events.db"))
            {
                //using (File.Create("events.db")) ;

                using SQLiteConnection connection = new SQLiteConnection("Data Source=events.db;");
                using SQLiteCommand command = new SQLiteCommand(
@"CREATE TABLE 'Events' ( 
            `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            `NodeId` INTEGER NOT NULL,
            `EventType` TEXT NOT NULL,
            `Value` INTEGER,
            `Added` DATETIME NOT NULL)",
connection);
                connection.Open();
                command.ExecuteNonQuery();

            }

            LinqToDB.Data.DataConnection.DefaultSettings = new DatabaseSettings();




            //var events = new List<Event>();


            //for (int i = 0; i < 100; i++)
            //{
            //    Event evt = new Event();
            //    evt.Value =  i;
            //    evt.EventType = EventType.BATTERY;
            //    evt.NodeId = 10;
            //    events.Add(evt);

            //}
            //var dbTask = EventDAO.InsertEventsAsync(events);
            //Console.WriteLine("done");
            //Console.WriteLine("now waiting");
            //dbTask.Wait();
            //Console.WriteLine("really done!");




            //Console.WriteLine("getting...");
            //var eventos = EventDAO.GetAll();

            ////foreach (var eventoss in eventos)
            ////{
            ////    Console.WriteLine(eventoss.id);
            ////    Console.WriteLine(eventoss.Name);
            ////    Console.WriteLine(eventoss.EventType);
            ////}
            //Console.WriteLine(eventos.Count());

            //return;
            //  if (File.Exists("logs\\database.db"))
            //       return;



            //string[] ports = SerialPort.GetPortNames();

            //Console.WriteLine("The following serial ports were found:");

            //// Display each port name to the console.
            //foreach (string porta in ports)
            //{
            //    Console.WriteLine(porta);
            //}

            //Console.ReadLine();
            //return;


            ICommand currentCommand = null;

            Console.CancelKeyPress += new ConsoleCancelEventHandler((evtSender, evtArgs) =>
            {
                evtArgs.Cancel = true;

                if (currentCommand?.Active ?? false)
                {
                    Common.logger.Info("master: stopping current command!");
                    currentCommand.Stop();
                }
                else
                {
                    Common.logger.Info("No current command, stopping application");

                }
            });




            Common.logger.Info("==== ZWave Command Center 5000 ====");
            Common.logger.Info("-----------------------------------");
            Common.logger.Info("Loading device configuration database...");
            if (!File.Exists("config.yaml"))
            {
                Common.logger.Info("configuration file config.yaml does not exist!");
                return;
            }
            var config = ParseConfig("config.yaml");
            if (config == null)
            {
                Common.logger.Info("Could not parse configuration file config.yaml!");
                return;
            }
            Common.logger.Info("Got configuration for " + config.Count + " devices.");
            Common.logger.Info("-----------------------------------");

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
                Common.logger.Info("Error connecting with port {0}! Error Mesage:", port);
                Common.logger.Info(errorMessage);
                return;

            }
            Common.logger.Info("Version: {0}", controller.Version);
            Common.logger.Info("Included nodes: {0}", controller.IncludedNodes.Length);
            Common.logger.Info("-----------------------------------");

            Task.Delay(2000).Wait();






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
                //  new ReconfigureCommand(controller, config).Start();
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


                new PingCommand(controller, nodeId).Start();
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
                new ReconfigureCommand(controller, config).Start();
                // new ListenCommand(controller).Start();
            }
            else if (args[1] == "it" || args[1] == "interactive")
            {
                currentCommand = new InteractiveCommand(controller, config);
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

        private static List<ConfigItem> ParseConfig(string configFile)
        {

            var yamlText = File.ReadAllText(configFile);
            //  var input = new StringReader(yamlText);


            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            List<ConfigItem> configList = null;
            try
            {
                configList = deserializer.Deserialize<List<ConfigItem>>(yamlText);
            }
            catch
            {

            }

            return configList;


        }


    }

    internal class InteractiveCommand : ICommand
    {
        private readonly Controller controller;
        private readonly List<ConfigItem> configList;
        private ICommand currentCommand = null;

        public InteractiveCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;
        }

        public bool Active { get; private set; } = false;

        public void Start()
        {
            Active = true;
            Common.logger.Info("-----------");
            Common.logger.Info("Interaction mode");
            Common.logger.Info("-----------");

            while (Active)
            {
                Common.logger.Info("press l girl!");
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.L)
                {
                    Common.logger.Info("yes my master");
                    currentCommand = new ListenCommand(controller, configList);
                    currentCommand.Start();
                }
            }


            Common.logger.Info("goodby master,,,");
        }

        public void Stop()
        {
            if (currentCommand?.Active ?? false)
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

    internal class ExcludeCommand
    {




        private readonly Controller controller;

        public ExcludeCommand(Controller controller)
        {
            this.controller = controller;
        }

        internal void Start()
        {

            Common.logger.Info("-----------");
            Common.logger.Info("Exclusion mode");
            Common.logger.Info("-----------");

            Common.logger.Info("Starting exclusion, please wake up device...");

            var nodeExcluded = Common.ExcludeNode(controller, out byte nodeId);
            while (!nodeExcluded)
            {
                Common.logger.Info("Could not exclude any node, trying again...");
                nodeExcluded = Common.ExcludeNode(controller, out nodeId);
            }

            Common.logger.Info("Success! node with id: {0} excluded.", nodeId);


            Common.logger.Info("Exclusion done!");
        }
    }

    internal class IncludeCommand
    {




        private readonly Controller controller;
        private readonly List<ConfigItem> configList;

        public IncludeCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;
        }

        internal void Start()
        {
            Common.logger.Info("-----------");
            Common.logger.Info("Inclusion mode");
            Common.logger.Info("-----------");

            Common.logger.Info("Starting inclusion, please wake up device...");

            var nodeIncluded = Common.IncludeNode(controller, out byte nodeId);
            while (!nodeIncluded)
            {
                Common.logger.Info("Could not include any node, trying again...");
                nodeIncluded = Common.IncludeNode(controller, out nodeId);
            }

            Common.logger.Info("Success! New node id: {0}", nodeId);

            new ConfigCommand(controller, nodeId, configList).Start();


            Common.logger.Info("Inclusion done!");



        }
    }

    internal class ConfigCommand
    {




        private readonly Controller controller;
        private readonly byte nodeId;
        private readonly List<ConfigItem> configList;

        public ConfigCommand(Controller controller, byte nodeId, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.nodeId = nodeId;
            this.configList = configList;
        }

        internal void Start()
        {
            Common.logger.Info("-----------");
            Common.logger.Info("Configuration mode");
            Common.logger.Info("node to configure: " + nodeId);
            Common.logger.Info("-----------");




            Common.logger.Info("Getting configuration for device...");
            ConfigItem config = Common.GetConfigurationForDevice(controller, nodeId, configList);
            if (config == null)
            {
                Common.logger.Info("could not find configuration!");
                Common.logger.Info("you need to add this device to the configuration file.");
                return;
            }

            Common.logger.Info("configuration found for {0}!", config.deviceName);
            Common.logger.Info("Setting values.");
            Common.SetConfiguration(controller, nodeId, config);

            Common.logger.Info("Configuration done!");
            Common.logger.Info("-------------------");



        }
    }

    internal class ReplaceCommand
    {



        private readonly Controller controller;
        private readonly byte nodeId;
        private readonly List<ConfigItem> configList;

        public ReplaceCommand(Controller controller, byte nodeId, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.nodeId = nodeId;
            this.configList = configList;
        }

        internal void Start()
        {
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
                return;
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
                return;
            }
            else
            {
                Common.logger.Info("OK, node is marked as failed");
            }
            Common.logger.Info("Replacing Node... Set new device to inclusion mode!");
            bool nodeReplaced = Common.ReplaceNode(controller, nodeId);
            if (!nodeReplaced)
            {
                Common.logger.Info("Could not replace device!");
                Common.logger.Info("Please try again.");
                return;
            }
            else
            {
                Common.logger.Info("Node sucessfully replaced!");
            }

            //     Common.logger.Info("Write new Configuration...");
            //     bool configurationSet = SetConfiguration();

            //   Common.logger.Info("Get Association");
            //  GetAssociation();
            //GetConfig();

            //   GetManufactor();
            //GetWakeUp();
            //Console.ReadLine();

            new ConfigCommand(controller, nodeId, configList).Start();



            Common.logger.Info("Replacement done!");




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
        private readonly List<ConfigItem> configList;

        public ListenCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;

        }


        //public NetworkStream ConnectToServer()
        //{
        //    var client = new TcpClient("", 54321);
        //    var stream = client.GetStream();
        //    return stream;
        //}

        //public void SendBytes(UdpClient udpClient, byte[] bytes)
        //{
        //    lock (syncObject)
        //    {


        //        try
        //        {
        //            udpClient.Send(bytes, bytes.Length);
        //            messageCounter++;
        //            Common.logger.Info("Number of packets send: {0}", messageCounter);
        //        }
        //        catch (Exception e)
        //        {
        //            Common.logger.Info(e.ToString());
        //        }
        //    }
        //}


        //List<byte> byteBuffer = new List<byte>();
        //readonly object syncObject = new object();
        //readonly object syncObjectServer = new object();


        public bool Active { get; private set; } = false;

        private readonly BlockingCollection<Action> queueItems = new BlockingCollection<Action>();
        private ActionToken dataListener;
        private ActionToken controllerListener;

        public void Start()
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
            var nestedCommandClasses = Common.GetAllNestedCommandClasses(commandClasses.Values);
            Common.logger.Info("Got all inner command classes for {0} command classes", commandClasses.Count);
            Common.logger.Info("Listening...");


            dataListener = controller.ListenData((x) =>
            {
                var _commandClass = commandClasses.TryGetValue(x.Command[0], out Type commandClass);
                var nestedDict = nestedCommandClasses[commandClass];
                var _nestedType = nestedDict.TryGetValue(x.Command[1], out Type nestedType);

                Common.logger.Info("{0}: {2}:{3} from node {1}", x.TimeStamp, x.SrcNodeId, _commandClass ? commandClass.Name : string.Format("unknown(id:{0})", x.Command[0]), _nestedType ? nestedType.Name : string.Format("unknown(id:{0})", x.Command[1]));

                Common.logger.Info(string.Join(",", x.Command));
                // Common.logger.Info("command type: {0}, type name: {1}", x.CommandType, x.GetType().Name);

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

                var dummyInstance = Activator.CreateInstance(nestedType);


                switch (dummyInstance)
                {
                    case COMMAND_CLASS_NOTIFICATION_V3.NOTIFICATION_REPORT _:
                        var alarmReport = (COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT)x.Command;
                        Common.logger.Info("notificationStatus: {0}", alarmReport.notificationStatus);
                        Common.logger.Info("notificationType: {0}", alarmReport.notificationType);
                        Common.logger.Info("mevent: {0}", alarmReport.mevent);
                        Common.logger.Info("v1AlarmLevel: {0}", alarmReport.v1AlarmLevel);
                        Common.logger.Info("v1AlarmType: {0}", alarmReport.v1AlarmType);
                        var evtNotification = new Event
                        {
                            NodeId = x.SrcNodeId,
                            EventType = EventType.NOTIFICATION
                        };
                        EventDAO.InsertEventAsync(evtNotification);

                        // buffer = new byte[] { x.SrcNodeId, x.Command[0], alarmReport.zwaveAlarmEvent };
                        return;
                    case COMMAND_CLASS_BASIC.BASIC_SET _:
                        var basicSet = (COMMAND_CLASS_BASIC.BASIC_SET)x.Command;
                        Common.logger.Info("value: {0}", basicSet.value);
                        //      buffer = new byte[] { x.SrcNodeId, x.Command[0], basicSet.value };
                        break;
                    case COMMAND_CLASS_SWITCH_BINARY.SWITCH_BINARY_REPORT _:
                        var binaryReport = (COMMAND_CLASS_SWITCH_BINARY.SWITCH_BINARY_REPORT)x.Command;
                        //     buffer = new byte[] { x.SrcNodeId, x.Command[0], binaryReport.value };
                        break;
                    case COMMAND_CLASS_WAKE_UP.WAKE_UP_NOTIFICATION _:
                        var evtWakeup = new Event
                        {
                            NodeId = x.SrcNodeId,
                            EventType = EventType.WAKEUP,
                        };
                        EventDAO.InsertEventAsync(evtWakeup);
                        queueItems.Add(() => Common.RequestBatteryReport(controller, x.SrcNodeId));
                        return;
                    case COMMAND_CLASS_BATTERY.BATTERY_REPORT _:
                        var batteryReport = (COMMAND_CLASS_BATTERY.BATTERY_REPORT)x.Command;
                        Common.logger.Info("battery value: {0}", batteryReport.batteryLevel);
                        var evtBattery = new Event
                        {
                            NodeId = x.SrcNodeId,
                            EventType = EventType.BATTERY,
                            Value = batteryReport.batteryLevel
                        };
                        EventDAO.InsertEventAsync(evtBattery);
                        //      buffer = new byte[] { x.SrcNodeId, x.Command[0], batteryReport.batteryLevel };
                        break;
                    case COMMAND_CLASS_SENSOR_MULTILEVEL.SENSOR_MULTILEVEL_REPORT _:
                        var multilevelReport = (COMMAND_CLASS_SENSOR_MULTILEVEL.SENSOR_MULTILEVEL_REPORT)x.Command;
                        Common.logger.Info("properties: {0}, type: {1}, value: {2}", multilevelReport.properties1.ToString(), multilevelReport.sensorType, multilevelReport.sensorValue);
                        return;
                    case COMMAND_CLASS_SENSOR_BINARY_V2.SENSOR_BINARY_REPORT _:
                        var sensorBinaryReport = (COMMAND_CLASS_SENSOR_BINARY_V2.SENSOR_BINARY_REPORT)x.Command;
                        Common.logger.Info("values: {0} - {1}", sensorBinaryReport.sensorType, sensorBinaryReport.sensorValue);
                        var evtBinaryreport = new Event
                        {
                            NodeId = x.SrcNodeId,
                            EventType = EventType.SENSOR_BINARY,
                            Value = sensorBinaryReport.sensorValue
                        };
                        EventDAO.InsertEventAsync(evtBinaryreport);
                        break;

                    default:
                        Common.logger.Info("Unhandled command class: {0}", nestedType.Name);
                        return;

                }

                //    SendBytes(udpClient, buffer);


            });

            controllerListener = controller.HandleControllerUpdate((r) =>
            {
                Common.logger.Info("{0}: Got {2} for node {1}", DateTime.Now, r.NodeId, r.Status);
                queueItems.Add(() => Common.RequestBatteryReport(controller, r.NodeId));

            });





            while (!queueItems.IsCompleted)
            {
                try
                {
                    var action = queueItems.Take();
                    action();

                }
                catch (InvalidOperationException) { }


            }

            Active = false;
            Common.logger.Info("Listening done!");
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
    }


    internal class ReconfigureCommand
    {
        private readonly Controller controller;
        private readonly List<ConfigItem> configList;

        public ReconfigureCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;
        }


        public readonly object lockObject = new object();



        internal void Start()
        {
            Common.logger.Info("-----------");
            Common.logger.Info("Listening mode");
            Common.logger.Info("Press Enter to exit");
            Common.logger.Info("-----------");

            Common.logger.Info("Loading available command classes...");
            var assembly = typeof(COMMAND_CLASS_BASIC).GetTypeInfo().Assembly;
            var commandClasses = Common.GetAllCommandClasses(assembly, "CommandClasses");
            Common.logger.Info("Got {0} command classes", commandClasses.Count);


            var nodesToCheck = new HashSet<byte>(controller.IncludedNodes);
            nodesToCheck.Remove(1);


            // ping all nodes
            // all nodes that respond are probably not battery powered and we can overwrite the configuration without delay
            Common.logger.Info("There are {0} to check/reconfigure. Pinging each node...", nodesToCheck.Count);
            foreach (var nodeId in nodesToCheck.ToList())
            {
                Common.logger.Info("Pinging node {0}...", nodeId);
                if (!Common.CheckReachable(controller, nodeId))
                {
                    Common.logger.Info("node {0} is not reachable, waiting for a wakuep.", nodeId);
                    continue;
                }

                Common.logger.Info("node {0} is reachable! Configure directly.", nodeId);
                nodesToCheck.Remove(nodeId);
                Common.logger.Info("{0} nodes left to check.", nodesToCheck.Count);
                new ConfigCommand(controller, nodeId, configList).Start();
            }

            Common.logger.Info("There are {0} nodes left to check/reconfigure. Waiting for wake up notifications...", nodesToCheck.Count);
            Common.logger.Info("Exit by pressing ENTER");

            var queueItems = new BlockingCollection<byte>(nodesToCheck.Count);

            bool inProgress = false;


            var token = controller.ListenData((x) =>
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


            var waitingForInput = Task.Run(() =>
            {
                Console.ReadLine();
                token.SetCompleted();
                queueItems.CompleteAdding();
            });


            while (!queueItems.IsCompleted)
            {
                try
                {
                    var nodeId = queueItems.Take();
                    //   Common.logger.Info("Got queue element for node {0}", nodeId);
                    //token.SetCancelled();
                    inProgress = true;
                    new ConfigCommand(controller, nodeId, configList).Start();
                    Common.logger.Info("{0} nodes left to check.", nodesToCheck.Count);
                    if (nodesToCheck.Count == 1)
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

        public void Start()
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

        }

        public void Stop()
        {
            running = false;
        }
    }
}
