using hyper.Command;
using hyper.commands;
using hyper.config;
using hyper.Database.DAO;
using hyper.Helper;
using hyper.Inputs;
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
    public class QueueCommand : BaseCommand
    {
        private readonly Controller controller;
        private List<ConfigItem> configList;
        private InputManager inputManager;
        private EventDAO eventDao = new EventDAO();
        private readonly object lockObject = new object();

        public QueueCommand(Controller controller, List<ConfigItem> configList, InputManager inputManager)
        {
            this.controller = controller;
            this.configList = configList;
            this.inputManager = inputManager;
        }

        public bool Active
        {
            get; set;
        } = false;

        private Dictionary<byte, SortedSet<string>> nodeToCommandMap = new Dictionary<byte, SortedSet<string>>();

        //private readonly BlockingCollection<Action> queueItems = new BlockingCollection<Action>();
        private ActionToken dataListener;

        private ActionToken controllerListener;

        public void AddToMap(byte nodeId, string command)
        {
            var sortedSet = nodeToCommandMap.GetValueOrDefault(nodeId, new SortedSet<string>());
            sortedSet.Add(command);
            nodeToCommandMap[nodeId] = sortedSet;
        }

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

                    //  Common.logger.Info("{0}: {2}:{3} from node {1}", x.TimeStamp, x.SrcNodeId, _commandClass ? commandClass.Name : string.Format("unknown(id:{0})", x.Command[0]), _nestedType ? nestedType.Name : string.Format("unknown(id:{0})", x.Command[1]));

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

                    switch (report)
                    {
                        case COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_NOTIFICATION _:

                            var commandsPresent = nodeToCommandMap.TryGetValue(x.SrcNodeId, out SortedSet<string> commands);
                            if (!commandsPresent)
                            {
                                //Common.logger.Warn($"no commands for {x.SrcNodeId}; check battery if needed");
                                //var lastDate = eventDao.GetLastEvent(typeof(COMMAND_CLASS_BATTERY.BATTERY_REPORT).Name, x.SrcNodeId);
                                //if ((DateTime.Now - lastDate).TotalHours >= 1)
                                {
                                    inputManager.InjectCommand($"battery {x.SrcNodeId}");
                                }
                                return;
                            }

                            var command = commands.First();
                            if (command == "config")
                            {
                                command = command + " " + x.SrcNodeId + "!";
                            }
                            Common.logger.Warn($"injecting {command}");
                            inputManager.InjectCommand(command);
                            commands.Remove(commands.First());
                            if (commands.Count == 0)
                            {
                                nodeToCommandMap.Remove(x.SrcNodeId);
                            }

                            break;

                        default:
                            break;
                    }
                }
            });

            controllerListener = controller.HandleControllerUpdate((r) =>
            {
                var commandsPresent = nodeToCommandMap.TryGetValue(r.NodeId, out SortedSet<string> commands);
                if (!commandsPresent)
                {
                    //Common.logger.Warn($"no commands for {r.NodeId}; check battery if needed");
                    var lastDate = eventDao.GetLastEvent(typeof(COMMAND_CLASS_BATTERY.BATTERY_REPORT).Name, r.NodeId);
                    if ((DateTime.Now - lastDate).TotalHours >= 1)
                    {
                        inputManager.InjectCommand($"battery {r.NodeId}");
                    }
                    return;
                }

                var command = commands.First();
                if (command == "config")
                {
                    command = command + " " + r.NodeId + "!";
                }
                Common.logger.Warn($"injecting {command}");
                inputManager.InjectCommand(command);
                commands.Remove(commands.First());
                if (commands.Count == 0)
                {
                    nodeToCommandMap.Remove(r.NodeId);
                }
            });

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
            controllerListener.WaitCompletedSignal();
            //Active = false;
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
            controllerListener?.SetCancelled();
            // controllerListener?.SetCompletedSignal();
            // queueItems?.CompleteAdding();
        }

        internal void UpdateConfig(List<ConfigItem> configList)
        {
            this.configList = configList;
        }
    }
}