using System;
using System.IO;
using System.Threading;
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
        public void TestCacheRemoveOld()
        {
            var tempFilePath = Path.GetTempFileName();
            var store = new CacheKeyStorageJsonFile(tempFilePath);
            var cache = new Cache(store);

            cache["test1"] = 42;
            Thread.Sleep(50);
            cache["test2"] = 43;

            cache.RemoveOlderThan(TimeSpan.FromMilliseconds(25));

            Assert.IsFalse(cache.ContainsKey("test1"));
            Assert.IsTrue(cache.ContainsKey("test2"));

            cache.Clear();
            Assert.IsFalse(cache.ContainsKey("test2"));

            File.Delete(tempFilePath);
        }

        [TestMethod]
        public void TestCacheCleanup()
        {
            var tempFilePath = Path.GetTempFileName();
            var store = new CacheKeyStorageJsonFile(tempFilePath);
            var cache = new Cache(store);

            cache["test"] = 42;

            var fileInfo = cache.GetCacheInfo("test");
            Assert.IsTrue(fileInfo.Exists);
            Assert.IsTrue(cache.ContainsKey("test"));

            Assert.AreEqual(cache.Cleanup(), 0);

            File.Delete(fileInfo.FullName);
            Assert.IsFalse(cache.ContainsKey("test"));

            Assert.AreEqual(cache.Cleanup(), 1);

            File.Delete(tempFilePath);
        }

        [TestMethod]
        public void TestCacheBinaryStreamFormatter() =>
            TestCacheFormatter(new BinaryStreamFormatter());

        [TestMethod]
        public void TestCacheBinaryCompressedStreamFormatter() =>
            TestCacheFormatter(new BinaryCompressedStreamFormatter());

        [TestMethod]
        public void TestCacheProtobufStreamFormatter() =>
            TestCacheFormatter(new ProtobufStreamFormatter<int[]>());

        public void TestCacheFormatter(IStreamFormatter formatter)
        {
            var n = 10000;
            var data1 = new int[n];
            var data2 = new int[n];
            for (var i = 0; i < n; i++)
            {
                data1[i] = i;
                data2[i] = i * 2;
            }

            var tempFilePath = Path.GetTempFileName();

            {
                var store = new CacheKeyStorageJsonFile(tempFilePath);
                var cache = new Cache(store);

                Assert.IsFalse(cache.ContainsKey("test1"));
                Assert.IsFalse(cache.TryGetValue<int>("test2", out var _));

                cache.Set("test1", data1, formatter);
                cache.Set("test2", data2, formatter);

                Assert.IsTrue(cache.ContainsKey("test1"));
                Assert.IsTrue(cache.ContainsKey("test2"));
                Assert.IsFalse(cache.ContainsKey("test3"));
            }

            {
                var store = new CacheKeyStorageJsonFile(tempFilePath);
                var cache = new Cache(store);

                Assert.AreEqual(cache.Keys.Count, 2);

                Assert.IsTrue(cache.ContainsKey("test1"));
                Assert.IsTrue(cache.ContainsKey("test2"));
                Assert.IsFalse(cache.ContainsKey("test3"));

                Assert.IsTrue(cache.TryGetValue<int[]>("test1", out var test1));
                Assert.IsTrue(cache.TryGetValue<int[]>("test2", out var test2));

                CollectionAssert.AreEqual(test1, data1);
                CollectionAssert.AreEqual(test2, data2);

                CollectionAssert.AreEqual(cache.Get<int[]>("test1"), test1);
                CollectionAssert.AreEqual(cache.Get<int[]>("test2"), test2);
                cache.Remove("test2");
            }

            {
                var store = new CacheKeyStorageJsonFile(tempFilePath);
                var cache = new Cache(store);

                Assert.IsTrue(cache.ContainsKey("test1"));
                Assert.IsFalse(cache.ContainsKey("test2"));

                cache.Clear();

                Assert.AreEqual(cache.Keys.Count, 0);

                Assert.IsFalse(cache.ContainsKey("test1"));
                Assert.IsFalse(cache.ContainsKey("test2"));
            }

            File.Delete(tempFilePath);
        }
    }
}
