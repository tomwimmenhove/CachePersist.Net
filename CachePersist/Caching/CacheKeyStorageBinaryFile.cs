using System.IO;
using System.Collections.Generic;

namespace CachePersist.Net.Caching
{
    public class CacheKeyStorageBinaryFile : ICacheKeyStorage
    {
        private readonly string _filename;

        public IDictionary<string, string> Dictionary { get; }

        public CacheKeyStorageBinaryFile(string filename)
        {
            _filename = filename;

            var fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                Dictionary = Deserialize(filename);

                return;
            }

            Dictionary = new Dictionary<string, string>();
        }

        public void Save()
        {
            Serialize(_filename, Dictionary);
        }

        private static IDictionary<string, string> Deserialize(string filename)
        {
            var dictionary = new Dictionary<string, string>();

            using (var stream = File.OpenRead(filename))
            using (var reader = new BinaryReader(stream))
            {
                while (stream.Position != stream.Length)
                {
                    var key = reader.ReadString();
                    var value = reader.ReadString();
                    dictionary[key] = value;
                }
            }

            return dictionary;
        }

        private static void Serialize(string filename, IDictionary<string, string> dictionary)
        {
            using (var stream = File.Create(filename))
            using (var writer = new BinaryWriter(stream))
            {
                foreach (var kvp in dictionary)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
        }
    }
}
