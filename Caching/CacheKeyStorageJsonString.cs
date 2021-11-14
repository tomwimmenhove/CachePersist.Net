using Serialization.Persistence;

namespace Serialization.Caching
{
    public class CacheKeyStorageJsonString : DictionaryStoreJsonString<string, string>
    {
        public CacheKeyStorageJsonString(string initialJson)
            : base(initialJson)
        { }
    }
}
