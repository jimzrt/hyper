using hyper.commands;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class ForceRemoveCommand : ICommand
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
}