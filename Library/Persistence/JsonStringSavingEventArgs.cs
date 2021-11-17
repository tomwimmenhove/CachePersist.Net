/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;

namespace CachePersist.Net.Persistence
{
    public class JsonStringSavingEventArgs : EventArgs
    {
        public string Json { get; }

        public JsonStringSavingEventArgs(string json)
        {
            Json = json;
        }
    }
}
