using System.IO;

namespace Serialization.Formatters
{
    public interface IStreamFormatter
    {
        void Serialize(Stream stream, object value);
        object Deserialize(Stream stream);
    }
}
