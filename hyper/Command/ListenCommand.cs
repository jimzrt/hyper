using hyper.Command;
using hyper.commands;
using hyper.config;
using hyper.Database.DAO;
using hyper.Helper;
using hyper.Helper.Extension;
using hyper.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Utils;
using ZWave;
using ZWave.BasicApplication.Devices;
using ZWave.CommandClasses;

namespace hyper
{
    public class ListenCommand : BaseCommand
    {
        private readonly Controller controller;
        private List<ConfigItem> configList;

        private EventDAO eventDao = new EventDAO();
        private readonly object lockObject = new object();

        public ListenCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;
        }

        // private bool _active = true;

        public bool Active
        {
            get; set;
        } = false;

        private byte _filterNodeId = 0;

        public byte Filter
        {
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

        //private readonly BlockingCollection<Action> queueItems = new BlockingCollection<Action>();
        private ActionToken dataListener;

        //private ActionToken controllerListener;

        //public void AddToQueue(Action action)
        //{
        //    queueItems.Add(action);
        //}

        public override bool Start()
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

            //byte[] numArray = File.ReadAllBytes(@"C:\Users\james\Desktop\tmp\MultiSensor 6_OTA_EU_A_V1_13.exe");
            //int length = (int)numArray[numArray.Length - 4] << 24 | (int)numArray[numArray.Length - 3] << 16 | (int)numArray[numArray.Length - 2] << 8 | (int)numArray[numArray.Length - 1];
            //byte[] flashDataB = new byte[length];
            //Array.Copy((Array)numArray, numArray.Length - length - 4 - 4 - 256, (Array)flashDataB, 0, length);
            //List<byte> flashData = new List<byte>(flashDataB);

            dataListener = controller.ListenData((x) =>
            {
                lock (lockObject)
                {
                    var filterActive = Filter != 0 && Filter != x.SrcNodeId;

                    var _commandClass = commandClasses.TryGetValue(x.Command[0], out var commandClass);
                    if (!_commandClass)
                    {
                        Common.logger.Error("node id: {1} - command class {0} not found!", x.Command[0], x.SrcNodeId);
                        return;
                    }
                    var _nestedDict = nestedCommandClasses.TryGetValue(commandClass, out var nestedDict);
                    if (!_nestedDict)
                    {
                        Common.logger.Error("node id: {1} - nested command classes for command class {0} not found!", commandClass.Name, x.SrcNodeId);
                        return;
                    }
                    var _nestedType = nestedDict.TryGetValue(x.Command[1], out Type nestedType);
                    if (!_nestedType)
                    {
                        Common.logger.Error("node id: {2} - nested command class {0} for command class {1} not found!", x.Command[1], commandClass.Name, x.SrcNodeId);
                        return;
                    }

                    if (!filterActive)
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
                    report.GetKeyValue(out Enums.EventKey eventKey, out float eventValue);
                    Common.logger.Info($"key: {eventKey} - value: {eventValue}");

                    if (!filterActive && Debug)
                        Common.logger.Info(Util.ObjToJson(report));

                    if (!(report is COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_GET))
                    {
                        OutputManager.HandleCommand(report, x.SrcNodeId, x.DestNodeId);
                    }

                    //    Handle(dummyInstance, x.SrcNodeId, x.Command);
                    if (!Active)
                    {
                        return;
                    }

                    //switch (report)
                    //{
                    //    case COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_NOTIFICATION _:
                    //        // TODO EVENT, last Battery
                    //        var lastDate = eventDao.GetLastEvent(typeof(COMMAND_CLASS_BATTERY.BATTERY_REPORT).Name, x.SrcNodeId);
                    //        if ((DateTime.Now - lastDate).TotalHours >= 6)
                    //        {
                    //            queueItems.Add(() => Common.RequestBatteryReport(controller, x.SrcNodeId));
                    //        }

                    //        break;

                    //    //case COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_GET fupdateReport:

                    //    //    var rep1 = fupdateReport.properties1.reportNumber1;
                    //    //    var rep2 = fupdateReport.reportNumber2;
                    //    //    var count = fupdateReport.numberOfReports;

                    //    //    queueItems.Add(() =>
                    //    //    {
                    //    //        var take = 40;
                    //    //        var cmd = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REPORT();
                    //    //        cmd.properties1.last = 0;
                    //    //        cmd.properties1.reportNumber1 = rep1;
                    //    //        cmd.reportNumber2 = rep2;
                    //    //        short repNumber = BitConverter.ToInt16(new byte[] { rep2, rep1 });
                    //    //        //Console.WriteLine($"Repnumber: {repNumber}");
                    //    //        var offset = (repNumber - 1) * 40;
                    //    //        Common.logger.Info("Progress: {0}", (offset / (float)flashData.Count).ToString("0.00%"));

                    //    //        if ((flashData.Count - offset) < 40)
                    //    //        {
                    //    //            // Console.WriteLine("ima full!");
                    //    //            Console.WriteLine(offset);
                    //    //            Console.WriteLine(flashData.Count);
                    //    //            take = flashData.Count - offset;
                    //    //            Console.WriteLine(take);
                    //    //            cmd.properties1.last = 1;
                    //    //            Common.logger.Info("Progress: {0}", (1).ToString("0.00%"));
                    //    //        }
                    //    //        var data = flashData.Skip(offset).Take(take).ToArray();
                    //    //        cmd.data = data;
                    //    //        //7A 97
                    //    //        // Common.logger.Info(Util.ObjToJson((byte[])cmd));
                    //    //        // Common.logger.Info(Util.ObjToJson(new byte[] { COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.ID, COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REPORT.ID }.Concat(new byte[] { cmd.properties1, cmd.reportNumber2 }).Concat(data).ToArray()));
                    //    //        cmd.checksum = Tools.CalculateCrc16Array((byte[])cmd, 0, ((byte[])cmd).Length - 2);
                    //    //        //cmd.checksum =  Tools.CalculateCrc16Array(new byte[] { COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.ID, COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REPORT .ID}.Concat(new byte[] { cmd.properties1, cmd.reportNumber2 }).Concat(data));
                    //    //        controller.SendData(x.SrcNodeId, cmd, Common.txOptions);
                    //    //    });

                    //    //    break;

                    //    default:
                    //        break;
                    //}
                }
            });

            //controllerListener = controller.HandleControllerUpdate((r) =>
            //{
            //    if (!Active)
            //    {
            //        return;
            //    }
            //    Common.logger.Info("{0}: Got {2} for node {1}", DateTime.Now, r.NodeId, r.Status);
            //    var lastDate = eventDao.GetLastEvent(typeof(COMMAND_CLASS_BATTERY.BATTERY_REPORT).Name, r.NodeId);
            //    if ((DateTime.Now - lastDate).TotalHours >= 6)
            //    {
            //        queueItems.Add(() => Common.RequestBatteryReport(controller, r.NodeId));
            //    }
            //});

            //Active = true;
            //while (!queueItems.IsCompleted)
            //{
            //    try
            //    {
            //        var action = queueItems.Take();
            //        if (Active)
            //            action();
            //    }
            //    catch (InvalidOperationException) { }
            //}
            dataListener.WaitCompletedSignal();

            //   Active = false;
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

        public override void Stop()
        {
            Common.logger.Info("stop listening!");
            dataListener?.SetCancelled();
            //dataListener?.SetCompletedSignal();
            //  controllerListener?.SetCancelled();
            // controllerListener?.SetCompletedSignal();
            //  queueItems?.CompleteAdding();
        }

        internal void UpdateConfig(List<ConfigItem> configList)
        {
            this.configList = configList;
        }
    }
}