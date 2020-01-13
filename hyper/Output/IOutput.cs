namespace hyper.Output
{
    internal interface IOutput
    {
        void HandleCommand(object command, byte srcNodeId, byte destNodeId);
    }
}