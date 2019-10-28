using System.Collections.Generic;

namespace Utils
{
    public class LazyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _innerDictionary;
        public LazyDictionary()
        {
            _innerDictionary = new Dictionary<TKey, TValue>();
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_innerDictionary.ContainsKey(key))
                {
                    return _innerDictionary[key];
                }
                else
                {
                    return default(TValue);
                }
            }

            set
            {
                if (_innerDictionary.ContainsKey(key))
                {
                    _innerDictionary[key] = value;
                }
                else
                {
                    _innerDictionary.Add(key, value);
                }
            }
        }

        public int Count
        {
            get
            {
                return _innerDictionary.Count;
            }
        }


        public void Add(TKey key, TValue value)
        {
            _innerDictionary.Add(key, value);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return _innerDictionary.ContainsKey(key);
        }
    }
}
