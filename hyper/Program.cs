using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ZWave.BasicApplication;
using ZWave.BasicApplication.Devices;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Session;
using ZWave.Layers.Transport;

namespace hyper
{


    class ConfigItemList
    {
        public List<ConfigItem> configItems;
    }

    class ConfigItem
    {
        public string deviceName;
        public int manufacturerId;
        public int productTypeId;
        public Dictionary<byte, byte> groups = new Dictionary<byte, byte>();
        public Dictionary<string, ushort> config = new Dictionary<string, ushort>();
        public int wakeup;

    }

    class GroupConfig
    {
        public int identifier;
        public int member;
    }

    class ParameterConfig
    {
        public int parameter;
        public int value;
    }


    class Program
    {




        static void Main(string[] args)
        {
            Console.WriteLine("==== ZWave Command Center 5000 ====");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Loading device configuration database...");
            if (!File.Exists("config.yaml"))
            {
                Console.WriteLine("configuration file config.yaml does not exist!");
                return;
            }
            var config = ParseConfig("config.yaml");
            if (config == null)
            {
                Console.WriteLine("Could not parse configuration file config.yaml!");
                return;
            }
            Console.WriteLine("Got configuration for " + config.Count + " devices.");
            Console.WriteLine("-----------------------------------");

            if(args.Length  < 2)
            {
                Console.WriteLine("usage:");
                Console.WriteLine("./hyper [serialPort] [command]");
                Console.WriteLine("valid commands:");
                Console.WriteLine("r/replace, c/config, i/include, e/exclude");
                return;
            }

            var port = args[0];
            Console.WriteLine("Initialize Serialport: {0}", port);
            var initController = Common.InitController(port, out Controller controller, out string errorMessage);
            if (!initController)
            {
                Console.WriteLine("Error connecting with port {0}! Error Mesage:");
                Console.WriteLine(errorMessage);
                return;

            }
            Console.WriteLine("Version: {0}", controller.Version);
            Console.WriteLine("Included nodes: {0}", controller.IncludedNodes.Length);
            Console.WriteLine("-----------------------------------");

            Task.Delay(2000).Wait();


            if (args[1] == "r" || args[1] == "replace" || args[1] == "c" || args[1] == "config")
            {
              

                if (args.Length != 3)
                {
                    Console.WriteLine("wrong arguments!");
                    Console.WriteLine("correct usage:");
                    if(args[1] == "r" || args[1] == "replace")
                    {
                        Console.WriteLine("./hyper [serialPort] r [nodeid]");
                    } else
                    {
                        Console.WriteLine("./hyper [serialPort] c [nodeid]");
                    }
                    
                    return;
                }

                if (!byte.TryParse(args[2], out byte nodeId))
                {
                    Console.WriteLine("argument 1 should be node id! " + args[2] + " is not a number!");
                    return;

                }
               
               

               



                if (!controller.IncludedNodes.Contains(nodeId))
                {
                    Console.WriteLine("NodeID " + nodeId + " not included in network!");
                    Console.WriteLine(string.Join(", ", controller.IncludedNodes));
                    return;
                }

                if(args[1] == "r" || args[1] == "replace")
                {
                    new ReplaceCommand(controller, nodeId, config).Start();

                } else
                {
                    new ConfigCommand(controller, nodeId, config).Start();
                }


                controller.Disconnect();
                controller.Dispose();

            } else if(args[1] == "i" || args[1] == "include")
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("wrong arguments!");
                    Console.WriteLine("correct usage:");

                        Console.WriteLine("./hyper [serialPort] i");
                    


                    return;
                }


                    new IncludeCommand(controller, config).Start();




                controller.Disconnect();
                controller.Dispose();
            }
            else if (args[1] == "e" || args[1] == "exclude")
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("wrong arguments!");
                    Console.WriteLine("correct usage:");

                    Console.WriteLine("./hyper [serialPort] e");



