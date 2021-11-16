using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CachePersist.Net.Persistence
{
    public class DictionaryStoreJsonFile<TKey, TValue> : IDictionaryStore<TKey, TValue>
    {
        private string _filename;
        private JsonSerializer _serializer = new JsonSerializer();

        public IDictionary<TKey, TValue> Dictionary { get; }

        public DictionaryStoreJsonFile(string filename)
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
