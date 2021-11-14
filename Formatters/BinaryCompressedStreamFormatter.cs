using System.IO;
using System.IO.Compression;

namespace Serialization.Formatters
{
    public class BinaryCompressedStreamFormatter : IStreamFormatter
    {
        private BinaryStreamFormatter _formatter = new BinaryStreamFormatter();
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;

        public BinaryCompressedStreamFormatter() { }

        public BinaryCompressedStreamFormatter(CompressionLevel compressionLevel)
        {
            _compressionLevel = compressionLevel;
        }

        public void Serialize(Stream stream, object value)
        {
            using (var compressor = new GZipStream(stream, _compressionLevel))
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
