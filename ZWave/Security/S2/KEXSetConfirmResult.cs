using System.Collections.Generic;
using ZWave.Enums;

namespace ZWave.Security.S2
{
    public class KEXSetConfirmResult
    {
        public bool IsConfirmed { get; set; }
        public bool IsAllowedCSA { get; set; }
        public List<SecuritySchemes> GrantedSchemes { get; set; }

        public KEXSetConfirmResult()
        {
            GrantedSchemes = new List<SecuritySchemes>();
        }

        public static byte ConvertToNetworkKeyFlags(params SecuritySchemes[] schemes)
        {
            byte ret = 0;
            if (schemes != null && schemes.Length > 0)
            {
                foreach (var item in schemes)
                {
                    if (item == SecuritySchemes.S2_UNAUTHENTICATED)
                    {
                        ret = (byte)(ret | (byte)NetworkKeyS2Flags.S2Class0);
                    }
                    else if (item == SecuritySchemes.S2_AUTHENTICATED)
                    {
                        ret = (byte)(ret | (byte)NetworkKeyS2Flags.S2Class1);
                    }
                    else if (item == SecuritySchemes.S2_ACCESS)
                    {
                        ret = (byte)(ret | (byte)NetworkKeyS2Flags.S2Class2);
                    }
                    else if (item == SecuritySchemes.S0)
                    {
                        ret = (byte)(ret | (byte)NetworkKeyS2Flags.S0);
                    }
                }
            }
            return ret;
        }

    }
}
