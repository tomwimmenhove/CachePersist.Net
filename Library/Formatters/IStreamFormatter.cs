/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.IO;

namespace CachePersist.Net.Formatters
{
    public interface IStreamFormatter
    {
        void Serialize(Stream stream, object value);
        object Deserialize(Stream stream);
    }
}
