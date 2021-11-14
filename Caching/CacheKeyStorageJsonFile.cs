using Serialization.Persistence;

namespace Serialization.Caching
{
    public class CacheKeyStorageJsonFile : DictionaryStoreJsonFile<string, string>
    {
        public CacheKeyStorageJsonFile(string initialJson)
            : base(initialJson)
        { }
    }
}
