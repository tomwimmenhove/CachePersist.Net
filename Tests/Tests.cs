using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CachePersist.Net.Persistence;
using CachePersist.Net.Formatters;
using CachePersist.Net.Caching;

namespace Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestDictionaryStoreJsonFile()
        {
            var tempFilePath = Path.GetTempFileName();
            var testString = "Hello world";

            {
                var store = new DictionaryStoreJsonFile<int, string>(tempFilePath);
                var dict = new PersistentDictionary<int, string>(store);

                dict[42] = testString;
            }

            {
                var store = new DictionaryStoreJsonFile<int, string>(tempFilePath);
                var dict = new PersistentDictionary<int, string>(store);

                Assert.AreEqual(dict[42], testString);
                Assert.IsTrue(dict.TryGetValue(42, out var _));
                Assert.IsFalse(dict.TryGetValue(43, out var _));
            }

            File.Delete(tempFilePath);
        }

        [TestMethod]
        public void TestDictionaryStoreJsonString()
        {
            var tempFilePath = Path.GetTempFileName();
            var testString1 = "Hello world";
            var testString2 = "Beep";

            string jsonString = $"{{\"16\":\"{testString2}\"}}";

            var store = new DictionaryStoreJsonString<int, string>(jsonString);
            var dict = new PersistentDictionary<int, string>(store);

            store.Saving += (o, e) => jsonString = e.Json;

            dict[42] = testString1;

            Assert.AreEqual(dict[42], testString1);
            Assert.AreEqual(dict[16], testString2);
            Assert.IsTrue(dict.TryGetValue(42, out var _));
            Assert.IsTrue(dict.TryGetValue(16, out var _));
            Assert.IsFalse(dict.TryGetValue(43, out var _));

            File.Delete(tempFilePath);
        }


        [TestMethod]
        public void TestCache()
        {
            var data = new int[10000];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }

            var tempFilePath = Path.GetTempFileName();

            {
                var store = new CacheKeyStorageJsonFile(tempFilePath);
                var cache = new Cache(store);

                Assert.IsFalse(cache.ContainsKey("defaultFormatter"));
                Assert.IsFalse(cache.ContainsKey("protoFormatter"));
                Assert.IsFalse(cache.ContainsKey("binaryFormatter"));
                Assert.IsFalse(cache.ContainsKey("compressedFormatter"));

                Assert.IsFalse(cache.TryGetValue<int>("defaultFormatter", out var _));
                Assert.IsFalse(cache.TryGetValue<int>("protoFormatter", out var _));
                Assert.IsFalse(cache.TryGetValue<int>("binaryFormatter", out var _));
                Assert.IsFalse(cache.TryGetValue<int>("compressedFormatter", out var _));

                cache["defaultFormatter"] = data;
                cache.Set("protoFormatter", data, new ProtobufStreamFormatter<int[]>());
                cache.Set("binaryFormatter", data, new BinaryStreamFormatter());
                cache.Set("compressedFormatter", data, new BinaryCompressedStreamFormatter());

                Assert.IsTrue(cache.ContainsKey("defaultFormatter"));
                Assert.IsTrue(cache.ContainsKey("protoFormatter"));
                Assert.IsTrue(cache.ContainsKey("binaryFormatter"));
                Assert.IsTrue(cache.ContainsKey("compressedFormatter"));
            }

            {
                var store = new CacheKeyStorageJsonFile(tempFilePath);
                var cache = new Cache(store);

                Assert.AreEqual(cache.Keys.Count, 4);

                Assert.IsTrue(cache.ContainsKey("defaultFormatter"));
                Assert.IsTrue(cache.ContainsKey("protoFormatter"));
                Assert.IsTrue(cache.ContainsKey("binaryFormatter"));
                Assert.IsTrue(cache.ContainsKey("compressedFormatter"));

                Assert.IsTrue(cache.TryGetValue<int[]>("defaultFormatter", out var defaultFormatterResult));
                Assert.IsTrue(cache.TryGetValue<int[]>("protoFormatter", out var protoFormatterResult));
                Assert.IsTrue(cache.TryGetValue<int[]>("binaryFormatter", out var binaryFormatterrResult));
                Assert.IsTrue(cache.TryGetValue<int[]>("compressedFormatter", out var compressedFormatterResult));

                CollectionAssert.AreEqual(defaultFormatterResult, data);
                CollectionAssert.AreEqual(protoFormatterResult, data);
                CollectionAssert.AreEqual(binaryFormatterrResult, data);
                CollectionAssert.AreEqual(compressedFormatterResult, data);

                CollectionAssert.AreEqual(cache.Get<int[]>("defaultFormatter"), data);
                CollectionAssert.AreEqual(cache.Get<int[]>("protoFormatter"), data);
                CollectionAssert.AreEqual(cache.Get<int[]>("binaryFormatter"), data);
                CollectionAssert.AreEqual(cache.Get<int[]>("compressedFormatter"), data);

                Assert.IsTrue(cache.ContainsKey("defaultFormatter"));
                cache.Remove("binaryFormatter");
                cache.Remove("compressedFormatter");
            }

            {
                var store = new CacheKeyStorageJsonFile(tempFilePath);
                var cache = new Cache(store);

                Assert.IsTrue(cache.ContainsKey("defaultFormatter"));
                Assert.IsTrue(cache.ContainsKey("protoFormatter"));
                Assert.IsFalse(cache.ContainsKey("binaryFormatter"));
                Assert.IsFalse(cache.ContainsKey("compressedFormatter"));

                cache.Clear();

                Assert.AreEqual(cache.Keys.Count, 0);

                Assert.IsFalse(cache.ContainsKey("defaultFormatter"));
                Assert.IsFalse(cache.ContainsKey("protoFormatter"));
            }

            File.Delete(tempFilePath);
        }
    }
}
