using System.Collections.Generic;

namespace serialization
{
    public interface IPersistentDictionaryStore<TKey, TValue>
    {
        IDictionary<TKey, TValue> Dictionary { get; }

        void Save();
    }
}
