using System;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.CommandClasses
{
    public class FirmwareUpdateMdSupport : ApiAchOperation
    {
        const ushort START_REPORT_NUMBER = 0x01;
        byte[] FIRMWARE_ID = { 0x05, 0x01 }; //Hardcoded devkit 6.80
        byte[] MANUFACTURE_ID = { 0x00, 0x00 }; //(Former Zensys) 0x0000 by default (SDS13425) or device.ManufacturerId,
        int MAX_FRAGMENT_SIZE = 40;

        private int _firmwareOffset;
        private ushort _lastUsedReportNumber;
        private byte[] _myOtaFirmwareCrc;
        private byte _pStatus = 0x00;
        private NetworkViewPoint _network;

        private FirmwareUpdateNvmSetNewImageOperation _firmwareUpdateNvmSetNewImageTrue;
        private FirmwareUpdateNvmSetNewImageOperation _firmwareUpdateNvmSetNewImageFalse;
        private FirmwareUpdateNvmInitOperation _firmwareUpdateNvmInit;

        public TransmitOptions TxOptions { get; set; }
        public TransmitOptions2 TxOptions2 { get; set; }
        public TransmitSecurityOptions TxSecOptions { get; set; }

        Action _setNewImageCompletedCallback;

        /// <summary>
        /// Over The Air support task.
        /// Firmaware Update Meta Data version 5
        /// </summary>
        public FirmwareUpdateMdSupport(NetworkViewPoint network, TransmitOptions txOptions, Action SetNewImageCompletedCallback)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.ID))
        {
            _network = network;
            TxOptions = txOptions;
            TxOptions2 = TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE;
            TxSecOptions = TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY;

            _setNewImageCompletedCallback = SetNewImageCompletedCallback;
        }

        protected override void CreateWorkflow()
        {
            base.CreateWorkflow();
            ActionUnits.Add(new ActionCompletedUnit(_firmwareUpdateNvmSetNewImageTrue, OnSetNewImageCompleted));
        }

        protected override void CreateInstance()
        {
            _firmwareUpdateNvmSetNewImageTrue = new FirmwareUpdateNvmSetNewImageOperation(true);
            _firmwareUpdateNvmSetNewImageFalse = new FirmwareUpdateNvmSetNewImageOperation(false);
            _firmwareUpdateNvmInit = new FirmwareUpdateNvmInitOperation();

            base.CreateInstance();
        }

        private void OnSetNewImageCompleted(ActionCompletedUnit ou)
        {
            _setNewImageCompletedCallback();
        }

        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            ou.SetNextActionItems();
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] command = ReceivedAchData.Command;
            var scheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
            bool isSuportedScheme = IsSupportedScheme(_network, command, scheme);
            if (command != null && command.Length > 1 && isSuportedScheme)
            {
                switch (command[1])
                {
                    case (COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_MD_GET.ID):
                        {
                            ou.SetNextActionItems(SendFwuMdReport(nodeId, scheme));
                        }
                        break;
                    case (COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_REQUEST_GET.ID):
                        {
                            ou.SetNextActionItems(HandleCmdClassFirmwareUpdateMdReqGet(nodeId, command, scheme));
                            if (_pStatus == 0xFF)
                            {
                                _firmwareOffset = 0;
                                ou.AddNextActionItems(SendFwuMdGet(nodeId, START_REPORT_NUMBER, scheme));
                                _firmwareUpdateNvmInit.NewToken();
                                ou.AddNextActionItems(_firmwareUpdateNvmInit);
                                _firmwareUpdateNvmSetNewImageFalse.NewToken();
                                ou.AddNextActionItems(_firmwareUpdateNvmSetNewImageFalse);
                            }
                        }
                        break;
                    case (COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_REPORT.ID):
                        {
                            HandleCmdClassFirmwareUpdateMdReport(ou, nodeId, command, scheme);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>handleCmdClassFirmwareUpdateMdReport</summary>
        private void HandleCmdClassFirmwareUpdateMdReport(DataReceivedUnit ou, byte nodeId, byte[] command, SecuritySchemes scheme)
        {
            var fwuMdReport = (COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_REPORT)command;
            var lastReportNumber = Tools.GetBytes(_lastUsedReportNumber);
            if (fwuMdReport.properties1.reportNumber1 == lastReportNumber[0] &&
                fwuMdReport.reportNumber2 == lastReportNumber[1])
            {
                var crc16Res = Tools.CalculateCrc16Array(command.Take(command.Length - 2));
                if (crc16Res.SequenceEqual(fwuMdReport.checksum))
                {
                    var data = fwuMdReport.data;
                    ushort dataLen = (ushort)data.Count;
                    var _firmwareUpdateNvmWrite = new FirmwareUpdateNvmWriteOperation(_firmwareOffset, dataLen, data.ToArray());
                    ou.SetNextActionItems(_firmwareUpdateNvmWrite);
                    _firmwareOffset += dataLen;

                    if (fwuMdReport.properties1.last == 0x00)
                    {
                        ushort nextReportNumber = (ushort)(_lastUsedReportNumber + 1);
                        ou.AddNextActionItems(SendFwuMdGet(nodeId, nextReportNumber, scheme));
                    }
                    else
                    {
                        _firmwareUpdateNvmSetNewImageTrue.NewToken();
                        ou.AddNextActionItems(_firmwareUpdateNvmSetNewImageTrue);
                        ou.AddNextActionItems(SendFwuMdStatusReport(nodeId, 0xFF, scheme));
                    }
                }
                else //FW_EV_UPDATE_STATUS_UNABLE_TO_RECEIVE
                {
                    "invalid MD report"._DLOG();
                    //retransmit request on the previous frame
                    ou.SetNextActionItems(SendFwuMdGet(nodeId, _lastUsedReportNumber, scheme));
                }
            }

        }

        /// <summary>handleCmdClassFirmwareUpdateMdReqGet</summary>
        private ActionBase HandleCmdClassFirmwareUpdateMdReqGet(byte nodeId, byte[] command, SecuritySchemes scheme)
        {
            var fwuMdRequestGet = (COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_REQUEST_GET)command;
            _myOtaFirmwareCrc = fwuMdRequestGet.checksum;
            var fragmentSize = Tools.GetInt32(fwuMdRequestGet.fragmentSize);
            if (fwuMdRequestGet.firmwareId.SequenceEqual(FIRMWARE_ID) &&
                fwuMdRequestGet.manufacturerId.SequenceEqual(MANUFACTURE_ID) &&
                fragmentSize <= MAX_FRAGMENT_SIZE
                )
            {
                _pStatus = 0xFF;
            }
            return SendFwuMdRequestReport(nodeId, _pStatus, scheme);
        }

        /// <summary>ZCB_CmdClassFwUpdateMdReqReport</summary>
        private ActionBase SendFwuMdRequestReport(byte nodeId, byte reportStatus, SecuritySchemes scheme)
        {
            var fwuMdRequestReport = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_REQUEST_REPORT() { status = reportStatus };
            var ret = new SendDataExOperation(nodeId, fwuMdRequestReport, TxOptions, scheme);
            return ret;
        }

        /// <summary>ZCB_UpdateStatusSuccess</summary>
        private ActionBase SendFwuMdStatusReport(byte nodeId, byte reportStatus, SecuritySchemes scheme)
        {
            var fwuMdRequestReport = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_STATUS_REPORT() { status = reportStatus };
            var ret = new SendDataExOperation(nodeId, fwuMdRequestReport, TxOptions, scheme);
            return ret;
        }

        /// <summary>CmdClassFirmwareUpdateMdGet</summary>
        private ActionBase SendFwuMdGet(byte nodeId, ushort fwuReportNumber, SecuritySchemes scheme)
        {
            _lastUsedReportNumber = fwuReportNumber;
            var reportNumber = Tools.GetBytes(fwuReportNumber);
            var fwuMdGet = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_GET()
            {
                numberOfReports = 0x01,
                reportNumber2 = reportNumber[1],
                properties1 = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_UPDATE_MD_GET.Tproperties1()
                {
                    reportNumber1 = reportNumber[0]
                }
            };
            var ret = new SendDataExOperation(nodeId, fwuMdGet, TxOptions, scheme);
            return ret;
        }

        /// <summary>handleCommandClassFWUpdate</summary>
        private ActionBase SendFwuMdReport(byte nodeId, SecuritySchemes scheme)
        {
            var fwuMdReport = new COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.FIRMWARE_MD_REPORT()
            {
                manufacturerId = MANUFACTURE_ID,
                firmware0Id = FIRMWARE_ID,
                firmwareUpgradable = 0xFF,
                maxFragmentSize = Tools.GetBytes(MAX_FRAGMENT_SIZE).Skip(2).ToArray(),
            };
            var ret = new SendDataExOperation(nodeId, fwuMdReport, TxOptions, scheme);
            return ret;
        }

    }
}
