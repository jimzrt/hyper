using hyper.commands;
using hyper.config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Utils;
using ZWave;
using ZWave.BasicApplication.Devices;
using ZWave.CommandClasses;

namespace hyper
{
    public class ReconfigureCommand : ICommand
    {
        private readonly Controller controller;
        private readonly List<ConfigItem> configList;
        private readonly BlockingCollection<byte> queueItems = new BlockingCollection<byte>(255);

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
}