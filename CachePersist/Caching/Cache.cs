/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;
using System.IO;
using System.Collections.Generic;
using CachePersist.Net.Formatters;
using System.Linq;

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

            Cleanup();
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
            if (_keyDictionary.TryGetValue(key, out var filename))
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                _keyDictionary.Remove(key);
                
                return true;
            }

            return false;
        }

        public int RemoveBefore(DateTime time)
        {
            var removeKeys = new List<string>();
            foreach(var kvp in _keyDictionary)
            {
                if (File.Exists(kvp.Value))
                {
                    var fileInfo = new FileInfo(kvp.Value);
                    if (fileInfo.CreationTimeUtc < time.ToUniversalTime())
                    {
                        File.Delete(kvp.Value);
                        removeKeys.Add(kvp.Key);
                    }
                }
                else
                {
                    removeKeys.Add(kvp.Key);
                }
            }

            foreach(var key in removeKeys)
            {
                _keyDictionary.Remove(key);
            }

            return removeKeys.Count;
        }

        public int RemoveOlderThan(TimeSpan age) => RemoveBefore(DateTime.Now - age);

        public void Clear()
        {
            foreach (var filename in _keyDictionary.Values.Where(File.Exists))
            {
                File.Delete(filename);
            }

            _keyDictionary.Clear();
        }

        public int Cleanup()
        {
            var removeKeys = new List<string>();
            foreach(var kvp in _keyDictionary.Where(x => !File.Exists(x.Value)))
            {
                removeKeys.Add(kvp.Key);
            }

            foreach(var key in removeKeys)
            {
                _keyDictionary.Remove(key);
            }

            return removeKeys.Count;
        }
    }
}
