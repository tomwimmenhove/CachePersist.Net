using Serialization.Persistence;

namespace Serialization.Caching
{
    public class CacheKeyDictionary : PersistentDictionary<string, string>
    {
        public CacheKeyDictionary(IDictionaryStore<string, string> store)
            : base(store)
        { }
    }
}
