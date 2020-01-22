using hyper.Command;
using hyper.commands;
using System.Threading.Tasks;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class PingCommand : BaseCommand
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

        public override bool Start()
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

        public override void Stop()
        {
            Common.logger.Info("aborting... Please wait.");
            running = false;
        }
    }
}