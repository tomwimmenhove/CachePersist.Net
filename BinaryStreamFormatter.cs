using System.Runtime.Serialization.Formatters.Binary;

namespace serialization
{
    public class BinaryStreamFormatter : FormatterWrapper
    {
        public BinaryStreamFormatter() : base(new BinaryFormatter()) { }
    }
}
