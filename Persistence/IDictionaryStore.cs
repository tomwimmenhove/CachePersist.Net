using System.Collections.Generic;

namespace CachePersist.Net.Persistence
{
    public interface IDictionaryStore<TKey, TValue>
    {
        IDictionary<TKey, TValue> Dictionary { get; }

        void Save();
    }
}
