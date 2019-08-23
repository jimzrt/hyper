using Utils.UI.Bind;
using Utils.UI.Wrappers;

namespace Utils.UI.MVVM
{
    public class ConnectViewModel : DialogVMBase
    {
        public MainVMBase MainVM { get; protected set; }
        public ConnectViewModel(MainVMBase mainVM)
        {
            MainVM = mainVM;
            Title = "Connect";
            Description = "Select port to connect";
            CommandOk = new ConnectCommand(MainVM, this);
        }

        public void RefreshDataSources()
        {
            DataSources.Clear();
            FillSerialPorts();
            FillSocketAddresses();
        }

        public virtual void DiscoverZipAddresses()
        {
        }

        protected virtual void FillSerialPorts()
        {
        }

        protected virtual void FillSocketAddresses()
        {
        }

        private ISubscribeCollection<SelectableItemWithComment<IDataSource>> _dataSources;
        public ISubscribeCollection<SelectableItemWithComment<IDataSource>> DataSources
        {
            get
            {
                return _dataSources ?? (_dataSources = MainVM.SubscribeCollectionFactory.Create<SelectableItemWithComment<IDataSource>>());
            }
            set
            {
                _dataSources = value;
                RaisePropertyChanged("DataSources");
            }
        }
    }
}
