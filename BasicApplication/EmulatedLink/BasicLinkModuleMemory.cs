using System;

namespace ZWave.BasicApplication.EmulatedLink
{
    public class BasicLinkModuleMemory
    {
        static int sharedCounter = 1;

        public byte NodeId { get; set; }
        public byte[] HomeId { get; set; }
        public byte[] CmdClasses { get; set; }
        public byte AddOrReplaceNodeId { get; set; }
        public bool IsAddingNode { get; set; }
        public bool IsReplacingNode { get; set; }
        public bool IsRemovingNode { get; set; }
        public byte FuncId { get; set; }
        public bool IsRFReceiveMode { get; set; }
        public byte SucNodeId { get; set; }
        public byte Basic { get; set; }
        public byte Generic { get; set; }
        public byte Specific { get; set; }

        private byte _nextNodeId;
        private byte _defaultNodeId;

        public BasicLinkModuleMemory(byte defaultNodeId)
        {
            _defaultNodeId = defaultNodeId;
            Basic = 1;
            Generic = 2;
            Specific = 1;
            Reset();
        }

        public void Reset()
        {
            NodeId = _defaultNodeId;
            HomeId = BitConverter.GetBytes(++sharedCounter);
            _nextNodeId = (byte)(NodeId + 1);
            IsRFReceiveMode = true;
            SucNodeId = 0;
            IsAddingNode = false;
            IsRemovingNode = false;
            IsReplacingNode = false;
            FuncId = 0;
        }

        internal byte SeedNextNodeId()
        {
            if (_nextNodeId < NodeId)
            {
                _nextNodeId = (byte)(NodeId + 1);
            }
            return _nextNodeId++;
        }
    }
}
