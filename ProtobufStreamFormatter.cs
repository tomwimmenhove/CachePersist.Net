using System.IO;

namespace serialization
{
    public class ProtobufStreamFormatter<T> : IStreamFormatter
    {
        public void Serialize(Stream stream, object value) => ProtoBuf.Serializer.Serialize(stream, value);
        public object Deserialize(Stream stream) => ProtoBuf.Serializer.Deserialize<T>(stream);
    }
}
