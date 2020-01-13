using hyper.commands;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class ExcludeCommand : ICommand
    {
        private readonly Controller controller;

        private bool abort = false;

        public ExcludeCommand(Controller controller)
        {
            this.controller = controller;
        }

        public bool Start()
        {
            Common.logger.Info("-----------");
            Common.logger.Info("Exclusion mode");
            Common.logger.Info("-----------");

            Common.logger.Info("Starting exclusion, please wake up device...");

            var nodeExcluded = Common.ExcludeNode(controller, out byte nodeId);
            while (!nodeExcluded && !abort)
            {
                Common.logger.Info("Could not exclude any node, trying again...");
                nodeExcluded = Common.ExcludeNode(controller, out nodeId);
            }

            if (abort)
            {
                Common.logger.Info("Aborted!");
                return false;
            }

            Common.logger.Info("Success! node with id: {0} excluded.", nodeId);

            Common.logger.Info("Exclusion done!");
            return true;
        }

        public void Stop()
        {
            Common.logger.Info("aborting... please wait.");
            abort = true;
        }
    }
}