/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System.Runtime.Serialization.Formatters.Binary;

namespace CachePersist.Net.Formatters
{
    public class BinaryStreamFormatter : FormatterWrapper
    {
        public BinaryStreamFormatter() : base(new BinaryFormatter()) { }
    }
}
