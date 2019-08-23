using ZWave.CommandClasses;
using ZWave.Security;

namespace ZWave.BasicApplication.Security
{
    public class SecurityS0CryptoProvider : SecurityS0CryptoProviderBase
    {
        private SecurityManagerInfo _securityManagerInfo;

        public SecurityS0CryptoProvider(SecurityManagerInfo securityManagerInfo)
        {
            _securityManagerInfo = securityManagerInfo;
            _securityManagerInfo.NetworkKeyS0Changed += OnNetworkKeyS0Changed;
        }

        protected override byte[] GetInternalNonceArray(byte senderNodeId, byte receiverNodeId)
        {
            if (_securityManagerInfo.TestSenderNonceS0 != null)
            {
                return _securityManagerInfo.TestSenderNonceS0;
            }
            else
            {
                return base.GetInternalNonceArray(senderNodeId, receiverNodeId);
            }
        }

        protected override byte[] GetExternalNonceArray(byte[] nonce)
        {
            if (_securityManagerInfo.TestReceiverNonceS0 != null)
            {
                return _securityManagerInfo.TestReceiverNonceS0;
            }
            else
            {
                return base.GetExternalNonceArray(nonce);
            }
        }

        protected override byte GetNonceIdByte(byte[] externalNonce)
        {
            if (_securityManagerInfo.TestReceiverNonceS0Id != null)
            {
                return (byte)_securityManagerInfo.TestReceiverNonceS0Id;
            }
            else
            {
                return base.GetNonceIdByte(externalNonce);
            }
        }

        protected override byte[] GetMacArray(byte senderNodeId, byte receiverNodeId, byte[] IV, byte cmdId, byte[] payload)
        {
            if (_securityManagerInfo.TestMacS0 != null)
            {
                return _securityManagerInfo.TestMacS0;
            }
            else
            {
                return base.GetMacArray(senderNodeId, receiverNodeId, IV, cmdId, payload);
            }
        }

        protected override byte GetPropertyByte(byte property)
        {
            COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.Tproperties1 prop = property;
            if (prop.sequenced > 0 && prop.secondFrame > 0 && _securityManagerInfo.TestPropertyByteS0SecondFrame != null)
            {
                return (byte)_securityManagerInfo.TestPropertyByteS0SecondFrame;
            }
            else if (_securityManagerInfo.TestPropertyByteS0 != null)
            {
                return (byte)_securityManagerInfo.TestPropertyByteS0;
            }
            else
                return base.GetPropertyByte(property);
        }
    }
}
