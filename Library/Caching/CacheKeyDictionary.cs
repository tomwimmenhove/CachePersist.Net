/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using CachePersist.Net.Persistence;

namespace CachePersist.Net.Caching
{
    public class CacheKeyDictionary : PersistentDictionary<string, string>
    {
        public CacheKeyDictionary(ICacheKeyStorage store)
            : base(store)
        { }
    }
}
