using System;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Frame;

namespace ZWave.BasicApplication
{
    public class BasicFrameClient : IFrameClient
    {
        private const byte MAX_FRAME_SIZE = 255;
        private const byte MIN_FRAME_SIZE = 3;
        private static byte[] ACK = new[] { (byte)HeaderTypes.Acknowledge };
        private static byte[] NAK = new[] { (byte)HeaderTypes.NotAcknowledged };
        private static byte[] CAN = new[] { (byte)HeaderTypes.Can };

        public byte SessionId { get; set; }
        public Action<CustomDataFrame> ReceiveFrameCallback { get; set; }
        public Func<byte[], int> SendDataCallback { get; set; }
        public DataFrame ReceivingDataFrame { get; set; }
        public FrameBufferBlock FrameBufferBlock { get; set; }
        public int ReceivingDataFrameLength { get; set; }

        private int _receivingDataFrameLengthCounter;
        private byte[] _receivingDataFrameBuffer = new byte[MAX_FRAME_SIZE];
        private FrameReceiveStates _parserState;
        private readonly object _locker = new object();
        private bool _isDisposed = false;
        private Action<DataFrame> _transmitCallback;

        public BasicFrameClient(Action<DataFrame> transmitCallback)
        {
            _transmitCallback = transmitCallback;
            FrameBufferBlock = new FrameBufferBlock(WriteDataSafe, CreateBuffer);
            FrameBufferBlock.Start();
        }

        private byte[] CreateBuffer(CommandMessage frame)
        {
            byte[] data = frame.Data;
            if (frame.IsSequenceNumberRequired)
            {
                if (frame.SequenceNumberCustomIndex > 0)
                {
                    data[frame.SequenceNumberCustomIndex] = frame.SequenceNumber;
                }
                else
                {
                    data = new byte[frame.Data.Length + 1];
                    Array.Copy(frame.Data, 0, data, 0, frame.Data.Length);
                    data[frame.Data.Length] = frame.SequenceNumber;
                }
            }

            byte[] tmp = DataFrame.CreateFrameBuffer(data);

            return tmp;
        }

        private int WriteDataSafe(byte[] data)
        {
            if (_transmitCallback != null)
            {
                DataFrame dataFrame = new DataFrame(SessionId, DataFrameTypes.Data, false, true, DateTime.Now);
                dataFrame.SetBuffer(data, 0, data.Length);
                _transmitCallback(dataFrame);
            }

            return WriteData(data);
        }

        private int WriteData(byte[] data)
        {
            if (SendDataCallback != null)
            {
                return SendDataCallback(data);
            }
            else
            {
                return -1;
            }
        }


        public void ResetParser()
        {
            lock (_locker)
            {
                _parserState = FrameReceiveStates.FRS_SOF_HUNT;
                ResetReceivingDataFrameBuffer();
            }
        }

        private int AddToReceivingDataFrameBuffer(byte buffer)
        {
            if (_receivingDataFrameLengthCounter < MAX_FRAME_SIZE)
            {
                _receivingDataFrameBuffer[_receivingDataFrameLengthCounter] = buffer;
                _receivingDataFrameLengthCounter++;
                return 1;
            }
            else return 0;
        }

        private void ResetReceivingDataFrameBuffer()
        {
            _receivingDataFrameLengthCounter = 0;
        }

        public bool SendFrames(ActionHandlerResult frameData)
        {
            bool ret = false;
            ret = FrameBufferBlock.Send(frameData);
            return ret;
        }

