/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;
using System.IO;
using System.Collections.Generic;
using CachePersist.Net.Formatters;

namespace CachePersist.Net.Caching
{
    public class Cache
    {
        private CacheKeyDictionary _keyDictionary;

        public IStreamFormatter DefaultFormatter { get; set; } = new BinaryStreamFormatter();
        public ICollection<string> Keys => _keyDictionary.Keys;
        public int Count => _keyDictionary.Count;

        public Cache(ICacheKeyStorage store)
        {
            _keyDictionary = new CacheKeyDictionary(store);
        }

        public bool ContainsKey(string key) =>
            _keyDictionary.TryGetValue(key, out var filename) && File.Exists(filename);

        public FileInfo GetCacheInfo(string key) => new FileInfo(_keyDictionary[key]);

        public T Get<T>(string key)
        {
            var filename = _keyDictionary[key];

            return AnyFormatter.Deserialize<T>(filename);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (_keyDictionary.TryGetValue(key, out var filename) && File.Exists(filename))
            {
                value = default;

                try
                {
                    value = AnyFormatter.Deserialize<T>(filename);
                }
                catch (NotSupportedException)
                {
                    return false;
                }
                catch (InvalidCastException)
                {
                    return false;
                }

                return true;
            }

            value = default;

            return false;
        }

        public void Set(string key, object value, IStreamFormatter formatter)
        {
            /* Remove the key if it already exists. Don't keep the file to avoid issues with
             * Windows' "File System Tunneling" */
            if (_keyDictionary.TryGetValue(key, out var filename) && File.Exists(filename))
            {
                File.Delete(filename);
            }

            filename = Path.GetTempFileName();

            AnyFormatter.Serialize(filename, value, formatter);

            _keyDictionary[key] = filename;
        }

        public void Set(string key, object value) => Set(key, value, DefaultFormatter);

        public object this[string key]
        {
            set => Set(key, value);
        }

        public bool Remove(string key)
        {
            if (_keyDictionary.TryGetValue(key, out var filename) && File.Exists(filename))
            {
                File.Delete(filename);
                _keyDictionary.Remove(key);

                return true;
            }

            return false;
        }

        public int RemoveBefore(DateTime time)
        {
            var n = 0;
            foreach (var key in Keys)
            {
                if (_keyDictionary.TryGetValue(key, out var filename) &&
                    File.Exists(filename) &&
                    new FileInfo(filename).CreationTimeUtc < time.ToUniversalTime())
                {
                    File.Delete(filename);
                    _keyDictionary.Remove(key);
                    n++;
                }
            }

            return n;
        }

        public int RemoveOlderThan(TimeSpan age) => RemoveBefore(DateTime.Now - age);

        public void Clear()
        {
            foreach (var key in Keys)
            {
                Remove(key);
            }
        }

        public int Cleanup()
        {
            var n = 0;
            foreach (var key in Keys)
            {
                if (_keyDictionary.TryGetValue(key, out var filename) &&
                    !File.Exists(filename))
                {
                    _keyDictionary.Remove(key);

                    n++;
                }
            }

            return n;
        }
    }
}
