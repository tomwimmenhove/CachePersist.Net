/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;

namespace CachePersist.Net.Persistence
{
    public class DictionaryStoreJsonString<TKey, TValue> : IDictionaryStore<TKey, TValue>
    {
        public IDictionary<TKey, TValue> Dictionary { get; }

        public event EventHandler<JsonStringSavingEventArgs> Saving;

        public DictionaryStoreJsonString(string initialJson)
        {
            Dictionary = string.IsNullOrEmpty(initialJson)
                ? new Dictionary<TKey, TValue>()
                : Deserialize(initialJson);
        }

        public void Save()
        {
            var json = Serialize(Dictionary);
            Saving?.Invoke(this, new JsonStringSavingEventArgs(json));
        }

        private static string Serialize(IDictionary<TKey, TValue> item)
        {
            var serializer = new DataContractJsonSerializer(typeof(Dictionary<TKey, TValue>));
            using var stream = new MemoryStream();
            serializer.WriteObject(stream, item);
            return Encoding.Default.GetString(stream.ToArray());
        }

        private static IDictionary<TKey, TValue> Deserialize(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(Dictionary<TKey, TValue>));
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;

            return (IDictionary<TKey, TValue>)serializer.ReadObject(stream);
        }
    }
}
