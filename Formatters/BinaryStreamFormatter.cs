using System.Runtime.Serialization.Formatters.Binary;

namespace CachePersist.Net.Formatters
{
    public class BinaryStreamFormatter : FormatterWrapper
    {
        public BinaryStreamFormatter() : base(new BinaryFormatter()) { }
    }
}
