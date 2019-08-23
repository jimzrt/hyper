using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utils.UI.Enums;

namespace Utils.UI.MVVM
{
    public class CommandBase : ICloneable
    {
        public event Action OnCompleted;
        public event Action OnCancelled;
        public event Action<object> CanExecuteChanged;
        public static bool IsEnabledCanExecuteChanged { get; set; }

        protected Action<object> _execute;
        protected Predicate<object> _canCancel;
        protected Action<object> _cancel;
        protected Func<object, bool> _canExecute;
        private Expression<Func<object, bool>> _canExecuteExpression;

        private readonly Dictionary<INotifyPropertyChanged, List<string>> _observedObjects = new Dictionary<INotifyPropertyChanged, List<string>>();

        public MainVMBase MainViewModel { get; set; }
        public VMBase ViewModel { get; set; }
        public string Text { get; set; }
        public string IconText { get; set; }
        public bool HasBusy { get; set; }
        public bool UseBackgroundThread { get; set; }
        public bool IsCancelling { get; set; }
        public CommandBase(MainVMBase mainViewModel, VMBase viewModel)
        {
            MainViewModel = mainViewModel;
            ViewModel = viewModel;
            HasBusy = true;
            UseBackgroundThread = true;
            InferiorCommands = new List<CommandBase>();
        }

        public CommandBase()
        {
            InferiorCommands = new List<CommandBase>();
        }

        public CommandBase(Action<object> execute)
            : this(execute, null, null, null)
        {
        }

        public CommandBase(Action<object> execute, Expression<Func<object, bool>> canExecuteExpression)
            : this(execute, canExecuteExpression, null, null)
        {
        }

        public CommandBase(Action<object> execute, Expression<Func<object, bool>> canExecuteExpression, Action<object> cancel, Predicate<object> canCancel)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            if (canExecuteExpression != null)
            {
                SetCanExecute(canExecuteExpression);
            }

            _cancel = cancel;
            _canCancel = canCancel;
            InferiorCommands = new List<CommandBase>();
        }

        protected void SetCanExecute(Expression<Func<object, bool>> canExecuteExpression)
        {
            _canExecuteExpression = canExecuteExpression;
            _canExecute = _canExecuteExpression.Compile();
            if (IsEnabledCanExecuteChanged)
            {
                UpdateCanExecuteChanged();
            }
        }

        private void UpdateCanExecuteChanged()
        {
            foreach (KeyValuePair<INotifyPropertyChanged, List<string>> obj in _observedObjects)
            {
                obj.Key.PropertyChanged -= Notifier_PropertyChanged;
            }
            var propFinder = new NotifierFindingExpressionVisitor(_canExecuteExpression);
            var allNotifiers = propFinder.Notifiers;
            _observedObjects.Clear();
            foreach (var obj in allNotifiers)
            {
                FindAllNotifyableObjects(obj, propFinder.PropNames.ToList());
            }
            foreach (KeyValuePair<INotifyPropertyChanged, List<string>> obj in _observedObjects)
            {
                obj.Key.PropertyChanged += Notifier_PropertyChanged;
            }
        }

        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var obj = sender as INotifyPropertyChanged;
            if (obj == null)
            {
                return;
            }
            if (_observedObjects.ContainsKey(obj) &&
                _observedObjects[obj].Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged(this);
                UpdateCanExecuteChanged();
            }
        }

        private void RaiseCanExecuteChanged(object sender)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(sender);
            }
        }

        private void FindAllNotifyableObjects(INotifyPropertyChanged obj, IList<string> propNames)
        {
            _observedObjects.Add(obj, new List<string>());
            PropertyInfo[] objProperties = obj.GetType().GetProperties();
            foreach (var property in objProperties)
            {
                var founded = propNames.FirstOrDefault(val => string.CompareOrdinal(val, property.Name) == 0);
                if (!string.IsNullOrEmpty(founded))
                {
                    _observedObjects[obj].Add(founded);
                }
            }
        }

        public List<CommandBase> InferiorCommands { get; set; }

        protected virtual bool CanCancelInner(object parameter)
        {
            return false;
        }

        protected virtual void CancelInner(object parameter)
        {
        }

        protected virtual void ExecuteInner(object parameter)
        {
        }

        internal void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
            else
            {
                IsCancelling = false;
                ExecuteInner(parameter);
            }
            ReleaseBusy();

            if (OnCompleted != null)
            {
                OnCompleted();
            }
        }

        public void Cancel(object parameter)
        {
            if (_cancel != null)
            {
                _cancel(parameter);
            }
            else
            {
                CancelInner(parameter);
            }
            if (OnCancelled != null)
                OnCancelled();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public bool CanCancel(object parameter)
        {
            return _canExecute != null && _canCancel != null ? _canCancel(parameter) : CanCancelInner(parameter);
        }

        public void SetBusy()
        {
            if (MainViewModel != null)
            {
                MainViewModel.ActiveCommand = this;
                MainViewModel.SetBusy(true);
            }
        }


        public void ReleaseBusy()
        {
            if (MainViewModel != null && HasBusy)
            {
                MainViewModel.Invoke(ReleaseBusyInner);
            }
            if (MainViewModel != null)
            {
                if (MainViewModel.ReadySignal != null)
                {
                    MainViewModel.ReadySignal.Set();
                }
            }
        }

        public void ReleaseBusyInner()
        {
            MainViewModel.SetBusy(false);
            MainViewModel.ActiveCommand = null;
        }

        public bool Confirmation(string text)
        {
            return MainViewModel.Confirmation(text);
        }

        /// <summary>
        /// Add log text, at current indent 
        /// </summary>
        /// <param name="text"></param>
        public void Log(string text)
        {
            MainViewModel.Log(text);
        }

        /// <summary>
        /// Add log text, at current indent 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="logLevel"></param>
        public void Log(string text, LogLevels logLevel)
        {
            MainViewModel.Log(text, logLevel);
        }


        /// <summary>
        /// Add log fail text, reset current indent 
        /// </summary>
        /// <param name="text"></param>
        public void LogFail(string text)
        {
            MainViewModel.LogFail(text);
        }

        /// <summary>
        /// Add OK text, reset current indent 
        /// </summary>
        /// <param name="text"></param>
        public void LogOk(string text)
        {
            MainViewModel.LogOk(text);
        }

        public void AnnounceProgress(string text, int current, int total)
        {
            MainViewModel.AnnounceProgress(text, current, total);
        }

        public virtual bool IsSupportedAndReady()
        {
            return true;
        }

        #region ICloneable Members

        public object Clone()
        {
            CommandBase ret = (CommandBase)MemberwiseClone();
            if (ret.ViewModel != null)
            {
                ret.ViewModel = (VMBase)ret.ViewModel.Clone();
            }
            return ret;
        }

        #endregion

        public virtual void OnExecuteCompleted()
        {

        }
    }
}
