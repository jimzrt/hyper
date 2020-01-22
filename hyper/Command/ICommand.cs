namespace hyper.commands
{
    internal interface ICommand
    {
        byte NodeId { get; set; }

        bool Retry { get; set; }

        bool Start();

        void Stop();
    }
}