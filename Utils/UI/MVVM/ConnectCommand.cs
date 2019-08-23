namespace Utils.UI.MVVM
{
    public class ConnectCommand : CommandBase
    {
        public ConnectViewModel ConnectVM { get; set; }
        public ConnectCommand(MainVMBase mainVM, ConnectViewModel connectVM)
            : base(mainVM, null)
        {
            ConnectVM = connectVM;
        }

        protected override void ExecuteInner(object parameter)
        {
            if (ConnectVM.DataSources != null)
            {
                foreach (var item in ConnectVM.DataSources)
                {
                    if (item.IsSelected)
                    {
                        MainViewModel.ConnectionName = item.Item.SourceName;
                        break;
                    }
                }
            }
            ConnectVM.IsDialogOk = true;
            ConnectVM.Close();
        }
    }
}
