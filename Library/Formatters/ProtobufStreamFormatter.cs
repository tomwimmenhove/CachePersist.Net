/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.IO;

namespace CachePersist.Net.Formatters
{
    public class ProtobufStreamFormatter<T> : IStreamFormatter
    {
        public void Serialize(Stream stream, object value) => ProtoBuf.Serializer.Serialize(stream, value);
        public object Deserialize(Stream stream) => ProtoBuf.Serializer.Deserialize<T>(stream);
    }
}
