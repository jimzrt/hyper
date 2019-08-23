using hyper.config;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utils;
using ZWave.BasicApplication;
using ZWave.BasicApplication.Devices;
using ZWave.BasicApplication.Tasks;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Session;
using ZWave.Layers.Transport;

namespace hyper
{
    public class Common
    {
        public static TransmitOptions txOptions = TransmitOptions.TransmitOptionAcknowledge | TransmitOptions.TransmitOptionAutoRoute | TransmitOptions.TransmitOptionExplore;
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static bool InitController(string port, out Controller controller, out string errorMessage)
        {
            BasicApplicationLayer AppLayer = new BasicApplicationLayer(
                new SessionLayer(),
                new BasicFrameLayer(),
                new SerialPortTransportLayer());
            var _controller = AppLayer.CreateController();
            var serialPortDataSource = new SerialPortDataSource(port, BaudRates.Rate_115200);
            var controllerConnected = _controller.Connect(serialPortDataSource);
            if (controllerConnected != CommunicationStatuses.Done)
            {
                for (int i = 2; i < 5; i++)
                {
                    Common.logger.Info("Serialport - connect: try {0}", i);
                    controllerConnected = _controller.Connect(serialPortDataSource);
                    if (controllerConnected == CommunicationStatuses.Done)
                    {
                        break;
                    }
                }
            }
            if (controllerConnected == CommunicationStatuses.Done)
            {
                //   Common.logger.Info("connection done!");
                var versionRes = _controller.GetVersion();
                if (!versionRes)
                {
                    for (int i = 2; i < 5; i++)
                    {
                        Common.logger.Info("Controller - get version: try {0}", i);
                        versionRes = _controller.GetVersion();
                        if (versionRes)
                        {
                            break;
                        }
                    }
                }
                if (versionRes)
                {
                    //     Common.logger.Info("connection success!!");
                    //  Common.logger.Info(versionRes.Version);
                    _controller.GetPRK();
                    _controller.SerialApiGetInitData();
                    _controller.SerialApiGetCapabilities();
                    _controller.GetControllerCapabilities();
                    _controller.GetSucNodeId();
                    _controller.MemoryGetId();
                    //      Common.logger.Info("Initialization done!");
                    //  Common.logger.Info("Included Nodes: " + string.Join(", ", controller.IncludedNodes));

                }
                else
                {
                    //   Common.logger.Info("could not get version...");
                    errorMessage = "Could not communicate with controller.";
                    _controller.Disconnect();
                    _controller.Dispose();
                    controller = null;
                    return false;
                }

            }
            else
            {
                // Common.logger.Info("could not connect to " + port);
                errorMessage = string.Format("Could not connect to port {0}. Reason: {1}", port, controllerConnected);
                _controller.Disconnect();
                _controller.Dispose();
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
                Common.logger.Info("could not get device data! Trying again, wake up device!");
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
                Common.logger.Info("ManufacturerId: " + manufacturerId);
                Common.logger.Info("ProductId: " + Tools.GetInt32(rpt.productId));
                productTypeId = Tools.GetInt32(rpt.productTypeId);
                Common.logger.Info("ProductTypeId: " + productTypeId);
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
            if (config.groups != null && config.groups.Count != 0)
            {
                Common.logger.Info("Setting " + config.groups.Count + " associtions");
                foreach (var group in config.groups)
                {
                    var groupIdentifier = group.Key;
                    var member = group.Value;

                    bool associationCleared = ClearAssiciation(controller, nodeId, groupIdentifier);
                    while (!associationCleared)
                    {
                        Common.logger.Info("Not successful! Trying again, please wake up device.");
                        associationCleared = ClearAssiciation(controller, nodeId, groupIdentifier);
                    }

                    var associationAdded = AddAssociation(controller, nodeId, groupIdentifier, member);
                    var associationValidated = false;
                    if (associationAdded)
                    {
                        associationValidated = AssociationContains(controller, nodeId, groupIdentifier, member);
                    }

                    while (!associationAdded || !associationValidated)
                    {
                        Common.logger.Info("Not successful! Trying again, please wake up device.");
                        associationAdded = AddAssociation(controller, nodeId, groupIdentifier, member);
                        if (associationAdded)
                        {
                            associationValidated = AssociationContains(controller, nodeId, groupIdentifier, member);

                        }
                    }
                }
            }

            if (config.config != null && config.config.Count != 0)
            {
                Common.logger.Info("Setting " + config.config.Count + " configuration parameter");
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
                        Common.logger.Info("Not successful! Trying again, please wake up device.");
                        parameterSet = SetParameter(controller, nodeId, configParameter, configValue);
                        if (parameterSet)
                        {
                            parameterValidated = ValidateParameter(controller, nodeId, configParameter, configValue);
                        }
                    }


                }
            }
            if(config.wakeup != 0)
            {
               // var wakeupSet = SetWakeup(controller, nodeId, config.wakeup);
              //  if (wakeupSet)
              //  {
                    GetWakeUp(controller, nodeId);
             //   }
            }

