using System;
using System.IO;
using System.Diagnostics;
using ProtoBuf;
using Serialization.Formatters;
using Serialization.Caching;

namespace Serialization
{
    [Serializable, ProtoContract]
    class Test
    {
        [ProtoMember(1)]
        public int Something { get; set; }
    }

    [Serializable, ProtoContract]
    enum eTest : short
    {
        One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten
    }

    [Serializable, ProtoContract]
    struct sTest
    {
        [ProtoMember(1)]
        public eTest A {get; set;}

        [ProtoMember(2)]
        public eTest B {get; set;}
    }

    class Program
    {
        static void Test<T>(T value, IStreamFormatter formatter)
        {
            var tpmFile = "/tmp/formattertest";//Path.GetTempFileName();

            var sw = new Stopwatch();
            sw.Start();
            AnyFormatter.Serialize(tpmFile, value, formatter);
            sw.Stop();
            Console.WriteLine($"Serialization using {formatter} took {sw.Elapsed}");

            sw.Reset();

            sw.Start();
            var restored = AnyFormatter.Deserialize<sTest[]>(tpmFile);
            sw.Stop();
            Console.WriteLine($"Deserialization using {formatter} took {sw.Elapsed}");

            //File.Delete(tpmFile);
            
            var size = new FileInfo(tpmFile).Length;

            Console.WriteLine($"File size: {size / 1024}KB");
        }

        static void Main(string[] args)
        {
            var storeFile = "/tmp/store";

            var store = new CacheKeyStorageJsonFile(storeFile);
            var pd = new CacheKeyDictionary(store);

            Console.WriteLine(pd["hoi"]);

            pd["hoi"] = "test";

            return;
            var n = 1000000;
            var manyThings = new sTest[n];

            var rnd = new Random();
            for (var i = 0; i < n; i++)
            {
                manyThings[i].A = (eTest) rnd.Next();
                manyThings[i].B = (eTest) rnd.Next();
            }

            //Test(manyThings, new BinaryCompressedStreamFormatter());
            //Test(manyThings, new BinaryCompressedStreamFormatter(System.IO.Compression.CompressionLevel.Fastest));
            //Test(manyThings, new BinaryCompressedStreamFormatter(System.IO.Compression.CompressionLevel.NoCompression));
            //Test(manyThings, new BinaryStreamFormatter());
            Test(manyThings, new ProtobufStreamFormatter<sTest[]>());
        }
    }
}
