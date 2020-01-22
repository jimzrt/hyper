using hyper.Command;
using hyper.commands;
using hyper.config;
using System.Collections.Generic;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class ReplaceCommand : BaseCommand
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

        public override bool Start()
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

        public override void Stop()
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
}