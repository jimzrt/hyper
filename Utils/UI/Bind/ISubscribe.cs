namespace Utils.UI.Bind
{
    public interface ISubscribe
    {
        void UnSubscribe();
        void Subscribe();
        bool IsSubscribed { get; }
    }
}
