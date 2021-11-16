using System.IO;

namespace CachePersist.Net.Formatters
{
    public interface IStreamFormatter
    {
        void Serialize(Stream stream, object value);
        object Deserialize(Stream stream);
    }
}
