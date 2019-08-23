using System;
using System.Linq;
using ZWave.Enums;

namespace ZWave.Devices
{
    public class NodeViewPointCollection
    {
        private readonly int MaxNodes;
        private readonly int MaxEndpoints;
        private NodeViewPoint[,] Nodes { get; set; }
        public NodeViewPointCollection(int maxNodes, int maxEndpoints)
        {
            MaxNodes = maxNodes;
            MaxEndpoints = maxEndpoints;
            Nodes = new NodeViewPoint[MaxNodes, MaxEndpoints];
            for (int i = 0; i < MaxNodes; i++)
            {
                for (int j = 0; j < MaxEndpoints; j++)
                {
                    Nodes[i, j] = new NodeViewPoint();
                }
            }
        }

        public NodeViewPoint this[NodeTag index]
        {
            get
            {
                return Nodes[index.Id, index.EndPointId];
            }
            set
            {
                Nodes[index.Id, index.EndPointId] = value;
            }
        }

        public NodeViewPoint this[byte index]
        {
            get
            {
                return Nodes[index, 0];
            }
            set
            {
                Nodes[index, 0] = value;
            }
        }

        internal void Reset()
        {
            for (int i = 0; i < MaxNodes; i++)
            {
                for (int j = 0; j < MaxEndpoints; j++)
                {
                    Nodes[i, j].Reset();
                }
            }
        }

        internal void ResetCurrentSecurityScheme()
        {
            for (int i = 0; i < MaxNodes; i++)
            {
                for (int j = 0; j < MaxEndpoints; j++)
                {
                    Nodes[i, j].ResetCurrentSecurityScheme();
                }
            }
        }

        internal void ResetSecuritySchemes()
        {
            for (int i = 0; i < MaxNodes; i++)
            {
                for (int j = 0; j < MaxEndpoints; j++)
                {
                    Nodes[i, j].ResetSecuritySchemes();
                }
            }
        }
    }

