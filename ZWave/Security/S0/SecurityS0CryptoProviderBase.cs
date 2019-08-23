using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using ZWave.CommandClasses;

namespace ZWave.Security
{
    public class SecurityS0CryptoProviderBase
    {
        private const int RANDOM_NUMBER_BYTES_NEEDED = 32;
        private const int INTERNAL_NONCE_LIFE = 20000;
        private const int EXTERNAL_NONCE_LIFE = 20000;
        private const int NONCES_CAPACITY = 100;

        private ZWaveAES _aesEngine;
        private ZWaveAES AesEngine
        {
            get { return _aesEngine ?? (_aesEngine = new ZWaveAES()); }
        }

        private byte[] _authKey;
        private byte[] _encKey;

        private ZWavePRNG _prng;
        public ZWavePRNG PRNG
        {
            get
            {
                if (_prng == null)
                {
                    byte[] randomResult = new byte[RANDOM_NUMBER_BYTES_NEEDED + 2];
                    new Random().NextBytes(randomResult);
                    randomResult[0] = 1;
                    randomResult[1] = RANDOM_NUMBER_BYTES_NEEDED;
                    _prng = new ZWavePRNG(randomResult);
                }
                return _prng;
            }
        }

        private NonceS0Storage _nonceS0Storage;
        public NonceS0Storage NonceS0Storage
        {
            get
            {
                return _nonceS0Storage ?? (_nonceS0Storage = new NonceS0Storage(NONCES_CAPACITY));
            }
        }

        public void OnNetworkKeyS0Changed(byte[] networkKey)
        {
            SecurityS0Utils.LoadKeys(AesEngine, networkKey, out _authKey, out _encKey);
        }

        public byte[] Encrypt(byte property, byte[] command, byte senderNodeId, byte receiverNodeId, byte[] externalNonce1)
        {
            COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION ret = new COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION();
            COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.Tproperties1 prop = property;
            byte[] payload = new byte[command.Length + 1];
            payload[0] = GetPropertyByte(property);
            Array.Copy(command, 0, payload, 1, command.Length);
            var internalNonce = GetInternalNonceArray(senderNodeId, receiverNodeId);
            ret.initializationVectorByte = internalNonce;
            var externalNonce = GetExternalNonceArray(externalNonce1);
            if (internalNonce != null && internalNonce.Length == 8 &&
                externalNonce != null && externalNonce.Length == 8)
            {
                byte[] IV = new byte[16];
                Array.Copy(internalNonce, 0, IV, 0, 8);
                Array.Copy(externalNonce, 0, IV, 8, 8);

                SecurityS0Utils.Encrypt(AesEngine, _encKey, IV, ref payload);

                ret.properties1 = payload[0];
                if (payload.Length > 0)
                {
                    ret.commandByte = new List<byte>();
                    for (int i = 1; i < payload.Length; i++)
                    {
                        ret.commandByte.Add(payload[i]);
                    }
                }
                ret.receiversNonceIdentifier = GetNonceIdByte(externalNonce);

                byte cmdId;
                if (prop.sequenced > 0 && prop.secondFrame == 0)
                    cmdId = COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET.ID;
                else
                    cmdId = COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.ID;

                ret.messageAuthenticationCodeByte = GetMacArray(senderNodeId, receiverNodeId, IV, cmdId, payload);
            }
            return ret;
        }

        protected virtual byte[] GetInternalNonceArray(byte senderNodeId, byte receiverNodeId)
        {
            byte[] ret = null;
            while (ret == null)
            {
                ret = PRNG.PRNGOutput(new byte[8]);
                NonceS0Storage.RegisterInternal(new OrdinalPeerNodeId(receiverNodeId, senderNodeId), ret);
            }
            return ret;
        }

        protected virtual byte[] GetExternalNonceArray(byte[] nonce)
        {
            return nonce;
        }

        protected virtual byte[] GetMacArray(byte senderNodeId, byte receiverNodeId, byte[] IV, byte cmdId, byte[] payload)
        {
            byte[] header = new byte[20];
            Array.Copy(IV, 0, header, 0, IV.Length);
            header[16] = cmdId;
            header[17] = senderNodeId;
            header[18] = receiverNodeId;
            header[19] = (byte)payload.Length;

            return SecurityS0Utils.MakeAuthTag(AesEngine, _authKey, header, payload);
        }

        protected virtual byte GetPropertyByte(byte property)
        {
            return property;
        }

        protected virtual byte GetNonceIdByte(byte[] externalNonce)
        {
            return externalNonce[0];
        }

