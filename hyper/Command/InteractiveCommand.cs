using hyper.commands;
using hyper.config;
using hyper.Inputs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using ZWave.BasicApplication.Devices;

namespace hyper
{
    public class InteractiveCommand : ICommand
    {
        private readonly Controller controller;
        private List<ConfigItem> configList;
        private ICommand currentCommand = null;

        private bool blockExit = false;

        public InteractiveCommand(Controller controller, List<ConfigItem> configList)
        {
            this.controller = controller;
            this.configList = configList;
        }

        public bool Active { get; private set; } = false;

        private void CancelHandler(object evtSender, ConsoleCancelEventArgs evtArgs)
        {
            if (evtArgs != null)
            {
                evtArgs.Cancel = true;
            }
            if (blockExit)
            {
                Common.logger.Info("Cannot abort application now!\nPlease wait for operation to finish.");
                return;
            }

            if (currentCommand == null)
            {
                Common.logger.Info("No current command, stopping application");
                if (evtArgs != null)
                {
                    evtArgs.Cancel = false;
                }
                Environment.Exit(0);
                return;
            }

            Common.logger.Info("Stopping current command!");
            currentCommand.Stop();
        }

        public bool Start()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);
            InputManager.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);
            // InputManager.AddCancelEventHandler(CancelHandler);

            var oneTo255Regex = @"((?<!\d)(?:1\d{2}|2[0-4]\d|[1-9]?\d|25[0-5])(?!\d))";

            var pingRegex = new Regex(@$"^ping\s*{oneTo255Regex}");
            var configRegex = new Regex(@"^config\s*" + oneTo255Regex);
            var replaceRegex = new Regex(@"^replace\s*" + oneTo255Regex);
            var basicRegex = new Regex(@"^basic\s*" + oneTo255Regex + @"\s*(false|true)");
            var listenRegex = new Regex(@"^listen\s*(stop|start|filter\s*" + oneTo255Regex + ")");
            //var testRegex = new Regex(@"^firmware\s*" + oneTo255Regex);
            var forceRemoveRegex = new Regex(@"^remove\s*" + oneTo255Regex);
            var debugRegex = new Regex(@"^debug\s*(false|true)");

            Active = true;
            Common.logger.Info("-----------");
            Common.logger.Info("Interaction mode");
            Common.logger.Info("-----------");

            ListenCommand listenComand = new ListenCommand(controller, configList);

            Thread InstanceCaller = new Thread(
            new ThreadStart(() => listenComand.Start()));

            InstanceCaller.Start();

            while (Active)
            {
                //   while (Console.KeyAvailable)
                //       Console.ReadKey(true);

                Common.logger.Info("choose your destiny girl!");
                var input = InputManager.ReadAny();
                if (input == null)
                {
                    return false;
                }
                Common.logger.Info("Command: {0}", input);
                switch (input.Trim().ToLower())
                {
                    case "reset!":
                        {
                            blockExit = true;
                            Common.logger.Info("Resetting controller...");
                            controller.SetDefault();
                            controller.SerialApiGetInitData();
                            Common.logger.Info("Done!");

                            blockExit = false;
                            break;
                        }

                    case "include":
                        {
                            currentCommand = new IncludeCommand(controller, configList);
                            break;
                        }
                    case "exclude":
                        {
                            currentCommand = new ExcludeCommand(controller);
                            break;
                        }
                    case "backup":
                        {
                            blockExit = true;
                            var result = Common.ReadNVRam(controller, out byte[] eeprom);
                            if (result)
                            {
                                File.WriteAllBytes("eeprom.bin", eeprom);
                                Common.logger.Info("Result is {0}", result);
                            }
                            blockExit = false;
                            break;
                        }
                    case "restore!":
                        {
                            blockExit = true;
                            byte[] read = File.ReadAllBytes("eeprom.bin");
                            var result = Common.WriteNVRam(controller, read);
                            Common.logger.Info("Result is {0}", result);
                            controller.SerialApiGetInitData();

                            blockExit = false;
                            break;
                        }
                    case "reload":
                        {
                            Common.logger.Info("Reloading conifg!");
                            configList = Common.ParseConfig("config.yaml");
                            listenComand.UpdateConfig(configList);
                            break;
                        }
                    case var listenVal when listenRegex.IsMatch(listenVal):
                        {
                            var val = listenRegex.Match(listenVal).Groups[1].Value;
                            if (val == "start" || val == "stop")
                            {
                                listenComand.Active = val == "start";
                            }
                            else if (val.StartsWith("filter"))
                            {
                                var nodeId = Byte.Parse(listenRegex.Match(listenVal).Groups[2].Value);
                                listenComand.Filter = nodeId;
                                //   Console.WriteLine("FILTER {0}", nodeId);
                            }
                            break;
                        }
                    case var debugVal when debugRegex.IsMatch(debugVal):
                        {
                            var val = debugRegex.Match(debugVal).Groups[1].Value;
                            var debug = bool.Parse(val);
                            listenComand.Debug = debug;
                            break;
                        }
                    case var removeVal when forceRemoveRegex.IsMatch(removeVal):
                        {
                            var val = forceRemoveRegex.Match(removeVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            currentCommand = new ForceRemoveCommand(controller, nodeId);
                            break;
                        }
                    /*                        case var testVal when testRegex.IsMatch(testVal):
                                                {
                                                    var val = testRegex.Match(testVal).Groups[1].Value;
                                                    var nodeId = Byte.Parse(val);

                                                    byte[] bytes = new byte[256];
                                                    byte[] numArray = File.ReadAllBytes(@"C:\Users\james\Desktop\tmp\MultiSensor 6_OTA_EU_A_V1_13.exe");
                                                    int length = (int)numArray[numArray.Length - 4] << 24 | (int)numArray[numArray.Length - 3] << 16 | (int)numArray[numArray.Length - 2] << 8 | (int)numArray[numArray.Length - 1];
                                                    byte[] flashData = new byte[length];
                                                    Array.Copy((Array)numArray, numArray.Length - length - 4 - 4 - 256, (Array)flashData, 0, length);
                                                    Array.Copy((Array)numArray, numArray.Length - 256 - 4 - 4, (Array)bytes, 0, 256);

                                                    var cmd = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REQUEST_GET();
                                                    cmd.manufacturerId = new byte[] { 0, 0x86 };
                                                    cmd.firmwareId = new byte[] { 0, 0 };
                                                    cmd.checksum = Tools.CalculateCrc16Array(flashData);
                                                    controller.SendData(nodeId, cmd, Common.txOptions);

                                                    break;
                                                }*/
                    case var pingVal when pingRegex.IsMatch(pingVal):
                        {
                            var val = pingRegex.Match(pingVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            currentCommand = new PingCommand(controller, nodeId);
                            break;
                        }
                    case var basicSetVal when basicRegex.IsMatch(basicSetVal):
                        {
                            blockExit = true;
                            var val = basicRegex.Match(basicSetVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            val = basicRegex.Match(basicSetVal).Groups[2].Value;
                            var value = bool.Parse(val);
                            var trys = 5;
                            Common.SetBinary(controller, nodeId, value);
                            /* Thread.Sleep(500);
                             while(trys >= 0 && (!Common.GetBinary(controller, nodeId, out bool retValue) || retValue != value))
                             {
                                 Common.SetBinary(controller, nodeId, value);
                                 Thread.Sleep(500);
                                 trys--;
                             }*/
                            blockExit = false;
                            break;
                        }
                    case var configVal when configRegex.IsMatch(configVal):
                        {
                            var val = configRegex.Match(configVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            currentCommand = new ConfigCommand(controller, nodeId, configList);
                            break;
                        }
                    case var replaceVal when replaceRegex.IsMatch(replaceVal):
                        {
                            var val = replaceRegex.Match(replaceVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            currentCommand = new ReplaceCommand(controller, nodeId, configList);
                            break;
                        }

                    default:
                        Common.logger.Warn("unknown command");
                        break;
                }
                if (currentCommand == null)
                {
                    continue;
                }
                listenComand.Active = false;
                currentCommand.Start();
                currentCommand = null;
                listenComand.Active = true;
            }

            Common.logger.Info("goodby master,,,");
            return true;
        }

        public void Stop()
        {
            if (currentCommand != null)
            {
                Common.logger.Info("stoppping current command!");
                currentCommand.Stop();
            }
            else
            {
                Common.logger.Info("stopping interactive mode");
                Common.logger.Info("press any key to exit");
                Active = false;
            }
        }
    }
}