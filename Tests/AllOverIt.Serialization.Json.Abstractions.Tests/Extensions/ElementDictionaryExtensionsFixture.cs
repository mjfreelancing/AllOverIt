using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using AllOverIt.Serialization.Json.Abstractions.Extensions;
using FluentAssertions;

namespace AllOverIt.Serialization.Json.Abstractions.Tests.Extensions
{
    public class ElementDictionaryExtensionsFixture : FixtureBase
    {
        private sealed class ElementItem
        {
        }

        private readonly IDictionary<string, object> _prop1;
        private readonly IDictionary<string, object> _prop2a;
        private readonly IDictionary<string, object> _prop2b;
        private readonly IDictionary<string, object> _prop3a;
        private readonly IDictionary<string, object> _prop3b;
        private readonly IDictionary<string, object> _prop4a;
        private readonly IDictionary<string, object> _prop4b;
        private readonly IElementDictionary _elementDictionary;

        public ElementDictionaryExtensionsFixture()
        {
            _prop4a = new Dictionary<string, object>
            {
                {"Prop1", Create<int>()}
            };

            _prop4b = new Dictionary<string, object>
            {
                {"Prop1", Create<int>()}
            };

            _prop3a = new Dictionary<string, object>
            {
                {"Prop1", Create<int>()},
                {"Prop2", new[] { CreateMany<string>() }},
                {"Prop3", DateTime.Now},
                {"Prop4", new[] { _prop4a, _prop4b }}
            };

            _prop3b = new Dictionary<string, object>
            {
                {"Prop1", Create<int>()},
                {"Prop2", new[] { CreateMany<string>() }},
                {"Prop3", DateTime.Now.AddDays(1)},
                {"Prop4", new[] { _prop4b, _prop4a }}
            };

            _prop2a = new Dictionary<string, object>
            {
                {"Prop1", Create<string>()},
                {"Prop2", new[] { _prop3a, _prop3b }},
            };

            _prop2b = new Dictionary<string, object>
            {
                {"Prop1", Create<string>()},
                {"Prop2", new[] { _prop3b, _prop3a }},
            };

            _prop1 = new Dictionary<string, object>
            {
                {"Prop1", Create<double>()},
                {"Prop2", new[] { _prop2a, _prop2b }},
                {"Prop3", CreateMany<int>()},
                {"Prop4", new[] { new ElementItem() }},
                {"Prop5", new string[]{ null, null } },
                {"Prop6", (string[]) null }
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
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetValue<int>(_elementDictionary, stringValue, out _);
                    }, "propertyName");
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

