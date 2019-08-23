using Utils;

namespace ZWave.BasicApplication.Operations
{
    public class FilterAchOperation : ApiAchOperation
    {
        private byte _filterNodeId;
        private byte _filterSucNodeId;
        public FilterAchOperation()
            : base(0, 0, null)
        {
            IsFirstPriority = true;
        }

        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            if (_filterNodeId > 0)
            {
                if (ReceivedAchData.SrcNodeId == _filterNodeId ||
                     (_filterSucNodeId > 0 && ReceivedAchData.SrcNodeId == _filterSucNodeId))
                {
                }
                else
                {
                    ou.DataFrame.IsHandled = true;
                    "DENIED FilterNodeId: {0}"._DLOG(ReceivedAchData.SrcNodeId);
                }
            }
        }

        public void SetFilterNodeId(byte nodeId)
        {
            _filterNodeId = nodeId;
        }

        public void SetFilterSucNodeId(byte sucNodeId)
        {
            _filterSucNodeId = sucNodeId;
        }
    }
}
