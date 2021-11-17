/* This file is part of CachePersist.Net
 *
 * Copyright (c) 2021 Tom Wimmenhove. All rights reserved.
 * Licensed under the MIT license. See LICENSE file in the project root for details.
 */

using System;
using ProtoBuf;

namespace CachePersistExmaple
{
    [Serializable, ProtoContract]
    public class TestData
    {
        [Serializable, ProtoContract]
        public enum eCount : short
        {
            One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten
        }

        [ProtoMember(1)]
        public eCount SomeEnum { get; set; }

        [ProtoMember(2)]
        public eCount AnotherEnum { get; set; }

        [ProtoMember(3)]
        public int Something { get; set; }

        public static TestData[] GenerateSomeData(int n)
        {
            var rnd = new Random();
            var arr = new TestData[n];
            for (var i = 0; i < n; i++)
            {
                var entry = new TestData()
                {
                    SomeEnum = (TestData.eCount)rnd.Next(),
                    AnotherEnum = (TestData.eCount)rnd.Next(),
                    Something = rnd.Next()
                };

                arr[i] = entry;
            }

            return arr;
        }
    }

}
