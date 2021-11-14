using System.Collections.Generic;

namespace Serialization.Persistence
{
    public interface IDictionaryStore<TKey, TValue>
    {
        IDictionary<TKey, TValue> Dictionary { get; }

        void Save();
    }
}
