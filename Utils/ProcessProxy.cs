using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Utils
{
    public class ProcessProxy
    {
        private readonly bool _hasGui;
        private readonly string _path;
        private readonly string _argsTemplate;
        private readonly string _cmd;
        private string _args;
        private Process _runningProcess;

        public bool IsReady { get; private set; }

        public IntPtr MainWindowHandle
        {
            get
            {
                if (!IsReady || _runningProcess == null)
                {
                    return IntPtr.Zero;
                }

                return _runningProcess.MainWindowHandle;
            }
        }

        public ProcessProxy(string path, string cmd, string argsTemplate)
            : this(true, path, cmd, argsTemplate)
        {
        }

        public ProcessProxy(bool hasGui, string path, string cmd, string argsTemplate)
        {
            _hasGui = hasGui;
            try
            {
                _path = Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432") ?? "", path);
                if (!Directory.Exists(_path))
                {
                    _path = null;
                }
            }
            catch
            {
                // ignored
            }

            if (_path == null)
            {
                try
                {
                    _path = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? "", path);
                    if (!Directory.Exists(_path))
                    {
                        _path = null;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            if (_path == null)
            {
                try
                {
                    _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), path);
                    if (!Directory.Exists(_path))
                    {
                        _path = null;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            if (_path != null)
            {
                IsReady = true;
            }
            _argsTemplate = argsTemplate;
            _cmd = cmd;
        }

        public void Start(params object[] args)
        {
            _args = string.Format(_argsTemplate, args);
            if (IsReady)
            {
                _runningProcess =
                    new Process { StartInfo = new ProcessStartInfo(_cmd, _args) { WorkingDirectory = _path } };
                _runningProcess.Start();
                if (_hasGui)
                {
                    _runningProcess.WaitForInputIdle();
                }
                Thread.Sleep(1000);
                while (_runningProcess.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(200);
                    _runningProcess.Refresh();
                }
            }
            else
            {
                throw new InvalidOperationException("IsReady == false");
            }
        }

        public void Close()
        {
            if (IsReady)
            {
                if (_runningProcess != null)
                {
                    try
                    {
                        _runningProcess.CloseMainWindow();
                        _runningProcess.WaitForExit();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    _runningProcess = null;
                }
            }
        }
    }
}