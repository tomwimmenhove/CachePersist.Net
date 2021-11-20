/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.IO.Compression;

namespace CachePersist.Net.Formatters
{
    public class BinaryCompressedStreamFormatter : StreamFormatterCompressor
    {
        public BinaryCompressedStreamFormatter()
            : base(new BinaryStreamFormatter(), CompressionLevel.Optimal)
        { }

        public BinaryCompressedStreamFormatter(CompressionLevel compressionLevel)
            : base(new BinaryStreamFormatter(), compressionLevel)
        { }
    }
}
