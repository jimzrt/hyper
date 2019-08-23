using System.Collections.Generic;
using System.Collections.Specialized;

namespace Utils.UI.Bind
{
    public interface ISubscribeCollection<T> : ISubscribe, IList<T> where T : class
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
