using hyper.commands;
using hyper.config;
using System.Collections.Generic;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class IncludeCommand : ICommand
    {
        private readonly Controller controller;
        private readonly List<ConfigItem> configList;

        private bool abort = false;

        public bool Active { get; private set; } = false;

        private ICommand currentCommand = null;

        public IncludeCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;
        }

        public bool Start()
        {
            Active = true;
            Common.logger.Info("-----------");
            Common.logger.Info("Inclusion mode");
            Common.logger.Info("-----------");

            Common.logger.Info("Starting inclusion, please wake up device...");

            var nodeIncluded = Common.IncludeNode(controller, out byte nodeId);
            while (!nodeIncluded && !abort)
            {
                Common.logger.Info("Could not include any node, trying again...");
                nodeIncluded = Common.IncludeNode(controller, out nodeId);
            }

            if (abort)
            {
                Common.logger.Info("aborted!");
                return false;
            }
            Common.logger.Info("Success! New node id: {0}", nodeId);

            Common.logger.Info("Inclusion done!");

            currentCommand = new ConfigCommand(controller, nodeId, configList);
            return currentCommand.Start();
        }

        public void Stop()
        {
            Common.logger.Info("aborting... Please wait.");
            abort = true;
            currentCommand?.Stop();
        }
    }
}