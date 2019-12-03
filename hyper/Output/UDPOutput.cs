﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ZWave.CommandClasses;

namespace hyper.Output
{
    class UDPOutput : IOutput
    {
        private Socket socket;
        IPEndPoint ep;

        public UDPOutput(string ipAdress, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse(ipAdress);

            ep = new IPEndPoint(broadcast, port);

        }


        public void HandleCommand(object command, byte srcNodeId, byte destNodeId)
        {
            byte[] buffer;
            byte[] nodeId = BitConverter.GetBytes((short)srcNodeId);
            byte[] commandClass;
            byte[] instance = { 0, 1};
            byte[] index = { 0, 0 };
            byte[] values;


            

            switch (command)
            {
                case COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT alarmReport:
                    commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_NOTIFICATION_V8.ID);
                    if(alarmReport.mevent == 255)
                    {
                        values = BitConverter.GetBytes((short)1);
                    }
                    else
                    {
                        values = BitConverter.GetBytes((short)alarmReport.mevent);
                    }
                    break;
                case COMMAND_CLASS_BASIC_V2.BASIC_SET basicSet:
                    commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_METER_V5.ID);
                    if (basicSet.value == 255)
                    {
                        values = BitConverter.GetBytes((short)1);
                    }
                    else
                    {
                        values = BitConverter.GetBytes((short)basicSet.value);
                    }
                    break;
                case COMMAND_CLASS_BATTERY.BATTERY_REPORT batteryReport:
                    commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_BATTERY.ID);
                    if (batteryReport.batteryLevel == 255)
                    {
                        values = BitConverter.GetBytes((short)1);
                    }
                    else
                    {
                        values = BitConverter.GetBytes((short)batteryReport.batteryLevel);
                    }
                    break;
                case COMMAND_CLASS_SENSOR_BINARY_V2.SENSOR_BINARY_REPORT binaryReport:
                    commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_SENSOR_BINARY_V2.ID);
                    if(binaryReport.sensorValue == 255)
                    {
                        values = BitConverter.GetBytes((short)1);
                    }
                    else
                    {
                        values = BitConverter.GetBytes((short)binaryReport.sensorValue);
                    }
                    break;
                case COMMAND_CLASS_SENSOR_MULTILEVEL_V11.SENSOR_MULTILEVEL_REPORT multiReport:
                    commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_SENSOR_MULTILEVEL_V11.ID);
                    //values = multiReport.sensorValue.ToArray();
                    var shortVal = BitConverter.ToInt16(multiReport.sensorValue.Reverse().ToArray(),0);
                    Console.WriteLine("SHORT: {0}", shortVal);
                    var floatVal = (float)shortVal;
                    values = BitConverter.GetBytes(floatVal / 10.0f);
                    break;
                case COMMAND_CLASS_SWITCH_BINARY_V2.SWITCH_BINARY_REPORT binaryReport:
                    commandClass = BitConverter.GetBytes((short)COMMAND_CLASS_SWITCH_BINARY_V2.ID);
                    if (binaryReport.currentValue == 255)
                    {
                        values = BitConverter.GetBytes((short)1);
                    }
                    else
                    {
                        values = BitConverter.GetBytes((short)binaryReport.currentValue);
                    }
                    break;
                default:
                    return;
            }
            buffer = nodeId.Reverse().Concat(commandClass.Reverse()).Concat(instance).Concat(index).Concat(values.Reverse()).ToArray();
            Console.WriteLine(ByteArrayToString(buffer));
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
