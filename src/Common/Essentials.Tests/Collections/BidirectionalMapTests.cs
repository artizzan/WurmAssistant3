using System.Collections.Generic;
using AldursLab.Essentials.Collections;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Collections
{
    public class BidirectionalMapTests : AssertionHelper
    {
        BidirectionalMap<int, string> map;

        [SetUp]
        public void Setup()
        {
            map = new BidirectionalMap<int, string>();
        }

        [Test]
        public void AddsAndGets()
        {
            map.Add(1, "1");
            map.Add(2, "2");

            AssertGets(1, "1");
            AssertGets(2, "2");
        }

        [Test]
        public void Removes()
        {
            map.Add(1, "1");
            map.Add(2, "2");
            map.Add(3, "3");

            map.Remove(1);
            map.Remove("3");

            AssertNotGets(1, "1");
            AssertGets(2, "2");
            AssertNotGets(3, "3");
        }

        void AssertGets(int key1, string key2)
        {
            Expect(map.GetByKey1(key1), EqualTo(key2));
            Expect(map.GetByKey2(key2), EqualTo(key1));

            int outkey1;
            string outkey2;
            var outkey1Result = map.TryGetByKey1(key1, out outkey2);
            var outkey2Result = map.TryGetByKey2(key2, out outkey1);

            Expect(outkey1, EqualTo(key1));
            Expect(outkey2, EqualTo(key2));
            Expect(outkey1Result, True);
            Expect(outkey2Result, True);
        }

        void AssertNotGets(int key1, string key2)
        {
            Assert.Throws<KeyNotFoundException>(() => map.GetByKey1(key1));
            Assert.Throws<KeyNotFoundException>(() => map.GetByKey2(key2));

            int outkey1;
            string outkey2;
            var outkey1Result = map.TryGetByKey1(key1, out outkey2);
            var outkey2Result = map.TryGetByKey2(key2, out outkey1);

            Expect(outkey1, EqualTo(default(int)));
            Expect(outkey2, Null);
            Expect(outkey1Result, False);
            Expect(outkey2Result, False);
        }
    }
}
