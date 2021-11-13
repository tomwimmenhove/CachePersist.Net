using System;
using System.IO;
using System.Text;
using ProtoBuf;

namespace serialization
{
    [Serializable, ProtoContract]
    class Test
    {
        [ProtoMember(1)]
        public int Something { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var formatter = new BinaryCompressedStreamFormatter();
            var formatter = new ProtobufStreamFormatter<Test>();

            var tpmFile = "/tmp/formattertest";//Path.GetTempFileName();

            var tmp = new Test();
            tmp.Something = 42;

            AnyFormatter.Serialize(tpmFile, tmp, formatter);

            var bla = AnyFormatter.Deserialize<Test>(tpmFile);

            var name = formatter.GetType().AssemblyQualifiedName;

            var nameb = Encoding.Unicode.GetBytes(name);

            var size = new FileInfo(tpmFile).Length;

            //File.Delete(tpmFile);

            Console.WriteLine(size);

            Console.ReadLine();
        }
    }
}
