using Serialization.Persistence;

namespace Serialization.Caching
{
    public interface ICacheKeyStorage : IDictionaryStore<string, string>
    { }
}
