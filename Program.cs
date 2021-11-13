using System;
using System.IO;

namespace serialization
{
    [Serializable]
    class Test
    {
        public int Something { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var formatter = new BinaryCompressedStreamFormatter();

            var tpmFile = Path.GetTempFileName();

            var tmp = new Test();
            tmp.Something = 42;

            AnyFormatter.Serialize(tpmFile, tmp, formatter);

            var bla = AnyFormatter.Deserialize<Test>(tpmFile);

            var size = new FileInfo(tpmFile).Length;

            File.Delete(tpmFile);

            Console.WriteLine(size);

            Console.ReadLine();
        }
    }
}
