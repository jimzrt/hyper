using hyper.commands;
using hyper.config;
using hyper.Database.DAO;
using hyper.Helper;
using hyper.Inputs;
using hyper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using ZWave.BasicApplication.Devices;
using ZWave.CommandClasses;

namespace hyper
{
    public class InteractiveCommand : ICommand
    {
        private ICommand currentCommand = null;

        private bool blockExit = false;
        private string args;

        private EventDAO eventDao = new EventDAO();

        public InteractiveCommand(string args)
        {
            this.args = args;
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

            var oneTo255Regex = @"\b(?:[1-9]|[1-8][0-9]|9[0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\b";
            var zeroTo255Regex = @"\b(?:[0-9]|[1-8][0-9]|9[0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\b";

            var pingRegex = new Regex(@$"^ping\s*({oneTo255Regex})");
            var configRegex = new Regex(@$"^config\s*({oneTo255Regex})");
            var replaceRegex = new Regex(@$"^replace\s*({oneTo255Regex})");
            var basicRegex = new Regex(@$"^(basic|binary)\s*({oneTo255Regex})\s*(false|true)");
            var basicGetRegex = new Regex(@$"^(basic|binary)\s*({oneTo255Regex})");
            var listenRegex = new Regex(@$"^listen\s*(stop|start|filter\s*({oneTo255Regex}))");
            //var testRegex = new Regex(@"^firmware\s*" + oneTo255Regex);
            var forceRemoveRegex = new Regex(@$"^remove\s*({oneTo255Regex})");
            var debugRegex = new Regex(@"^debug\s*(false|true)");
            var lastEventsRegex = new Regex(@$"^show\s*({zeroTo255Regex})\s*({zeroTo255Regex})?\s*([a-zA-Z_]+)?");

            Active = true;
            bool oneShot = args.Length > 0;

            Common.logger.Info("-----------");
            if (oneShot)
            {
                Common.logger.Info("Oneshot mode");
            }
            else
            {
                Common.logger.Info("Interaction mode");
            }
            Common.logger.Info("-----------");

            ListenCommand listenComand = new ListenCommand(Program.controller, Program.configList);

            Thread InstanceCaller = new Thread(
            new ThreadStart(() => listenComand.Start()));

            InstanceCaller.Start();

            do
            {
                Common.logger.Info("choose your destiny girl!");

                var input = "";
                if (oneShot)
                {
                    input = args;
                }
                else
                {
                    input = InputManager.ReadAny();
                }
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
                            Program.controller.SetDefault();
                            Program.controller.SerialApiGetInitData();
                            Common.logger.Info("Done!");

                            blockExit = false;
                            break;
                        }

                    case "include":
                        {
                            currentCommand = new IncludeCommand(Program.controller, Program.configList);
                            break;
                        }
                    case "included":
                        {
                            Common.logger.Info("included nodes: " + string.Join(", ", Program.controller.IncludedNodes));
                            break;
                        }
                    case "exclude":
                        {
                            currentCommand = new ExcludeCommand(Program.controller);
                            break;
                        }
                    case "backup":
                        {
                            blockExit = true;
                            var result = Common.ReadNVRam(Program.controller, out byte[] eeprom);
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
                            var result = Common.WriteNVRam(Program.controller, read);
                            Common.logger.Info("Result is {0}", result);
                            Program.controller.SerialApiGetInitData();

                            blockExit = false;
                            break;
                        }
                    case "reload":
                        {
                            Common.logger.Info("Reloading conifg!");
                            Program.configList = Common.ParseConfig("config.yaml");
                            listenComand.UpdateConfig(Program.configList);
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
                    case var eventsVal when lastEventsRegex.IsMatch(eventsVal):
                        {
                            var match = lastEventsRegex.Match(eventsVal);
                            var nodeId = byte.Parse(match.Groups[1].Value);

                            var count = match.Groups[2].Value;
                            var command = match.Groups[3].Value;

                            Common.logger.Info($"node: {nodeId} - count: {(count.IsNullOrEmpty() ? "all" : count)} - command: {(command.IsNullOrEmpty() ? "all" : command)}");
                            EventFilter filter = new EventFilter();
                            if (nodeId != 0)
                            {
                                filter.NodeId = nodeId;
                            }
                            if (!count.IsNullOrEmpty())
                            {
                                filter.Count = int.Parse(count);
                            }
                            if (!command.IsNullOrEmpty())
                            {
                                filter.Command = command;
                            }
                            List<Event> events = eventDao.GetByFilter(filter);
                            foreach (var evt in events)
                            {
                                Common.logger.Debug(evt);
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
                            currentCommand = new ForceRemoveCommand(Program.controller, nodeId);
                            break;
                        }
                    //case var testVal when testRegex.IsMatch(testVal):
                    //    {
                    //        var val = testRegex.Match(testVal).Groups[1].Value;
                    //        var nodeId = Byte.Parse(val);

                    //        byte[] bytes = new byte[256];
                    //        byte[] numArray = File.ReadAllBytes(@"C:\Users\james\Desktop\tmp\MultiSensor 6_OTA_EU_A_V1_13.exe");
                    //        int length = (int)numArray[numArray.Length - 4] << 24 | (int)numArray[numArray.Length - 3] << 16 | (int)numArray[numArray.Length - 2] << 8 | (int)numArray[numArray.Length - 1];
                    //        byte[] flashData = new byte[length];
                    //        Array.Copy((Array)numArray, numArray.Length - length - 4 - 4 - 256, (Array)flashData, 0, length);
                    //        Array.Copy((Array)numArray, numArray.Length - 256 - 4 - 4, (Array)bytes, 0, 256);

                    //        var cmd = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V2.FIRMWARE_UPDATE_MD_REQUEST_GET();
                    //        cmd.manufacturerId = new byte[] { 0, 0x86 };
                    //        cmd.firmwareId = new byte[] { 0, 0 };
                    //        cmd.checksum = Tools.CalculateCrc16Array(flashData);
                    //        Program.controller.SendData(nodeId, cmd, Common.txOptions);

                    //        break;
                    //    }
                    case var pingVal when pingRegex.IsMatch(pingVal):
                        {
                            blockExit = true;
                            var val = pingRegex.Match(pingVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            Common.logger.Info("Pinging node {0}...", nodeId);
                            var reachable = Common.CheckReachable(Program.controller, nodeId);
                            Common.logger.Info("node {0} is{1}reachable!", nodeId, reachable ? " " : " NOT ");
                            blockExit = false;
                            // currentCommand = new PingCommand(Program.controller, nodeId);
                            break;
                        }
                    case var basicSetVal when basicRegex.IsMatch(basicSetVal):
                        {
                            blockExit = true;
                            var match = basicRegex.Match(basicSetVal);
                            var val = match.Groups[2].Value;
                            var nodeId = byte.Parse(val);
                            val = match.Groups[3].Value;
                            var value = bool.Parse(val);

                            var success = false;
                            if (match.Groups[1].Value == "basic")
                            {
                                success = Common.SetBasic(Program.controller, nodeId, value);
                            }
                            else if (match.Groups[1].Value == "binary")
                            {
                                success = Common.SetBinary(Program.controller, nodeId, value);
                            }
                            Common.logger.Info("{0}successful!", success ? "" : "not ");
                            blockExit = false;
                            break;
                        }
                    case var basicGetVal when basicGetRegex.IsMatch(basicGetVal):
                        {
                            blockExit = true;
                            var match = basicGetRegex.Match(basicGetVal);
                            var val = match.Groups[2].Value;
                            var nodeId = byte.Parse(val);

                            var success = false;
                            bool ret;
                            if (match.Groups[1].Value == "basic")
                            {
                                success = Common.GetBasic(Program.controller, nodeId, out ret);
                            }
                            else
                            {
                                success = Common.GetBinary(Program.controller, nodeId, out ret);
                            }
                            Common.logger.Info("{0}successful!", success ? "" : "not ");
                            if (success)
                            {
                                Common.logger.Info("value is: {0}", ret);
                            }
                            blockExit = false;
                            break;
                        }
                    case var configVal when configRegex.IsMatch(configVal):
                        {
                            var val = configRegex.Match(configVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            currentCommand = new ConfigCommand(Program.controller, nodeId, Program.configList);
                            break;
                        }
                    case var replaceVal when replaceRegex.IsMatch(replaceVal):
                        {
                            var val = replaceRegex.Match(replaceVal).Groups[1].Value;
                            var nodeId = byte.Parse(val);
                            currentCommand = new ReplaceCommand(Program.controller, nodeId, Program.configList);
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
            } while (Active && !oneShot);

            while (!listenComand.Active)
            {
                Thread.Sleep(100);
            }
            InputManager.Interrupt();
            listenComand.Stop();
            Common.logger.Info("goodby master...");
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