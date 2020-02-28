using hyper.Inputs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace hyper.Input
{
    internal class UDPInput : IInput
    {
        //     public bool CanRead { get; set; } = false;
        private readonly object _syncObj = new object();

        private BlockingCollection<string> messageQueue = new BlockingCollection<string>();

        //private string currentMessage = "";
        // private ManualResetEvent resetEvent;

        public event ConsoleCancelEventHandler CancelKeyPress;

        private UdpClient listener;

        public UDPInput(int listenPort)
        {
            try
            {
                listener = new UdpClient(listenPort);
                // IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

                Task.Run(async () =>
                {
                    while (true)
                    {
                        var received = await listener.ReceiveAsync();

                        byte[] bytes = received.Buffer;
                        Common.logger.Info($"Received broadcast from {received.RemoteEndPoint.Address}:");
                        if (bytes.Length >= 10)
                        {
                            var nodeId = BitConverter.ToInt16(bytes.Skip(0).Take(2).Reverse().ToArray());
                            var commandClass = BitConverter.ToInt16(bytes.Skip(2).Take(2).Reverse().ToArray());
                            var endpoint = BitConverter.ToInt16(bytes.Skip(4).Take(2).Reverse().ToArray());
                            var value = BitConverter.ToBoolean(bytes.Skip(8).Take(1).ToArray());
                            Common.logger.Info($"node id: {nodeId} - command class: {commandClass} - endpoint: {endpoint} - value: {value}");
                            if (endpoint > 1)
                            {
                                lock (_syncObj) messageQueue.Add($"multi {nodeId} {endpoint - 1} {value}");
                            }
                            else
                            {
                                lock (_syncObj) messageQueue.Add($"basic {nodeId} {value}");
                            }
                        }

                        //    resetEvent.Set();
                    }
                });
            }
            catch (Exception e)
            {
                Common.logger.Error(this.GetType().Name + ": " + e.Message);
            }
        }

        //public bool Available()
        //{
        //    lock (_syncObj)
        //        return messageQueue.Count > 0;
        //}

        //public void Flush()
        //{
        //    currentMessage = "";
        //}

        //public string Read()
        //{
        //    lock (_syncObj)
        //        return messageQueue.Dequeue();
        //}

        //public void SetResetEvent(ManualResetEvent resetEvent)
        //{
        //    this.resetEvent = resetEvent;
        //}

        public void Interrupt()
        {
            listener?.Close();
        }

        public void SetQueue(BlockingCollection<string> ownQueue)
        {
            messageQueue = ownQueue;
        }
    }
}