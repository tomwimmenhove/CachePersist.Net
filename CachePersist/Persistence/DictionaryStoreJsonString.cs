/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;
using System.Collections.Generic;
using System.Text.Json;

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
                : JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(initialJson);
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(Dictionary);
            Saving?.Invoke(this, new JsonStringSavingEventArgs(json));
        }
    }
}
