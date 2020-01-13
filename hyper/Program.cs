using hyper.commands;
using hyper.Input;
using hyper.Inputs;
using hyper.Output;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class Program
    {
        private static void SetupInputs()
        {
            var configuration = new LoggingConfiguration();

            var tcpTarget = new TCPInput(5432)
            {
                Layout = @"${longdate} ${uppercase:${level}} ${message}"
            };
            var consoleTarget = new ConsoleInput()
            {
                Layout = @"${longdate} ${uppercase:${level}} ${message}"
            };

            var fileTarget = new FileTarget()
            {
                Name = "FileTarget",
                Layout = @"${longdate} ${uppercase:${level}} ${message}",
                AutoFlush = true,
                FileName = "${basedir}/logs/log.${shortdate}.txt",
                ArchiveFileName = "${basedir}/logs/archives/log.{#####}.zip",
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ConcurrentWrites = true,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveOldFileOnStartup = true,
                EnableArchiveFileCompression = true,
                MaxArchiveFiles = 14,
                OptimizeBufferReuse = true,
                CreateDirs = true
            };

            configuration.AddTarget(fileTarget);
            configuration.AddTarget(consoleTarget);
            configuration.AddTarget(tcpTarget);

            configuration.AddRuleForAllLevels(fileTarget);
            configuration.AddRuleForAllLevels(consoleTarget);
            configuration.AddRuleForAllLevels(tcpTarget);

            LogManager.Configuration = configuration;

            InputManager.AddInput(consoleTarget);
            InputManager.AddInput(tcpTarget);
            var udpInput = new UDPInput(54322);
            InputManager.AddInput(udpInput);
        }

        private static void SetupOutputs()
        {
            var udpOutput = new UDPOutput("127.0.0.1", 54321);
            var databaseOutput = new DatabaseOutput("events.db");
            OutputManager.AddOutput(udpOutput);
            OutputManager.AddOutput(databaseOutput);
        }

        private static void Main(string[] args)
        {
            SetupInputs();
            SetupOutputs();

            //  Target.Register("MyFirst", typeof(MyNamespace.MyFirstTarget)); //OR, dynamic

            ICommand currentCommand = null;

            Common.logger.Debug("==== ZWave Hyper Hyper 5000 ====");
            Common.logger.Debug("-----------------------------------");
            Common.logger.Debug("Loading device configuration database...");
            if (!File.Exists("config.yaml"))
            {
                Common.logger.Error("configuration file config.yaml does not exist!");
                return;
            }
            var config = Common.ParseConfig("config.yaml");
            if (config == null)
            {
                Common.logger.Error("Could not parse configuration file config.yaml!");
                return;
            }
            Common.logger.Info("Got configuration for " + config.Count + " devices.");
            Common.logger.Debug("-----------------------------------");

            if (args.Length < 2)
            {
                Common.logger.Info("usage:");
                Common.logger.Info("./hyper [serialPort] [command]");
                Common.logger.Info("valid commands:");
                Common.logger.Info("r/replace, c/config, i/include, e/exclude, l/listen, p/ping");
                return;
            }

            var port = args[0];
            Common.logger.Info("Initialize Serialport: {0}", port);
            var initController = Common.InitController(port, out Controller controller, out string errorMessage);
            if (!initController)
            {
                Common.logger.Error("Error connecting with port {0}! Error Mesage:", port);
                Common.logger.Error(errorMessage);
                return;
            }
            Common.logger.Info("Version: {0}", controller.Version);
            Common.logger.Info("Included nodes: {0}", controller.IncludedNodes.Length);
            Common.logger.Info("-----------------------------------");

            if (args[1] == "r" || args[1] == "replace" || args[1] == "c" || args[1] == "config")
            {
                if (args.Length != 3)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");
                    if (args[1] == "r" || args[1] == "replace")
                    {
                        Common.logger.Info("./hyper [serialPort] r [nodeid]");
                    }
                    else
                    {
                        Common.logger.Info("./hyper [serialPort] c [nodeid]");
                    }

                    return;
                }

                if (!byte.TryParse(args[2], out byte nodeId))
                {
                    Common.logger.Info("argument 1 should be node id! " + args[2] + " is not a number!");
                    return;
                }

                if (!controller.IncludedNodes.Contains(nodeId))
                {
                    Common.logger.Info("NodeID " + nodeId + " not included in network!");
                    Common.logger.Info(string.Join(", ", controller.IncludedNodes));
                    //    return;
                }

                if (args[1] == "r" || args[1] == "replace")
                {
                    new ReplaceCommand(controller, nodeId, config).Start();
                }
                else
                {
                    new ConfigCommand(controller, nodeId, config).Start();
                }
            }
            else if (args[1] == "i" || args[1] == "include")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] i");

                    return;
                }

                new IncludeCommand(controller, config).Start();
            }
            else if (args[1] == "e" || args[1] == "exclude")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] e");

                    return;
                }

                new ExcludeCommand(controller).Start();
            }
            else if (args[1] == "l" || args[1] == "listen")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] l");

                    return;
                }
                currentCommand = new ListenCommand(controller, config);
                currentCommand.Start();
            }
            else if (args[1] == "p" || args[1] == "ping")
            {
                if (args.Length != 3)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] p [nodeid]");

                    return;
                }

                if (!byte.TryParse(args[2], out byte nodeId))
                {
                    Common.logger.Info("argument should be node id! " + args[2] + " is not a number!");
                    return;
                }

                if (!controller.IncludedNodes.Contains(nodeId))
                {
                    Common.logger.Info("NodeID " + nodeId + " not included in network!");
                    Common.logger.Info(string.Join(", ", controller.IncludedNodes));
                    return;
                }

                currentCommand = new PingCommand(controller, nodeId);
                currentCommand.Start();
            }
            else if (args[1] == "rc" || args[1] == "reconfigure")
            {
                if (args.Length != 2)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] rc");

                    return;
                }
                currentCommand = new ReconfigureCommand(controller, config);
                currentCommand.Start();
            }
            else if (args[1] == "it" || args[1] == "interactive")
            {
                currentCommand = new InteractiveCommand(controller, config);
                currentCommand.Start();
            }
            else if (args[1] == "fr" || args[1] == "forceRemove")
            {
                if (args.Length != 3)
                {
                    Common.logger.Info("wrong arguments!");
                    Common.logger.Info("correct usage:");

                    Common.logger.Info("./hyper [serialPort] fr [nodeid]");

                    return;
                }

                if (!byte.TryParse(args[2], out byte nodeId))
                {
                    Common.logger.Info("argument should be node id! " + args[2] + " is not a number!");
                    return;
                }

                if (!controller.IncludedNodes.Contains(nodeId))
                {
                    Common.logger.Info("NodeID " + nodeId + " not included in network!");
                    Common.logger.Info(string.Join(", ", controller.IncludedNodes));
                    return;
                }

                currentCommand = new ForceRemoveCommand(controller, nodeId);
                currentCommand.Start();
            }
            else
            {
                Common.logger.Info("unknown command: {0}", args[1]);
                Common.logger.Info("valid commands:");
                Common.logger.Info("r/replace, c/config, i/include, e/exclude, l/listen, p/ping");
            }

            Common.logger.Info("----------");
            Common.logger.Info("Press any key to exit...");
            Console.ReadKey();
        }
    }
}