using System;
using System.Linq;
using System.Threading;
using Utils;
using Utils.Threading;

namespace ZWave.Layers.Frame
{
    public class FrameBufferBlock : IDisposable
    {
        private const int ACK_TIME = 2000;
        private Func<byte[], int> _writeData;
        private Func<CommandMessage, byte[]> _createFrameBuffer;
        private ConsumerQueue<ActionHandlerResult> _queue;
        private Signal _ackSignal = new Signal();
        private volatile bool _isTransmitted = false;

        public FrameBufferBlock(Func<byte[], int> writeData, Func<CommandMessage, byte[]> createFrameBuffer)
        {
            _writeData = writeData;
            _createFrameBuffer = createFrameBuffer;
            _queue = new ConsumerQueue<ActionHandlerResult>();
        }

        public void Stop()
        {
            _queue.Stop();
        }

        public void Start()
        {
            _queue.Start("FrameBufferBlock", Consume);
        }

        public void Add(ActionHandlerResult frame)
        {
            _queue.Add(frame);
        }

        public void Acknowledge(bool isTransmitted)
        {
            _isTransmitted = isTransmitted;
            if (_queue.IsOpen)
            {
                _ackSignal.Set();
            }
        }

        public void Consume(ActionHandlerResult ahResult)
        {
            Send(ahResult);
        }

        public bool Send(ActionHandlerResult ahResult)
        {
            bool ret = false;
            if (ahResult != null && ahResult.NextActions != null)
            {
                var sendFrames = ahResult.NextActions.Where(x => x is CommandMessage);
                if (sendFrames.Any())
                {
                    ret = true;
                    foreach (CommandMessage frame in sendFrames)
                    {
                        byte[] tmp = _createFrameBuffer(frame);
                        byte[] dataTmp = frame.Data;
                        if (frame.IsSequenceNumberRequired)
                        {
                            dataTmp = new byte[frame.Data.Length + 1];
                            Array.Copy(frame.Data, 0, dataTmp, 0, frame.Data.Length);
                            dataTmp[dataTmp.Length - 1] = frame.SequenceNumber;
                        }
                        _ackSignal.Reset();
                        _isTransmitted = false;
                        _writeData(tmp);
                        ahResult.Parent.AddTraceLogItem(DateTime.Now, dataTmp, true);
                        if (!frame.IsNoAck)
                        {
                            int countCAN = 0;
                            int countNAK = 0;
                            while (countCAN < 10 && countNAK < 2 && !_isTransmitted && _queue.IsOpen)
                            {
                                bool res = _ackSignal.WaitOne(ACK_TIME);
                                if (!_isTransmitted && _queue.IsOpen)
                                {
                                    _writeData(tmp);
                                    ahResult.Parent.AddTraceLogItem(DateTime.Now, dataTmp, true);
                                    if (res)
                                    {
                                        countCAN++;
                                        Thread.Sleep(countCAN * 200); //wait after CAN received (for example ctrl is busy with smart start)
                                        $"CAN RECEIVED {countCAN}"._DLOG();
                                    }
                                    else
                                    {
                                        countNAK++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            _isTransmitted = true;
                        }
                        ret &= _isTransmitted;
                    }
                    if (ahResult.NextFramesCompletedCallback != null)
                        ahResult.NextFramesCompletedCallback(ret);
                }
            }
            return ret;
        }

        public void Dispose()
        {
            Stop();
            ((IDisposable)_ackSignal).Dispose();
        }
    }
}
