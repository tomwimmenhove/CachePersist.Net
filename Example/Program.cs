/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;
using System.IO;
using CachePersist.Net.Persistence;
using CachePersist.Net.Formatters;
using CachePersist.Net.Caching;

namespace CachePersistExmaple
{
    class Program
    {
        static void PersistentDictionaryExample()
        {
            var tempFilePath = Path.GetTempFileName();

            /* Open the dictionary and write something */
            {
                var store = new DictionaryStoreJsonFile<int, string>(tempFilePath);
                var dict = new PersistentDictionary<int, string>(store);

                dict[42] = "Hello world!";
            }

            /* Open the dictionary and read something back */
            {
                var store = new DictionaryStoreJsonFile<int, string>(tempFilePath);
                var dict = new PersistentDictionary<int, string>(store);

                var s = dict[42];
                Console.WriteLine($"String from dictionary: {s}");

                if (dict.TryGetValue(43, out var s2))
                {
                    Console.WriteLine($"This shouldn't be here: {s2}");
                }
            }

            File.Delete(tempFilePath);
        }

        private static void CacheExample()
        {
            var cacheFilePath = Path.Combine(Path.GetTempPath(), "CachePersist.Net.CacheExample.json");
            var store = new CacheKeyStorageJsonFile(cacheFilePath);
            var cache = new Cache(store);

            /* Check if our values have been cached already */
            if (cache.TryGetValue<TestData[]>("nonsense", out var cachedNonsense))
            {
                Console.WriteLine($"Found {cachedNonsense.Length} entries of TestData in the cache!");

                /* We can also get the entries that are added with a non-default formatter, since the type of
                 * the formatter that was used is also stored with the data */
                var moreNonsense = cache.Get<TestData[]>("moreNonsense");
                var compressedNonsense = cache.Get<TestData[]>("compressedNonsense");

                /* Clear all the data from the cache */
                cache.Clear();

                /* And delete the json store */
                File.Delete(cacheFilePath);

                return;
            }

            Console.WriteLine("Looks like there's no data cached. Let's generate some nonsense data and cache it");

            /* Generate some test data */
            var nonsense = TestData.GenerateSomeData(10000);

            /* We can simply cache the result directly */
            cache["nonsense"] = nonsense;

            /* Or we could use a non-default formatter that uses another serializer to store the data */
            cache.Set("moreNonsense", nonsense, new ProtobufStreamFormatter<TestData[]>());

            /* Or */
            cache.Set("compressedNonsense", nonsense, new BinaryCompressedStreamFormatter());
        }

        private static void DictionaryStoreJsonStringExample()
        {
            /* This is where we will store the JSON in. In a real-world application you would use
             * something like the System.Configuration.ApplicationSettingsBase to store this string */
            string jsonString = "";

            var store = new DictionaryStoreJsonString<int, string>(jsonString);
            var dict = new PersistentDictionary<int, string>(store);

            /* When a value is added to the dictionary, the store will fire the Saving event, which
             * will contain the new value of the JSON string in its EventArgs */
            store.Saving += (o, e) => {
                jsonString = e.Json;
                Console.WriteLine($"Our JSON string is now: {jsonString}");
            };

            Console.WriteLine("Adding an entry to the dictionary.");
            dict[42] = "Hello world!";

            var s = dict[42];
            Console.WriteLine($"String from dictionary: {s}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("PersistentDictionary example:");  
            PersistentDictionaryExample();

            Console.WriteLine("");
            Console.WriteLine("Run cache example for the first time:");
            CacheExample();
            Console.WriteLine("");
            Console.WriteLine("Run cache example for the second time:");
            CacheExample();
            
            Console.WriteLine("");
            Console.WriteLine("DictionaryStoreJsonString example:");
            DictionaryStoreJsonStringExample();
        }
    }
}
