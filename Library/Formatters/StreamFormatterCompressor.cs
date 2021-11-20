/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.IO;
using System.IO.Compression;

namespace CachePersist.Net.Formatters
{
    public class  StreamFormatterCompressor : IStreamFormatter
    {
        private IStreamFormatter _formatter;
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;

        public StreamFormatterCompressor(IStreamFormatter formatter, CompressionLevel compressionLevel)
        {
            _formatter = formatter;
            _compressionLevel = compressionLevel;
        }

        public void Serialize(Stream stream, object value)
        {
            using (var compressor = new GZipStream(stream, _compressionLevel))
            {
                _formatter.Serialize(compressor, value);
            }
        }

        public object Deserialize(Stream stream)
        {
            using (var compressor = new GZipStream(stream, CompressionMode.Decompress))
            {
                return _formatter.Deserialize(compressor);
            }
        }
    }
}