    public class NodeViewPoint
    {
        public NodeInfo NodeInfo { get; set; }
        private byte[] CommandClasses { get; set; }
        public RoleTypes RoleType { get; set; }
        public bool WakeupInterval { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsFailed { get; set; }
        public bool IsForcedListening { get; set; }

        public bool SecuritySchemesSpecified { get; set; }
        private SecuritySchemes[] SecuritySchemes { get; set; }
        private int SecuritySchemeIndex { get; set; }
        private byte[] SecureCommandClasses { get; set; }

        public NodeViewPoint()
        {
            Reset();
        }

        public void Reset()
        {
            CommandClasses = null;
            SecureCommandClasses = null;
            SecuritySchemeIndex = -1;
            SecuritySchemes = null;
            SecuritySchemesSpecified = false;
            NodeInfo = new NodeInfo();
            WakeupInterval = false;
            IsVirtual = false;
            IsFailed = false;
            IsForcedListening = false;
            RoleType = RoleTypes.None;
        }

        public bool IsFlirs
        {
            get
            {
                return (NodeInfo.Security & 0x20) != 0 || (NodeInfo.Security & 0x40) != 0;
            }
        }

        public bool IsFlirs250ms
        {
            get
            {
                return (NodeInfo.Security & 0x20) != 0;
            }
        }

        public bool IsFlirs1000ms
        {
            get
            {
                return (NodeInfo.Security & 0x40) != 0;
            }
        }

        public bool IsOptionalFunctionality
        {
            get
            {
                return (NodeInfo.Security & 0x80) != 0;
            }
        }

        public bool IsListening
        {
            get
            {
                return (NodeInfo.Capability & 0x80) > 0;
            }
        }

        public bool IsSlaveApi
        {
            get { return (NodeInfo.Security & 0x02) == 0; }
        }

        public DeviceOptions DeviceOption
        {
            get
            {
                var ret = DeviceOptions.Listening;
                if (IsListening)
                {
                    ret = DeviceOptions.Listening;
                }
                else if (IsFlirs250ms)
                {
                    ret = DeviceOptions.FreqListeningMode250ms;
                }
                else if (IsFlirs1000ms)
                {
                    ret = DeviceOptions.FreqListeningMode1000ms;
                }
                else if (IsOptionalFunctionality)
                {
                    ret = DeviceOptions.OptionalFunctionality;
                }
                else
                {
                    ret = DeviceOptions.NoneListening;
                }
                return ret;
            }
        }

        public bool IsDeviceListening()
        {
            bool ret = true;
            {
                ret = IsListening || (NodeInfo.Security & 0x20) != 0 || (NodeInfo.Security & 0x40) != 0;
            }
            return ret;
        }

        internal void SetSecuritySchemes(SecuritySchemes[] schemes)
        {
            SecuritySchemes = schemes;
        }

        internal SecuritySchemes[] GetSecuritySchemes()
        {
            return SecuritySchemes;
        }

        internal bool HasSecurityScheme(SecuritySchemes scheme)
        {
            var ret = false;
            if (SecuritySchemes != null)
            {
                ret = SecuritySchemes.Contains(scheme);
            }
            return ret;
        }

        internal bool HasCommandClass(byte cmdClass)
        {
            var ret = false;
            if (CommandClasses != null)
            {
                ret = CommandClasses.Contains(cmdClass);
            }
            return ret;
        }

        internal bool HasSecureCommandClass(byte cmdClass)
        {
            var ret = false;
            if (SecureCommandClasses != null)
            {
                ret = SecureCommandClasses.Contains(cmdClass);
            }
            return ret;
        }

        public void SetCurrentSecurityScheme(SecuritySchemes securityScheme)
        {
            if (SecuritySchemes != null && SecuritySchemes.Length > 0)
            {
                SecuritySchemeIndex = Array.IndexOf<SecuritySchemes>(SecuritySchemes, securityScheme);
            }
        }

        public void ResetCurrentSecurityScheme()
        {
            SecuritySchemeIndex = -1;
        }

        public void ResetSecuritySchemes()
        {
            SetSecuritySchemes(null);
            SetSecureCommandClasses(null);
            SecuritySchemesSpecified = false;
        }

        public SecuritySchemes GetCurrentSecurityScheme()
        {
            SecuritySchemes ret = Enums.SecuritySchemes.NONE;
            if (SecuritySchemes != null && SecuritySchemes.Length > 0)
            {
                if (SecuritySchemeIndex < 0 || SecuritySchemeIndex >= SecuritySchemes.Length)
                {
                    SecuritySchemeIndex = GetHighestKeyIndex(SecuritySchemes);
                }
                return SecuritySchemes[SecuritySchemeIndex];
            }
            return ret;
        }

        private int GetHighestKeyIndex(SecuritySchemes[] securitySchemes)
        {
            int ret = 0;
            SecuritySchemes highScheme = Enums.SecuritySchemes.S0;
            if (securitySchemes.Contains(Enums.SecuritySchemes.S2_ACCESS))
            {
                highScheme = Enums.SecuritySchemes.S2_ACCESS;
            }
            else if (securitySchemes.Contains(Enums.SecuritySchemes.S2_AUTHENTICATED))
            {
                highScheme = Enums.SecuritySchemes.S2_AUTHENTICATED;
            }
            else if (securitySchemes.Contains(Enums.SecuritySchemes.S2_UNAUTHENTICATED))
            {
                highScheme = Enums.SecuritySchemes.S2_UNAUTHENTICATED;
            }
            else if (securitySchemes.Contains(Enums.SecuritySchemes.S0))
            {
                highScheme = Enums.SecuritySchemes.S0;
            }
            else
            {
                highScheme = securitySchemes[0]; // unexpected
            }
            for (int i = 0; i < securitySchemes.Length; i++)
            {
                if (securitySchemes[i] == highScheme)
                {
                    ret = i;
                    break;
                }
            }
            return ret;
        }

        internal byte[] GetCommandClasses()
        {
            return CommandClasses;
        }

        internal void SetCommandClasses(byte[] commandClasses)
        {
            CommandClasses = commandClasses;
        }

        internal byte[] GetSecureCommandClasses()
        {
            return SecureCommandClasses;
        }

        internal void SetSecureCommandClasses(byte[] commandClasses)
        {
            SecureCommandClasses = commandClasses;
        }
    }
}
