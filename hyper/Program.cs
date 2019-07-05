using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ZWave.BasicApplication;
using ZWave.BasicApplication.Devices;
using ZWave.BasicApplication.Operations;
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
        public int manufacturerId;
        public int productTypeId;
        public Dictionary<byte, byte> groups = new Dictionary<byte, byte>();
        public Dictionary<byte, ushort> config = new Dictionary<byte, ushort>();
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
        enum VERB
        {
            REPLACE
        }


        public static Controller InitController(string port)
        {
            BasicApplicationLayer AppLayer = new BasicApplicationLayer(
                new SessionLayer(),
                new BasicFrameLayer(),
                new SerialPortTransportLayer());
            var controller = AppLayer.CreateController();
            
            if (controller.Connect(new SerialPortDataSource(port, BaudRates.Rate_115200)) == CommunicationStatuses.Done)
            {
                Console.WriteLine("connection done!");
                var versionRes = controller.GetVersion();
                if (versionRes)
                {
                    Console.WriteLine("connection success!!");
                    Console.WriteLine(versionRes.Version);
                    controller.GetPRK();
                    controller.SerialApiGetInitData();
                    controller.SerialApiGetCapabilities();
                    controller.GetControllerCapabilities();
                    controller.GetSucNodeId();
                    controller.MemoryGetId();
                    Console.WriteLine("Initialization done!");
                  //  Console.WriteLine("Included Nodes: " + string.Join(", ", controller.IncludedNodes));

                }
                else
                {
                    Console.WriteLine("could not get version...");
                    return null;
                }

            }
            else
            {
                Console.WriteLine("could not connect to " + port);
                return null;
            }
            return controller;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("====ZWave Node Replace 5000====");
            if(args.Length != 3)
            {
                Console.WriteLine("wrong arguments!");
                Console.WriteLine("correct usage:");
                Console.WriteLine("./hyper [serialPort] [nodeid] [configurationFile]");
                return;
            }
            var port = args[0];
            if (!byte.TryParse(args[1], out byte nodeId))
            {
                Console.WriteLine("argument 1 should be node id! " + args[1] + " is not a number!");
                return;

            }
            if (!File.Exists(args[2]))
            {
                Console.WriteLine("configuration file " + args[2] + " does not exist!");
                return;

            }
            var config = ParseConfig(args[2]);
            if(config == null)
            {
                Console.WriteLine("Could not parse file " + args[2]);
                return;
            }
            Console.WriteLine("Got configuration for " + config.Count + " devices!");

            var controller = InitController(port);
            if(controller == null)
            {
                return;

            }



            if (!controller.IncludedNodes.Contains(nodeId))
            {
                Console.WriteLine("NodeID " + nodeId + " not included in network!");
                Console.WriteLine(string.Join(", ", controller.IncludedNodes));
                return;
            }

            new ReplaceCommand(controller, nodeId, config).Start();

            


            controller.Disconnect();
            controller.Dispose();

            Console.ReadLine();

            //new ReplaceCommand(controller, nodeId, config);



            //ValidateCommandLineArguments(args);


            //VERB command = (VERB) Enum.Parse(typeof(VERB), args[0], true);

            //switch (command)
            //{
            //    case VERB.REPLACE:
            //        var nodeFrom = byte.Parse(args[1]);
            //        var configParameter = ParseConfig(args[2]);
            //        //Console.WriteLine(string.Join(", ", configParameter));
            //        new ReplaceCommand(nodeFrom, configParameter);
            //        break;
            //}
            //Console.WriteLine(command);
            //return;

            //BasicApplicationLayer AppLayer = new BasicApplicationLayer(
            //    new SessionLayer(),
            //    new BasicFrameLayer(),
            //    new SerialPortTransportLayer());
            //var controller = AppLayer.CreateController();
            //if (controller.Connect(new SerialPortDataSource("/dev/ttyACM0", BaudRates.Rate_115200)) == CommunicationStatuses.Done)
            //{
            //    Console.WriteLine("connection done!");
            //    var versionRes = controller.GetVersion();
            //    if (versionRes)
            //    {
            //        Console.WriteLine("connection success!!");
            //        Console.WriteLine(versionRes.Version);
            //        controller.GetPRK();
            //        controller.SerialApiGetInitData();
            //        controller.SerialApiGetCapabilities();
            //        controller.GetControllerCapabilities();
            //        controller.GetSucNodeId();
            //        controller.MemoryGetId();
            //        Console.WriteLine("Initialization done!");
            //        Console.WriteLine("Included Nodes: " + string.Join(", ", controller.IncludedNodes));

            //    } else
            //    {
            //        Console.WriteLine("could not get version...");
            //    }

            //} else
            //{
            //    Console.WriteLine("could not connect...");
            //}
            //controller.Disconnect();
            //controller.Dispose();
            //Console.ReadLine();
        }

        private static List<ConfigItem> ParseConfig(string configFile)
        {

            var yamlText = File.ReadAllText("yaml.yaml");
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

            //var configParameter = new List<KeyValuePair<string, string>>();
            //var allLines = File.ReadAllLines(configFile);
            //foreach (var line in allLines)
            //{
            //    var split = line.Split(":");
            //    if(split.Length != 2)
            //    {
            //        continue;
            //    }
            //    var key = split[0].Trim();
            //    var value = split[1].Trim();
            //    var parameter = new KeyValuePair<string, string>(key, value);
            //    configParameter.Add(parameter);
            //}
            //return configParameter;
        }



        private static bool ValidConfig(string configFile)
        {
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Config file " + configFile + " does not exist!");
                return false;
            }
            return true;
        }
    }

    internal class ReplaceCommand
    {

        private TransmitOptions txOptions = TransmitOptions.TransmitOptionAcknowledge | TransmitOptions.TransmitOptionAutoRoute | TransmitOptions.TransmitOptionExplore;

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
            Console.WriteLine("node to replace: " + nodeId);
            Console.WriteLine("-----------");
            Console.WriteLine("Check if node is reachable...");
            //var notReachable = CheckNotReachable();
            //if (!notReachable)
            //{
            //    Console.WriteLine("Node is reachable!");
            //    Console.WriteLine("If node is reachable, we cannot replace it!");
            //    return;
            //} else
            //{
            //    Console.WriteLine("OK, node is not reachable");
            //}
            //Console.WriteLine("Mark node as failed...");
            //var markedAsFailed = MarkNodeFailed();
            //if (!markedAsFailed)
            //{
            //    Console.WriteLine("Node could not be marked as failed!");
            //    Console.WriteLine("Try again and ensure that node is not reachable.");
            //    return;
            //} else
            //{
            //    Console.WriteLine("OK, node is marked as failed");
            //}
            //Console.WriteLine("Replacing Node... Set new device to inclusion mode!");
            //bool nodeReplaced = ReplaceNode();
            //if (!nodeReplaced)
            //{
            //    Console.WriteLine("Could not replace device!");
            //    Console.WriteLine("Please try again.");
            //    return;
            //} else
            //{
            //    Console.WriteLine("Node sucessfully replaced!");
            //}

            //     Console.WriteLine("Write new Configuration...");
            //     bool configurationSet = SetConfiguration();

            //   Console.WriteLine("Get Association");
            //  GetAssociation();
            //GetConfig();

            //   GetManufactor();
            //GetWakeUp();
            //Console.ReadLine();



            // Console.WriteLine(order);
            Console.WriteLine("Getting configuration for device...");
           ConfigItem config = GetConfigurationForDevice();
            if(config == null)
            {
                Console.WriteLine("could not find configuration!");
                Console.WriteLine("you need to add this device to the configuration file.");
                return;
            }

            Console.WriteLine("configuration found! Setting values.");
            SetConfiguration(config);


          //  GetConfig();



        }

        private ConfigItem GetConfigurationForDevice()
        {
            var gotDeviceIds = GetManufactor(out int manufacturerId, out int productTypeId);
            while (!gotDeviceIds)
            {
                Console.WriteLine("could not get device data! Trying again, wake up device!");
                gotDeviceIds = GetManufactor(out manufacturerId, out productTypeId);
            }
            var config = configList.Find(item => item.manufacturerId == manufacturerId && item.productTypeId == productTypeId);
            return config;
        }

        private void GetWakeUp()
        {
            var cmd = new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_GET();
            var result = controller.RequestData(nodeId, cmd, txOptions, new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT(), 20000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT)result.Command;
                Console.WriteLine("wake up interval: " + Tools.GetInt32(rpt.seconds));

            }
            else
            {
                Console.WriteLine("Could Not get wake up!!");
            }
        }

        private bool GetManufactor(out int manufacturerId, out int productTypeId)
        {
            var cmd = new COMMAND_CLASS_MANUFACTURER_SPECIFIC.MANUFACTURER_SPECIFIC_GET();
            var result = controller.RequestData(nodeId, cmd, txOptions, new COMMAND_CLASS_MANUFACTURER_SPECIFIC.MANUFACTURER_SPECIFIC_REPORT(), 5000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_MANUFACTURER_SPECIFIC.MANUFACTURER_SPECIFIC_REPORT)result.Command;
                manufacturerId = Tools.GetInt32(rpt.manufacturerId);
                Console.WriteLine("ManufacturerId: " + manufacturerId);
                Console.WriteLine("ProductId: " + Tools.GetInt32(rpt.productId));
                productTypeId = Tools.GetInt32(rpt.productTypeId);
                Console.WriteLine("ProductTypeId: " + productTypeId);
                return true;
            } else
            {
                manufacturerId = 0;
                productTypeId = 0;
                return false;
            }
        }

        private void GetAssociation()
        {
            for(byte id = 1; id <= 5; id++)
            {
                var cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_GET();
                cmd.groupingIdentifier = id;
                var result = controller.RequestData(nodeId, cmd, txOptions, new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT(), 20000);
                if (result)
                {
                    var rpt = (COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT)result.Command;
                    var _groupId = rpt.groupingIdentifier;
                    if(_groupId != id)
                    {
                        continue;
                    }
                    var maxSupported = rpt.maxNodesSupported;
                    var reportToFollow = rpt.reportsToFollow;
                    var member = string.Join(", ", rpt.nodeid);

                    Console.WriteLine("Group: {0} - Member: {1} - Max: {2} - Follow: {3}", _groupId, member, maxSupported, reportToFollow);
                }
            }
            
        }


        private void GetConfig()
        {
            for(byte parameterNumber = 1; parameterNumber < 100; parameterNumber++)
            {
                var cmd = new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET();
                cmd.parameterNumber = parameterNumber;
                RequestDataResult result;
                result = controller.RequestData(nodeId, cmd, txOptions, new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT(), 100);
                if (result)
                {
                    var rpt = (COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT)result.Command;
                    
                    var _parameterNumber = rpt.parameterNumber;
                    if (_parameterNumber != parameterNumber)
                    {
                        continue;
                    }
                    var value = Tools.GetInt32(rpt.configurationValue.ToArray());
                    

                    Console.WriteLine("ParameterNumber: {0} - {1}", _parameterNumber, value);
                } else
                {
                  //  Console.WriteLine("Parameter {0} does not exist!", parameterNumber);
                }
            }
          
        }




        private bool SetConfiguration(ConfigItem config)
        {
            if(config.groups.Count != 0)
            {
                Console.WriteLine("Setting " + config.groups.Count + " associtions");
                foreach(var group in config.groups)
                {
                    var groupIdentifier = group.Key;
                    var member = group.Value;

                    var associationAdded = AddAssociation(groupIdentifier, member);
                    var associationValidated = false;
                    if (associationAdded)
                    {
                        associationValidated = AssociationContains(groupIdentifier, member);
                    }

                    while (!associationAdded || !associationValidated)
                    {
                        Console.WriteLine("Not successful! Trying again, please wake up device.");
                        associationAdded = AddAssociation(groupIdentifier, member);
                        if (associationAdded)
                        {
                            associationValidated = AssociationContains(groupIdentifier, member);

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


                    var parameterSet = SetParameter(configParameter, configValue);
                    var parameterValidated = false;
                    if (parameterSet)
                    {
                        parameterValidated = ValidateParameter(configParameter, configValue);
                    }
                    while (!parameterSet || !parameterValidated)
                    {
                        Console.WriteLine("Not successful! Trying again, please wake up device.");
                        parameterSet = SetParameter(configParameter, configValue);
                        if (parameterSet)
                        {
                            parameterValidated = ValidateParameter(configParameter, configValue);
                        }
                    }


                }
            }

            //    foreach (var configItem in config)
            //{
            //    var key = configItem.Key;
            //    var value = configItem.Value;
            //    if (key.StartsWith("group_"))
            //    {
            //        var groupSplit = key.Split("_");
            //        var groupIdentifier = byte.Parse(groupSplit[1]);
            //        var member = byte.Parse(value);

            //        var associationAdded = AddAssociation(groupIdentifier, member);
            //        var associationValidated = false;
            //        if (associationAdded)
            //        {
            //            associationValidated = AssociationContains(groupIdentifier, member);
            //        }

            //        while (!associationAdded || !associationValidated)
            //        {
            //            Console.WriteLine("Not successful! Trying again, please wake up device.");
            //            associationAdded = AddAssociation(groupIdentifier, member);
            //            if (associationAdded)
            //            {
            //                associationValidated = AssociationContains(groupIdentifier, member);

            //            }
            //        }

            //    }
            //    else if (key.StartsWith("config_"))
            //    {
            //        var configSplit = key.Split("_");
            //        var configParameter = byte.Parse(configSplit[1]);
            //        var configValue = ushort.Parse(value);

            //        var parameterSet = SetParameter(configParameter, configValue);
            //        var parameterValidated = false;
            //        if (parameterSet)
            //        {
            //            parameterValidated = ValidateParameter(configParameter, configValue);
            //        }
            //        while (!parameterSet || !parameterValidated)
            //        {
            //            Console.WriteLine("Not successful! Trying again, please wake up device.");
            //            parameterSet = SetParameter(configParameter, configValue);
            //            if (parameterSet)
            //            {
            //                parameterValidated = ValidateParameter(configParameter, configValue);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("Unknown identifier: " + key);
            //    }
           // }
            return true;
        }

        private bool SetParameter(byte configParameter, ushort configValue)
        {
            Console.WriteLine("Set configuration - parameter " + configParameter + " - value " + configValue);
            COMMAND_CLASS_CONFIGURATION.CONFIGURATION_SET cmd = new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_SET();
            cmd.parameterNumber = configParameter;
            cmd.configurationValue = Tools.GetBytes(configValue);
            cmd.properties1.mdefault = 0;
            cmd.properties1.size = 2;

            var setAssociation = controller.SendData(nodeId, cmd, txOptions);
            return setAssociation.TransmitStatus == TransmitStatuses.CompleteOk;
        }

        private bool ValidateParameter(byte configParameter, ushort configValue)
        {
            Console.WriteLine("Validate configuration - parameter " + configParameter + " - value " + configValue);
            COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET cmd = new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET();
            cmd.parameterNumber = configParameter;
            var result = controller.RequestData(nodeId, cmd, txOptions, new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT(), 20000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT)result.Command;
                var value = Tools.GetInt32(rpt.configurationValue.ToArray());
                if(configValue == value)
                {
                    Console.WriteLine("parameter ist set correctly!");
                    return true;
                } else
                {
                    Console.WriteLine("parametr has value {0} instead of {1}", value, configValue);
                    return false;
                }
                
            } else
            {
                Console.WriteLine("Could not get configuration parameter!");
                return false;
            }
        }

        private bool AssociationContains(byte groupIdentifier, byte member)
        {
            Console.WriteLine("Validate associtation - group " + groupIdentifier + " - node " + member);

            var cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_GET();
            cmd.groupingIdentifier = groupIdentifier;
            var result = controller.RequestData(nodeId, cmd, txOptions, new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT(), 20000);
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


            } else
            {
                Console.WriteLine("could not get association for group " + groupIdentifier);
                return false;
            }

        }

        private bool AddAssociation(byte groupIdentifier, byte member)
        {
            Console.WriteLine("Add associtation - group " + groupIdentifier + " - node " + member);
            COMMAND_CLASS_ASSOCIATION.ASSOCIATION_SET cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_SET();
            cmd.groupingIdentifier = groupIdentifier;
            cmd.nodeId = new List<byte>() { member };
            var setAssociation = controller.SendData(nodeId, cmd, txOptions);
            return setAssociation.TransmitStatus == TransmitStatuses.CompleteOk;
        }

        private bool ReplaceNode()
        {
            var replacedNode = controller.ReplaceFailedNode(nodeId);
            return replacedNode.AddRemoveNode.AddRemoveNodeStatus == ZWave.BasicApplication.Enums.AddRemoveNodeStatuses.Replaced;
            
        }

        private bool MarkNodeFailed()
        {
            var isFailed = controller.IsFailedNode(nodeId);
            return isFailed.RetValue;
        }

        private bool CheckNotReachable()
        {
            var sendData = controller.SendData(nodeId, new byte[1], txOptions);
          //  Console.WriteLine(sendData);
            return sendData.TransmitStatus == TransmitStatuses.CompleteNoAcknowledge;

        }
    }
}
