using System.IO;

namespace serialization
{
    public interface IStreamFormatter
    {
        void Serialize(Stream stream, object value);
        object Deserialize(Stream stream);
    }
}