        public static byte[] EncryptCommand(byte property, byte[] command, byte senderNodeId, byte receiverNodeId, byte[] internalNonce, byte[] externalNonce, byte[] networkKey, bool isWithNonceGet)
        {
            byte[] _authKey = new byte[16];
            byte[] _encKey = new byte[16];
            COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.Tproperties1 prop = property;
            COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION ret = new COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION();
            ret.initializationVectorByte = internalNonce;
            byte[] payload = new byte[command.Length + 1];
            payload[0] = prop;
            Array.Copy(command, 0, payload, 1, command.Length);

            byte[] IV = new byte[16];
            Array.Copy(internalNonce, 0, IV, 0, 8);
            Array.Copy(externalNonce, 0, IV, 8, 8);

            ZWaveAES _aesEngine = new ZWaveAES();
            SecurityS0Utils.LoadKeys(_aesEngine, networkKey, out _authKey, out _encKey);

            SecurityS0Utils.Encrypt(_aesEngine, _encKey, IV, ref payload);

            ret.properties1 = payload[0];
            if (payload.Length > 0)
            {
                ret.commandByte = new List<byte>();
                for (int i = 1; i < payload.Length; i++)
                {
                    ret.commandByte.Add(payload[i]);
                }
            }
            ret.receiversNonceIdentifier = externalNonce[0];
            byte[] header = new byte[20];
            Array.Copy(IV, 0, header, 0, IV.Length);
            header[16] = COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.ID;
            if (isWithNonceGet)
                header[16] = COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET.ID;
            else
                header[16] = COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.ID;
            header[17] = senderNodeId;
            header[18] = receiverNodeId;
            header[19] = (byte)payload.Length;
            ret.messageAuthenticationCodeByte = SecurityS0Utils.MakeAuthTag(_aesEngine, _authKey, header, payload);
            return ret;
        }

        private byte[] Decrypt(COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION cmd, byte cmdId, byte senderNodeId, byte receivedNodeId, out byte decryptedProperties)
        {
            decryptedProperties = 0;
            byte[] ret = null;
            byte[] internalNonce = NonceS0Storage.Find(new OrdinalPeerNodeId(senderNodeId, receivedNodeId), cmd.receiversNonceIdentifier);
            if (internalNonce != null)
            {
                byte[] IV = new byte[16];
                Array.Copy(cmd.initializationVectorByte.ToArray(), 0, IV, 0, 8);
                Array.Copy(internalNonce, 0, IV, 8, 8);
                int len = 1;
                if (cmd.commandByte != null)
                    len += cmd.commandByte.Count;
                byte[] payload = new byte[len];
                payload[0] = cmd.properties1;
                for (int i = 0; i < len - 1; i++)
                {
                    payload[i + 1] = cmd.commandByte[i];
                }
                byte[] header = new byte[20];
                Array.Copy(IV, 0, header, 0, IV.Length);
                header[16] = cmdId;
                header[17] = senderNodeId;
                header[18] = receivedNodeId;
                header[19] = (byte)payload.Length;
                if (SecurityS0Utils.VerifyAuthTag(AesEngine, _authKey, header, payload, cmd.messageAuthenticationCodeByte.ToArray()))
                {
                    SecurityS0Utils.Decrypt(AesEngine, _encKey, IV, ref payload);
                    ret = new byte[payload.Length - 1]; // exclude properties
                    Array.Copy(payload, 1, ret, 0, ret.Length);
                    decryptedProperties = payload[0];
                }
                else
                {
                    "^{0} <<|<< {1} {2}"._DLOG(0, "N/D", internalNonce.GetHex());
                }
            }
            else
            {
                "^{0} <<|<< {1} {2}"._DLOG(0, "N/D", "N/Nonce");
            }
            return ret;
        }

