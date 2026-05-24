using AllOverIt.Caching;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using System.Collections;
using AllOverIt.Shouldly.Extensions;
namespace AllOverIt.Tests.Caching
{
    public class GenericCacheFixture : FixtureBase
    {
        private record KeyType1 : GenericCacheKey<int, string>
        {
            public KeyType1(int val1, string val2)
                : base(val1, val2)
            {
            }
        }

        private record KeyType2 : GenericCacheKey<bool, int?, string>
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

        public class Default : GenericCacheFixture
        {
            [Fact]
            public void Should_Be_A_Default_Constructed_Cache()
            {
                var cache = GenericCache.Default;

                cache.ShouldBeOfType<GenericCache>();
                cache.Count.ShouldBe(0);
            }
        }
        public class Count : GenericCacheFixture
        {
            [Fact]
            public void Should_Return_Expected_Count()
            {
                _cache.Count.ShouldBe(PerKeyCount * 2);
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
                    actual1.Key.GetType().ShouldBe(typeof((int, string)));
                    actual1.Key.ShouldBeEquivalentTo((i, $"{i}"));

                    var actual1Key = (KeyType1) actual1;
                    actual1Key.Key1.ShouldBe(i);
                    actual1Key.Key2.ShouldBe($"{i}");

                    var actual2 = keyType2.ElementAt(i - 1);
                    actual2.Key.GetType().ShouldBe(typeof((bool, int?, string)));

                    var isEven = i % 2 == 0;

                    actual2.Key.ShouldBeEquivalentTo((isEven, isEven ? i : -i, isEven ? $"{i}" : default));

                    var actual2Key = (KeyType2) actual2;
                    actual2Key.Key1.ShouldBe(isEven);
                    actual2Key.Key2.ShouldBe(isEven ? i : -i);
                    actual2Key.Key3.ShouldBe(isEven ? $"{i}" : default);
                }
            }

            [Fact]
            public void Should_Find_Key()
            {
                var keyType1 = GetAllKeyType1(_cache).First();
                var keyType2 = GetAllKeyType2(_cache).First();

                _cache.ContainsKey(keyType1).ShouldBeTrue();
                _cache.ContainsKey(keyType2).ShouldBeTrue();
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
                    _cache[actual1].ShouldBe(i * i);

                    var actual2 = keyType2.ElementAt(i - 1);
                    _cache[actual2].ShouldBe(i * i * i);
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
                _cache[key1].ShouldBe(index * index);

                var isEven = index % 2 == 0;
                var key2 = new KeyType2(isEven, isEven ? index : -index, isEven ? $"{index}" : default);
                _cache[key2].ShouldBe(index * index * index);
            }
        }

        public class CreateKey_1 : GenericCacheFixture
        {
            [Fact]
            public void Should_Create_Key()
            {
                var value = Create<int>();

                var actual = GenericCache.CreateKey<int>(value);

                actual.ShouldBeOfType<GenericCacheKey<int>>();

                actual.Key.ShouldBe(value);

                var typedKey = (GenericCacheKey<int>) actual;

                typedKey.Key1.ShouldBe(value);
            }
        }

        public class CreateKey_2 : GenericCacheFixture
        {
            [Fact]
            public void Should_Create_Key()
            {
                var value1 = Create<int>();
                var value2 = Create<string>();

                var actual = GenericCache.CreateKey<int, string>(value1, value2);

                actual.ShouldBeOfType<GenericCacheKey<int, string>>();

                actual.Key.ShouldBe((value1, value2));

                var typedKey = (GenericCacheKey<int, string>) actual;

                typedKey.Key1.ShouldBe(value1);
                typedKey.Key2.ShouldBe(value2);
            }
        }

        public class CreateKey_3 : GenericCacheFixture
        {
            [Fact]
            public void Should_Create_Key()
            {
                var value1 = Create<int>();
                var value2 = Create<string>();
                var value3 = Create<double>();

                var actual = GenericCache.CreateKey<int, string, double>(value1, value2, value3);

                actual.ShouldBeOfType<GenericCacheKey<int, string, double>>();

                actual.Key.ShouldBe((value1, value2, value3));

                var typedKey = (GenericCacheKey<int, string, double>) actual;

                typedKey.Key1.ShouldBe(value1);
                typedKey.Key2.ShouldBe(value2);
                typedKey.Key3.ShouldBe(value3);
            }
        }

