using System;
using System.IO;
using System.Threading;
using Utils.Events;
using ZWave.Layers;

namespace ZWave
{
    /// <summary>
    /// Note: Closing this thread is called from the thread that owns event handlers so cross blocking may occurs. 
    /// Check isClosing before fire an event
    /// </summary>
    public sealed class StorageReader : IDisposable
    {
        private Thread worker;
        private bool isClosing = false;
        public bool IsOpen { get; private set; }
        public byte ProgressValue { get; private set; }
        public StorageHeader StorageHeader { get; set; }
        public event EventHandler<EventArgs<DataChunk>> DataReceived;
        public event EventHandler<DataSourceResetEventArgs> FileDataSourceReset;
        public event EventHandler<DataSourceSetEventArgs> FileDataSourceSet;
        private long mFromPosition, mToPosition = 0;
        private string mFileName = null;
        public StorageReader()
        {
        }

        public void Open(string fileName, long fromPosition, long toPosition)
        {
            if (!IsOpen)
            {
                try
                {
                    ProgressValue = 0;
                    mFromPosition = fromPosition;
                    mToPosition = toPosition;
                    mFileName = fileName;
                    worker = new Thread(DoWork);
                    worker.Name = "Storage Reader worker";
                    worker.IsBackground = true;
                    worker.Priority = ThreadPriority.Normal;
                    worker.Start();
                    IsOpen = true;
                }
                catch (OutOfMemoryException)
                {
                    IsOpen = false;
                }
                catch (IOException)
                {
                    IsOpen = false;
                }
            }
        }

        public void Close()
        {
            if (IsOpen)
            {
                isClosing = true;
                if (worker.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                {
                    worker.Join();
                }
                isClosing = false;
            }
        }

        public StorageHeader OpenHeader(string fileName)
        {
            StorageHeader = null;
            if (!IsOpen)
            {
                using (BinaryReader sReader = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 100)))
                {
                    byte[] buffer = new byte[StorageHeader.STORAGE_HEADER_SIZE];
                    if (StorageHeader.STORAGE_HEADER_SIZE == sReader.Read(buffer, 0, StorageHeader.STORAGE_HEADER_SIZE))
                    {
                        StorageHeader = new StorageHeader(buffer);
                    }
                }
            }
            return StorageHeader;
        }

        private void DoWork()
        {
            bool isReportSetSignaled = false;
            if (File.Exists(mFileName))
            {
                using (BinaryReader sReader = new BinaryReader(new FileStream(mFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 100)))
                {
                    bool isEOF = false;
                    long fromPosition = mFromPosition > StorageHeader.STORAGE_HEADER_SIZE ? mFromPosition : StorageHeader.STORAGE_HEADER_SIZE;
                    long toPosition = mToPosition > 0 && mToPosition < sReader.BaseStream.Length ? mToPosition : sReader.BaseStream.Length;
                    long total = toPosition - fromPosition;
                    long fileLength = sReader.BaseStream.Length;
                    bool isPartial = toPosition < sReader.BaseStream.Length;

                    ReadHeader(sReader, false);
                    sReader.BaseStream.Position = fromPosition;

                    ReportReset();
                    while (!isClosing)
                    {
                        if (!isEOF)
                        {
                            DataChunk dataChunk = DataChunk.ReadDataChunk(sReader);
                            if (dataChunk != null)
                                ReportReceiveData(dataChunk);
                            else
                                isEOF = true;

                            if (isPartial && sReader.BaseStream.Position > toPosition)
                                isEOF = true;

                            if (total > 0)
                                ProgressValue = (byte)((sReader.BaseStream.Position - fromPosition) * 100 / total);
                        }
                        else
                        {
                            ProgressValue = 100;
                            ReportSet();
                            isReportSetSignaled = true;
                            if (isPartial)
                                break;
                            while (!isClosing && isEOF)
                            {
                                Thread.Sleep(100);
                                long newFileLength = sReader.BaseStream.Length;
                                if (newFileLength < fileLength)
                                {
                                    fileLength = newFileLength;
                                    isEOF = false;
                                    ReadHeader(sReader, false);
                                    ReportReset();
                                }
                                else if (newFileLength > fileLength)
                                {
                                    fileLength = newFileLength;
                                    isEOF = false;
                                }
                            }
                        }
                    }
                }
            }

            IsOpen = false;
            ProgressValue = 100;
            if (!isReportSetSignaled)
                ReportSet();
        }

        private void ReadHeader(BinaryReader sReader, bool keepOriginalPosition)
        {
            long currentPosition = sReader.BaseStream.Position;
            sReader.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[StorageHeader.STORAGE_HEADER_SIZE];
            if (StorageHeader.STORAGE_HEADER_SIZE == sReader.Read(buffer, 0, StorageHeader.STORAGE_HEADER_SIZE))
            {
                StorageHeader = new StorageHeader(buffer);
            }
            if (keepOriginalPosition)
                sReader.BaseStream.Position = currentPosition;
        }

        private void ReportReset()
        {
            if (FileDataSourceReset != null)
                FileDataSourceReset(this, new DataSourceResetEventArgs(StorageHeader));
        }

        private void ReportSet()
        {
            if (FileDataSourceSet != null)
                FileDataSourceSet(this, new DataSourceSetEventArgs());
        }

        private void ReportReceiveData(DataChunk dataChunk)
        {
            if (dataChunk != null && DataReceived != null)
            {
                DataReceived(this, new EventArgs<DataChunk>(dataChunk));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }

    public class DataSourceSetEventArgs : EventArgs
    {
        public DataSourceSetEventArgs()
        {
        }
    }

    public class DataSourceResetEventArgs : EventArgs
    {
        public StorageHeader StorageHeader { get; set; }
        public DataSourceResetEventArgs(StorageHeader storageHeader)
        {
            StorageHeader = storageHeader;
        }
    }
}
