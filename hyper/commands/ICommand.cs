namespace hyper.commands
{
    internal interface ICommand
    {
        bool Active { get; }
        void Start();
        void Stop();

    }
}
