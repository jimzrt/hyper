namespace Utils.UI.MVVM
{
    public class ClearLogCommand : CommandBase
    {
        public ClearLogCommand(MainVMBase mainVM)
            : base(mainVM, null)
        {
            Text = "Clear log command";
            UseBackgroundThread = false;
            HasBusy = false;
        }

        protected override void ExecuteInner(object parameter)
        {
            if (MainViewModel.LogVM.LogPackets != null)
            {
                MainViewModel.LogVM.LogPackets.Clear();
                MainViewModel.LogVM.IsAutoScrollLogMessages = true;
            }
        }
    }
}
