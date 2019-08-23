using System;
using System.Collections.Generic;
using System.Linq;
using Utils.UI.Bind;
using Utils.UI.Enums;
using Utils.UI.Logging;

namespace Utils.UI.MVVM
{
    public class LogViewModel : DialogVMBase
    {
        public Action<string, LogLevels, LogIndents, LogIndents> LogCallback { private get; set; }
        public MainVMBase MainVM { get; protected set; }
        public ClearLogCommand ClearLogCommand { get; set; }
        public LogViewModel(MainVMBase mainVM)
        {
            MainVM = mainVM;
            Title = "Log";
            DialogInfo.IsTopmost = false;
            ClearLogCommand = new ClearLogCommand(MainVM);
            SetDefaultLogCallback();
        }

        private string _currentIndent = "";
        private readonly Queue<LogPacket> _logPacketsTotal = new Queue<LogPacket>();

        private ISubscribeCollection<LogPacket> _logPackets;


        public ISubscribeCollection<LogPacket> LogPackets
        {
            get { return _logPackets ?? (_logPackets = MainVM.SubscribeCollectionFactory.Create<LogPacket>()); }
            set
            {
                _logPackets = value;
                Notify("LogPackets");
            }
        }

        private LogPacket _lastLogPacket;
        public LogPacket LastLogPacket
        {
            get { return _lastLogPacket; }
            set
            {
                _lastLogPacket = value;
                Notify("LastLogPacket");
            }
        }

        private LogPacket _lastLogActionPacket;
        public LogPacket LastLogActionPacket
        {
            get { return _lastLogActionPacket; }
            set
            {
                _lastLogActionPacket = value;
                Notify("LastLogActionPacket");
            }
        }

        private int _maxNumerOfLogBlocks = 900;
        public int MaxNumerOfLogBlocks
        {
            get { return _maxNumerOfLogBlocks; }
            set
            {
                _maxNumerOfLogBlocks = value;
                Notify("MaxNumerOfLogBlocks");
            }
        }

        private bool _isAutoScrollLogPackets = true;
        public bool IsAutoScrollLogPackets
        {
            get { return _isAutoScrollLogPackets; }
            set
            {
                _isAutoScrollLogPackets = value;
                Notify("IsAutoScrollLogPackets");
            }
        }

        private bool _isAutoScrollLogMessages = true;
        public bool IsAutoScrollLogMessages
        {
            get { return _isAutoScrollLogMessages; }
            set
            {
                _isAutoScrollLogMessages = value;
                Notify("IsAutoScrollLogMessages");
            }
        }

        public void FeedStoredLogPackets()
        {
            while (_logPacketsTotal.Count > 0)
            {
                var itemRef = _logPacketsTotal.Dequeue();
                if (itemRef != null)
                {
                    while (LogPackets.Count > 1000)
                        LogPackets.RemoveAt(0);
                    LogPackets.Add(itemRef);
                }
            }
        }

        public virtual void Log(string text, LogLevels level, LogIndents indentBefore, LogIndents indentAfter)
        {
            LogCallback?.Invoke(text, level, indentBefore, indentAfter);
        }

        private void SetDefaultLogCallback()
        {
            LogCallback = (text, level, indentBefore, indentAfter) =>
            {
                Dyes dye = Dyes.Black;
                bool isBold = false;
                switch (level)
                {
                    case LogLevels.Title:
                        isBold = true;
                        break;
                    case LogLevels.Error:
                    case LogLevels.Fail:
                        dye = Dyes.Red;
                        isBold = true;
                        break;
                    case LogLevels.Done:
                    case LogLevels.Ok:
                        dye = Dyes.Green;
                        isBold = true;
                        break;
                    case LogLevels.Warning:
                        dye = Dyes.DarkOrange;
                        break;
                }

                const int indentStep = 2;
                switch (indentBefore)
                {
                    case LogIndents.None:
                        _currentIndent = "";
                        break;
                    case LogIndents.Current:
                        break;
                    case LogIndents.Increase:
                        _currentIndent = _currentIndent.PadRight(_currentIndent.Length + indentStep, ' ');
                        break;
                    case LogIndents.Decrease:
                        if (_currentIndent.Length > indentStep)
                            _currentIndent = _currentIndent.Substring(0, _currentIndent.Length - indentStep);
                        else
                            _currentIndent = "";
                        break;
                }

                LogPacket lp = new LogPacket("{0}{1}".FormatStr((object)_currentIndent, text), dye, isBold, true, true);
                _logPacketsTotal.Enqueue(lp);
                LastLogPacket = lp;
                var actionLevels = new[] { LogLevels.Done, LogLevels.Ok, LogLevels.Fail };
                if (actionLevels.Contains(level))
                {
                    LastLogActionPacket = lp;
                }
                switch (indentAfter)
                {
                    case LogIndents.None:
                        _currentIndent = "";
                        break;
                    case LogIndents.Current:
                        break;
                    case LogIndents.Increase:
                        _currentIndent = _currentIndent.PadRight(_currentIndent.Length + indentStep, ' ');
                        break;
                    case LogIndents.Decrease:
                        if (_currentIndent.Length > indentStep)
                            _currentIndent = _currentIndent.Substring(0, _currentIndent.Length - indentStep);
                        else
                            _currentIndent = "";
                        break;
                }
            };
        }
    }
}
