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
        public void TestCacheKeyStorageBinaryFile()
        {
            var tempFilePath = Path.GetTempFileName();

            {
                var store = new CacheKeyStorageBinaryFile(tempFilePath);

                store.Dictionary["hello"] = "world";
                store.Dictionary["fruit"] = "apple";
                store.Dictionary["animal"] = "dog";

                store.Save();
            }

            {
                var store = new CacheKeyStorageBinaryFile(tempFilePath);

                store.Dictionary.Remove("fruit");

                store.Save();
            }

            {
                var store = new CacheKeyStorageBinaryFile(tempFilePath);

                Assert.AreEqual(store.Dictionary["hello"], "world");
                Assert.AreEqual(store.Dictionary["animal"], "dog");
                Assert.IsFalse(store.Dictionary.ContainsKey("fruit"));

                store.Save();
            }

            File.Delete(tempFilePath);
        }

        [TestMethod]
        public void TestCacheBinaryStreamFormatterJson() =>
            TestCacheFormatter(new BinaryStreamFormatter(), typeof(CacheKeyStorageJsonFile));

        [TestMethod]
        public void TestCacheBinaryCompressedStreamFormatterJson() =>
            TestCacheFormatter(new BinaryCompressedStreamFormatter(), typeof(CacheKeyStorageJsonFile));

        [TestMethod]
        public void TestCacheProtobufStreamFormatterJson() =>
            TestCacheFormatter(new ProtobufStreamFormatter<int[]>(), typeof(CacheKeyStorageJsonFile));

        [TestMethod]
        public void TestCacheProtobufCompressedStreamFormatterJson() =>
            TestCacheFormatter(new ProtobufCompressedStreamFormatter<int[]>(), typeof(CacheKeyStorageJsonFile));


        [TestMethod]
        public void TestCacheBinaryStreamFormatterBinary() =>
            TestCacheFormatter(new BinaryStreamFormatter(), typeof(CacheKeyStorageBinaryFile));

        [TestMethod]
        public void TestCacheBinaryCompressedStreamFormatterBinary() =>
            TestCacheFormatter(new BinaryCompressedStreamFormatter(), typeof(CacheKeyStorageBinaryFile));

        [TestMethod]
        public void TestCacheProtobufStreamFormatterBinary() =>
            TestCacheFormatter(new ProtobufStreamFormatter<int[]>(), typeof(CacheKeyStorageBinaryFile));

        [TestMethod]
        public void TestCacheProtobufCompressedStreamFormatterBinary() =>
            TestCacheFormatter(new ProtobufCompressedStreamFormatter<int[]>(), typeof(CacheKeyStorageBinaryFile));

        public void TestCacheFormatter(IStreamFormatter formatter, Type storageType)
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
                var store = (ICacheKeyStorage) Activator.CreateInstance(storageType, tempFilePath);
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
                var store = (ICacheKeyStorage) Activator.CreateInstance(storageType, tempFilePath);
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

                try
                {
                    data2 = cache.Get<int[]>("test2");
                }
                catch (Exception)
                {
                    Assert.Fail();
                }

                Assert.IsTrue(cache.Keys.Contains("test2"));
                cache.Remove("test2");
                Assert.IsFalse(cache.Keys.Contains("test2"));

                try
                {
                    data2 = cache.Get<int[]>("test2");
                    Assert.Fail();
                }
                catch (Exception)
                {
                    /* Pass */
                }
            }

            {
                var store = (ICacheKeyStorage)Activator.CreateInstance(storageType, tempFilePath);
                var cache = new Cache(store);

                Assert.IsTrue(cache.ContainsKey("test1"));
                Assert.IsFalse(cache.ContainsKey("test2"));

                cache.Clear();

                Assert.AreEqual(cache.Keys.Count, 0);

                Assert.IsFalse(cache.ContainsKey("test1"));
                Assert.IsFalse(cache.ContainsKey("test2"));

                cache["manualDelete"] = 42;
                File.Delete(cache.GetCacheInfo("manualDelete").FullName);
            }

            {
                var store = (ICacheKeyStorage)Activator.CreateInstance(storageType, tempFilePath);
                var cache = new Cache(store);

                Assert.IsFalse(cache.ContainsKey("manualDelete"));
            }

            File.Delete(tempFilePath);
        }
    }
}
