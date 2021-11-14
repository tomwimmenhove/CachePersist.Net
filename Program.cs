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
        static void Main(string[] args)
        {
            var store = new CacheKeyStorageJsonFile("/tmp/store");
            var cache = new Cache(store);

            //cache.Clear();
            cache.Cleanup();

            //cache["test"] = 42;
            //cache["test"] = "hallo";

            //var ci = cache.GetCacheInfo("test");

            cache.Set("test", "hello world");
            
            var s = cache.Get<string>("test");

            Console.WriteLine(s);

            //Console.WriteLine($"one: {cache.GetCacheInfo("one")}");
            //Console.WriteLine($"two: {cache.GetCacheInfo("two")}");
            Console.WriteLine($"three: {cache.GetCacheInfo("three")}");

            //var one = cache.Get<sTest[]>("one");
            //var two = cache.Get<sTest[]>("two");
            var three333 = cache.Get<sTest[]>("three");

            if (cache.TryGetValue<sTest[]>("three", out var three))
            {

            }

            return;

            var n = 1000000;
            var manyThings = new sTest[n];

            var rnd = new Random();
            for (var i = 0; i < n; i++)
            {
                manyThings[i].A = (eTest) rnd.Next();
                manyThings[i].B = (eTest) rnd.Next();
            }

            cache.Set("one", manyThings, new BinaryCompressedStreamFormatter());
            cache.Set("two", manyThings, new BinaryStreamFormatter());
            cache.Set("three", manyThings, new ProtobufStreamFormatter<sTest[]>());

            //Test(manyThings, new BinaryCompressedStreamFormatter());
            //Test(manyThings, new BinaryCompressedStreamFormatter(System.IO.Compression.CompressionLevel.Fastest));
            //Test(manyThings, new BinaryCompressedStreamFormatter(System.IO.Compression.CompressionLevel.NoCompression));
            //Test(manyThings, new BinaryStreamFormatter());
        }
    }
}
