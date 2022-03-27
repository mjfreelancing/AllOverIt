using System;
using System.Collections;
using System.Collections.Generic;
using AllOverIt.Caching;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Linq;
using AllOverIt.Fixture.Extensions;
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
                var keyType1 = GetAllKeyType1(_cache);
                var keyType2 = GetAllKeyType2(_cache);

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
                var keyType1 = GetAllKeyType1(_cache);
                var keyType2 = GetAllKeyType2(_cache);

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
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.ContainsKey(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

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
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _cache.Add(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Add_Key_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache.Add(key, value);

                _ = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out var actual);

                value.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Throw_When_Key_Exists()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _cache.Add(key, CreateMany<string>());

                Invoking(() =>
                    {
                        _cache.Add(key, CreateMany<string>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("The key already existed in the dictionary.");
            }
        }

        public class TryAdd_TValue : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryAdd(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Add_Key_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                var success = _cache.TryAdd(key, value);

                success.Should().BeTrue();

                _ = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out var actual);

                value.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Fail_To_Add_When_Key_Exists()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _cache.Add(key, CreateMany<string>());

                var success = _cache.TryAdd(key, CreateMany<string>());

                success.Should().BeFalse();
            }
        }

        public class TryGetValue_Object : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryGetValue(null, out var _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Get_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                var success = _cache.TryGetValue(key, out var actual);

                success.Should().BeTrue();

                value.Should().BeEquivalentTo((IReadOnlyCollection<string>)actual);
            }

            [Fact]
            public void Should_Fail_To_Get_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var success = _cache.TryGetValue(key, out var _);

                success.Should().BeFalse();
            }
        }

        public class TryGetValue_TValue : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryGetValue<string>(null, out var _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Get_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                var success = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out var actual);

                success.Should().BeTrue();

                value.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Fail_To_Get_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var success = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out var _);

                success.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Default_Value_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _ = _cache.TryGetValue<double>(key, out var actual1);
                actual1.Should().Be(default);

                _ = _cache.TryGetValue<string>(key, out var actual2);
                actual2.Should().Be(default);
            }
        }

        public class Remove : GenericCacheFixture
        {
            [Fact]
            public void Should_Remove_From_Cache()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                // Confirm the key is in the cache
                value.Should().BeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);

                var success = _cache.Remove(new KeyValuePair<GenericCacheKeyBase, object>(key, value));

                success.Should().BeTrue();

                _cache.TryGetValue(key, out var _).Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Remove_From_Cache_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var actual = _cache.Remove(new KeyValuePair<GenericCacheKeyBase, object>(key, Create<string>()));

                actual.Should().BeFalse();
            }
        }

        public class TryRemove_TValue : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryRemove<string>(null, out var _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Remove_From_Cache()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                // Confirm the key is in the cache
                value.Should().BeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);

                var success = _cache.TryRemove<IReadOnlyCollection<string>>(key, out var actual);

                success.Should().BeTrue();

                value.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Not_Remove_From_Cache_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var actual = _cache.TryRemove<string>(key, out var _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Default_Value_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _ = _cache.TryRemove<double>(key, out var actual1);
                actual1.Should().Be(default);

                _ = _cache.TryRemove<string>(key, out var actual2);
                actual2.Should().Be(default);
            }
        }

#if NET5_0_OR_GREATER
        public class TryRemove_TValue_KeyValuePair : GenericCacheFixture
        {
            [Fact]
            public void Should_Remove_From_Cache()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                // Confirm the key is in the cache
                value.Should().BeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);

                var success = _cache.TryRemove(new KeyValuePair<GenericCacheKeyBase, IReadOnlyCollection<string>>(key, value));

                success.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Remove_From_Cache_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                var actual = _cache.TryRemove(new KeyValuePair<GenericCacheKeyBase, IReadOnlyCollection<string>>(key, value));

                actual.Should().BeFalse();
            }
        }
