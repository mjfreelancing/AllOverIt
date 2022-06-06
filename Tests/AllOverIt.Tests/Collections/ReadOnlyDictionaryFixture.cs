using AllOverIt.Collections;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Collections
{
    public class ReadOnlyDictionaryFixture : FixtureBase
    {
        private readonly IReadOnlyDictionary<int, string> _data;
        private readonly ReadOnlyDictionary<int, string> _dictionary;

        public ReadOnlyDictionaryFixture()
        {
            _data = CreateMany<int>(5)
                .Select(item => new KeyValuePair<int, string>(item, $"{item}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            _dictionary = new ReadOnlyDictionary<int, string>(_data);
        }

        public class Constructor_Default : ReadOnlyListFixture
        {
            [Fact]
            public void Should_Be_Empty()
            {
                var actual = new ReadOnlyDictionary<int, string>();
                actual.Should().BeEmpty();
            }
        }

        public class IndexOperator : ReadOnlyDictionaryFixture
        {
            [Fact]
            public void Should_Return_Value_By_Index()
            {
                var index = GetWithinRange(0, 4);

                var key = _data.Keys.ElementAt(index);
                var expected = _data.Values.ElementAt(index);
                var actual = _dictionary[key];

                actual.Should().Be(expected);
            }
        }

        public class Keys : ReadOnlyDictionaryFixture
        {
            [Fact]
            public void Should_Return_Keys()
            {
                _dictionary.Keys.Should().BeEquivalentTo(_data.Keys);
            }
        }

        public class Values : ReadOnlyDictionaryFixture
        {
            [Fact]
            public void Should_Return_Values()
            {
                _dictionary.Values.Should().BeEquivalentTo(_data.Values);
            }
        }

        public class Count : ReadOnlyDictionaryFixture
        {
            [Fact]
            public void Should_Return_Count()
            {
                _dictionary.Count.Should().Be(_data.Count);
            }
        }

        public class ContainsKey : ReadOnlyDictionaryFixture
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Expected_Result(bool expected)
            {
                var key = expected ? _data.Keys.ElementAt(3) : _data.Keys.Sum();

                _dictionary.ContainsKey(key).Should().Be(expected);
            }
        }

        public class TryGetValue : ReadOnlyDictionaryFixture
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Expected_Result(bool success)
            {
                var key = success ? _data.Keys.ElementAt(3) : _data.Keys.Sum();
                var expected = success ? $"{key}" : default;

                var actual = _dictionary.TryGetValue(key, out var result);

                actual.Should().Be(success);
                result.Should().Be(expected);
            }
        }

        public class GetEnumerator : ReadOnlyDictionaryFixture
        {
            [Fact]
            public void Should_Iterate_Data()
            {
                var results = new List<string>();
                var enumerator = _dictionary.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    results.Add(enumerator.Current.Value);
                }

                results.Should().BeEquivalentTo(_data.Values);
            }
        }

        [Fact]
        public void Should_Be_Immutable()
        {
            _dictionary.Should().NotBeAssignableTo<ICollection<int>>();
        }
    }
}
