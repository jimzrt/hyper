using System;
using System.Threading;
using Utils.Events;
using Utils.UI.Bind;
using Utils.UI.Enums;

namespace Utils.UI.MVVM
{
    public class MainVMBase : VMBase
    {
        #region Properties For Unit Tests

        private readonly ManualResetEvent _readySignal = new ManualResetEvent(false);
        public ManualResetEvent ReadySignal
        {
            get { return _readySignal; }
        }

        #endregion

        public Func<string, LogLevels, LogIndents, LogIndents, bool> ConfirmationCallback { private get; set; }
        public Action<string, int, int> ProgressCallback { private get; set; }

        public ISubscribeCollectionFactory SubscribeCollectionFactory { get; set; }
        public IDispatch Dispatcher { get; set; }

        public event EventDelegate<EventArgs<DialogVMBase>> DialogShown;
        public event EventDelegate<EventArgs<bool>> BusySet;

        public AboutViewModel AboutVM { get; set; }
        public ConnectViewModel ConnectVM { get; set; }
        public LogViewModel LogVM { get; set; }
        public OpenFileDialogViewModel OpenFileDialogVM { get; set; }
        public SaveFileDialogViewModel SaveFileDialogVM { get; set; }
        public FolderBrowserDialogViewModel FolderBrowserDialogVM { get; set; }
        public AboutCommand AboutCommand { get; set; }
        public ConnectShowCommand ConnectShowCommand { get; set; }

        public MainVMBase(ISubscribeCollectionFactory subscribeCollectionFactory, IDispatch dispatcher)
        {
            SetDefaultProgressCallback();

            AboutVM = new AboutViewModel();
            OpenFileDialogVM = new OpenFileDialogViewModel();
            SaveFileDialogVM = new SaveFileDialogViewModel();
            FolderBrowserDialogVM = new FolderBrowserDialogViewModel();

            AboutCommand = new AboutCommand(this);
            LogVM = new LogViewModel(this);
            ConnectShowCommand = new ConnectShowCommand(this);

            SubscribeCollectionFactory = subscribeCollectionFactory;
            Dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            if (Dispatcher != null)
                Dispatcher.Invoke(action);
            else
                action();
        }

        private void SetDefaultProgressCallback()
        {
            ProgressCallback = (text, val, max) =>
            {
                ProgressMessage = text;
                ProgressValue = val;
                ProgressMaximum = max;
                //LogMessageToAdd = val + "/" + max + " " + ProgressMessage;
            };
        }

        public void DialogShow(DialogVMBase viewModel)
        {
            if (DialogShown != null)
            {
                EventArgs<DialogVMBase> args = new EventArgs<DialogVMBase>(viewModel);
                Dispatcher.Invoke(() => DialogShown(args));
            }
        }

        private DateTime _lastGcCollect = DateTime.MinValue;
        public virtual void OnUITimerTick()
        {
            DateTime timerTick = DateTime.Now;
            if (IsElapsed(timerTick, 300))
            {
                LogVM.FeedStoredLogPackets();
            }
            if ((timerTick - _lastGcCollect).TotalMilliseconds > 65000)
            {
                GC.Collect();
                _lastGcCollect = timerTick;
            }
        }

        private DateTime _lastUpdate = DateTime.Now;
        private bool IsElapsed(DateTime timerTick, int timoutMs)
        {
            bool ret = (timerTick - _lastUpdate).TotalMilliseconds > timoutMs;
            if (ret)
                _lastUpdate = timerTick;
            return ret;
        }

        public virtual void SetBusy(bool value)
        {
            IsBusy = value;
            IsEnabled = !value;
            if (BusySet != null)
            {
                EventArgs<bool> args = new EventArgs<bool>(value);
                BusySet(args);
            }
        }

        public bool Confirmation(string text)
        {
            if (ConfirmationCallback != null)
                return ConfirmationCallback(text, LogLevels.Text, LogIndents.None, LogIndents.None);
            return false;
        }

        public virtual void LogTitle(string text)
        {
            Log(text, LogLevels.Title, LogIndents.None, LogIndents.None);
        }

        public virtual void LogFail(string text)
        {
            Log(text, LogLevels.Fail, LogIndents.None, LogIndents.None);
        }

        public virtual void LogWarning(string text)
        {
            Log(text, LogLevels.Warning, LogIndents.None, LogIndents.None);
        }

        public virtual void LogOk(string text)
        {
            Log(text, LogLevels.Ok, LogIndents.None, LogIndents.None);
        }

        public virtual void Log(string text)
        {
            LogVM.Log(text, LogLevels.Text, LogIndents.Current, LogIndents.Current);
        }

        public virtual void Log(string text, LogLevels level, LogIndents indentBefore, LogIndents indentAfter)
        {
            LogVM.Log(text, level, indentBefore, indentAfter);
        }

        public void Log(string message, LogLevels level)
        {
            LogVM.Log(message, level, LogIndents.Current, LogIndents.Current);
        }

        public void AnnounceProgress(string text, int current, int total)
        {
            if (ProgressCallback != null)
                ProgressCallback(text, current, total);
        }

        #region Connections
        private string _connectionName;
        public string ConnectionName
        {
            get { return _connectionName; }
            set
            {
                _connectionName = value;
                RaisePropertyChanged("ConnectionName");
            }
        }
        #endregion

        private CommandBase _activeCommand;
        public CommandBase ActiveCommand
        {
            get { return _activeCommand; }
            set
            {
                _activeCommand = value;
                Notify("ActiveCommand");
            }
        }



        private string _progressMessage;
        public string ProgressMessage
        {
            get { return _progressMessage; }
            set
            {
                _progressMessage = value;
                Notify("ProgressMessage");
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                Notify("ProgressValue");
            }
        }

        private int _progressMaximum = 100;
        public int ProgressMaximum
        {
            get { return _progressMaximum; }
            set
            {
                _progressMaximum = value;
                Notify("ProgressMaximum");
            }
        }

        private bool? _progressVisibility = false;
        public bool? ProgressVisibility
        {
            get { return _progressVisibility; }
            set
            {
                _progressVisibility = value;
                Notify("ProgressVisibility");
            }
        }
    }
}
