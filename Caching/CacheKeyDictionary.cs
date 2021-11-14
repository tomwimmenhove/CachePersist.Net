using Serialization.Persistence;

namespace Serialization.Caching
{
    public class CacheKeyDictionary : PersistentDictionary<string, string>
    {
        public CacheKeyDictionary(ICacheKeyStorage store)
            : base(store)
        { }
    }
}
