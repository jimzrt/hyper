using System;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Operations;
using ZWave.BasicApplication.Tasks;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication
{
    public class SmartStartSupport : ApiOperation
    {
        private NetworkViewPoint _network;
        private byte[] _homeId;
        private Action<NodeStatuses> _setNodeStatusSignal;
        private Func<byte[], Tuple<byte, byte[], int>> _dskNeededCallback;
        private Action<bool, byte[], ActionResult> _busyCallback;
        private int _timeoutMs;
        public SmartStartSupport(NetworkViewPoint network, Action<NodeStatuses> setNodeStatusSignal, Func<byte[], Tuple<byte, byte[], int>> dskNeededCallback, Action<bool, byte[], ActionResult> busyCallback, int timeoutMs)
            : base(false, null, false)
        {
            _network = network;
            _busyCallback = busyCallback;
            _setNodeStatusSignal = setNodeStatusSignal;
            _dskNeededCallback = dskNeededCallback;
            _timeoutMs = timeoutMs;
        }

        private ApiHandler _handler;
        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 0));
            ActionUnits.Add(new DataReceivedUnit(_handler, OnReceived));
        }

        private bool _isRunning = false;
        private void OnReceived(DataReceivedUnit unit)
        {
            var payload = ((ApiHandler)unit.ActionHandler).DataFrame.Payload;
            _homeId = new byte[] { payload[3], payload[4], payload[5], payload[6] };
            var dsk = _dskNeededCallback(_homeId);

            if (!_isRunning && dsk != null && dsk.Item2 != null && dsk.Item2.Length >= 16)
            {
                SetRunning();
                byte[] dskPart = new byte[8];
                Array.Copy(dsk.Item2, 8, dskPart, 0, 8);
                IAddRemoveNode addNode = null;
                if (dsk.Item1 > 0)
                {
                    //addNode = new ReplaceFailedNodeOperation(dsk.First, _setNodeStatusSignal, _timeoutMs, dskPart) { SequenceNumber = SequenceNumber, DskValue = dsk.Item2 };
                    addNode = new AddNodeOperation((Modes.NodeOptionNetworkWide | Modes.NodeHomeId), _setNodeStatusSignal, _timeoutMs, dskPart) { SequenceNumber = SequenceNumber, DskValue = dsk.Item2, GrantSchemesValue = dsk.Item3 };
                }
                else
                {
                    addNode = new AddNodeOperation((Modes.NodeOptionNetworkWide | Modes.NodeHomeId), _setNodeStatusSignal, _timeoutMs, dskPart) { SequenceNumber = SequenceNumber, DskValue = dsk.Item2, GrantSchemesValue = dsk.Item3 };
                }
                var action = new InclusionTask(_network, addNode, true);
                action.CompletedCallback = (x) =>
                {
                    var act = x as ActionBase;
                    if (act != null)
                    {
                        ReleaseRunning();
                        _busyCallback(false, dsk.Item2, act.Result);
                    }
                };
                unit.SetNextActionItems(new ActionSerialGroup(action, new SetSmartStartAction(true)));
                _busyCallback(true, null, null);
            }
            else
            {
                unit.SetNextActionItems(new SetSmartStartAction(true));
            }
        }
        private void SetRunning()
        {
            _isRunning = true;
        }

        private void ReleaseRunning()
        {
            _isRunning = false;
        }

        protected override void CreateInstance()
        {
            _handler = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationControllerUpdate);
            _handler.AddConditions(new ByteIndex(0x85));
            _handler.AddConditions(ByteIndex.AnyValue);
            _handler.AddConditions(ByteIndex.AnyValue);
            _handler.AddConditions(ByteIndex.AnyValue);
            _handler.AddConditions(ByteIndex.AnyValue);
            _handler.AddConditions(ByteIndex.AnyValue);
            _handler.AddConditions(ByteIndex.AnyValue);
        }
    }
}
