namespace Utils.UI.MVVM
{
    public class UserInputViewModel : DialogVMBase
    {
        public string InputData { get; set; }
        public bool IsInputDataVisible { get; set; }
        public object InputOptions { get; set; }
        public bool IsInputOptionsVisible { get; set; }
        public int SelectedInputOptionIndex { get; set; }
        public object SelectedInputOption { get; set; }
        public string AdditionalText { get; set; }
        public bool IsAdditionalTextVisible { get; set; }
        public bool IsCancelButtonVisible { get; set; }
        public UserInputViewModel()
        {
            IsCancelButtonVisible = true;
            DialogInfo.IsModal = true;
            DialogInfo.IsTopmost = true;
        }
    }
}