            return true;
        }


        public static bool SetWakeup(Controller controller, byte nodeId, int configValue)
        {
            Common.logger.Info("Set Wakeup - value " + configValue);
            COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_SET cmd = new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_SET();
            cmd.nodeid = nodeId;
            cmd.seconds = Tools.GetBytes(configValue);
            Console.WriteLine(cmd.seconds[0]);
            Console.WriteLine(cmd.seconds[1]);
            Console.WriteLine(cmd.seconds[2]);
            Console.WriteLine(cmd.seconds[3]);
            var setWakeup = controller.SendData(nodeId, cmd, Common.txOptions);
            return setWakeup.TransmitStatus == TransmitStatuses.CompleteOk;
        }



        public static bool GetWakeUp(Controller controller, byte nodeId)
        {
            var cmd = new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_GET();
            var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT(), 20000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_REPORT)result.Command;
                logger.Info("wake up interval: " + Tools.GetInt32(rpt.seconds));

            }
            else
            {
                Common.logger.Info("Could Not get wake up!!");
            }
            return result;
        }

        public static bool SetParameter(Controller controller, byte nodeId, string configParameterLong, ushort configValue)
        {

            var configParameter = byte.Parse(configParameterLong.Split("_")[0]);
            var configWordSize = byte.Parse(configParameterLong.Split("_")[1]);

            Common.logger.Info("Set configuration - parameter " + configParameter + " - value " + configValue);
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
            //    Common.logger.Info("configuration parameter {0}: wordSize {1} not implemented!");
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
            Common.logger.Info("Validate configuration - parameter " + configParameter + " - value " + configValue);
            COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET cmd = new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_GET();
            cmd.parameterNumber = configParameter;
            var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT(), 20000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_CONFIGURATION.CONFIGURATION_REPORT)result.Command;
                var value = Tools.GetInt32(rpt.configurationValue.ToArray());
                if (configValue == value)
                {
                    Common.logger.Info("parameter ist set correctly!");
                    return true;
                }
                else
                {
                    Common.logger.Info("parametr has value {0} instead of {1}", value, configValue);
                    return false;
                }

            }
            else
            {
                Common.logger.Info("Could not get configuration parameter!");
                return false;
            }
        }

        public static bool AssociationContains(Controller controller, byte nodeId, byte groupIdentifier, byte member)
        {
            Common.logger.Info("Validate associtation - group " + groupIdentifier + " - node " + member);

            var cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_GET();
            cmd.groupingIdentifier = groupIdentifier;
            var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT(), 20000);
            if (result)
            {
                var rpt = (COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REPORT)result.Command;
                if (rpt.nodeid.Contains(member))
                {
                    Common.logger.Info(member + " is a member of association group " + groupIdentifier);
                    return true;
                }
                else
                {
                    Common.logger.Info(member + " is not a member of association group " + groupIdentifier);
                    return false;
                }


            }
            else
            {
                Common.logger.Info("could not get association for group " + groupIdentifier);
                return false;
            }

        }

        public static bool AddAssociation(Controller controller, byte nodeId, byte groupIdentifier, byte member)
        {
            Common.logger.Info("Add associtation - group " + groupIdentifier + " - node " + member);
            var cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_SET();
            cmd.groupingIdentifier = groupIdentifier;
            cmd.nodeId = new List<byte>() { member };
            var setAssociation = controller.SendData(nodeId, cmd, Common.txOptions);
            return setAssociation.TransmitStatus == TransmitStatuses.CompleteOk;
        }

        private static bool ClearAssiciation(Controller controller, byte nodeId, byte groupIdentifier)
        {
            Common.logger.Info("Clear associtation - group " + groupIdentifier + " for node " + nodeId);
            var cmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_REMOVE();
            cmd.groupingIdentifier = groupIdentifier;
            var clearAssociation = controller.SendData(nodeId, cmd, Common.txOptions);
            return clearAssociation.TransmitStatus == TransmitStatuses.CompleteOk;
        }

        public static bool RequestBatteryReport(Controller controller, byte nodeId)
        {
            //var cmd = new COMMAND_CLASS_BATTERY.BATTERY_GET();
            //var result = controller.RequestData(nodeId, cmd, Common.txOptions, new COMMAND_CLASS_BATTERY.BATTERY_REPORT(), 20000);
            //if (result)
            //{
            //    var rpt = (COMMAND_CLASS_BATTERY.BATTERY_REPORT)result.Command;
            //    logger.Info("battery report: " + rpt.batteryLevel);

            //}
            //else
            //{
            //    Common.logger.Info("Could not get battery!!");
            //}
            //return result;



            Common.logger.Info("Request battery value for node {0}", nodeId);
            var cmd = new COMMAND_CLASS_BATTERY.BATTERY_GET();
            var getBattery = controller.SendData(nodeId, cmd, txOptions);
            return getBattery.TransmitStatus == TransmitStatuses.CompleteOk;
        }


        public static bool ReplaceNode(Controller controller, byte nodeId)
        {
            var replacedNode = controller.ReplaceFailedNode(nodeId, null, Modes.NodeOptionHighPower | Modes.NodeOptionNetworkWide, 20000);
            replacedNode.WaitCompletedSignal();
            var result = (InclusionResult)replacedNode.Result;
            return result.AddRemoveNode.AddRemoveNodeStatus == ZWave.BasicApplication.Enums.AddRemoveNodeStatuses.Replaced;

        }

        public static bool IncludeNode(Controller controller, out byte nodeId)
        {
            var includeNode = controller.IncludeNode(Modes.NodeOptionHighPower | Modes.NodeOptionNetworkWide, 20000);
            nodeId = includeNode.AddRemoveNode.Id;
            if (nodeId == 0)
            {
                return false;
            }
            return true;
        }

        public static bool ExcludeNode(Controller controller, out byte nodeId)
        {
            var excludeNode = controller.ExcludeNode(Modes.NodeOptionHighPower | Modes.NodeOptionNetworkWide, 20000);
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

        public static bool CheckReachable(Controller controller, byte nodeId)
        {
            var sendData = controller.SendData(nodeId, new byte[1], Common.txOptions);
            return sendData.TransmitStatus == TransmitStatuses.CompleteOk;
        }


        public static Type GetNestedType(Type baseType, byte id)
        {
            var nestedTypes = baseType.GetNestedTypes(BindingFlags.Public);
            var matchedType = nestedTypes.Where(t =>
            {
                var value = t.GetField("ID")?.GetRawConstantValue();
                if (value == null)
                {
                    return false;
                }

                return (byte)value == id;

            }).FirstOrDefault();
            return matchedType;
        }

        public static Dictionary<byte, Type> GetAllCommandClasses(Assembly assembly, string nameSpace)
        {
            var keySet = new HashSet<byte>();

            var allClasses = assembly.GetTypes().Where(a => a.IsClass && a.Namespace != null && a.Namespace.Contains(nameSpace) && a.Name.StartsWith("COMMAND_CLASS") && !a.Name.Contains("CLASS_ALARM")).Select(t =>
            {
                var constValue = t?.GetField("ID")?.GetRawConstantValue();
                object returnValue = null;
                if (constValue != null && keySet.Add((byte)constValue))
                {
                    returnValue = new KeyValuePair<byte, Type>((byte)constValue, t);//.Name.ToLower());
                }
                return returnValue;
            }).Where(v => v != null).Select(v => (KeyValuePair<byte, Type>)v).ToDictionary(t => t.Key, t => t.Value);

            return allClasses;
        }

        public static Dictionary<Type, Dictionary<byte, Type>> GetAllNestedCommandClasses(ICollection<Type> commandClasses)
        {
            var nestedTypeDict = commandClasses.Select(t =>
            {
                var nestedTypes = t.GetNestedTypes(BindingFlags.Public);
                var dict = nestedTypes.Select(nt =>
                {
                    var id = nt.GetField("ID")?.GetRawConstantValue();
                    object returnVal = null;
                    if (id != null)
                    {
                        returnVal = new KeyValuePair<byte, Type>((byte)id, nt);
                    }
                    return returnVal;
                }).Where(v => v != null).Select(v => (KeyValuePair<byte, Type>)v).ToDictionary(nt => nt.Key, nt => nt.Value);
                return new KeyValuePair<Type, Dictionary<byte, Type>>(t, dict);
            }).ToDictionary(t => t.Key, t => t.Value);
            return nestedTypeDict;
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

        //            Common.logger.Info("Group: {0} - Member: {1} - Max: {2} - Follow: {3}", _groupId, member, maxSupported, reportToFollow);
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


        //            Common.logger.Info("ParameterNumber: {0} - {1}", _parameterNumber, value);
        //        }
        //        else
        //        {
        //            //  Common.logger.Info("Parameter {0} does not exist!", parameterNumber);
        //        }
        //    }

        //}

    }
}
