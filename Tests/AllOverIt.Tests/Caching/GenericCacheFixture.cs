using System;
using System.Collections.Generic;
using AllOverIt.Caching;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Caching
{
    public class GenericCacheFixture : FixtureBase
    {
        private class KeyType1 : GenericCacheKey<int, string>
        {
            public KeyType1(int val1, string val2)
                : base(val1, val2)
            {
            }
        }

        private class KeyType2 : GenericCacheKey<bool, int?, string>
        {
            public KeyType2(bool val1, int? val2, string val3)
                : base(val1, val2, val3)
            {
            }
        }

        private const int PerKeyCount = 100;

        // Not using GenericCache.Default - to simplify avoiding tests overrunning each other
        private readonly GenericCache _cache;

        protected GenericCacheFixture()
        {
            _cache = new GenericCache();
            PopulateCache(_cache);
        }

        public class Count : GenericCacheFixture
        {
            [Fact]
            public void Should_Return_Expected_Count()
            {
                _cache.Count.Should().Be(PerKeyCount * 2);
            }
        }

        public class Keys : GenericCacheFixture
        {
            [Fact]
            public void Should_Have_Expected_Keys()
            {
                var keyType1 = _cache.Keys.Where(item => item.GetType() == typeof(KeyType1)).OrderBy(item => item.Key).AsReadOnlyCollection();
                var keyType2 = _cache.Keys.Where(item => item.GetType() == typeof(KeyType2)).OrderBy(item => Math.Abs((int)((KeyType2) item).Key2)).AsReadOnlyCollection();

                for (var i = 1; i <= PerKeyCount; i++)
                {
                    var actual1 = keyType1.ElementAt(i - 1);
                    actual1.Key.GetType().Should().Be(typeof((int, string)));
                    actual1.Key.Should().BeEquivalentTo((i, $"{i}"));

                    var actual1Key = (KeyType1) actual1;
                    actual1Key.Key1.Should().Be(i);
                    actual1Key.Key2.Should().Be($"{i}");

                    var actual2 = keyType2.ElementAt(i - 1);
                    actual2.Key.GetType().Should().Be(typeof((bool, int?, string)));

                    var isEven = i % 2 == 0;

                    actual2.Key.Should().BeEquivalentTo((isEven, isEven ? i : -i, isEven ? $"{i}" : default));

                    var actual2Key = (KeyType2) actual2;
                    actual2Key.Key1.Should().Be(isEven);
                    actual2Key.Key2.Should().Be(isEven ? i : -i);
                    actual2Key.Key3.Should().Be(isEven ? $"{i}" : default);
                }
            }
        }

        public class Values : GenericCacheFixture
        {
            [Fact]
            public void Should_Have_Expected_Values()
            {
                // Get values based on sorted keys
                var keyType1 = _cache.Keys.Where(item => item.GetType() == typeof(KeyType1)).OrderBy(item => item.Key).AsReadOnlyCollection();
                var keyType2 = _cache.Keys.Where(item => item.GetType() == typeof(KeyType2)).OrderBy(item => Math.Abs((int) ((KeyType2) item).Key2)).AsReadOnlyCollection();

                for (var i = 1; i <= PerKeyCount; i++)
                {
                    var actual1 = keyType1.ElementAt(i - 1);
                    _cache[actual1].Should().Be(i * i);

                    var actual2 = keyType2.ElementAt(i - 1);
                    _cache[actual2].Should().Be(i * i * i);
                }
            }
        }

        public class Items : GenericCacheFixture
        {
            [Fact]
            public void Should_Get_Item_By_Index_Operator()
            {
                var index = GetWithinRange(1, 100);

                var key1 = new KeyType1(index, $"{index}");
                _cache[key1].Should().Be(index * index);

                var isEven = index % 2 == 0;
                var key2 = new KeyType2(isEven, isEven ? index : -index, isEven ? $"{index}" : default);
                _cache[key2].Should().Be(index * index * index);
            }
        }

        public class GetEnumerator : GenericCacheFixture
        {
            [Fact]
            public void Should_Enumerate_All_Items()
            {
                var count = 0;

                foreach (var (key, value) in _cache)
                {
                    count++;
                }

                count.Should().Be(PerKeyCount * 2);
            }
        }

        public class ContainsKey : GenericCacheFixture
        {
            [Fact]
            public void Should_Find_Key()
            {
                var index = GetWithinRange(1, 99);

                // test odd/even
                for (var idx = index; idx <= index + 1; idx++)
                {
                    var key1 = new KeyType1(idx, $"{idx}");
                    _cache.ContainsKey(key1).Should().BeTrue();

                    var isEven = idx % 2 == 0;
                    var key2 = new KeyType2(isEven, isEven ? idx : -idx, isEven ? $"{idx}" : default);
                    _cache.ContainsKey(key2).Should().BeTrue();
                }
            }

            [Fact]
            public void Should_Not_Find_Key()
            {
                var index = GetWithinRange(1, 99);

                // test odd/even
                for (var idx = index; idx <= index + 1; idx++)
                {
                    var key1 = new KeyType1(idx, $"{-idx}");
                    _cache.ContainsKey(key1).Should().BeFalse();

                    var key2 = new KeyType2(idx % 2 == 0, idx * 2, $"{idx}");
                    _cache.ContainsKey(key2).Should().BeFalse();
                }
            }
        }

        public class Clear : GenericCacheFixture
        {
            [Fact]
            public void Should_Clear_Cache()
            {
                _cache.Count.Should().Be(PerKeyCount * 2);

                _cache.Clear();

                _cache.Count.Should().Be(0);
            }
        }

        public class Add_TValue : GenericCacheFixture
        {

        }





        private static void PopulateCache(IGenericCache cache)
        {
            for (var i = 1; i <= PerKeyCount; i++)
            {
                var key1 = new KeyType1(i, $"{i}");
                cache.Add(key1, i * i);

                var isEven = i % 2 == 0;
                var key2 = new KeyType2(isEven, isEven ? i : -i, isEven ? $"{i}" : default);
                cache.Add(key2, i * i * i);
            }
        }

        private static IReadOnlyCollection<GenericCacheKeyBase> GetAllKeyType1(IGenericCache cache)
        {
            return cache.Keys.Where(item => item.GetType() == typeof(KeyType1)).OrderBy(item => item.Key).AsReadOnlyCollection();
        }
    }
}