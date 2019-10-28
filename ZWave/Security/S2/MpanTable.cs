using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace ZWave.Security
{
    public class MpanTable
    {
        public const int MAX_RECORDS_COUNT = 10;

        private readonly object _synchObject = new object();
        private readonly SizeLimitedTable<NodeGroupId, MpanContainer> _table;

        public MpanContainer this[NodeGroupId peerGroupId]
        {
            get
            {
                if (!_table.ContainsKey(peerGroupId))
                    throw new IndexOutOfRangeException("mpan container for such group id doesn't exist");

                return _table[peerGroupId];
            }
        }

        /// <summary>
        /// Creates mpan table with default MPAN records capacity of 10
        /// </summary>
        public MpanTable() : this(MAX_RECORDS_COUNT)
        {
        }

        /// <summary>
        /// Creates mpan table with specified MPAN records capacity
        /// </summary>
        /// <param name="maxRecordsCount">MPAN records capacity</param>
        public MpanTable(int maxRecordsCount)
        {
            _table = new SizeLimitedTable<NodeGroupId, MpanContainer>(MAX_RECORDS_COUNT);
        }

        public MpanContainer GetContainer(NodeGroupId peerGroupId)
        {
            return _table[peerGroupId];
        }

        public MpanContainer AddOrReplace(NodeGroupId nodeGroupId, byte sequenceNumber, byte[] receiverGroupHandle, byte[] mpanState)
        {
            lock (_synchObject)
            {
                if (_table.ContainsKey(nodeGroupId))
                {
                    _table.Remove(nodeGroupId);
                }
                var mpanContainer = new MpanContainer(nodeGroupId, mpanState, sequenceNumber, receiverGroupHandle);
                _table.Add(nodeGroupId, mpanContainer);
                return mpanContainer;
            }
        }

        public void RemoveRecord(NodeGroupId peerGroupId)
        {
            lock (_synchObject)
            {
                if (_table.ContainsKey(peerGroupId))
                {
                    _table.Remove(peerGroupId);
                }
            }
        }

        public void ClearMpanTable()
        {
            lock (_synchObject)
            {
                _table.Clear();
            }
        }

        public bool CheckMpanExists(NodeGroupId nodeGroupId)
        {
            lock (_synchObject)
            {
                return _table.ContainsKey(nodeGroupId);
            }
        }

        public byte FindGroup(byte[] destNodesIds)
        {
            lock (_synchObject)
            {
                var mpanPair = _table.FirstOrDefault(mpan => mpan.Value.DestNodesEquals(destNodesIds));
                return mpanPair.Value != null ? mpanPair.Value.NodeGroupId.GroupId : (byte)0;
            }
        }

        public bool IsRecordInMOSState(NodeGroupId nodeGroupId)
        {
            lock (_synchObject)
            {
                if (_table.ContainsKey(nodeGroupId))
                {
                    return _table[nodeGroupId].IsMosState;
                }
                return false;
            }
        }

        public byte[] SelectGroupIds(byte ownerId)
        {
            lock (_synchObject)
            {
                var groupIds = from mpan in _table
                               where mpan.Value.NodeGroupId.NodeId == ownerId
                               select mpan.Value.NodeGroupId.GroupId;

                return groupIds.ToArray();
            }
        }

        public List<MpanContainer> GetExistingRecords()
        {
            return _table.Select(x => x.Value).ToList();
        }

        public MpanContainer GetLatestContainerByOwnerId(byte ownerId)
        {
            MpanContainer ret = null;
            lock (_synchObject)
            {
                var record = _table.Where(item => item.Value.NodeGroupId.NodeId == ownerId)
                     .OrderByDescending(item => item.Value.ReceivedTimeStamp)
                     .FirstOrDefault();

                ret = record.Value;
            }
            return ret;
        }
    }
}