                    return;
                }

              



                new ExcludeCommand(controller).Start();




                controller.Disconnect();
                controller.Dispose();
            }
            else
            {
                Console.WriteLine("unknown command: {0}", args[1]);
                Console.WriteLine("valid commands:");
                Console.WriteLine("r/replace, c/config, i/include");
            }







            Console.WriteLine("----------");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();

        }

        private static List<ConfigItem> ParseConfig(string configFile)
        {

            var yamlText = File.ReadAllText(configFile);
            var input = new StringReader(yamlText);


            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            List<ConfigItem> configList = null;
            try
            {
                configList = deserializer.Deserialize<List<ConfigItem>>(input);
            } catch{
              
            }
           
            return configList;


        }


    }

    internal class Common
    {
        public static TransmitOptions txOptions = TransmitOptions.TransmitOptionAcknowledge | TransmitOptions.TransmitOptionAutoRoute | TransmitOptions.TransmitOptionExplore;


        public static bool InitController(string port, out Controller controller, out string errorMessage)
        {
            BasicApplicationLayer AppLayer = new BasicApplicationLayer(
                new SessionLayer(),
                new BasicFrameLayer(),
                new SerialPortTransportLayer());
            var _controller = AppLayer.CreateController();

            if (_controller.Connect(new SerialPortDataSource(port, BaudRates.Rate_115200)) == CommunicationStatuses.Done)
            {
                //   Console.WriteLine("connection done!");
                var versionRes = _controller.GetVersion();
                if (versionRes)
                {
                    //     Console.WriteLine("connection success!!");
                    //  Console.WriteLine(versionRes.Version);
                    _controller.GetPRK();
                    _controller.SerialApiGetInitData();
                    _controller.SerialApiGetCapabilities();
                    _controller.GetControllerCapabilities();
                    _controller.GetSucNodeId();
                    _controller.MemoryGetId();
                    //      Console.WriteLine("Initialization done!");
                    //  Console.WriteLine("Included Nodes: " + string.Join(", ", controller.IncludedNodes));

                }
                else
                {
                    Console.WriteLine("could not get version...");
                    errorMessage = "Could not communicate with controller.";
                    controller = null;
                    return false;
                }

            }
            else
            {
                Console.WriteLine("could not connect to " + port);
                errorMessage = string.Format("Could not connect to port {0}", port);
                controller = null;
                return false;
            }
            controller = _controller;
            errorMessage = "";
            return true;
        }

        public static ConfigItem GetConfigurationForDevice(Controller controller, byte nodeId, List<ConfigItem> configList)
        {
            var gotDeviceIds = GetManufactor(controller, nodeId, out int manufacturerId, out int productTypeId);
            while (!gotDeviceIds)
            {
                Console.WriteLine("could not get device data! Trying again, wake up device!");
                gotDeviceIds = GetManufactor(controller, nodeId, out manufacturerId, out productTypeId);
            }
            var config = configList.Find(item => item.manufacturerId == manufacturerId && item.productTypeId == productTypeId);
            return config;
        }

        public static bool GetManufactor(Controller controller, byte nodeId, out int manufacturerId, out int productTypeId)
        {
            var cmd = new COMMAND_CLASS_MANUFACTURER_SPECIFIC.MANUFACTURER_SPECIFIC_GET();
            var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_MANUFACTURER_SPECIFIC.MANUFACTURER_SPECIFIC_REPORT(), 5000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_MANUFACTURER_SPECIFIC.MANUFACTURER_SPECIFIC_REPORT)result.Command;
                manufacturerId = Tools.GetInt32(rpt.manufacturerId);
                Console.WriteLine("ManufacturerId: " + manufacturerId);
                Console.WriteLine("ProductId: " + Tools.GetInt32(rpt.productId));
                productTypeId = Tools.GetInt32(rpt.productTypeId);
                Console.WriteLine("ProductTypeId: " + productTypeId);
                return true;
            }
            else
            {
                manufacturerId = 0;
                productTypeId = 0;
                return false;
            }
        }



        public static bool SetConfiguration(Controller controller, byte nodeId, ConfigItem config)
        {
            if (config.groups.Count != 0)
            {
                Console.WriteLine("Setting " + config.groups.Count + " associtions");
                foreach (var group in config.groups)
                {
                    var groupIdentifier = group.Key;
                    var member = group.Value;

                    var associationAdded = AddAssociation(controller, nodeId, groupIdentifier, member);
                    var associationValidated = false;
                    if (associationAdded)
                    {
                        associationValidated = AssociationContains(controller, nodeId, groupIdentifier, member);
                    }

                    while (!associationAdded || !associationValidated)
                    {
                        Console.WriteLine("Not successful! Trying again, please wake up device.");
                        associationAdded = AddAssociation(controller, nodeId, groupIdentifier, member);
                        if (associationAdded)
                        {
                            associationValidated = AssociationContains(controller, nodeId, groupIdentifier, member);

                        }
                    }
                }
            }

            if (config.config.Count != 0)
            {
                Console.WriteLine("Setting " + config.config.Count + " configuration parameter");
                foreach (var configurationEntry in config.config)
                {
                    var configParameter = configurationEntry.Key;
                    var configValue = configurationEntry.Value;


                    var parameterSet = SetParameter(controller, nodeId, configParameter, configValue);
                    var parameterValidated = false;
                    if (parameterSet)
                    {
                        parameterValidated = ValidateParameter(controller, nodeId, configParameter, configValue);
                    }
                    while (!parameterSet || !parameterValidated)
                    {
                        Console.WriteLine("Not successful! Trying again, please wake up device.");
                        parameterSet = SetParameter(controller, nodeId, configParameter, configValue);
                        if (parameterSet)
                        {
                            parameterValidated = ValidateParameter(controller, nodeId, configParameter, configValue);
                        }
                    }


                }
            }

            return true;
        }

        public static bool SetParameter(Controller controller, byte nodeId, string configParameterLong, ushort configValue)
        {

            var configParameter = byte.Parse(configParameterLong.Split("_")[0]);
            var configWordSize = byte.Parse(configParameterLong.Split("_")[1]);

            Console.WriteLine("Set configuration - parameter " + configParameter + " - value " + configValue);
            COMMAND_CLASS_CONFIGURATION.CONFIGURATION_SET cmd = new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_SET();
            cmd.parameterNumber = configParameter;
            if (configWordSize == 1)
            {
                cmd.configurationValue = new byte[] { (byte)configValue };
            }
            else if (configWordSize == 2)
            {
                cmd.configurationValue = Tools.GetBytes(configValue);
            }
            else
            {
                cmd.configurationValue = Tools.GetBytes((int)configValue);
            }
            //else
            //{
            //    Console.WriteLine("configuration parameter {0}: wordSize {1} not implemented!");
            //    return false;
            //}
            // Tools.GetBytes(((byte)configValue));
            cmd.properties1.mdefault = 0;
            cmd.properties1.size = configWordSize;


            var setAssociation = controller.SendData(nodeId, cmd, Common.txOptions);
            return setAssociation.TransmitStatus == TransmitStatuses.CompleteOk;
        }

        public static bool ValidateParameter(Controller controller, byte nodeId, string configParameterLong, ushort configValue)
        {
            var configParameter = byte.Parse(configParameterLong.Split("_")[0]);
            Console.WriteLine("Validate configuration - parameter " + configParameter + " - value " + configValue);
            COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET cmd = new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET();
            cmd.parameterNumber = configParameter;
            var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT(), 20000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT)result.Command;
                var value = Tools.GetInt32(rpt.configurationValue.ToArray());
                if (configValue == value)
                {
                    Console.WriteLine("parameter ist set correctly!");
                    return true;
                }
                else
                {
                    Console.WriteLine("parametr has value {0} instead of {1}", value, configValue);
                    return false;
                }

            }
            else
            {
                Console.WriteLine("Could not get configuration parameter!");
                return false;
            }
        }

        public static bool AssociationContains(Controller controller, byte nodeId, byte groupIdentifier, byte member)
        {
            Console.WriteLine("Validate associtation - group " + groupIdentifier + " - node " + member);

            var cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_GET();
            cmd.groupingIdentifier = groupIdentifier;
            var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT(), 20000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT)result.Command;
                if (rpt.nodeid.Contains(member))
                {
                    Console.WriteLine(member + " is a member of association group " + groupIdentifier);
                    return true;
                }
                else
                {
                    Console.WriteLine(member + " is not a member of association group " + groupIdentifier);
                    return false;
                }


            }
            else
            {
                Console.WriteLine("could not get association for group " + groupIdentifier);
                return false;
            }

        }

        public static bool AddAssociation(Controller controller, byte nodeId, byte groupIdentifier, byte member)
        {
            Console.WriteLine("Add associtation - group " + groupIdentifier + " - node " + member);
            COMMAND_CLASS_ASSOCIATION.ASSOCIATION_SET cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_SET();
            cmd.groupingIdentifier = groupIdentifier;
            cmd.nodeId = new List<byte>() { member };
            var setAssociation = controller.SendData(nodeId, cmd, Common.txOptions);
            return setAssociation.TransmitStatus == TransmitStatuses.CompleteOk;
        }


        public static bool ReplaceNode(Controller controller, byte nodeId)
        {
            var replacedNode = controller.ReplaceFailedNode(nodeId);
            return replacedNode.AddRemoveNode.AddRemoveNodeStatus == ZWave.BasicApplication.Enums.AddRemoveNodeStatuses.Replaced;

        }

        public static bool IncludeNode(Controller controller,out byte nodeId)
        {
            var includeNode = controller.IncludeNode(Modes.None, 10000);
            nodeId = includeNode.AddRemoveNode.Id;
            if (nodeId == 0)
            {
                return false;
            }
            return true;
        }

        public static bool ExcludeNode(Controller controller, out byte nodeId)
        {
            var excludeNode = controller.ExcludeNode(Modes.None, 10000);
            nodeId = excludeNode.AddRemoveNode.Id;
            if (nodeId == 0)
            {
                return false;
            }
            return true;
        }

        public static bool MarkNodeFailed(Controller controller, byte nodeId)
        {
            var isFailed = controller.IsFailedNode(nodeId);
            return isFailed.RetValue;
        }

        public static bool CheckNotReachable(Controller controller, byte nodeId)
        {
            var sendData = controller.SendData(nodeId, new byte[1], Common.txOptions);
            return sendData.TransmitStatus == TransmitStatuses.CompleteNoAcknowledge;
        }

        //public static int GetValueWithBitmask(int value, int bitmask)
        //{
        //    value = value & bitmask;
        //    int bits = bitmask;
        //    while ((bits & 0x01) == 0)
        //    {
        //        value = value >> 1;
        //        bits = bits >> 1;
        //    }


        //    return value;
        //}


        //public static void GetAssociation(Controller controller, byte nodeId)
        //{
        //    for (byte id = 1; id <= 5; id++)
        //    {
        //        var cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_GET();
        //        cmd.groupingIdentifier = id;
        //        var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT(), 20000);
        //        if (result)
        //        {
        //            var rpt = (COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT)result.Command;
        //            var _groupId = rpt.groupingIdentifier;
        //            if (_groupId != id)
        //            {
        //                continue;
        //            }
        //            var maxSupported = rpt.maxNodesSupported;
        //            var reportToFollow = rpt.reportsToFollow;
        //            var member = string.Join(", ", rpt.nodeid);

        //            Console.WriteLine("Group: {0} - Member: {1} - Max: {2} - Follow: {3}", _groupId, member, maxSupported, reportToFollow);
        //        }
        //    }

        //}


        //public static void GetConfig(Controller controller, byte nodeId)
        //{
        //    for (byte parameterNumber = 1; parameterNumber < 100; parameterNumber++)
        //    {
        //        var cmd = new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET();
        //        cmd.parameterNumber = parameterNumber;
        //        RequestDataResult result;
        //        result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT(), 100);
        //        if (result)
        //        {
        //            var rpt = (COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT)result.Command;

        //            var _parameterNumber = rpt.parameterNumber;
        //            if (_parameterNumber != parameterNumber)
        //            {
        //                continue;
        //            }
        //            var value = Tools.GetInt32(rpt.configurationValue.ToArray());


        //            Console.WriteLine("ParameterNumber: {0} - {1}", _parameterNumber, value);
        //        }
        //        else
        //        {
        //            //  Console.WriteLine("Parameter {0} does not exist!", parameterNumber);
        //        }
        //    }

        //}

    }


    internal class ExcludeCommand
    {




        private Controller controller;

        public ExcludeCommand(Controller controller)
        {
            this.controller = controller;
        }

        internal void Start()
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Exclusion mode");
            Console.WriteLine("-----------");

            Console.WriteLine("Starting exclusion, please wake up device...");

            var nodeExcluded = Common.ExcludeNode(controller, out byte nodeId);
            while (!nodeExcluded)
            {
                Console.WriteLine("Could not exclude any node, trying again...");
                nodeExcluded = Common.ExcludeNode(controller, out nodeId);
            }

            Console.WriteLine("Success! node with id: {0} excluded.", nodeId);


            Console.WriteLine("Exclusion done!");
        }
    }

        internal class IncludeCommand
    {




        private Controller controller;
        private byte nodeId;
        private List<ConfigItem> configList;

        public IncludeCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;
        }

        internal void Start()
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Inclusion mode");
            Console.WriteLine("-----------");

            Console.WriteLine("Starting inclusion, please wake up device...");

            var nodeIncluded = Common.IncludeNode(controller, out byte nodeId);
            while (!nodeIncluded)
            {
                Console.WriteLine("Could not include any node, trying again...");
                nodeIncluded = Common.IncludeNode(controller, out nodeId);
            }

            Console.WriteLine("Success! New node id: {0}", nodeId);

            new ConfigCommand(controller, nodeId, configList).Start();


            Console.WriteLine("Inclusion done!");



        }
    }

    internal class ConfigCommand
    {
      



            private Controller controller;
            private byte nodeId;
            private List<ConfigItem> configList;

            public ConfigCommand(Controller controller, byte nodeId, List<ConfigItem> configList)
            {
                this.controller = controller;
                this.nodeId = nodeId;
                this.configList = configList;
            }

            internal void Start()
            {
                Console.WriteLine("-----------");
                Console.WriteLine("Configuration mode");
                Console.WriteLine("node to configure: " + nodeId);
                Console.WriteLine("-----------");




                Console.WriteLine("Getting configuration for device...");
                ConfigItem config = Common.GetConfigurationForDevice(controller, nodeId, configList);
                if (config == null)
                {
                    Console.WriteLine("could not find configuration!");
                    Console.WriteLine("you need to add this device to the configuration file.");
                    return;
                }

                Console.WriteLine("configuration found for {0}!", config.deviceName);
                Console.WriteLine("Setting values.");
                Common.SetConfiguration(controller, nodeId, config);

                Console.WriteLine("Configuration done!");



            }
        }

        internal class ReplaceCommand
    {

         

        private Controller controller;
        private byte nodeId;
        private List<ConfigItem> configList;

        public ReplaceCommand(Controller controller, byte nodeId, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.nodeId = nodeId;
            this.configList = configList;
        }

        internal void Start()
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Replacement mode");
            Console.WriteLine("node to replace: " + nodeId);
            Console.WriteLine("-----------");
            Console.WriteLine("Check if node is reachable...");
            var notReachable = Common.CheckNotReachable(controller, nodeId);
            if (!notReachable)
            {
                Console.WriteLine("Node is reachable!");
                Console.WriteLine("If node is reachable, we cannot replace it!");
                return;
            }
            else
            {
                Console.WriteLine("OK, node is not reachable");
            }
            Console.WriteLine("Mark node as failed...");
            var markedAsFailed = Common.MarkNodeFailed(controller, nodeId);
            if (!markedAsFailed)
            {
                Console.WriteLine("Node could not be marked as failed!");
                Console.WriteLine("Try again and ensure that node is not reachable.");
                return;
            }
            else
            {
                Console.WriteLine("OK, node is marked as failed");
            }
            Console.WriteLine("Replacing Node... Set new device to inclusion mode!");
            bool nodeReplaced = Common.ReplaceNode(controller, nodeId);
            if (!nodeReplaced)
            {
                Console.WriteLine("Could not replace device!");
                Console.WriteLine("Please try again.");
                return;
            }
            else
            {
                Console.WriteLine("Node sucessfully replaced!");
            }

            //     Console.WriteLine("Write new Configuration...");
            //     bool configurationSet = SetConfiguration();

            //   Console.WriteLine("Get Association");
            //  GetAssociation();
            //GetConfig();

            //   GetManufactor();
            //GetWakeUp();
            //Console.ReadLine();

            new ConfigCommand(controller, nodeId, configList).Start();



            Console.WriteLine("Replacement done!");




        }






        //private void GetWakeUp()
        //{
        //    var cmd = new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_GET();
        //    var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT(), 20000);
        //    if (result)
        //    {
        //        var rpt = (COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT)result.Command;
        //        Console.WriteLine("wake up interval: " + Tools.GetInt32(rpt.seconds));

        //    }
        //    else
        //    {
        //        Console.WriteLine("Could Not get wake up!!");
        //    }
        //}

        







        


    }
}
