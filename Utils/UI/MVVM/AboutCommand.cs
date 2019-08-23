namespace Utils.UI.MVVM
{
    public class AboutCommand : CommandBase
    {
        public AboutCommand(MainVMBase mainVM)
            : base(mainVM, null)
        {
        }

        protected override void ExecuteInner(object parameter)
        {
            MainViewModel.DialogShow(new AboutViewModel());
        }
    }
}
