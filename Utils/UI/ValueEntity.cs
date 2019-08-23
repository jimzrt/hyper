namespace Utils.UI
{
    public class ValueEntity<T> : EntityBase
    {
        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Notify("Value");
            }
        }
    }
}
