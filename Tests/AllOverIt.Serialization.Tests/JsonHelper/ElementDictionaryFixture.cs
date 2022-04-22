using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.JsonHelper;
using AllOverIt.Serialization.JsonHelper.Exceptions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.Tests.JsonHelper
{
    public class ElementDictionaryFixture : FixtureBase
    {
        private readonly IDictionary<string, object> _dictionary;
        private readonly ElementDictionary _elementDictionary;

        public ElementDictionaryFixture()
        {
            _dictionary = CreateMany<KeyValuePair<string, int>>().ToDictionary(kvp => kvp.Key, kvp => (object) kvp.Value);
            _elementDictionary = new ElementDictionary(_dictionary);
        }

        public class Constructor : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Throw_When_Element_null()
            {
                Invoking(() =>
                {
                    _ = new ElementDictionary(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }
        }

        public class TryGetValue : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    _ = _elementDictionary.TryGetValue(null, out _);
                })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Empty()
            {
                Invoking(() =>
                {
                    _ = _elementDictionary.TryGetValue(string.Empty, out _);
                })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _elementDictionary.TryGetValue("  ", out _);
                })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_Found()
            {
                var actual = _elementDictionary.TryGetValue(Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Property_Found()
            {
                var actual = _elementDictionary.TryGetValue(_dictionary.Keys.First(), out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Value()
            {
                _ = _elementDictionary.TryGetValue(_dictionary.Keys.First(), out var value);

                value.Should().Be(_dictionary.Values.First());
            }
        }

        public class GetValue : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    _ = _elementDictionary.GetValue(null);
                })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Empty()
            {
                Invoking(() =>
                {
                    _ = _elementDictionary.GetValue(string.Empty);
                })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _elementDictionary.GetValue("  ");
                })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Get_Value()
            {
                var value = _elementDictionary.GetValue(_dictionary.Keys.First());

                value.Should().Be(_dictionary.Values.First());
            }

            [Fact]
            public void Should_Throw_When_Property_Not_Found()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _ = _elementDictionary.GetValue(propertyName);
                })
                   .Should()
                   .Throw<JsonHelperException>()
                   .WithMessage($"The property {propertyName} was not found.")
                   .WithInnerException<KeyNotFoundException>();
            }
        }
    }
}