                value.Should().Be((double) _prop1["Prop1"]);
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
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetValue<int>(_elementDictionary, stringValue);
                    }, "propertyName");
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

        public class TryGetValues : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetValues<int>(null, Create<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetValues<int>(_elementDictionary, stringValue, out _);
                    }, "propertyName");
            }

            [Fact]
            public void Should_Return_True_When_Property_Exists()
            {
                var actual = ElementDictionaryExtensions.TryGetValues<double>(_elementDictionary, "Prop3", out _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Value_When_Property_Exists()
            {
                _ = ElementDictionaryExtensions.TryGetValues<int>(_elementDictionary, "Prop3", out var value);

                value.Should().BeEquivalentTo((List<int>) _prop1["Prop3"]);
            }

            [Fact]
            public void Should_Return_False_When_Property_Does_Not_Exist()
            {
                var actual = ElementDictionaryExtensions.TryGetValues<double>(_elementDictionary, Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Default_Value_When_Property_Does_Not_Exist()
            {
                _ = ElementDictionaryExtensions.TryGetValues<double>(_elementDictionary, Create<string>(), out var value);

                value.Should().BeNull();
            }

            [Fact]
            public void Should_Convert_Value_When_Can_Convert()
            {
                _ = ElementDictionaryExtensions.TryGetValues<string>(_elementDictionary, "Prop3", out var value);

                var prop3Values = (List<int>) _prop1["Prop3"];
                var expected = prop3Values.Select(item => $"{item}");

                value.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Value_When_Typed_Object()
            {
                _ = ElementDictionaryExtensions.TryGetValues<ElementItem>(_elementDictionary, "Prop4", out var value);

                value.Should().BeEquivalentTo((ElementItem[]) _prop1["Prop4"]);
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetValues<double[]>(_elementDictionary, "Prop3", out _);
                })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'Int32' to type 'Double[]'.");      // referring to each element
            }
        }

        public class GetValues : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetValues<int>(null, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetValues<int>(_elementDictionary, stringValue);
                    }, "propertyName");
            }

            [Fact]
            public void Should_Return_Value_When_Property_Exists()
            {
                var value = ElementDictionaryExtensions.GetValues<int>(_elementDictionary, "Prop3");

                value.Should().BeEquivalentTo((List<int>) _prop1["Prop3"]);
            }

            [Fact]
            public void Should_Throw_When_Property_Does_Not_Exist()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetValues<double>(_elementDictionary, propertyName);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propertyName} was not found.");
            }

            [Fact]
            public void Should_Convert_Value_When_Can_Convert()
            {
                var value = ElementDictionaryExtensions.GetValues<string>(_elementDictionary, "Prop3");

                var prop3Values = (List<int>) _prop1["Prop3"];
                var expected = prop3Values.Select(item => $"{item}");

                value.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Value_When_Typed_Object()
            {
                var value = ElementDictionaryExtensions.GetValues<ElementItem>(_elementDictionary, "Prop4");

                value.Should().BeEquivalentTo((ElementItem[]) _prop1["Prop4"]);
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetValues<double[]>(_elementDictionary, "Prop3");
                })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'Int32' to type 'Double[]'.");      // referring to each element
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
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, stringValue, out _);
                    }, "arrayPropertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Null_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetObjectArray(_elementDictionary, Create<string>(), out var array);

                array.Should().BeNull();
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
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArray(_elementDictionary, stringValue);
                    }, "arrayPropertyName");
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
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, stringValue, Create<string>(), out _);
                    }, "arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), stringValue, out _);
                    }, "propertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Null_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetObjectArrayValues<int>(_elementDictionary, Create<string>(), Create<string>(), out var array);

                array.Should().BeNull();
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
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, stringValue, Create<string>());
                    }, "arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetObjectArrayValues<int>(_elementDictionary, Create<string>(), stringValue);
                    }, "propertyName");
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

        public class TryGetManyObjectValues : ElementDictionaryExtensionsFixture
        {
            private readonly IEnumerable<IElementDictionary> _elements;

            public TryGetManyObjectValues()
            {
                _elements = _elementDictionary.GetObjectArray("Prop2").AsReadOnlyCollection();
            }

            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetManyObjectValues<int>(null, Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("elements");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetManyObjectValues<int>(new[] { _elementDictionary }, stringValue, out _);
                    }, "propertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetManyObjectValues<int>(_elements, Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Null_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetManyObjectValues<int>(_elements, Create<string>(), out var array);

                array.Should().BeNull();
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                // same as:
                //
                // var values = _elements.SelectToArray(element => element.GetValue<string>("Prop1"));

                _ = ElementDictionaryExtensions.TryGetManyObjectValues<string>(_elements, "Prop1", out var values);

                var array = values.ToList();

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
                        _ = ElementDictionaryExtensions.TryGetManyObjectValues<double[]>(_elements, "Prop1", out _);
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'string' to type 'double[]'.");
            }
        }

        public class GetManyObjectArrayValues : ElementDictionaryExtensionsFixture
        {
            private readonly IEnumerable<IElementDictionary> _elements;

            public GetManyObjectArrayValues()
            {
                _elements = _elementDictionary.GetObjectArray("Prop2").AsReadOnlyCollection();
            }

            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetManyObjectArrayValues<int>(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("elements");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetManyObjectArrayValues<int>(new[] { _elementDictionary }, stringValue);
                    }, "arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_found()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetManyObjectArrayValues<int>(_elements, propertyName);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propertyName} was not found.");
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                var elements = ElementDictionaryExtensions.GetManyObjectArrayValues<string>(_elements, "Prop1");

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
                    _ = ElementDictionaryExtensions.GetManyObjectArrayValues<double[]>(_elements, "Prop1");
                })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'string' to type 'double[]'.");
            }
        }

        public class TryGetDescendantObjectArray_Many : ElementDictionaryExtensionsFixture
        {
            private readonly IEnumerable<IElementDictionary> _elements;

            public TryGetDescendantObjectArray_Many()
            {
                _elements = _elementDictionary.GetObjectArray("Prop2").AsReadOnlyCollection();
            }

            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArray((IEnumerable<IElementDictionary>) null, CreateMany<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("elements");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(new[] { _elementDictionary }, null, out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Return_Null_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elements, CreateMany<string>(), out var array);

                array.Should().BeNull();
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elements, new[] { "Prop1" }, out _);
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
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elements, new[] { "Prop2", "Prop2", "Prop2" }, out _);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop2 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Array()
            {
                // See equivalent test in TryGetDescendantObjectArray_Single to see how the same can be navigated from the root _elementDictionary

                _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elements, new[] { "Prop2" }, out var elements);

                elements.Should().HaveCount(4);

                var element0 = elements.ElementAt(0);

                element0.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element0.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element0.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);
                element0.GetValue("Prop4").Should().Be(_prop3a["Prop4"]);

                var element1 = elements.ElementAt(1);

                element1.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element1.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element1.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);
                element1.GetValue("Prop4").Should().Be(_prop3b["Prop4"]);

                var element2 = elements.ElementAt(2);

                element2.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element2.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element2.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);
                element2.GetValue("Prop4").Should().Be(_prop3b["Prop4"]);

                var element3 = elements.ElementAt(3);

                element3.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element3.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element3.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);
                element3.GetValue("Prop4").Should().Be(_prop3a["Prop4"]);
            }

            [Fact]
            public void Should_Get_Array_2()
            {
                // See equivalent test in TryGetDescendantObjectArray_Single to see how the same can be navigated from the root _elementDictionary

                _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elements, new[] { "Prop2", "Prop4" }, out var elements);

                elements.Should().HaveCount(8);

                // _elements is Prop2 (on root), which contains _prop2a, _prop2b
                // _prop2a.Prop2 contains _prop3a, _prop3b
                // _prop2b.Prop2 contains _prop3b, _prop3a
                // _prop3a.Prop4 contains _prop4a, _prop4b
                // _prop3b.Prop4 contains _prop4b, _prop4a

                elements.ElementAt(0).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(1).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
                elements.ElementAt(2).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
                elements.ElementAt(3).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(4).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
                elements.ElementAt(5).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(6).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(7).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
            }
        }

        public class GetDescendantObjectArray_Many : ElementDictionaryExtensionsFixture
        {
            private readonly IEnumerable<IElementDictionary> _elements;

            public GetDescendantObjectArray_Many()
            {
                _elements = _elementDictionary.GetObjectArray("Prop2").AsReadOnlyCollection();
            }

            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArray((IEnumerable<IElementDictionary>) null, CreateMany<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("elements");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArray(new[] { _elementDictionary }, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_found()
            {
                var propertyNames = CreateMany<string>();

                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArray(_elements, propertyNames);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {string.Join(".", propertyNames)} was not found.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArray(_elements, new[] { "Prop1" });
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
                        _ = ElementDictionaryExtensions.GetDescendantObjectArray(_elements, new[] { "Prop2", "Prop2", "Prop2" });
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop2 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Array()
            {
                var elements = ElementDictionaryExtensions.GetDescendantObjectArray(_elements, new[] { "Prop2" });

                elements.Should().HaveCount(4);

                var element0 = elements.ElementAt(0);

                element0.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element0.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element0.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);

                var element1 = elements.ElementAt(1);

                element1.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element1.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element1.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);

                var element2 = elements.ElementAt(2);

                element2.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element2.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element2.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);

                var element3 = elements.ElementAt(3);

                element3.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element3.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element3.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);
            }
        }

        public class TryGetDescendantObjectArray_Single : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetDescendantObjectArray((IElementDictionary) null, CreateMany<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elementDictionary, null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Return_Null_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elementDictionary, CreateMany<string>(), out var array);

                array.Should().BeNull();
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elementDictionary, new[] { "Prop2", "Prop1" }, out _);
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
                    _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elementDictionary, new[] { "Prop2", "Prop2", "Prop2", "Prop2" }, out _);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop2 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Array()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elementDictionary, new[] { "Prop2", "Prop2" }, out var elements);

                elements.Should().HaveCount(4);

                var element0 = elements.ElementAt(0);

                element0.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element0.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element0.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);

                var element1 = elements.ElementAt(1);

                element1.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element1.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element1.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);

                var element2 = elements.ElementAt(2);

                element2.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element2.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element2.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);

                var element3 = elements.ElementAt(3);

                element3.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element3.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element3.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);
            }

            [Fact]
            public void Should_Get_Array_2()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArray(_elementDictionary, new[] { "Prop2", "Prop2", "Prop4" }, out var elements);

                elements.Should().HaveCount(8);

                // _elementDictionary.Prop2 which contains _prop2a, _prop2b
                // _prop2a.Prop2 contains _prop3a, _prop3b
                // _prop2b.Prop2 contains _prop3b, _prop3a
                // _prop3a.Prop4 contains _prop4a, _prop4b
                // _prop3b.Prop4 contains _prop4b, _prop4a

                elements.ElementAt(0).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(1).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
                elements.ElementAt(2).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
                elements.ElementAt(3).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(4).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
                elements.ElementAt(5).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(6).GetValue("Prop1").Should().Be(_prop4a["Prop1"]);
                elements.ElementAt(7).GetValue("Prop1").Should().Be(_prop4b["Prop1"]);
            }
        }

        public class GetDescendantObjectArray_Single : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArray((IElementDictionary) null, CreateMany<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArray(_elementDictionary, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_found()
            {
                var propertyNames = CreateMany<string>();

                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArray(_elementDictionary, propertyNames);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {string.Join(".", propertyNames)} was not found.");
            }

            [Fact]
            public void Should_Throw_When_Not_Array_Type()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArray(_elementDictionary, new[] { "Prop2", "Prop1" });
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
                    _ = ElementDictionaryExtensions.GetDescendantObjectArray(_elementDictionary, new[] { "Prop2", "Prop2", "Prop2", "Prop2" });
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop2 is not an array of objects.");
            }

            [Fact]
            public void Should_Get_Array()
            {
                var elements = ElementDictionaryExtensions.GetDescendantObjectArray(_elementDictionary, new[] { "Prop2", "Prop2" });

                elements.Should().HaveCount(4);

                var element0 = elements.ElementAt(0);

                element0.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element0.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element0.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);

                var element1 = elements.ElementAt(1);

                element1.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element1.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element1.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);

                var element2 = elements.ElementAt(2);

                element2.GetValue("Prop1").Should().Be(_prop3b["Prop1"]);
                element2.GetValue("Prop2").Should().Be(_prop3b["Prop2"]);
                element2.GetValue("Prop3").Should().Be(_prop3b["Prop3"]);

                var element3 = elements.ElementAt(3);

                element3.GetValue("Prop1").Should().Be(_prop3a["Prop1"]);
                element3.GetValue("Prop2").Should().Be(_prop3a["Prop2"]);
                element3.GetValue("Prop3").Should().Be(_prop3a["Prop3"]);
            }
        }

        public class TryGetDescendantObjectArrayValues_Many : ElementDictionaryExtensionsFixture
        {
            private readonly IEnumerable<IElementDictionary> _elements;

            public TryGetDescendantObjectArrayValues_Many()
            {
                _elements = _elementDictionary.GetObjectArray("Prop2").AsReadOnlyCollection();
            }

            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>((IEnumerable<IElementDictionary>) null, CreateMany<string>(), Create<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("elements");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(new[] { _elementDictionary }, null, Create<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(new[] { _elementDictionary }, CreateMany<string>(), stringValue, out _);
                    }, "childPropertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elements, CreateMany<string>(), Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Null_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elements, CreateMany<string>(), Create<string>(), out var array);

                array.Should().BeNull();
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elements, new[] { "Prop2" }, "Prop1", out var elements);

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    (int) _prop3a["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3a["Prop1"]
                });
            }

            [Fact]
            public void Should_Get_Converted_Object_Array_Values()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<string>(_elements, new[] { "Prop2" }, "Prop1", out var elements);

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    $"{_prop3a["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3a["Prop1"]}"
                });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<double[]>(_elements, new[] { "Prop2" }, "Prop1", out _);
                })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'Int32' to type 'double[]'.");
            }
        }

        public class GetDescendantObjectArrayValues_Many : ElementDictionaryExtensionsFixture
        {
            private readonly IEnumerable<IElementDictionary> _elements;

            public GetDescendantObjectArrayValues_Many()
            {
                _elements = _elementDictionary.GetObjectArray("Prop2").AsReadOnlyCollection();
            }

            [Fact]
            public void Should_Throw_When_Elements_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>((IEnumerable<IElementDictionary>) null, CreateMany<string>(), Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("elements");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(new[] { _elementDictionary }, null, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(new[] { _elementDictionary }, CreateMany<string>(), stringValue);
                    }, "childPropertyName");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_found()
            {
                var arrayPropertyNames = CreateMany<string>();
                var childPropertyName = Create<string>();

                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(_elements, arrayPropertyNames, childPropertyName);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {string.Join(".", arrayPropertyNames.Concat(new[] { childPropertyName }))} was not found.");
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                var elements = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(_elements, new[] { "Prop2" }, "Prop1");

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    (int) _prop3a["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3a["Prop1"]
                });
            }

            [Fact]
            public void Should_Get_Converted_Object_Array_Values()
            {
                var elements = ElementDictionaryExtensions.GetDescendantObjectArrayValues<string>(_elements, new[] { "Prop2" }, "Prop1");

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    $"{_prop3a["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3a["Prop1"]}"
                });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<double[]>(_elements, new[] { "Prop2" }, "Prop1");
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'Int32' to type 'double[]'.");
            }
        }

        public class TryGetDescendantObjectArrayValues_Single : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>((IElementDictionary) null, CreateMany<string>(), Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elementDictionary, null, Create<string>(), out _);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elementDictionary, CreateMany<string>(), stringValue, out _);
                    }, "childPropertyName");
            }

            [Fact]
            public void Should_Return_False_When_Property_Not_found()
            {
                var actual = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elementDictionary, CreateMany<string>(), Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Null_When_Property_Not_found()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elementDictionary, CreateMany<string>(), Create<string>(), out var array);

                array.Should().BeNull();
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<int>(_elementDictionary, new[] { "Prop2", "Prop2" }, "Prop1", out var elements);

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    (int) _prop3a["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3a["Prop1"]
                });
            }

            [Fact]
            public void Should_Get_Converted_Object_Array_Values()
            {
                _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<string>(_elementDictionary, new[] { "Prop2", "Prop2" }, "Prop1", out var elements);

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    $"{_prop3a["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3a["Prop1"]}"
                });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                    {
                        _ = ElementDictionaryExtensions.TryGetDescendantObjectArrayValues<double[]>(_elementDictionary, new[] { "Prop2", "Prop2" }, "Prop1", out _);
                    })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'Int32' to type 'double[]'.");
            }
        }

        public class GetDescendantObjectArrayValues_Single : ElementDictionaryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Element_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>((IElementDictionary) null, CreateMany<string>(), Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("element");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(_elementDictionary, null, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(_elementDictionary, CreateMany<string>(), stringValue);
                    }, "childPropertyName");
            }

            [Fact]
            public void Should_Throw_When_Property_Not_found()
            {
                var arrayPropertyNames = CreateMany<string>();
                var childPropertyName = Create<string>();

                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(_elementDictionary, arrayPropertyNames, childPropertyName);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {string.Join(".", arrayPropertyNames.Concat(new[] { childPropertyName }))} was not found.");
            }

            [Fact]
            public void Should_Get_Object_Array_Values()
            {
                var elements = ElementDictionaryExtensions.GetDescendantObjectArrayValues<int>(_elementDictionary, new[] { "Prop2", "Prop2" }, "Prop1");

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    (int) _prop3a["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3b["Prop1"],
                    (int) _prop3a["Prop1"]
                });
            }

            [Fact]
            public void Should_Get_Converted_Object_Array_Values()
            {
                var elements = ElementDictionaryExtensions.GetDescendantObjectArrayValues<string>(_elementDictionary, new[] { "Prop2", "Prop2" }, "Prop1");

                var array = elements.ToList();

                array.Should().BeEquivalentTo(new[]
                {
                    $"{_prop3a["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3b["Prop1"]}",
                    $"{_prop3a["Prop1"]}"
                });
            }

            [Fact]
            public void Should_Throw_When_Cannot_Convert_Value()
            {
                Invoking(() =>
                {
                    _ = ElementDictionaryExtensions.GetDescendantObjectArrayValues<double[]>(_elementDictionary, new[] { "Prop2", "Prop2" }, "Prop1");
                })
                    .Should()
                    .Throw<InvalidCastException>()
                    .WithMessage("Unable to cast object of type 'Int32' to type 'double[]'.");
            }
        }
    }
}