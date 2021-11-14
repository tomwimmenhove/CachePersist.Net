using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace serialization
{
    public class PersistentCacheKeyDictionary : PersistentDictionary<string, string>
    {
        public PersistentCacheKeyDictionary(IPersistentDictionaryStore<string, string> store)
            : base(store)
        { }
    }

    public class CacheKeyStorageJsonString : PersistentDictionaryStoreJsonString<string, string>
    {
        public CacheKeyStorageJsonString(string initialJson)
            : base(initialJson)
        { }
    }

    public class CacheKeyStorageJsonFile : PersistentDictionaryStoreJsonFile<string, string>
    {
        public CacheKeyStorageJsonFile(string initialJson)
            : base(initialJson)
        { }
    }

    public class PersistentDictionaryStoreJsonStringSaving : EventArgs
    {
        public string Json { get; }

        public PersistentDictionaryStoreJsonStringSaving(string json)
        {
            Json = json;
        }
    }

    public class PersistentDictionaryStoreJsonString<TKey, TValue> : IPersistentDictionaryStore<TKey, TValue>
    {
        public IDictionary<TKey, TValue> Dictionary { get; }

        public event EventHandler<PersistentDictionaryStoreJsonStringSaving> Saving;

        public PersistentDictionaryStoreJsonString(string initialJson)
        {
            Dictionary = string.IsNullOrEmpty(initialJson)
                ? new Dictionary<TKey, TValue>()
                : JsonConvert.DeserializeObject<IDictionary<TKey, TValue>>(initialJson);
        }

        public void Save()
        {
            Saving?.Invoke(this, new PersistentDictionaryStoreJsonStringSaving(JsonConvert.SerializeObject(Dictionary)));
        }
    }

    public class PersistentDictionaryStoreJsonFile<TKey, TValue> : IPersistentDictionaryStore<TKey, TValue>
    {
        private string _filename;
        private JsonSerializer _serializer = new JsonSerializer();

        public IDictionary<TKey, TValue> Dictionary { get; }

        public PersistentDictionaryStoreJsonFile(string filename)
        {
            _filename = filename;

            if (File.Exists(filename))
            {
                using var file = File.OpenText(filename);
                Dictionary = (IDictionary<TKey, TValue>)_serializer.Deserialize(file, typeof(IDictionary<TKey, TValue>));
            }
            else
            {
                Dictionary = new Dictionary<TKey, TValue>();
            }
        }

        public void Save()
        {
            using var file = File.CreateText(_filename);
            _serializer.Serialize(file, Dictionary);
        }
    }
}
