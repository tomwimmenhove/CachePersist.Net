/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
                : JsonConvert.DeserializeObject<IDictionary<TKey, TValue>>(initialJson);
        }

        public void Save()
        {
            Saving?.Invoke(this, new JsonStringSavingEventArgs(JsonConvert.SerializeObject(Dictionary)));
        }
    }
}