#endif

        public class TryUpdate_TValue : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryUpdate<string>(null, Create<string>(), Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Not_Update_When_Comparison_Is_Different()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var originalValue = CreateMany<string>();
                var newValue = CreateMany<string>();

                _cache[key] = originalValue;

                var success = _cache.TryUpdate(key, newValue, CreateMany<string>());

                success.Should().BeFalse();

                originalValue.Should().BeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);
            }

            [Fact]
            public void Should_Update_When_Comparison_Is_Equal()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var originalValue = CreateMany<string>();
                var newValue = CreateMany<string>();

                _cache[key] = originalValue;

                var success = _cache.TryUpdate(key, newValue, originalValue);

                success.Should().BeTrue();

                newValue.Should().BeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);
            }

            [Fact]
            public void Should_Not_Update_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _cache.ContainsKey(key).Should().BeFalse();

                var success = _cache.TryUpdate(key, Create<string>(), Create<string>());

                success.Should().BeFalse();
            }
        }

        public class ToArray : GenericCacheFixture
        {
            [Fact]
            public void Should_Copy_To_Array()
            {
                var actual = _cache.ToArray();

                actual.Should().HaveCount(PerKeyCount * 2);

                var keys1 = GetAllKeyType1(_cache);
                keys1.Should().BeEquivalentTo(GetSortedKeyType1(actual.Select(item => item.Key)));

                foreach (var key1 in keys1)
                {
                    _cache[key1].Should().Be(actual.Single(item => item.Key == key1).Value);
                }

                var keys2 = GetAllKeyType2(_cache);
                keys2.Should().BeEquivalentTo(GetSortedKeyType2(actual.Select(item => item.Key)));

                foreach (var key2 in keys2)
                {
                    _cache[key2].Should().Be(actual.Single(item => item.Key == key2).Value);
                }
            }
        }

        public class GetOrAdd_Resolver : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.GetOrAdd(null, key => Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                    {
                        var key = new KeyType1(Create<int>(), Create<string>());
                        _ = _cache.GetOrAdd(key, (Func<GenericCacheKeyBase, string>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("addResolver");
            }

            [Fact]
            public void Should_Provide_Key_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actual = null;

                _ = _cache.GetOrAdd(key, k =>
                {
                    actual = k;
                    return Create<string>();
                });

                key.Should().Be(actual);
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).Should().BeFalse();

                var actual = _cache.GetOrAdd(key, _ => expected);

                expected.Should().Be(actual);
            }

            [Fact]
            public void Should_Get_Current_Value_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache[key] = expected;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.GetOrAdd(key, _ => Create<double>());

                expected.Should().Be(actual);
            }
        }

        public class GetOrAdd_Value : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.GetOrAdd(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).Should().BeFalse();

                var actual = _cache.GetOrAdd(key, expected);

                expected.Should().Be(actual);
            }

            [Fact]
            public void Should_Get_Current_Value_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache[key] = expected;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.GetOrAdd(key, Create<double>());

                expected.Should().Be(actual);
            }
        }





        public class GetOrAdd_Arg_Resolver : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.GetOrAdd(null, (_, _) => Create<string>(), Create<double>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                    {
                        var key = new KeyType1(Create<int>(), Create<string>());
                        _ = _cache.GetOrAdd(key, (Func<GenericCacheKeyBase, double, string>) null, Create<double>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("addResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).Should().BeFalse();

                var actual = _cache.GetOrAdd(key, (addKey, arg) => expected, Create<int>());

                expected.Should().Be(actual);
            }

            [Fact]
            public void Should_Provide_AddKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualAddKey = null;

                _cache.ContainsKey(key).Should().BeFalse();

                _ = _cache.GetOrAdd(key, (addKey, arg) =>
                {
                    actualAddKey = addKey;
                    return Create<double>();
                }, Create<int>());

                key.Should().BeSameAs(actualAddKey);
            }

            [Fact]
            public void Should_Get_Current_Value_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache[key] = expected;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.GetOrAdd(key, (addKey, arg) => Create<double>(), Create<double>());

                expected.Should().Be(actual);
            }

            [Fact]
            public void Should_Provider_Arg_When_Adding()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var argValue = Create<int>();
                var actualArgValue = 0;

                _cache.ContainsKey(key).Should().BeFalse();

                _ = _cache.GetOrAdd(key, (addKey, arg) =>
                {
                    actualArgValue = arg;
                    return Create<double>();
                }, argValue);

                argValue.Should().Be(actualArgValue);
            }
        }

        public class AddOrUpdate_AddResolver_UpdateResolver : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.AddOrUpdate(null, _ => Create<string>(), (_, _) => Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_AddResolver_Null()
            {
                Invoking(() =>
                    {
                        var key = new KeyType1(Create<int>(), Create<string>());
                        _ = _cache.AddOrUpdate(key, (Func<GenericCacheKeyBase, string>) null, (_, _) => Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("addResolver");
            }

            [Fact]
            public void Should_Throw_When_UpdateResolver_Null()
            {
                Invoking(() =>
                    {
                        var key = new KeyType1(Create<int>(), Create<string>());
                        _ = _cache.AddOrUpdate(key, _ => Create<string>(), (Func<GenericCacheKeyBase, string, string>) null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("updateResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).Should().BeFalse();

                var actual = _cache.AddOrUpdate(key, addKey => expected, (updateKey, value) => Create<double>());

                expected.Should().Be(actual);
            }

            [Fact]
            public void Should_Provide_AddKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualAddKey = null; 

                _cache.ContainsKey(key).Should().BeFalse();

                _ = _cache.AddOrUpdate(key, addKey =>
                    {
                        actualAddKey = addKey;
                        return Create<double>();
                    },
                    (updateKey, value) => Create<double>());

                key.Should().BeSameAs(actualAddKey);
            }

            [Fact]
            public void Should_Update_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var factor = GetWithinRange(2, 5);
                var expected = value * factor;

                _cache[key] = value;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.AddOrUpdate(key, addKey => value, (updateKey, currentValue) => currentValue * factor);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Provide_UpdateKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualUpdateKey = null;

                _cache[key] = Create<double>();

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.AddOrUpdate(key,
                    addKey => Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualUpdateKey = updateKey;
                        return Create<double>();
                    });

                key.Should().Be(actualUpdateKey);
            }

            [Fact]
            public void Should_Provide_Current_Value_When_Updating()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var actualCurrentValue = value;

                _cache[key] = value;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.AddOrUpdate(key,
                    addKey => Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualCurrentValue = currentValue;
                        return Create<double>();
                    });

                value.Should().Be(actualCurrentValue);
            }
        }

        public class AddOrUpdate_AddValue_Update_Resolver : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                {
                    _ = _cache.AddOrUpdate(null, Create<string>(), (_, _) => Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_UpdateResolver_Null()
            {
                Invoking(() =>
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    _ = _cache.AddOrUpdate(key, Create<string>(), (Func<GenericCacheKeyBase, string, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("updateResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).Should().BeFalse();

                var actual = _cache.AddOrUpdate(key, expected, (updateKey, value) => Create<double>());

                expected.Should().Be(actual);
            }

            [Fact]
            public void Should_Update_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var factor = GetWithinRange(2, 5);
                var expected = value * factor;

                _cache[key] = value;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.AddOrUpdate(key, value, (updateKey, currentValue) => currentValue * factor);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Provide_UpdateKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualUpdateKey = null;

                _cache[key] = Create<double>();

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.AddOrUpdate(key,
                    Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualUpdateKey = updateKey;
                        return Create<double>();
                    });

                key.Should().Be(actualUpdateKey);
            }

            [Fact]
            public void Should_Provide_Current_Value_When_Updating()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var actualCurrentValue = value;

                _cache[key] = value;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.AddOrUpdate(key,
                    Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualCurrentValue = currentValue;
                        return Create<double>();
                    });

                value.Should().Be(actualCurrentValue);
            }
        }

        public class AddOrUpdate_Arg_AddResolver_UpdateResolver : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.AddOrUpdate<int, string>(
                            null,
                            (addKey, addArg) => Create<string>(),
                            (updateKey, currentValue, updateArg) => Create<string>(),
                            Create<int>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_AddResolver_Null()
            {
                Invoking(() =>
                    {
                        var key = new KeyType1(Create<int>(), Create<string>());
                        _ = _cache.AddOrUpdate<int, string>(
                            key,
                            null,
                            (updateKey, currentValue, updateArg) => Create<string>(),
                            Create<int>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("addResolver");
            }

            [Fact]
            public void Should_Throw_When_UpdateResolver_Null()
            {
                Invoking(() =>
                    {
                        var key = new KeyType1(Create<int>(), Create<string>());
                        _ = _cache.AddOrUpdate<int, string>(
                            key,
                            (addKey, addArg) => Create<string>(),
                            null,
                            Create<int>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("updateResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<string>();

                _cache.ContainsKey(key).Should().BeFalse();

                var actual = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => expected,
                    (updateKey, currentValue, updateArg) => Create<string>(),
                    Create<int>());

                expected.Should().Be(actual);
            }

            [Fact]
            public void Should_Update_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var factor = GetWithinRange(2, 5);
                var expected = value * factor;

                _cache[key] = value;

                _cache.ContainsKey(key).Should().BeTrue();

                var actual = _cache.AddOrUpdate<int, double>(
                    key,
                    (addKey, addArg) => Create<double>(),
                    (updateKey, currentValue, updateArg) => currentValue * factor,
                    Create<int>());

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Provide_AddKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualKey = null;

                _cache.ContainsKey(key).Should().BeFalse();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) =>
                    {
                        actualKey = addKey;
                        return Create<string>();
                    },
                    (updateKey, currentValue, updateArg) => Create<string>(),
                    Create<int>());

                actualKey.Should().BeSameAs(key);
            }

            [Fact]
            public void Should_Provide_AddArg()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var arg = Create<int>();
                var actualArg = 0;

                _cache.ContainsKey(key).Should().BeFalse();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) =>
                    {
                        actualArg = addArg;
                        return Create<string>();
                    },
                    (updateKey, currentValue, updateArg) => Create<string>(),
                    arg);

                arg.Should().Be(actualArg);
            }

            [Fact]
            public void Should_Provide_UpdateKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<string>();
                GenericCacheKeyBase actualKey = null;

                _cache[key] = value;

                _cache.ContainsKey(key).Should().BeTrue();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => Create<string>(),
                    (updateKey, currentValue, updateArg) =>
                    {
                        actualKey = updateKey;
                        return Create<string>();
                    },
                    Create<int>());

                actualKey.Should().BeSameAs(key);
            }

            [Fact]
            public void Should_Provide_UpdateCurrentValue()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<string>();
                string actualCurrentValue = null;

                _cache[key] = value;

                _cache.ContainsKey(key).Should().BeTrue();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => Create<string>(),
                    (updateKey, currentValue, updateArg) =>
                    {
                        actualCurrentValue = currentValue;
                        return Create<string>();
                    },
                    Create<int>());

                value.Should().Be(actualCurrentValue);
            }

            [Fact]
            public void Should_Provide_UpdateArg()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var arg = Create<int>();
                var actualArg = 0;

                _cache[key] = Create<string>();

                _cache.ContainsKey(key).Should().BeTrue();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => Create<string>(),
                    (updateKey, currentValue, updateArg) =>
                    {
                        actualArg = updateArg;
                        return Create<string>();
                    },
                    arg);

                arg.Should().Be(actualArg);
            }
        }

        public class Explicit : GenericCacheFixture
        {
            public class IsReadOnly : Explicit
            {
                [Fact]
                public void Should_Be_False()
                {
                    ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>)_cache).IsReadOnly.Should().BeFalse();
                }
            }

            public class Keys : Explicit
            {
                [Fact]
                public void Should_Return_Keys()
                {
                    var expected = _cache.Keys;

                    expected.Should().BeEquivalentTo(((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).Keys);
                }
            }

            public class Values : Explicit
            {
                [Fact]
                public void Should_Return_Values()
                {
                    var expected = _cache.Values;

                    expected.Should().BeEquivalentTo(((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).Values);
                }
            }

            public class Add_KeyValuePair : Explicit
            {
                [Fact]
                public void Should_Add_Key_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();

                    _cache.ContainsKey(key).Should().BeFalse();

                    var kvp = new KeyValuePair<GenericCacheKeyBase, object>(key, value);

                    ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).Add(kvp);

                    _cache[key].Should().Be(value);
                }
            }

            public class Add_Key : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IDictionary<GenericCacheKeyBase, object>) _cache).Add(null, Create<string>());
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Add_Key_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();

                    _cache.ContainsKey(key).Should().BeFalse();

                    ((IDictionary<GenericCacheKeyBase, object>) _cache).Add(key, value);

                    _cache[key].Should().Be(value);
                }
            }

            public class Contains : Explicit
            {
                [Fact]
                public void Should_Not_Contain_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();
                    var kvp = new KeyValuePair<GenericCacheKeyBase, object>(key, value);

                    var actual = ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).Contains(kvp);

                    actual.Should().BeFalse();
                }

                [Fact]
                public void Should_Contain_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();
                    var kvp = new KeyValuePair<GenericCacheKeyBase, object>(key, value);

                    _cache.ContainsKey(key).Should().BeFalse();
                    _cache[key] = value;

                    var actual = ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).Contains(kvp);

                    actual.Should().BeTrue();
                }
            }

            public class CopyTo : Explicit
            {
                [Fact]
                public void Should_Copy_To_Array_At_Index()
                {
                    var array = new KeyValuePair<GenericCacheKeyBase, object>[PerKeyCount * 2 + 100];
                    var index = GetWithinRange(0, 99);

                    ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).CopyTo(array, index);

                    var actual = array.Skip(index).Take(PerKeyCount * 2);

                    _cache.ToArray().Should().BeEquivalentTo(actual);
                }
            }

            public class Remove : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(null);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Not_Remove_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).Should().BeFalse();

                    var actual = ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(key);

                    actual.Should().BeFalse();
                }

                [Fact]
                public void Should_Remove_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).Should().BeFalse();

                    _cache[key] = Create<string>();
                    _cache.ContainsKey(key).Should().BeTrue();

                    var actual = ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(key);

                    actual.Should().BeTrue();
                }
            }

            public class TryGetValue_Dictionary : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(null, out var _);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Not_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).Should().BeFalse();

                    var actual = ((IDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out var _);

                    actual.Should().BeFalse();
                }

                [Fact]
                public void Should_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<double>();

                    _cache.ContainsKey(key).Should().BeFalse();

                    _cache[key] = value;

                    var success = ((IDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out var actual);

                    success.Should().BeTrue();
                    value.Should().Be((double) actual);
                }
            }

            public class TryGetValue_ReadOnlyDictionary : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(null, out var _);
                        })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Not_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).Should().BeFalse();

                    var actual = ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out var _);

                    actual.Should().BeFalse();
                }

                [Fact]
                public void Should_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<double>();

                    _cache.ContainsKey(key).Should().BeFalse();

                    _cache[key] = value;

                    var success = ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out var actual);

                    success.Should().BeTrue();
                    value.Should().Be((double) actual);
                }
            }

            public class GetEnumerator : Explicit
            {
                [Fact]
                public void Should_Enumerate()
                {
                    var enumerator = ((IEnumerable) _cache).GetEnumerator();

                    enumerator.MoveNext().Should().BeTrue();

                    enumerator.Current.Should().BeEquivalentTo(_cache.Take(1).Single());
                }
            }
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
            return GetSortedKeyType1(cache.Keys);
        }

        private static IReadOnlyCollection<GenericCacheKeyBase> GetSortedKeyType1(IEnumerable<GenericCacheKeyBase> keys)
        {
            return keys.Where(item => item.GetType() == typeof(KeyType1)).OrderBy(item => item.Key).AsReadOnlyCollection();
        }

        private static IReadOnlyCollection<GenericCacheKeyBase> GetAllKeyType2(IGenericCache cache)
        {
            return GetSortedKeyType2(cache.Keys);
        }

        private static IReadOnlyCollection<GenericCacheKeyBase> GetSortedKeyType2(IEnumerable<GenericCacheKeyBase> keys)
        {
            return keys.Where(item => item.GetType() == typeof(KeyType2)).OrderBy(item => Math.Abs((int) ((KeyType2) item).Key2)).AsReadOnlyCollection();
        }
    }
}