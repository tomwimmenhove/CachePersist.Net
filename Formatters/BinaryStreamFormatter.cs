using System.Runtime.Serialization.Formatters.Binary;

namespace Serialization.Formatters
{
    public class BinaryStreamFormatter : FormatterWrapper
    {
        public BinaryStreamFormatter() : base(new BinaryFormatter()) { }
    }
}
