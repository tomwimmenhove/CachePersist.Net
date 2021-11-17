/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

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
                if (Dictionary != null)
                {
                    return;
                }
            }

            Dictionary = new Dictionary<TKey, TValue>();
        }

        public void Save()
        {
            using var file = File.CreateText(_filename);
            _serializer.Serialize(file, Dictionary);
        }
    }
}
