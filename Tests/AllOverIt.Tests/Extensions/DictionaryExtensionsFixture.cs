using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class DictionaryExtensionsFixture : AoiFixtureBase
    {
        private readonly IDictionary<string, string> _dictionary;
        private readonly string _key;
        private readonly string _value;

        protected DictionaryExtensionsFixture()
        {
            _key = Create<string>();
            _value = Create<string>();

            _dictionary = new Dictionary<string, string> {{_key, _value}};
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
    }
}