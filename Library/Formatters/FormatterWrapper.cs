using System.IO;
using System.Runtime.Serialization;

namespace CachePersist.Net.Formatters
{
    public class FormatterWrapper : IStreamFormatter
    {
        private IFormatter _formatter;

        public FormatterWrapper(IFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Serialize(Stream stream, object value) => _formatter.Serialize(stream, value);
        public object Deserialize(Stream stream) => _formatter.Deserialize(stream);
    }
}
