/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.IO.Compression;

namespace CachePersist.Net.Formatters
{
    public class ProtobufCompressedStreamFormatter<T> : StreamFormatterCompressor
    {
        public ProtobufCompressedStreamFormatter()
            : base(new ProtobufStreamFormatter<T>(), CompressionLevel.Optimal)
        { }

        public ProtobufCompressedStreamFormatter(CompressionLevel compressionLevel)
            : base(new ProtobufStreamFormatter<T>(), compressionLevel)
        { }
    }
}
