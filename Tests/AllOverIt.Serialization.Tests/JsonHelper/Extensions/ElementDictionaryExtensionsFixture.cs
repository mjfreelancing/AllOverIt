using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.JsonHelper;
using AllOverIt.Serialization.JsonHelper.Exceptions;
using AllOverIt.Serialization.JsonHelper.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.Tests.JsonHelper.Extensions
{
    public class ElementDictionaryExtensionsFixture : FixtureBase
    {
        private readonly IDictionary<string, object> _prop1;
        private readonly IDictionary<string, object> _prop2a;
        private readonly IDictionary<string, object> _prop2b;
        private readonly IDictionary<string, object> _prop3a;
        private readonly IDictionary<string, object> _prop3b;
        private IElementDictionary _elementDictionary;

        public ElementDictionaryExtensionsFixture()
        {
            _prop3a = new Dictionary<string, object>
            {
                {"Prop1", Create<int>()},
                {"Prop2", new[] {CreateMany<string>()}},
                {"Prop3", DateTime.Now}
            };

            _prop3b = new Dictionary<string, object>
            {
                {"Prop1", Create<int>()},
                {"Prop2", new[] {CreateMany<string>()}},
                {"Prop3", DateTime.Now.AddDays(1)}
            };

            _prop2a = new Dictionary<string, object>
            {
                {"Prop1", Create<string>()},
                {"Prop2", new[] {_prop3a, _prop3b}},
            };

            _prop2b = new Dictionary<string, object>
            {
                {"Prop1", Create<string>()},
                {"Prop2", new[] {_prop3b, _prop3a}},
            };

            _prop1 = new Dictionary<string, object>
            {
                {"Prop1", Create<double>()},
                {"Prop2", new[] {_prop2a, _prop2b}},
                {"Prop3", CreateMany<int>()}
            };

            _elementDictionary = new ElementDictionary(_prop1);
        }

        public class TryGetValue : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetValue<int>(null, Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetValue<int>(_elementDictionary, null, out _);
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
                        _ = ElementDictionaryExtensions.TryGetValue<int>(_elementDictionary, string.Empty, out _);
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
                        _ = ElementDictionaryExtensions.TryGetValue<int>(_elementDictionary, "  ", out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Return_True_When_Property_Exists()
            {
                var actual = ElementDictionaryExtensions.TryGetValue<double>(_elementDictionary, "Prop1", out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Value_When_Property_Exists()
            {
                _ = ElementDictionaryExtensions.TryGetValue<double>(_elementDictionary, "Prop1", out var value);

                value.Should().Be((double)_prop1["Prop1"]);
            }

            [Fact]
            public void Should_Return_False_When_Property_Does_Not_Exist()
            {
                var actual = ElementDictionaryExtensions.TryGetValue<double>(_elementDictionary, Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Default_Value_When_Property_Does_Not_Exist()
            {
                _ = ElementDictionaryExtensions.TryGetValue<double>(_elementDictionary, Create<string>(), out var value);

                value.Should().Be(default);
            }

            [Fact]
            public void Should_Convert_Value_When_Can_Convert()
            {
                _ = ElementDictionaryExtensions.TryGetValue<string>(_elementDictionary, "Prop1", out var value);

                var expectedValue = (double) _prop1["Prop1"];

                value.Should().Be($"{expectedValue}");
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetValue<double[]>(_elementDictionary, "Prop1", out _);
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'double' to type 'double[]'.");
            }
        }

        public class GetValue : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetValue<int>(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetValue<int>(_elementDictionary, null);
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
                        _ = ElementDictionaryExtensions.GetValue<int>(_elementDictionary, string.Empty);
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
                        _ = ElementDictionaryExtensions.GetValue<int>(_elementDictionary, "  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Return_Value_When_Property_Exists()
            {
                var value = ElementDictionaryExtensions.GetValue<double>(_elementDictionary, "Prop1");

                value.Should().Be((double) _prop1["Prop1"]);
            }

            [Fact]
            public void Should_Throw_When_Property_Does_Not_Exist()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetValue<double>(_elementDictionary, propertyName);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propertyName} was not found.");
            }

            [Fact]
            public void Should_Convert_Value_When_Can_Convert()
            {
                var value = ElementDictionaryExtensions.GetValue<string>(_elementDictionary, "Prop1");

                var expectedValue = (double) _prop1["Prop1"];

                value.Should().Be($"{expectedValue}");
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetValue<double[]>(_elementDictionary, "Prop1");
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'double' to type 'double[]'.");
            }
        }

        public class TryGetObjectArray : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArray(null, Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, null, out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Empty()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, string.Empty, out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Whitespace()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, "  ", out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Empty_Collection_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, Create<string>(), out var array);

                array.Should().BeEmpty();
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, "Prop1", out _);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Of_Objects()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, "Prop3", out _);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop3 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Array()
            {
                _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, "Prop2", out var elements);

                var array = elements.ToList();

                array.Should().HaveCount(2);

                var element0 = array.ElementAt(0);

                element0.GetValue("Prop1").Should().Be(_prop2a["Prop1"]);
                element0.GetValue("Prop2").Should().Be(_prop2a["Prop2"]);

                var element1 = array.ElementAt(1);

                element1.GetValue("Prop1").Should().Be(_prop2b["Prop1"]);
                element1.GetValue("Prop2").Should().Be(_prop2b["Prop2"]);
            }
        }

        public class GetObjectArray : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Empty()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, string.Empty);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Whitespace()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, "  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_found()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, propertyName);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propertyName} was not found.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, "Prop1");
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Of_Objects()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, "Prop3");
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop3 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Array()
            {
                var array = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, "Prop2").ToList();

                array.Should().HaveCount(2);

                var element0 = array.ElementAt(0);

                element0.GetValue("Prop1").Should().Be(_prop2a["Prop1"]);
                element0.GetValue("Prop2").Should().Be(_prop2a["Prop2"]);

                var element1 = array.ElementAt(1);

                element1.GetValue("Prop1").Should().Be(_prop2b["Prop1"]);
                element1.GetValue("Prop2").Should().Be(_prop2b["Prop2"]);
            }
        }

        public class TryGetObjectArrayValues : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(null, Create<string>(), Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, null, Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Empty()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, string.Empty, Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Whitespace()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, "  ", Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), null, out _);
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
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), string.Empty, out _);
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
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), "  ", out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Empty_Collection_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), Create<string>(), out var array);

                array.Should().BeEmpty();
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, "Prop1", Create<string>(), out _);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Of_Objects()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, "Prop3", Create<string>(), out _);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop3 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                _ = ElementDictionaryExtensions.TryGetObjectArrayValues<string>(_elementDictionary, "Prop2", "Prop1", out var elements);

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    (string) _prop2a["Prop1"],
                    (string) _prop2b["Prop1"],
                });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<double[]>(_elementDictionary, "Prop2", "Prop1", out _);
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'string' to type 'double[]'.");
            }
        }

        public class GetObjectArrayValues : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(null, Create<string>(), Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Empty()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, string.Empty, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Whitespace()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, "  ", Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, Create<string>(), null);
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
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, Create<string>(), string.Empty);
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
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, Create<string>(), "  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_found()
            {
                var prop1Name = Create<string>();
                var prop2Name = Create<string>();

                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, prop1Name, prop2Name);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {prop1Name}.{prop2Name} was not found.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, "Prop1", Create<string>());
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Of_Objects()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, "Prop3", Create<string>());
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop3 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                var elements = ElementDictionaryExtensions.GetObjectArrayValues<string>(_elementDictionary, "Prop2", "Prop1");

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    (string) _prop2a["Prop1"],
                    (string) _prop2b["Prop1"],
                });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<double[]>(_elementDictionary, "Prop2", "Prop1");
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'string' to type 'double[]'.");
            }
        }

        public class TryGetManyObjectArrayValues : ElementDictionaryExtensionsFixture
        {
            private readonly IEnumerable<IElementDictionary> _elements;

            public TryGetManyObjectArrayValues()
            {
                _elements = _elementDictionary.GetObjectArray("Prop2").AsReadOnlyCollection();
            }

            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetManyObjectArrayValues<int>(null, Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("elements");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetManyObjectArrayValues<int>(new[] {_elementDictionary}, null, out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Empty()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetManyObjectArrayValues<int>(new[] {_elementDictionary}, string.Empty, out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetManyObjectArrayValues<int>(_elements, Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Empty_Collection_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetManyObjectArrayValues<int>(_elements, Create<string>(), out var array);

                array.Should().BeEmpty();
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                _ = ElementDictionaryExtensions.TryGetManyObjectArrayValues<string>(_elements, "Prop1", out var elements);

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    (string) _prop2a["Prop1"],
                    (string) _prop2b["Prop1"],
                });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetManyObjectArrayValues<double[]>(_elements, "Prop1", out _);
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'string' to type 'double[]'.");
            }
        }
    }
}