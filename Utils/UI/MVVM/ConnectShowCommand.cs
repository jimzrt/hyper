namespace Utils.UI.MVVM
{
    public class ConnectShowCommand : CommandBase
    {
        public ConnectShowCommand(MainVMBase mainVM)
            : base(mainVM, null)
        {
        }

        protected override void ExecuteInner(object parameter)
        {
            MainViewModel.DialogShow(new ConnectViewModel(MainViewModel));
        }
    }
}
