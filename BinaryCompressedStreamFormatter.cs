using System.IO;
using System.IO.Compression;

namespace serialization
{
    public class BinaryCompressedStreamFormatter : IStreamFormatter
    {
        private BinaryStreamFormatter _formatter = new BinaryStreamFormatter();

        public void Serialize(Stream stream, object value)
        {
            using (var compressor = new GZipStream(stream, CompressionLevel.Fastest))
            {
                _formatter.Serialize(compressor, value);
            }
        }

        public object Deserialize(Stream stream)
        {
            using (var compressor = new GZipStream(stream, CompressionMode.Decompress))
            {
                return _formatter.Deserialize(compressor);
            }
        }
    }
}
