using CachePersist.Net.Persistence;

namespace CachePersist.Net.Caching
{
    public class CacheKeyStorageJsonFile : DictionaryStoreJsonFile<string, string>, ICacheKeyStorage
    {
        public CacheKeyStorageJsonFile(string initialJson)
            : base(initialJson)
        { }
    }
}
