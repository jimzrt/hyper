namespace hyper.commands
{
    internal interface ICommand
    {
       // bool Active { get; }
        bool Start();
        void Stop();

    }
}
