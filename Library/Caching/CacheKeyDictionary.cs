using CachePersist.Net.Persistence;

namespace CachePersist.Net.Caching
{
    public class CacheKeyDictionary : PersistentDictionary<string, string>
    {
        public CacheKeyDictionary(ICacheKeyStorage store)
            : base(store)
        { }
    }
}
