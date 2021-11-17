using CachePersist.Net.Persistence;

namespace CachePersist.Net.Caching
{
    public class CacheKeyStorageJsonString : DictionaryStoreJsonString<string, string>
    {
        public CacheKeyStorageJsonString(string initialJson)
            : base(initialJson)
        { }
    }
}