        public void HandleData(DataChunk dataChunk, bool isFromFile)
        {
            if (dataChunk.ApiType == ApiTypes.Basic)
            {
                byte[] data = dataChunk.GetDataBuffer();
                if (data != null && data.Length > 0)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        ParseRawData(data[i], dataChunk.IsOutcome, dataChunk.TimeStamp, isFromFile);
                    }
                }
            }
        }
        bool _isTestInterface = false;
        private byte[] ParseRawData(byte buffer, bool isOutcome, DateTime timeStamp, bool isFromFile)
        {
            byte[] ret = null;
            AddToReceivingDataFrameBuffer(buffer);
            lock (_locker)
            {
                switch (_parserState)
                {
                    case FrameReceiveStates.FRS_SOF_HUNT:
                        _isTestInterface = false;
                        if (HeaderTypes.StartOfFrame == (HeaderTypes)buffer)
                        {
                            ResetReceivingDataFrameBuffer();
                            AddToReceivingDataFrameBuffer(buffer);
                            ReceivingDataFrame = new DataFrame(SessionId, DataFrameTypes.Data, isFromFile, isOutcome, timeStamp);
                            _parserState = FrameReceiveStates.FRS_LENGTH;
                        }
                        // Acknowledge received from peer.
                        else if (HeaderTypes.Acknowledge == (HeaderTypes)buffer)
                        {
                            FrameBufferBlock.Acknowledge(true);
                            ReceivingDataFrame = new DataFrame(SessionId, DataFrameTypes.Ack, isFromFile, isOutcome, timeStamp);
                            ReceivingDataFrame.SetBuffer(ACK, 0, 1);
                            OnFrameReceived();
                        }
                        // Not Acknowledge received from peer.
                        else if (HeaderTypes.NotAcknowledged == (HeaderTypes)buffer)
                        {
                            ReceivingDataFrame = new DataFrame(SessionId, DataFrameTypes.NAck, isFromFile, isOutcome, timeStamp);
                            ReceivingDataFrame.SetBuffer(NAK, 0, 1);
                            OnFrameReceived();
                        }
                        // CAN frame received - peer dropped a data frame transmitted by us.
                        else if (HeaderTypes.Can == (HeaderTypes)buffer)
                        {
                            FrameBufferBlock.Acknowledge(false);
                            ReceivingDataFrame = new DataFrame(SessionId, DataFrameTypes.CAN, isFromFile, isOutcome, timeStamp);
                            ReceivingDataFrame.SetBuffer(CAN, 0, 1);
                            OnFrameReceived();
                        }
                        break;

                    case FrameReceiveStates.FRS_LENGTH:
                        if (buffer == 0)
                        {
                            _parserState = FrameReceiveStates.FRS_LENGHT2;
                            _isTestInterface = true;
                        }
                        else if (buffer < MIN_FRAME_SIZE || buffer > MAX_FRAME_SIZE)
                        {
                            _parserState = FrameReceiveStates.FRS_SOF_HUNT;
                        }
                        else
                        {
                            ReceivingDataFrameLength = (byte)(buffer + 1); // Payload size is excluding len field.
                            _parserState = FrameReceiveStates.FRS_TYPE;
                        }
                        break;
                    case FrameReceiveStates.FRS_LENGHT2:
                        if (buffer < MIN_FRAME_SIZE || buffer > MAX_FRAME_SIZE)
                        {
                            _parserState = FrameReceiveStates.FRS_SOF_HUNT;
                        }
                        else
                        {
                            ReceivingDataFrameLength = (byte)(buffer + 1); // Payload size is excluding len field.
                            _parserState = FrameReceiveStates.FRS_COMMAND;
                        }
                        break;
                    case FrameReceiveStates.FRS_TYPE:
                        if (buffer == (byte)FrameTypes.Request || buffer == (byte)FrameTypes.Response)
                        {
                            _parserState = FrameReceiveStates.FRS_COMMAND;
                        }
                        else
                        {
                            _parserState = FrameReceiveStates.FRS_SOF_HUNT;
                        }
                        break;

                    case FrameReceiveStates.FRS_COMMAND:
                        if (_receivingDataFrameLengthCounter == ReceivingDataFrameLength)
                        {
                            _parserState = FrameReceiveStates.FRS_CHECKSUM;
                        }
                        else
                        {
                            _parserState = FrameReceiveStates.FRS_DATA;
                        }
                        break;

                    case FrameReceiveStates.FRS_DATA:
                        if (_receivingDataFrameLengthCounter > ReceivingDataFrameLength)
                        {
                            _parserState = FrameReceiveStates.FRS_SOF_HUNT;
                        }
                        else if (_receivingDataFrameLengthCounter == ReceivingDataFrameLength)
                        {
                            _parserState = FrameReceiveStates.FRS_CHECKSUM;
                        }
                        break;

                    case FrameReceiveStates.FRS_CHECKSUM:
                        // Frame received successfully -> Send acknowledge (ACK).
                        ReceivingDataFrame.SetBuffer(_receivingDataFrameBuffer, 0, _receivingDataFrameLengthCounter);
                        if (_isTestInterface)
                        {
                            if (!isFromFile)
                            {
                                WriteData(ACK);
                            }
                        }
                        else
                        {
                            if (!isFromFile && ReceivingDataFrame.IsChecksumValid(buffer))
                            {
                                WriteData(ACK);
                            }
                            // Frame receive failed -> Send NAK.
                            else if (!isFromFile)
                            {
                                WriteData(NAK);
                            }
                        }
                        OnFrameReceived();
                        _parserState = FrameReceiveStates.FRS_SOF_HUNT;
                        break;

                    case FrameReceiveStates.FRS_RX_TIMEOUT:
                    default:
                        _parserState = FrameReceiveStates.FRS_SOF_HUNT;
                        break;

                }
            }
            return ret;
        }

        private void OnFrameReceived()
        {
            if (ReceivingDataFrame != null)
            {
                if (_transmitCallback != null && ReceivingDataFrame.DataFrameType == DataFrameTypes.Data)
                {
                    _transmitCallback(ReceivingDataFrame);
                }
                if (ReceiveFrameCallback != null && ReceivingDataFrame.DataFrameType == DataFrameTypes.Data)
                {
                    ReceiveFrameCallback(ReceivingDataFrame);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            FrameBufferBlock.Dispose();
            lock (_locker)
            {
                if (_isDisposed)
                {
                    return;
                }
                _isDisposed = true;
            }
        }

        #endregion
    }
}
