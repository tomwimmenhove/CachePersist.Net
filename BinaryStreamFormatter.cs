using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace serialization
{
    public class BinaryStreamFormatter : IStreamFormatter
    {
        private BinaryFormatter _formatter = new BinaryFormatter();

        public void Serialize(Stream stream, object value) => _formatter.Serialize(stream, value);
        public object Deserialize(Stream stream) => _formatter.Deserialize(stream);
    }
}
