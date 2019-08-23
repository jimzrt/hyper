using System;

namespace Utils
{
    public class Win32PnPEntityClass : BaseWin32DeviceClass
    {
        #region Private Fields

        #endregion
        #region Public Properties
        public string ClassGuid { get; set; }

        public bool? ErrorCleared { get; set; }

        public string ErrorDescription { get; set; }

        public DateTime? InstallDate { get; set; }

        public int? LastErrorCode { get; set; }

        public string Manufacturer { get; set; }

        public string Service { get; set; }

        #endregion
    }
}
