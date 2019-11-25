namespace hyper.Output
{
    interface IOutput
    {
        void HandleCommand(object command, byte srcNodeId, byte destNodeId);
    }
}