/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.Collections.Generic;

namespace CachePersist.Net.Persistence
{
    public interface IDictionaryStore<TKey, TValue>
    {
        IDictionary<TKey, TValue> Dictionary { get; }

        void Save();
    }
}
