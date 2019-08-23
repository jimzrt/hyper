using System;

namespace Utils.UI.MVVM
{
    public class VMBase : EntityBase, ICloneable
    {
        public bool IsDialogOk { get; set; }
        public Action CloseCallback { get; set; }

        public void Close()
        {
            if (CloseCallback != null)
                CloseCallback();
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Notify("Title");
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                Notify("Description");
            }
        }

        private int _tag;
        public int Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                Notify("Tag");
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (!value)
                    BusyMessage = null;
                _isBusy = value;
                Notify("IsBusy");
            }
        }

        private string _busyMessage;
        public string BusyMessage
        {
            get { return _busyMessage; }
            set
            {
                _busyMessage = value;
                Notify("BusyMessage");
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                Notify("IsEnabled");
            }
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}
