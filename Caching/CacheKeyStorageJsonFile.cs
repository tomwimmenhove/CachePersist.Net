using Serialization.Persistence;

namespace Serialization.Caching
{
    public class CacheKeyStorageJsonFile : DictionaryStoreJsonFile<string, string>, ICacheKeyStorage
    {
        public CacheKeyStorageJsonFile(string initialJson)
            : base(initialJson)
        { }
    }
}
