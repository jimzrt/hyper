using System;

namespace Utils.UI.MVVM
{
    public class DialogVMBase : VMBase
    {
        public CommandBase CommandOk { get; set; }
        public CommandBase CommandCancel { get; set; }
        public DialogInfo DialogInfo { get; set; }
        public DialogVMBase()
        {
            DialogInfo = new DialogInfo(this);
            CommandOk = new CommandBase(x =>
            {
                IsDialogOk = true;
                Close();
            }, null);
            CommandCancel = new CommandBase(x =>
            {
                IsDialogOk = false;
                Close();
            }, null);
        }
    }

    public class DialogInfo : EntityBase
    {
        public event EventHandler IsFloatingChanged;
        public DialogInfo(DialogVMBase dialog)
        {
            DialogName = dialog.GetType().Name;
        }

        public DialogInfo()
        {

        }

        public string DialogName { get; set; }

        private bool _mIsFloating;
        public bool IsFloating
        {
            get { return _mIsFloating; }
            set
            {
                _mIsFloating = value;
                Notify("IsFloating");
                if (IsFloatingChanged != null)
                {
                    IsFloatingChanged(this, EventArgs.Empty);
                }
            }
        }
        public bool IsResizable { get; set; }
        public bool IsTopmost { get; set; }
        public bool IsModal { get; set; }
        public bool IsPopup { get; set; }
        public bool IsRecreateOnShow { get; set; }
        public bool CenterOwner { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
