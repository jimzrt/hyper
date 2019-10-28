using System;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Operations;
using ZWave.Enums;

namespace ZWave.BasicApplication.Tasks
{
    public class ExclusionTask : ActionParallelGroup
    {
        public int WAKE_UP_INTERVAL = 5 * 60; //seconds
        private readonly Modes _mode;
        private readonly int _timeoutMs;
        private readonly Action<NodeStatuses> _nodeStatusCallback;


        private readonly FilterAchOperation _peerFilter;
        private readonly RemoveNodeOperation _removeNode;

        public ExclusionTask(Modes mode, Action<NodeStatuses> nodeStatusCallback, int timeoutMs)
            : base(false, null)
        {
            _mode = mode;
            _nodeStatusCallback = nodeStatusCallback;
            _timeoutMs = timeoutMs;

            _peerFilter = new FilterAchOperation();
            _peerFilter.SetFilterNodeId(0xFF);
            _removeNode = new RemoveNodeOperation(_mode, OnNodeStatus, _timeoutMs);

            SpecificResult.AddRemoveNode = _removeNode.SpecificResult;

            Actions = new ActionBase[]
            {
                _peerFilter,
                _removeNode
            };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            base.SetStateCompleted(ou);
        }

        private void OnNodeStatus(NodeStatuses nodeStatus)
        {
            if (nodeStatus == NodeStatuses.AddingRemovingController || nodeStatus == NodeStatuses.AddingRemovingSlave)
            {
                _peerFilter.SetFilterNodeId(_removeNode.SpecificResult.Id);
            }
            if (_nodeStatusCallback != null)
                _nodeStatusCallback(nodeStatus);
        }

        public ExclusionResult SpecificResult
        {
            get { return (ExclusionResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ExclusionResult();
        }
    }

    public class ExclusionResult : ActionResult
    {
        public AddRemoveNodeResult AddRemoveNode { get; set; }
    }
}
