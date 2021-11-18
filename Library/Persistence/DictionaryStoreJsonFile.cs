/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace CachePersist.Net.Persistence
{
    public class DictionaryStoreJsonFile<TKey, TValue> : IDictionaryStore<TKey, TValue>
    {
        private string _filename;

        public IDictionary<TKey, TValue> Dictionary { get; }

        public DictionaryStoreJsonFile(string filename)
        {
            _filename = filename;

            var fileInfo = new FileInfo(filename);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                var json = File.ReadAllText(filename);
                Dictionary = JsonSerializer.Deserialize<IDictionary<TKey, TValue>>(json);

                return;
            }

            Dictionary = new Dictionary<TKey, TValue>();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(Dictionary);
            File.WriteAllText(_filename, json);
        }
    }
}
