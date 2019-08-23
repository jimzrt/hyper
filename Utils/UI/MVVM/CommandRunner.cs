using System.ComponentModel;
using System.Diagnostics;

namespace Utils.UI.MVVM
{
    public static class CommandRunner
    {
        public static void ExecuteAsync(CommandBase cmd, object parameter)
        {
            var cmdRunner = new CommandRunnerInternal();
            cmdRunner.ExecuteAsync(cmd, parameter);
        }

        public static void Execute(CommandBase cmd, object parameter)
        {
            var cmdRunner = new CommandRunnerInternal();
            cmdRunner.Execute(cmd, parameter);
        }
    }

    internal class CommandRunnerInternal
    {
        private void ProcessInferiorCommands(CommandBase command, object parameter)
        {
            if (command.InferiorCommands != null)
            {
                foreach (CommandBase cmd in command.InferiorCommands)
                {
                    if (cmd.IsSupportedAndReady())
                    {
                        if (cmd.HasBusy)
                        {
                            cmd.SetBusy();
                        }
                        Execute(cmd, parameter);
                    }
                }
            }
        }
        private CommandExecuteArgs _arg;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public void ExecuteAsync(CommandBase cmd, object parameter)
        {
            if (cmd != null)
            {
                if (cmd.IsSupportedAndReady())
                {
                    if (cmd.HasBusy)
                    {
                        cmd.SetBusy();
                    }
                    if (cmd.UseBackgroundThread)
                    {
                        _stopwatch.Reset();
                        _stopwatch.Start();
                        BackgroundWorker bg = new BackgroundWorker();
                        bg.DoWork += bg_DoWork;
                        _arg = new CommandExecuteArgs { Command = cmd, Parameter = parameter };
                        bg.RunWorkerAsync(_arg);
                        bg.RunWorkerCompleted += bg_RunWorkerCompleted;
                    }
                    else
                    {
                        cmd.Execute(parameter);
                        ProcessInferiorCommands(cmd, parameter);
                    }
                }
            }
        }

        public void Execute(CommandBase cmd, object parameter)
        {
            if (cmd != null)
            {
                cmd.Execute(parameter);
            }
        }

        private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _stopwatch.Stop();
            _arg.Command.OnExecuteCompleted();
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                var argument = e.Argument as CommandExecuteArgs;
                if (argument != null)
                {
                    CommandExecuteArgs arg = argument;
                    if (arg.Command != null)
                    {
                        arg.Command.Execute(arg.Parameter);
                        ProcessInferiorCommands(arg.Command, arg.Parameter);
                    }
                }
            }
        }

        internal class CommandExecuteArgs
        {
            public CommandBase Command { get; set; }
            public object Parameter { get; set; }
        }
    }
}