        public class CreateKey_4 : GenericCacheFixture
        {
            [Fact]
            public void Should_Create_Key()
            {
                var value1 = Create<int>();
                var value2 = Create<string>();
                var value3 = Create<double>();
                var value4 = Create<string>();

                var actual = GenericCache.CreateKey<int, string, double, string>(value1, value2, value3, value4);

                actual.ShouldBeOfType<GenericCacheKey<int, string, double, string>>();

                actual.Key.ShouldBe((value1, value2, value3, value4));

                var typedKey = (GenericCacheKey<int, string, double, string>) actual;

                typedKey.Key1.ShouldBe(value1);
                typedKey.Key2.ShouldBe(value2);
                typedKey.Key3.ShouldBe(value3);
                typedKey.Key4.ShouldBe(value4);
            }
        }

        public class GetEnumerator : GenericCacheFixture
        {
            [Fact]
            public void Should_Enumerate_All_Items()
            {
                var count = 0;

                foreach (var _ in _cache)
                {
                    count++;
                }

                count.ShouldBe(PerKeyCount * 2);
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
                    .ShouldThrow<ArgumentNullException>()
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
                    _cache.ContainsKey(key1).ShouldBeTrue();

                    var isEven = idx % 2 == 0;
                    var key2 = new KeyType2(isEven, isEven ? idx : -idx, isEven ? $"{idx}" : default);
                    _cache.ContainsKey(key2).ShouldBeTrue();
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
                    _cache.ContainsKey(key1).ShouldBeFalse();

                    var key2 = new KeyType2(idx % 2 == 0, idx * 2, $"{idx}");
                    _cache.ContainsKey(key2).ShouldBeFalse();
                }
            }
        }

        public class Clear : GenericCacheFixture
        {
            [Fact]
            public void Should_Clear_Cache()
            {
                _cache.Count.ShouldBe(PerKeyCount * 2);

                _cache.Clear();

                _cache.Count.ShouldBe(0);
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Add_Key_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = CreateMany<string>();

                _cache.Add(key, expected);

                _ = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out var actual);

                expected.ShouldBeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Add_Different_Key_Types()
            {
                var key1 = new KeyType1(Create<int>(), Create<string>());
                var key2 = new KeyType2(Create<bool>(), Create<int>(), Create<string>());

                var expected1 = CreateMany<string>();
                var expected2 = CreateMany<int>();

                _cache.Add(key1, expected1);
                _cache.Add(key2, expected2);

                _ = _cache.TryGetValue<IReadOnlyCollection<string>>(key1, out var actual1);
                _ = _cache.TryGetValue<IReadOnlyCollection<int>>(key2, out var actual2);

                expected1.ShouldBeEquivalentTo(actual1);
                expected2.ShouldBeEquivalentTo(actual2);
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
                    .ShouldThrow<ArgumentException>()
                    .WithMessage("The key already existed in the dictionary.");
            }

            [Fact]
            public void Should_Not_Throw_When_Key_Element_Null()
            {
                var key = new KeyType1(Create<int>(), null);
                var expected = CreateMany<string>();

                key.Key2.ShouldBeNull();
                _cache.ContainsKey(key).ShouldBeFalse();

                Invoking(() =>
                    {
                        _cache.Add(key, expected);
                    })
                    .ShouldNotThrow();

                expected.ShouldBeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Add_Key_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                var success = _cache.TryAdd(key, value);

                success.ShouldBeTrue();

                _ = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out var actual);

                value.ShouldBeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Fail_To_Add_When_Key_Exists()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _cache.Add(key, CreateMany<string>());

                var success = _cache.TryAdd(key, CreateMany<string>());

                success.ShouldBeFalse();
            }
        }

        public class TryGetValue_Object : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryGetValue(null, out _);
                    })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Get_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                var success = _cache.TryGetValue(key, out var actual);

                success.ShouldBeTrue();

                value.ShouldBeEquivalentTo((IReadOnlyCollection<string>) actual);
            }

            [Fact]
            public void Should_Fail_To_Get_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var success = _cache.TryGetValue(key, out _);

                success.ShouldBeFalse();
            }
        }

        public class TryGetValue_TValue : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryGetValue<string>(null, out _);
                    })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Get_Value()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                var success = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out var actual);

                success.ShouldBeTrue();

                value.ShouldBeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Fail_To_Get_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var success = _cache.TryGetValue<IReadOnlyCollection<string>>(key, out _);

                success.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_Default_Value_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _ = _cache.TryGetValue<double>(key, out var actual1);
                actual1.ShouldBe(default);

                _ = _cache.TryGetValue<string>(key, out var actual2);
                actual2.ShouldBe(default);
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
                value.ShouldBeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);

                var success = _cache.Remove(new KeyValuePair<GenericCacheKeyBase, object>(key, value));

                success.ShouldBeTrue();

                _cache.TryGetValue(key, out _).ShouldBeFalse();
            }

            [Fact]
            public void Should_Not_Remove_From_Cache_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var actual = _cache.Remove(new KeyValuePair<GenericCacheKeyBase, object>(key, Create<string>()));

                actual.ShouldBeFalse();
            }
        }

        public class TryRemove_TValue : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryRemove<string>(null, out _);
                    })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Remove_From_Cache()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                // Confirm the key is in the cache
                value.ShouldBeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);

                var success = _cache.TryRemove<IReadOnlyCollection<string>>(key, out var actual);

                success.ShouldBeTrue();

                value.ShouldBeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Not_Remove_From_Cache_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                var actual = _cache.TryRemove<string>(key, out _);

                actual.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_Default_Value_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _ = _cache.TryRemove<double>(key, out var actual1);
                actual1.ShouldBe(default);

                _ = _cache.TryRemove<string>(key, out var actual2);
                actual2.ShouldBe(default);
            }
        }

        public class TryRemove_TValue_KeyValuePair : GenericCacheFixture
        {
            [Fact]
            public void Should_Remove_From_Cache()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                _cache[key] = value;

                // Confirm the key is in the cache
                value.ShouldBeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);

                var success = _cache.TryRemove(new KeyValuePair<GenericCacheKeyBase, IReadOnlyCollection<string>>(key, value));

                success.ShouldBeTrue();
            }

            [Fact]
            public void Should_Not_Remove_From_Cache_When_Key_Does_Not_Exist()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = CreateMany<string>();

                var actual = _cache.TryRemove(new KeyValuePair<GenericCacheKeyBase, IReadOnlyCollection<string>>(key, value));

                actual.ShouldBeFalse();
            }
        }

        public class TryUpdate_TValue : GenericCacheFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                    {
                        _ = _cache.TryUpdate<string>(null, Create<string>(), Create<string>());
                    })
                    .ShouldThrow<ArgumentNullException>()
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

                success.ShouldBeFalse();

                originalValue.ShouldBeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);
            }

            [Fact]
            public void Should_Update_When_Comparison_Is_Equal()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var originalValue = CreateMany<string>();
                var newValue = CreateMany<string>();

                _cache[key] = originalValue;

                var success = _cache.TryUpdate(key, newValue, originalValue);

                success.ShouldBeTrue();

                newValue.ShouldBeEquivalentTo((IReadOnlyCollection<string>) _cache[key]);
            }

            [Fact]
            public void Should_Not_Update_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());

                _cache.ContainsKey(key).ShouldBeFalse();

                var success = _cache.TryUpdate(key, Create<string>(), Create<string>());

                success.ShouldBeFalse();
            }
        }

        public class ToArray : GenericCacheFixture
        {
            [Fact]
            public void Should_Copy_To_Array()
            {
                var actual = _cache.ToArray();

                actual.Count.ShouldBe(PerKeyCount * 2);

                var keys1 = GetAllKeyType1(_cache);
                keys1.ShouldBeEquivalentTo(GetSortedKeyType1(actual.Select(item => item.Key)));

                foreach (var key1 in keys1)
                {
                    _cache[key1].ShouldBe(actual.Single(item => item.Key == key1).Value);
                }

                var keys2 = GetAllKeyType2(_cache);
                keys2.ShouldBeEquivalentTo(GetSortedKeyType2(actual.Select(item => item.Key)));

                foreach (var key2 in keys2)
                {
                    _cache[key2].ShouldBe(actual.Single(item => item.Key == key2).Value);
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Resolver_Null()
            {
                Invoking(() =>
                    {
                        var key = new KeyType1(Create<int>(), Create<string>());
                        _ = _cache.GetOrAdd(key, (Func<GenericCacheKeyBase, string>) null);
                    })
                    .ShouldThrow<ArgumentNullException>()
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

                key.ShouldBe(actual);
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).ShouldBeFalse();

                var actual = _cache.GetOrAdd(key, _ => expected);

                expected.ShouldBe(actual);
            }

            [Fact]
            public void Should_Get_Current_Value_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache[key] = expected;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.GetOrAdd(key, _ => Create<double>());

                expected.ShouldBe(actual);
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).ShouldBeFalse();

                var actual = _cache.GetOrAdd(key, expected);

                expected.ShouldBe(actual);
            }

            [Fact]
            public void Should_Get_Current_Value_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache[key] = expected;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.GetOrAdd(key, Create<double>());

                expected.ShouldBe(actual);
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
                    .ShouldThrow<ArgumentNullException>()
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("addResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).ShouldBeFalse();

                var actual = _cache.GetOrAdd(key, (addKey, arg) => expected, Create<int>());

                expected.ShouldBe(actual);
            }

            [Fact]
            public void Should_Provide_AddKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualAddKey = null;

                _cache.ContainsKey(key).ShouldBeFalse();

                _ = _cache.GetOrAdd(key, (addKey, arg) =>
                {
                    actualAddKey = addKey;
                    return Create<double>();
                }, Create<int>());

                key.ShouldBeSameAs(actualAddKey);
            }

            [Fact]
            public void Should_Get_Current_Value_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache[key] = expected;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.GetOrAdd(key, (addKey, arg) => Create<double>(), Create<double>());

                expected.ShouldBe(actual);
            }

            [Fact]
            public void Should_Provider_Arg_When_Adding()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var argValue = Create<int>();
                var actualArgValue = 0;

                _cache.ContainsKey(key).ShouldBeFalse();

                _ = _cache.GetOrAdd(key, (addKey, arg) =>
                {
                    actualArgValue = arg;
                    return Create<double>();
                }, argValue);

                argValue.ShouldBe(actualArgValue);
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
                    .ShouldThrow<ArgumentNullException>()
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
                    .ShouldThrow<ArgumentNullException>()
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("updateResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).ShouldBeFalse();

                var actual = _cache.AddOrUpdate(key, addKey => expected, (updateKey, value) => Create<double>());

                expected.ShouldBe(actual);
            }

            [Fact]
            public void Should_Provide_AddKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualAddKey = null;

                _cache.ContainsKey(key).ShouldBeFalse();

                _ = _cache.AddOrUpdate(key, addKey =>
                    {
                        actualAddKey = addKey;
                        return Create<double>();
                    },
                    (updateKey, value) => Create<double>());

                key.ShouldBeSameAs(actualAddKey);
            }

            [Fact]
            public void Should_Update_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var factor = GetWithinRange(2, 5);
                var expected = value * factor;

                _cache[key] = value;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.AddOrUpdate(key, addKey => value, (updateKey, currentValue) => currentValue * factor);

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Provide_UpdateKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualUpdateKey = null;

                _cache[key] = Create<double>();

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.AddOrUpdate(key,
                    addKey => Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualUpdateKey = updateKey;
                        return Create<double>();
                    });

                key.ShouldBe(actualUpdateKey);
            }

            [Fact]
            public void Should_Provide_Current_Value_When_Updating()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var actualCurrentValue = value;

                _cache[key] = value;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.AddOrUpdate(key,
                    addKey => Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualCurrentValue = currentValue;
                        return Create<double>();
                    });

                value.ShouldBe(actualCurrentValue);
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
                    .ShouldThrow<ArgumentNullException>()
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("updateResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<double>();

                _cache.ContainsKey(key).ShouldBeFalse();

                var actual = _cache.AddOrUpdate(key, expected, (updateKey, value) => Create<double>());

                expected.ShouldBe(actual);
            }

            [Fact]
            public void Should_Update_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var factor = GetWithinRange(2, 5);
                var expected = value * factor;

                _cache[key] = value;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.AddOrUpdate(key, value, (updateKey, currentValue) => currentValue * factor);

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Provide_UpdateKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualUpdateKey = null;

                _cache[key] = Create<double>();

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.AddOrUpdate(key,
                    Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualUpdateKey = updateKey;
                        return Create<double>();
                    });

                key.ShouldBe(actualUpdateKey);
            }

            [Fact]
            public void Should_Provide_Current_Value_When_Updating()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var actualCurrentValue = value;

                _cache[key] = value;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.AddOrUpdate(key,
                    Create<double>(),
                    (updateKey, currentValue) =>
                    {
                        actualCurrentValue = currentValue;
                        return Create<double>();
                    });

                value.ShouldBe(actualCurrentValue);
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
                    .ShouldThrow<ArgumentNullException>()
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
                    .ShouldThrow<ArgumentNullException>()
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("updateResolver");
            }

            [Fact]
            public void Should_Add_When_Key_Not_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var expected = Create<string>();

                _cache.ContainsKey(key).ShouldBeFalse();

                var actual = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => expected,
                    (updateKey, currentValue, updateArg) => Create<string>(),
                    Create<int>());

                expected.ShouldBe(actual);
            }

            [Fact]
            public void Should_Update_When_Key_Present()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<double>();
                var factor = GetWithinRange(2, 5);
                var expected = value * factor;

                _cache[key] = value;

                _cache.ContainsKey(key).ShouldBeTrue();

                var actual = _cache.AddOrUpdate<int, double>(
                    key,
                    (addKey, addArg) => Create<double>(),
                    (updateKey, currentValue, updateArg) => currentValue * factor,
                    Create<int>());

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Provide_AddKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                GenericCacheKeyBase actualKey = null;

                _cache.ContainsKey(key).ShouldBeFalse();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) =>
                    {
                        actualKey = addKey;
                        return Create<string>();
                    },
                    (updateKey, currentValue, updateArg) => Create<string>(),
                    Create<int>());

                actualKey.ShouldBeSameAs(key);
            }

            [Fact]
            public void Should_Provide_AddArg()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var arg = Create<int>();
                var actualArg = 0;

                _cache.ContainsKey(key).ShouldBeFalse();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) =>
                    {
                        actualArg = addArg;
                        return Create<string>();
                    },
                    (updateKey, currentValue, updateArg) => Create<string>(),
                    arg);

                arg.ShouldBe(actualArg);
            }

            [Fact]
            public void Should_Provide_UpdateKey()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<string>();
                GenericCacheKeyBase actualKey = null;

                _cache[key] = value;

                _cache.ContainsKey(key).ShouldBeTrue();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => Create<string>(),
                    (updateKey, currentValue, updateArg) =>
                    {
                        actualKey = updateKey;
                        return Create<string>();
                    },
                    Create<int>());

                actualKey.ShouldBeSameAs(key);
            }

            [Fact]
            public void Should_Provide_UpdateCurrentValue()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var value = Create<string>();
                string actualCurrentValue = null;

                _cache[key] = value;

                _cache.ContainsKey(key).ShouldBeTrue();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => Create<string>(),
                    (updateKey, currentValue, updateArg) =>
                    {
                        actualCurrentValue = currentValue;
                        return Create<string>();
                    },
                    Create<int>());

                value.ShouldBe(actualCurrentValue);
            }

            [Fact]
            public void Should_Provide_UpdateArg()
            {
                var key = new KeyType1(Create<int>(), Create<string>());
                var arg = Create<int>();
                var actualArg = 0;

                _cache[key] = Create<string>();

                _cache.ContainsKey(key).ShouldBeTrue();

                _ = _cache.AddOrUpdate<int, string>(
                    key,
                    (addKey, addArg) => Create<string>(),
                    (updateKey, currentValue, updateArg) =>
                    {
                        actualArg = updateArg;
                        return Create<string>();
                    },
                    arg);

                arg.ShouldBe(actualArg);
            }
        }

        public class Explicit : GenericCacheFixture
        {
            public class Explicit_IsReadOnly : Explicit
            {
                [Fact]
                public void Should_Be_False()
                {
                    ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).IsReadOnly.ShouldBeFalse();
                }
            }

            public class Explicit_Keys : Explicit
            {
                [Fact]
                public void Should_Return_Keys()
                {
                    var expected = _cache.Keys;

                    expected.ShouldBeEquivalentTo(((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).Keys);
                }
            }

            public class Explicit_Values : Explicit
            {
                [Fact]
                public void Should_Return_Values()
                {
                    var expected = _cache.Values;

                    expected.ShouldBeEquivalentTo(((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).Values);
                }
            }

            public class Explicit_Add_KeyValuePair : Explicit
            {
                [Fact]
                public void Should_Add_Key_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();

                    _cache.ContainsKey(key).ShouldBeFalse();

                    var kvp = new KeyValuePair<GenericCacheKeyBase, object>(key, value);

                    ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).Add(kvp);

                    _cache[key].ShouldBe(value);
                }
            }

            public class Explicit_Add_Key : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IDictionary<GenericCacheKeyBase, object>) _cache).Add(null, Create<string>());
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Add_Key_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();

                    _cache.ContainsKey(key).ShouldBeFalse();

                    ((IDictionary<GenericCacheKeyBase, object>) _cache).Add(key, value);

                    _cache[key].ShouldBe(value);
                }
            }

            public class Explicit_Contains : Explicit
            {
                [Fact]
                public void Should_Not_Contain_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();
                    var kvp = new KeyValuePair<GenericCacheKeyBase, object>(key, value);

                    var actual = ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).Contains(kvp);

                    actual.ShouldBeFalse();
                }

                [Fact]
                public void Should_Contain_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<string>();
                    var kvp = new KeyValuePair<GenericCacheKeyBase, object>(key, value);

                    _cache.ContainsKey(key).ShouldBeFalse();
                    _cache[key] = value;

                    var actual = ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).Contains(kvp);

                    actual.ShouldBeTrue();
                }
            }

            public class Explicit_CopyTo : Explicit
            {
                [Fact]
                public void Should_Copy_To_Array_At_Index()
                {
                    var array = new KeyValuePair<GenericCacheKeyBase, object>[PerKeyCount * 2 + 100];
                    var index = GetWithinRange(0, 99);

                    ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).CopyTo(array, index);

                    var actual = array.Skip(index).Take(PerKeyCount * 2);

                    _cache.ToArray().ShouldBeEquivalentTo(actual);
                }
            }

            public class Explicit_Remove : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(null);
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Not_Remove_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).ShouldBeFalse();

                    var actual = ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(key);

                    actual.ShouldBeFalse();
                }

                [Fact]
                public void Should_Remove_Key()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).ShouldBeFalse();

                    _cache[key] = Create<string>();
                    _cache.ContainsKey(key).ShouldBeTrue();

                    var actual = ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(key);

                    actual.ShouldBeTrue();
                }
            }

            public class Explicit_TryGetValue_Dictionary : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(null, out _);
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Not_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).ShouldBeFalse();

                    var actual = ((IDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out _);

                    actual.ShouldBeFalse();
                }

                [Fact]
                public void Should_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<double>();

                    _cache.ContainsKey(key).ShouldBeFalse();

                    _cache[key] = value;

                    var success = ((IDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out var actual);

                    success.ShouldBeTrue();
                    value.ShouldBe((double) actual);
                }
            }

            public class Explicit_TryGetValue_ReadOnlyDictionary : Explicit
            {
                [Fact]
                public void Should_Throw_When_Key_Null()
                {
                    Invoking(() =>
                        {
                            ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(null, out _);
                        })
                        .ShouldThrow<ArgumentNullException>()
                        .WithNamedMessageWhenNull("key");
                }

                [Fact]
                public void Should_Not_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());

                    _cache.ContainsKey(key).ShouldBeFalse();

                    var actual = ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out _);

                    actual.ShouldBeFalse();
                }

                [Fact]
                public void Should_Get_Value()
                {
                    var key = new KeyType1(Create<int>(), Create<string>());
                    var value = Create<double>();

                    _cache.ContainsKey(key).ShouldBeFalse();

                    _cache[key] = value;

                    var success = ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out var actual);

                    success.ShouldBeTrue();
                    value.ShouldBe((double) actual);
                }
            }

            public class Explicit_GetEnumerator : Explicit
            {
                [Fact]
                public void Should_Enumerate()
                {
                    var enumerator = ((IEnumerable) _cache).GetEnumerator();

                    enumerator.MoveNext().ShouldBeTrue();

                    enumerator.Current.ShouldBeEquivalentTo(_cache.Take(1).Single());
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

        private static IReadOnlyCollection<GenericCacheKeyBase> GetAllKeyType1(IDictionary<GenericCacheKeyBase, object> cache)
        {
            return GetSortedKeyType1(cache.Keys);
        }

        private static IReadOnlyCollection<GenericCacheKeyBase> GetSortedKeyType1(IEnumerable<GenericCacheKeyBase> keys)
        {
            return keys.Where(item => item.GetType() == typeof(KeyType1)).OrderBy(item => item.Key).AsReadOnlyCollection();
        }

        private static IReadOnlyCollection<GenericCacheKeyBase> GetAllKeyType2(IDictionary<GenericCacheKeyBase, object> cache)
        {
            return GetSortedKeyType2(cache.Keys);
        }

        private static IReadOnlyCollection<GenericCacheKeyBase> GetSortedKeyType2(IEnumerable<GenericCacheKeyBase> keys)
        {
            return keys.Where(item => item.GetType() == typeof(KeyType2)).OrderBy(item => Math.Abs((int) ((KeyType2) item).Key2)).AsReadOnlyCollection();
        }
    }
}



