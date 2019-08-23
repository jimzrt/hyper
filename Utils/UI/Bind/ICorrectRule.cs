namespace Utils.UI.Bind
{
    public interface ICorrectRule
    {
        object Correct(string value);
        string ToString(object value);
        bool HasName(string name);

        bool IsValid(object value);
        string ValidationMessage { get; set; }
    }
}
