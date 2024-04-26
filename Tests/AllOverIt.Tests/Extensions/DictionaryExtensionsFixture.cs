using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Tests.Extensions
{
    public class DictionaryExtensionsFixture : FixtureBase
    {
        private readonly IDictionary<string, string> _dictionary;
        private readonly string _key;
        private readonly string _value;

        protected DictionaryExtensionsFixture()
        {
            _key = Create<string>();
            _value = Create<string>();

            _dictionary = new Dictionary<string, string> { { _key, _value } };
        }

        public class GetValueOrDefault : DictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Get_Value()
            {
                var actual = _dictionary.GetValueOrDefault(_key);

                actual.Should().Be(_value);
            }

            [Fact]
            public void Should_Get_Type_Default()
            {
                var actual = _dictionary.GetValueOrDefault(_value);

                actual.Should().Be(default);
            }

            [Fact]
            public void Should_Get_Value_Not_Default()
            {
                var defaultValue = Create<string>();
                var actual = _dictionary.GetValueOrDefault(_key, defaultValue);

                actual.Should().Be(_value);
            }

            [Fact]
            public void Should_Get_Provided_Default()
            {
                var defaultValue = Create<string>();
                var actual = _dictionary.GetValueOrDefault(_value, defaultValue);

                actual.Should().Be(defaultValue);
            }
        }

        public class GetOrSet : DictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Get_Value()
            {
                var actual = _dictionary.GetOrSet(_key, Create<string>);

                actual.Should().Be(_value);
            }

            [Fact]
            public void Should_Set_Value()
            {
                _dictionary.Clear();

                var actual = _dictionary.GetOrSet(_key, () => _value);

                actual.Should().Be(_value);
            }
        }

        public class Combine : DictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Combine_Two_Dictionaries()
            {
                var first = Create<Dictionary<string, string>>();
                var second = Create<Dictionary<string, string>>();

                var expected = new Dictionary<string, string>();

                foreach (var kvp in first)
                {
                    expected.Add(kvp.Key, kvp.Value);
                }

                foreach (var kvp in second)
                {
                    expected.Add(kvp.Key, kvp.Value);
                }

                var actual = first.Combine(second);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Throw_When_Duplicate_Key()
            {
                var values = Create<Dictionary<string, string>>();

                Invoking(() =>
                    {
                        values.Combine(values);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"An item with the same key has already been added. Key: {values.First().Key}");
            }

            [Fact]
            public void Should_Return_New_Instance()
            {
                var first = Create<Dictionary<string, string>>();
                var second = Create<Dictionary<string, string>>();

                var actual = first.Combine(second);

                actual.Should().BeOfType<Dictionary<string, string>>();

                actual.Should().NotBeSameAs(first);
                actual.Should().NotBeSameAs(second);
            }
        }
    }
}