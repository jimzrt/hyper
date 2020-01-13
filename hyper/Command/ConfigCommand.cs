using hyper.commands;
using hyper.config;
using System.Collections.Generic;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class ConfigCommand : ICommand
    {
        private readonly Controller controller;
        private readonly byte nodeId;
        private readonly List<ConfigItem> configList;

        private bool abort = false;

        public bool Active { get; private set; } = false;

        public ConfigCommand(Controller controller, byte nodeId, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.nodeId = nodeId;
            this.configList = configList;
        }

        public bool Start()
        {
            Active = true;
            Common.logger.Info("-----------");
            Common.logger.Info("Configuration mode");
            Common.logger.Info("node to configure: " + nodeId);
            Common.logger.Info("-----------");

            Common.logger.Info("Getting configuration for device...");
            ConfigItem config = Common.GetConfigurationForDevice(controller, nodeId, configList, ref abort);
            if (config == null)
            {
                Common.logger.Info("could not find configuration!");
                Common.logger.Info("Either there is no configuration or device did not reply!");
                Active = false;
                return false;
            }

            Common.logger.Info("configuration found for {0}!", config.deviceName);
            Common.logger.Info("Setting values.");
            if (Common.SetConfiguration(controller, nodeId, config))
            {
                Common.logger.Info("Configuration successful!");
                Common.logger.Info("-------------------");
                Active = false;
                return true;
            }

            return false;
        }

        public void Stop()
        {
            Common.logger.Info("aborting... Please wait.");
            abort = true;
            // Common.logger.Warn("Cannot abort!");
        }
    }
}