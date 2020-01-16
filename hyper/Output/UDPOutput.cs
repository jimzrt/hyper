using hyper.Helper;
using hyper.Helper.Extension;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ZWave.CommandClasses;

namespace hyper.Output
{
    internal class UDPOutput : IOutput
    {
        private Socket socket;
        private IPEndPoint ep;

        public UDPOutput(string ipAdress, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse(ipAdress);

            ep = new IPEndPoint(broadcast, port);

            //string test = "{\"properties1\":{\"sourceEndPoint\":2,\"res\":0},\"properties2\":{\"destinationEndPoint\":1,\"bitAddress\":0},\"commandClass\":32,\"command\":1,\"parameter\":[255]}";
            //var testObj = (COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP)Util.JsonToObj(test, typeof(COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP));
            //HandleCommand(testObj, 80, 80);
        }

        public void HandleCommand(object command, byte srcNodeId, byte destNodeId)
        {
            byte[] buffer;
            byte[] nodeId = BitConverter.GetBytes((short)srcNodeId);
            byte[] commandClass;
            byte[] instance = { 0, 1 };
            byte[] index = { 0, 0 };
            byte[] values;

            switch (command)
            {
                case COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT alarmReport:
                    {
                        commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_NOTIFICATION_V8.ID);
                        var value = alarmReport.mevent;
                        values = BitConverter.GetBytes((short)(value == 255 ? 1 : value));
                        break;
                    }
                case COMMAND_CLASS_BASIC_V2.BASIC_SET basicSet:
                    {
                        commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_BASIC_V2.ID);
                        var value = basicSet.value;
                        values = BitConverter.GetBytes((short)(value == 255 ? 1 : value));
                    }
                    break;

                case COMMAND_CLASS_BATTERY.BATTERY_REPORT batteryReport:
                    {
                        commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_BATTERY.ID);
                        var value = batteryReport.batteryLevel;
                        values = BitConverter.GetBytes((short)(value == 255 ? 1 : value));

                        break;
                    }

                case COMMAND_CLASS_SENSOR_BINARY_V2.SENSOR_BINARY_REPORT binaryReport:
                    {
                        commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_SENSOR_BINARY_V2.ID);
                        var value = binaryReport.sensorValue;
                        values = BitConverter.GetBytes((short)(value == 255 ? 1 : value));

                        break;
                    }

                case COMMAND_CLASS_SENSOR_MULTILEVEL_V11.SENSOR_MULTILEVEL_REPORT multiReport:
                    {
                        commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_SENSOR_MULTILEVEL_V11.ID);
                        index = new byte[] { 0, multiReport.sensorType };

                        multiReport.GetKeyValue(out Enums.EventKey eventType, out float floatVal);
                        //Console.WriteLine("FLOAT: {0}", floatVal);
                        values = BitConverter.GetBytes(floatVal);
                        break;
                    }

                case COMMAND_CLASS_SWITCH_BINARY_V2.SWITCH_BINARY_REPORT binaryReport:
                    {
                        commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_SWITCH_BINARY_V2.ID);
                        var value = binaryReport.currentValue;
                        values = BitConverter.GetBytes((short)(value == 255 ? 1 : value));

                        break;
                    }

                case COMMAND_CLASS_BASIC_V2.BASIC_REPORT basicReport:
                    {
                        commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_BASIC_V2.ID);
                        var value = basicReport.currentValue;
                        values = BitConverter.GetBytes((short)(value == 255 ? 1 : value));

                        break;
                    }

                case COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP multiChannelReport:
                    {
                        //alfred shit!
                        instance[1] = (byte)(multiChannelReport.properties2.destinationEndPoint + 1);
                        commandClass = BitConverter.GetBytes((short)multiChannelReport.commandClass);
                        var value = multiChannelReport.parameter[0];
                        values = BitConverter.GetBytes((short)(value == 255 ? 1 : value));

                        break;
                    }

                default:
                    return;
            }
            buffer = nodeId.Reverse().Concat(commandClass.Reverse()).Concat(instance).Concat(index).Concat(values.Reverse()).ToArray();
            // Console.WriteLine(ByteArrayToString(buffer));
            socket.SendTo(buffer, ep);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }
    }
}