using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using FluentAssertions;
using System.Collections;

namespace AllOverIt.Serialization.Json.Abstractions.Tests
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

        public class Names : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Get_Names()
            {
                _elementDictionary.Names.Should().BeEquivalentTo(_dictionary.Keys);
            }
        }

        public class Values : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Get_Values()
            {
                _elementDictionary.Values.Should().BeEquivalentTo(_dictionary.Values);
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
            public void Should_Throw_When_Property_Not_Found()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _ = _elementDictionary.GetValue(propertyName);
                })
                   .Should()
                   .Throw<JsonHelperException>()
                   .WithMessage($"The property {propertyName} was not found.");
            }

            [Fact]
            public void Should_Get_Value()
            {
                var value = _elementDictionary.GetValue(_dictionary.Keys.First());

                value.Should().Be(_dictionary.Values.First());
            }
        }

        public class TrySetValue : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    _ = _elementDictionary.TrySetValue(null, new { });
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
                    _ = _elementDictionary.TrySetValue(string.Empty, new { });
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
                    _ = _elementDictionary.TrySetValue("  ", new { });
                })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_Found()
            {
                var actual = _elementDictionary.TrySetValue(Create<string>(), new { });

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Property_Found()
            {
                var actual = _elementDictionary.TrySetValue(_dictionary.Keys.First(), new { });

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Set_Value()
            {
                var expected = Create<string>();

                var key = _dictionary.Keys.First();

                _elementDictionary.GetValue(key).Should().NotBe(expected);

                var result = _elementDictionary.TrySetValue(key, expected);

                result.Should().BeTrue();

                _elementDictionary.GetValue(key).Should().Be(expected);
            }
        }

        public class SetValue : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    _elementDictionary.SetValue(null, new { });
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
                    _elementDictionary.SetValue(string.Empty, new { });
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
                    _elementDictionary.SetValue("  ", new { });
                })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_Found()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _elementDictionary.SetValue(propertyName, new { });
                })
                   .Should()
                   .Throw<JsonHelperException>()
                   .WithMessage($"The property {propertyName} was not found.");
            }

            [Fact]
            public void Should_Set_Value()
            {
                var expected = Create<string>();

                var key = _dictionary.Keys.First();

                _elementDictionary.GetValue(key).Should().NotBe(expected);

                _elementDictionary.SetValue(key, expected);

                _elementDictionary.GetValue(key).Should().Be(expected);
            }
        }

        public class GetEnumerator_Generic : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Enumerate_Keys()
            {
                var actual = _elementDictionary.Select(kvp => kvp.Value);

                actual.Should().BeEquivalentTo(_elementDictionary.Values);
            }
        }

        public class GetEnumerator : ElementDictionaryFixture
        {
            [Fact]
            public void Should_Enumerate_Keys()
            {
                var actual = new List<object>();

                var enumerator = ((IEnumerable) _elementDictionary).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    actual.Add(enumerator.Current);
                }

                actual.Should().BeEquivalentTo(_dictionary);
            }
        }
    }
}
