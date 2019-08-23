using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZWave.Enums;
using ZWave.Layers;

namespace ZWave
{
    public sealed class StorageWriter : IDisposable
    {
        private Thread worker;
        private AutoResetEvent signal = new AutoResetEvent(false);
        private bool isClosing = false;
        private object locker = new object();
        private List<DataChunk> innerListAdd = new List<DataChunk>();
        private List<DataChunk> innerListTmp = new List<DataChunk>();
        private List<DataChunk> innerListProcessing = new List<DataChunk>();
        public bool IsOpen { get; private set; }
        private StorageHeader mStorageHeader = new StorageHeader();
        public StorageHeader StorageHeader
        {
            get { return mStorageHeader; }
        }

        private bool isHeaderCommitRequired = false;
        public DateTime CreatedAt = DateTime.Now;
        public long TotalBytes = StorageHeader.STORAGE_HEADER_SIZE;

        public StorageWriter(byte apiType, int version)
        {
            mStorageHeader.ApiType = apiType;
            mStorageHeader.Version = version;
        }

        public void Open(string fileName)
        {
            if (!IsOpen)
            {
                try
                {
                    CreatedAt = DateTime.Now;
                    TotalBytes = StorageHeader.STORAGE_HEADER_SIZE;
                    worker = new Thread(DoWork);
                    worker.IsBackground = true;
                    worker.Start(fileName);
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
                signal.Set();
                if (worker.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                {
                    worker.Join();
                }
                isClosing = false;
            }
        }

        public void Reset()
        {
            if (IsOpen)
            {

            }
        }

        public void Write(DataChunk dc)
        {
            if (IsOpen && !isClosing)
            {
                lock (locker)
                {
                    TotalBytes += dc.TotalBytes;
                    innerListAdd.Add(dc);
                }
                signal.Set();
            }
        }

        private object headerLocker = new object();
        public void AddHeaderFrequencies(string fileName, Dictionary<byte, RFrequency> frequencies)
        {
            if (fileName != null && frequencies != null)
            {
                lock (headerLocker)
                {
                    mStorageHeader.Frequencies.Clear();
                    foreach (var item in frequencies)
                    {
                        mStorageHeader.Frequencies.Add(item.Key, item.Value);
                    }
                }
                if (IsOpen)
                {
                    isHeaderCommitRequired = true;
                    signal.Set();
                }
                else
                {
                    mStorageHeader.UpdateSourceAndFrequensiesToBuffer();
                    if (File.Exists(fileName))
                    {
                        using (BinaryWriter sWriter = new BinaryWriter(new FileStream(fileName, FileMode.Open)))
                        {
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                            sWriter.Seek(0, SeekOrigin.End);
                        }
                    }
                    else
                    {
                        using (BinaryWriter sWriter = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                        {
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                            sWriter.Seek(0, SeekOrigin.End);
                        }
                    }
                }
            }
        }

        public void AddHeaderSource(string fileName, byte sessionId, string sourceName, bool isClearBefore)
        {
            if (fileName != null && sourceName != null)
            {
                lock (headerLocker)
                {
                    if (isClearBefore)
                    {
                        mStorageHeader.Sessions.Clear();
                    }
                    if (mStorageHeader.Sessions.Count < 10 && !mStorageHeader.Sessions.ContainsKey(sessionId))//limit storing source names by 10
                    {
                        mStorageHeader.Sessions.Add(sessionId, sourceName);
                    }
                }

                if (IsOpen)
                {
                    isHeaderCommitRequired = true;
                    signal.Set();
                }
                else
                {
                    mStorageHeader.UpdateSourceAndFrequensiesToBuffer();
                    if (File.Exists(fileName))
                    {
                        using (BinaryWriter sWriter = new BinaryWriter(new FileStream(fileName, FileMode.Open)))
                        {
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                            sWriter.Seek(0, SeekOrigin.End);
                        }
                    }
                    else
                    {
                        using (BinaryWriter sWriter = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                        {
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                            sWriter.Seek(0, SeekOrigin.End);
                        }
                    }
                }
            }
        }


        public static void SetAttachments(string fileName, long position, byte sessionId, IList<StorageAttachment> attachments)
        {
            if (position > 800)
            {
                if (fileName != null && attachments != null)
                {
                    if (File.Exists(fileName))
                    {
                        using (BinaryWriter sWriter = new BinaryWriter(new FileStream(fileName, FileMode.Open)))
                        {
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.BaseStream.Position = position;
                            foreach (var attachment in attachments)
                            {
                                DataChunk dcWrite = new DataChunk(attachment.ToByteArray(), sessionId, false, ApiTypes.Attachment);
                                byte[] dataBuffer = dcWrite.ToByteArray();
                                sWriter.Write(dataBuffer, 0, dataBuffer.Length);
                            }
                        }
                    }
                }
            }
        }

        public void SetHeaderComment(string fileName, string comment)
        {
            if (fileName != null && comment != null)
            {
                mStorageHeader.Comment = comment;
                if (IsOpen)
                {
                    isHeaderCommitRequired = true;
                    signal.Set();
                }
                else
                {
                    mStorageHeader.UpdateSourceAndFrequensiesToBuffer();
                    if (File.Exists(fileName))
                    {
                        using (BinaryWriter sWriter = new BinaryWriter(new FileStream(fileName, FileMode.Open)))
                        {
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                            sWriter.Seek(0, SeekOrigin.End);
                        }
                    }
                    else
                    {
                        using (BinaryWriter sWriter = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                        {
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                            sWriter.Seek(0, SeekOrigin.End);
                        }
                    }
                }
            }
        }

        private void DoWork(object fileName)
        {
            using (BinaryWriter sWriter = new BinaryWriter(new FileStream((string)fileName, FileMode.Create)))
            {
                mStorageHeader.UpdateSourceAndFrequensiesToBuffer();
                sWriter.Seek(0, SeekOrigin.Begin);
                sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                sWriter.Seek(0, SeekOrigin.End);
                while (!isClosing)
                {
                    signal.WaitOne();
                    lock (locker)
                    {
                        innerListTmp = innerListProcessing;
                        innerListProcessing = innerListAdd;
                        innerListAdd = innerListTmp;
                    }
                    if (isHeaderCommitRequired)
                    {
                        lock (headerLocker)
                        {
                            isHeaderCommitRequired = false;
                            mStorageHeader.UpdateSourceAndFrequensiesToBuffer();
                            sWriter.Seek(0, SeekOrigin.Begin);
                            sWriter.Write(mStorageHeader.Buffer, 0, StorageHeader.STORAGE_HEADER_SIZE);
                            sWriter.Seek(0, SeekOrigin.End);
                        }
                    }
                    foreach (DataChunk dataChunk in innerListProcessing)
                    {
                        byte[] d = dataChunk.ToByteArray();
                        sWriter.Write(d, 0, d.Length);
                        sWriter.Flush();
                    }
                    innerListProcessing.Clear();
                    mStorageHeader.TraceTotalLength = sWriter.BaseStream.Position;
                }
            }
            IsOpen = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
            signal.Close();
        }

        #endregion

    }
}
