using System;
using System.Collections.Generic;
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

            switch (command)
            {
                case COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT alarmReport:
                    buffer = new byte[] { srcNodeId, COMMAND_CLASS_NOTIFICATION_V8.ID, alarmReport.mevent };
                    break;
                case COMMAND_CLASS_BASIC_V2.BASIC_SET basicSet:
                    buffer = new byte[] { srcNodeId, COMMAND_CLASS_BASIC_V2.ID, basicSet.value };
                    break;
                case COMMAND_CLASS_BATTERY.BATTERY_REPORT batteryReport:
                    buffer = new byte[] { srcNodeId, COMMAND_CLASS_BATTERY.ID, batteryReport.batteryLevel };
                    break;
                default:
                    return;
            }

            socket.SendTo(buffer, ep);
        }
    }
}