        public bool DecryptFrame(byte sourceNodeId, byte destNodeId, byte[] homeId, COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION cmd, byte cmdId, out byte[] data, out byte decryptedProperties/*, ref byte[] cmdData*/)
        {
            bool ret = false;
            data = null;
            decryptedProperties = 0;
            //byte rssi = 0;
            //int multiDestsOffsetNodeMaskLen = 0;
            //byte[] multiDestsNodeMask = new byte[0];
            //byte[] tail = new byte[2];

            data = Decrypt(cmd, cmdId, sourceNodeId, destNodeId, out decryptedProperties);

            //if (frameData[1] == (byte)CommandTypes.CmdApplicationCommandHandler_Bridge)
            //{
            //byte destNodeId = frameData[lenIndex - 2];
            //data = Decrypt(cmd, sourceNodeId, destNodeId, out decryptedProperties);

            //int multiDestsOffsetNodeMaskLenIndex = lenIndex + 1 + frameData[lenIndex];
            //if (frameData.Length > multiDestsOffsetNodeMaskLenIndex)
            //{
            //    multiDestsOffsetNodeMaskLen = frameData[multiDestsOffsetNodeMaskLenIndex];
            //}
            //if (multiDestsOffsetNodeMaskLen > 0 &&
            //   frameData.Length > multiDestsOffsetNodeMaskLenIndex + 1 + multiDestsOffsetNodeMaskLen)
            //{
            //    multiDestsNodeMask = frameData.Skip(multiDestsOffsetNodeMaskLenIndex + 1).Take(multiDestsOffsetNodeMaskLen).ToArray();
            //}
            //if (frameData.Length > multiDestsOffsetNodeMaskLenIndex + 1 + multiDestsOffsetNodeMaskLen)
            //{
            //    rssi = frameData[multiDestsOffsetNodeMaskLenIndex + 1 + multiDestsOffsetNodeMaskLen];
            //}
            //if (multiDestsOffsetNodeMaskLen == 0)
            //{
            //    tail = new byte[3];
            //}
            //else
            //{
            //    tail = new byte[multiDestsOffsetNodeMaskLen + 1 + tail.Length];
            //    Array.Copy(multiDestsNodeMask, 0, tail, 1, multiDestsOffsetNodeMaskLen);
            //}
            //tail[0] = (byte)multiDestsOffsetNodeMaskLen;
            //}
            //else
            //{
            //    data = Decrypt(cmd, sourceNodeId, out decryptedProperties);
            //if (frameData.Length > lenIndex + 1 + frameData[lenIndex])
            //{
            //    rssi = frameData[lenIndex + 1 + frameData[lenIndex]];
            //}
            //}
            //tail[tail.Length - 2] = rssi;
            //tail[tail.Length - 1] = (byte)SecuritySchemes.S0;

            if (data != null)
            {
                ret = true;
                //byte[] payload = new byte[lenIndex + 1 + decryptedCmd.Length + tail.Length];
                //for (int i = 0; i < lenIndex + 2; i++)
                //{
                //    payload[i] = frameData[i];
                //}
                //payload[lenIndex] = (byte)decryptedCmd.Length;
                //Array.Copy(decryptedCmd, 0, payload, lenIndex + 1, decryptedCmd.Length);
                //Array.Copy(tail, 0, payload, lenIndex + 1 + decryptedCmd.Length, tail.Length);
                //cmdData = decryptedCmd;
                //var dataFrame = new DataFrame(incomePacket.SessionId, incomePacket.DataFrameType, incomePacket.IsHandled,
                //    incomePacket.IsOutcome, incomePacket.SystemTimeStamp);
                //dataFrame.IsSubstituted = true;
                //byte[] frameBuffer = DataFrame.CreateFrameBuffer(payload);
                //dataFrame.SetBuffer(frameBuffer, 0, frameBuffer.Length);
                //dataFrame.SubstituteIncomingFlags = (int)(((SubstituteIncomingFlags)incomePacket.SubstituteIncomingFlags) |
                //    SubstituteIncomingFlags.Security);
            }
            return ret;
        }

        public byte[] GenerateNonceReport(OrdinalPeerNodeId peerNodeId)
        {
            byte[] internalNonce = null;
            while (internalNonce == null)
            {
                internalNonce = PRNG.PRNGOutput(new byte[8]);
                NonceS0Storage.RegisterInternal(peerNodeId, internalNonce);
            }
            internalNonce = GetInternalNonceArray(peerNodeId.NodeId2, peerNodeId.NodeId1);
            "new internal NONCE {0}"._DLOG(internalNonce.GetHex());
            return new COMMAND_CLASS_SECURITY.SECURITY_NONCE_REPORT() { nonceByte = internalNonce };
        }

        public static byte[] EncryptPayload(byte[] externalNonce, byte[] internalNonce, byte[] securityKey, byte[] message)
        {
            ZWaveAES AES = new ZWaveAES();
            byte[] _authKey = new byte[16];
            byte[] _encKey = new byte[16];
            SecurityS0Utils.LoadKeys(AES, securityKey, out _authKey, out _encKey);

            byte[] _IV = new byte[16];
            Array.Copy(internalNonce, 0, _IV, 0, internalNonce.Length);
            Array.Copy(externalNonce, 0, _IV, 8, externalNonce.Length);
            SecurityS0Utils.Encrypt(AES, _encKey, _IV, ref message);
            return message;
        }

        public static byte[] DecryptPayload(byte[] externalNonce, byte[] internalNonce, byte[] securityKey, byte[] message)
        {
            ZWaveAES AES = new ZWaveAES();
            byte[] _authKey = new byte[16];
            byte[] _encKey = new byte[16];
            SecurityS0Utils.LoadKeys(AES, securityKey, out _authKey, out _encKey);

            byte[] _IV = new byte[16];
            Array.Copy(internalNonce, 0, _IV, 0, internalNonce.Length);
            Array.Copy(externalNonce, 0, _IV, 8, externalNonce.Length);
            SecurityS0Utils.Decrypt(AES, _encKey, _IV, ref message);
            return message;
        }
    }
}
