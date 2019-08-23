namespace Utils
{
    public interface IDataSource
    {
        string SourceName { get; }
        string Alias { get; }
        string Args { get; }
        bool Validate();
    }
}
