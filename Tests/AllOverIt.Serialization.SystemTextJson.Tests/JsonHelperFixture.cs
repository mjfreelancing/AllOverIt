using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.JsonHelper.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests
{
    public class JsonHelperFixture : FixtureBase
    {
        private readonly Guid _prop1;
        private readonly IReadOnlyCollection<int> _prop2;
        private readonly string[] _prop3;
        private readonly IReadOnlyCollection<string> _prop6;
        private readonly string _prop7;
        private readonly double _prop8;
        private readonly DateTime _prop9;
        private readonly object _value;

        protected JsonHelperFixture()
        {
            _prop1 = Guid.NewGuid();
            _prop2 = CreateMany<int>(3);
            _prop3 = CreateMany<string>().ToArray();
            _prop6 = CreateMany<string>(3);
            _prop7 = $"{Create<double>()}";
            _prop8 = Create<double>();

            _value = new
            {
                Prop1 = _prop1,
                Prop2 = new[]
                {
                    new
                    {
                        Prop2a = new[]
                        {
                            new
                            {
                                Prop2b = new[]
                                {
                                    new
                                    {
                                        Value = _prop2.ElementAt(0)
                                    }
                                }
                            },
                            new
                            {
                                Prop2b = new[]
                                {
                                     new
                                     { 
                                         Value = _prop2.ElementAt(1)
                                     }
                                }
                            },
                            new
                            {
                                Prop2b = new[]
                                {
                                     new
                                     {
                                         Value = _prop2.ElementAt(2)
                                     }
                                }
                            }
                        }
                    },
                    new
                    {
                        Prop2a = new[]
                        {
                            new
                            {
                                Prop2b = new[]
                                {
                                     new
                                     {
                                         Value = _prop2.ElementAt(0) * 2
                                     }
                                }
                            },
                            new
                            {
                                Prop2b = new[]
                                {
                                     new
                                     {
                                         Value = _prop2.ElementAt(1) * 2
                                     }
                                }
                            },
                            new
                            {
                                Prop2b = new[]
                                {
                                     new
                                     {
                                         Value = _prop2.ElementAt(2) * 2
                                     }
                                }
                            }
                        }
                    }
                },
                Prop3 = _prop3,
                Prop4 = new
                {
                    Prop5 = new[]
                    {
                        new
                        {
                            Prop6 = _prop6.ElementAt(0)
                        },
                        new
                        {
                            Prop6 = _prop6.ElementAt(1)
                        },
                        new
                        {
                            Prop6 = _prop6.ElementAt(2)
                        }
                    }
                },
                Prop7 = _prop7,
                Prop8 = _prop8,
                Prop9 = _prop9,
                Prop10 = new[]
                {
                    new
                    {
                        Prop11 = _prop6.ElementAt(0)
                    },
                    new
                    {
                        Prop11 = _prop6.ElementAt(1)
                    },
                    new
                    {
                        Prop11 = _prop6.ElementAt(2)
                    },
                }
            };
        }

        public class Constructor_Object : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                    {
                        _ = new JsonHelper((object) null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        _ = new JsonHelper(_value, null);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class Constructor_String : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                    {
                        _ = new JsonHelper((string) null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Throw_When_Value_Empty()
            {
                Invoking(() =>
                    {
                        _ = new JsonHelper(string.Empty);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("value");
            }

            [Fact]
            public void Should_Throw_When_Value_Whitespace()
            {
                Invoking(() =>
                    {
                        _ = new JsonHelper("  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Settings_Null()
            {
                Invoking(() =>
                    {
                        _ = new JsonHelper("{}", null);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class TryGetValue : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetValue(null, out _);
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
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetValue(string.Empty, out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_WhiteSpace()
            {
                Invoking(() =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetValue("  ", out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Theory]
            [InlineData(true, "Prop1")]
            [InlineData(true, "Prop7")]
            [InlineData(true, "Prop8")]
            [InlineData(true, "Prop9")]
            [InlineData(false, "Prop1")]
            [InlineData(false, "Prop7")]
            [InlineData(false, "Prop8")]
            [InlineData(false, "Prop9")]
            public void Should_Get_Value(bool useObject, string propName)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue(propName, out var value);

                var expected = propName switch
                {
                    "Prop1" => (object) $"{_prop1}",        // Guid type - will be interpreted as a string
                    "Prop7" => _prop7,                      // string type - looks like a double but will be interpreted as a string
                    "Prop8" => _prop8,                      // double type
                    "Prop9" => _prop9,                      // DateTime
                    _ => throw new InvalidExpressionException($"Unexpected property name {propName}")
                };

                actual.Should().BeTrue();
                value.Should().Be(expected);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue(Create<string>(), out var value);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Default_When_Cannot_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetValue(Create<string>(), out var value);

                value.Should().Be(default);
            }
        }

        public class GetValue : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetValue(null);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetValue(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetValue("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Theory]
            [InlineData(true, "Prop1")]
            [InlineData(true, "Prop7")]
            [InlineData(true, "Prop8")]
            [InlineData(false, "Prop1")]
            [InlineData(false, "Prop7")]
            [InlineData(false, "Prop8")]
            public void Should_Get_Value(bool useObject, string propName)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetValue(propName);

                var expected = propName switch
                {
                    "Prop1" => (object) $"{_prop1}",        // Guid type - will be interpreted as a string
                    "Prop7" => _prop7,                      // string type - looks like a double but will be interpreted as a string
                    "Prop8" => _prop8,                      // double type
                    _ => throw new InvalidExpressionException($"Unexpected property name {propName}")
                };

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Cannot_Get_Value(bool useObject)
            {
                var propName = Create<string>();

                Invoking(() =>
                    {
                        var jsonHelper = CreateJsonHelper(useObject);
                        _ = jsonHelper.GetValue(propName);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propName} was not found.");
            }
        }

        public class TryGetValue_Typed : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetValue<int>(null, out _);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetValue<int>(string.Empty, out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetValue<int>("  ", out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Guid_As_String(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue<string>("Prop1", out var value);

                actual.Should().BeTrue();
                value.Should().Be($"{_prop1}");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Guid_As_Guid(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue<Guid>("Prop1", out var value);

                actual.Should().BeTrue();
                value.Should().Be(_prop1);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_String_Looking_As_Double_As_String(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue<string>("Prop7", out var value);

                actual.Should().BeTrue();
                value.Should().Be(_prop7);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Double(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue<double>("Prop8", out var value);

                actual.Should().BeTrue();
                value.Should().Be(_prop8);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_DateTime(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue<DateTime>("Prop9", out var value);

                actual.Should().BeTrue();
                value.Should().Be(_prop9);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue<int>(Create<string>(), out var value);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Default_When_Cannot_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetValue<int>(Create<string>(), out var value);

                value.Should().Be(default);
            }
        }

        public class GetValue_Typed : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetValue<int>(null);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetValue<int>(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetValue<int>("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Guid_As_String(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetValue<string>("Prop1");

                actual.Should().Be($"{_prop1}");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Guid_As_Guid(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetValue<Guid>("Prop1");

                actual.Should().Be(_prop1);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_String_Looking_As_Double_As_String(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetValue<string>("Prop7");

                actual.Should().Be(_prop7);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Double(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetValue<double>("Prop8");

                actual.Should().Be(_prop8);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_DateTime(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetValue<DateTime>("Prop9");

                actual.Should().Be(_prop9);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Cannot_Get_Value(bool useObject)
            {
                var propName = Create<string>();

                Invoking(() =>
                    {
                        var jsonHelper = CreateJsonHelper(useObject);
                        _ = jsonHelper.GetValue<int>(propName);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propName} was not found.");
            }
        }

        public class TryGetObjectArray : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetObjectArray(null, out _);
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
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetObjectArray(string.Empty, out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_WhiteSpace()
            {
                Invoking(() =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetObjectArray("  ", out _);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Array_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetObjectArray(Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Empty_Array_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetObjectArray(Create<string>(), out var array);

                array.Should().BeEmpty();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Not_An_Array_Type(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                Invoking(() =>
                    {
                        _ = jsonHelper.TryGetObjectArray("Prop1", out _);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Not_Json_Objects(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                Invoking(() =>
                    {
                        _ = jsonHelper.TryGetObjectArray("Prop3", out _);
                    })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop3 is not an array of JSON objects.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Array(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetObjectArray("Prop10", out var array);

                actual.Should().BeTrue();

                var arrayItems = array.ToList();

                arrayItems.Should().HaveCount(3);

                for (var idx = 0; idx < 3; idx++)
                {
                    var item = arrayItems.ElementAt(idx);
                    var expectedValue = _prop6.ElementAt(idx);

                    item.GetValue("Prop11").Should().Be(expectedValue);
                }
            }
        }

        public class GetObjectArray : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArray(null);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArray(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArray("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);
                var propName = Create<string>();

                Invoking(() =>
                {
                    _ = jsonHelper.GetObjectArray(propName);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propName} was not found.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Not_An_Array_Type(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                Invoking(() =>
                {
                    _ = jsonHelper.GetObjectArray("Prop1");
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Not_Json_Objects(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                Invoking(() =>
                {
                    _ = jsonHelper.GetObjectArray("Prop3");
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop3 is not an array of JSON objects.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Array(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var array = jsonHelper.GetObjectArray("Prop10");

                var arrayItems = array.ToList();

                arrayItems.Should().HaveCount(3);

                for (var idx = 0; idx < 3; idx++)
                {
                    var item = arrayItems.ElementAt(idx);
                    var expectedValue = _prop6.ElementAt(idx);

                    item.GetValue("Prop11").Should().Be(expectedValue);
                }
            }
        }

        public class TryGetObjectArrayValues : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetObjectArrayValues<int>(null, Create<string>(), out _);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetObjectArrayValues<int>(string.Empty, Create<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetObjectArrayValues<int>("  ", Create<string>(), out _);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetObjectArrayValues<int>(Create<string>(), null, out _);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetObjectArrayValues<int>(Create<string>(), string.Empty, out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetObjectArrayValues<int>(Create<string>(), "  ", out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Find_Object_Array_Property(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetObjectArrayValues<string>("Prop10", "Prop11", out _);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Object_Array_Property_Is_Not_An_Array(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.TryGetObjectArrayValues<string>("Prop1", "Prop11", out _);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Object_Array_Property_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetObjectArrayValues<string>("Prop10", "Prop11", out var arrayValues);

                arrayValues.Should().BeEquivalentTo(_prop6);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Empty_Array_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetObjectArrayValues<string>("Prop10", "Prop1", out var arrayValues);

                arrayValues.Should().BeEmpty();
            }
        }

        public class GetObjectArrayValues : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArrayValues<int>(null, Create<string>());
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArrayValues<int>(string.Empty, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArrayValues<int>("  ", Create<string>());
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArrayValues<int>(Create<string>(), null);
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
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArrayValues<int>(Create<string>(), string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_WhiteSpace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetObjectArrayValues<int>(Create<string>(), "  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Object_Array_Property_Is_Not_An_Array(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.GetObjectArrayValues<string>("Prop1", "Prop11");
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Property_Not_Found(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.GetObjectArrayValues<string>("Prop10", "Prop1");
                })
                   .Should()
                   .Throw<JsonHelperException>()
                   .WithMessage("The property Prop10.Prop1 was not found.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Object_Array_Property_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var arrayValues = jsonHelper.GetObjectArrayValues<string>("Prop10", "Prop11");

                arrayValues.Should().BeEquivalentTo(_prop6);
            }
        }

        public class TryGetDescendantObjectArray : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantObjectArray(null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantObjectArray(new string[] { }, out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyNames");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Find_Descendant_Object_Array_Property(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantObjectArray(new[] { "Prop10" }, out _);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Descendant_Object_Array_Property_Is_Not_An_Array(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.TryGetDescendantObjectArray(new[] { "Prop1" }, out _);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Descendant_Object_Array_Property(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantObjectArray(new[] { "Prop2", "Prop2a" }, out var array);

                var arrayItems = array.ToList();

                arrayItems.Should().HaveCount(6);

                for (var idx = 0; idx < 6; idx++)
                {
                    var item = arrayItems.ElementAt(idx);
                    var expectedValue = _prop2.ElementAt(idx % 3);

                    if (idx > 2)
                    {
                        expectedValue *= 2;
                    }

                    var element = (IList<object>) item.GetValue("Prop2b");

                    var expected = new[]
                    {
                        new Dictionary<string, object>
                        {
                            { "Value", expectedValue }
                        }
                    };

                    expected.Should().BeEquivalentTo(element);
                }
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_False_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantObjectArray(new[] { "Prop0" }, out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Empty_Array_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantObjectArray(new[] { "Prop0" }, out var array);

                array.Should().BeEmpty();
            }
        }

        public class GetDescendantObjectArray : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArray(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArray(new string[] { });
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyNames");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Descendant_Object_Array_Property_Is_Not_An_Array(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.GetDescendantObjectArray(new[] { "Prop1" });
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Descendant_Object_Array_Property(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var array = jsonHelper.GetDescendantObjectArray(new[] { "Prop2", "Prop2a" });

                var arrayItems = array.ToList();

                arrayItems.Should().HaveCount(6);

                for (var idx = 0; idx < 6; idx++)
                {
                    var item = arrayItems.ElementAt(idx);
                    var expectedValue = _prop2.ElementAt(idx % 3);

                    if (idx > 2)
                    {
                        expectedValue *= 2;
                    }

                    var element = (IList<object>) item.GetValue("Prop2b");

                    var expected = new[]
                    {
                        new Dictionary<string, object>
                        {
                            { "Value", expectedValue }
                        }
                    };

                    expected.Should().BeEquivalentTo(element);
                }
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Property_Not_Found(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    var array = jsonHelper.GetDescendantObjectArray(new[] { "Prop0" });
                })
                   .Should()
                   .Throw<JsonHelperException>()
                   .WithMessage("The property Prop0 was not found.");
            }
        }

        public class TryGetDescendantObjectArrayValues : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(null, Create<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(new string[] { }, Create<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(CreateMany<string>(), null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("childPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(CreateMany<string>(), string.Empty, out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("childPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Whitespace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(CreateMany<string>(), "  ", out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("childPropertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Descendant_Object_Array_Property_Is_Not_An_Array(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(new[] { "Prop1" }, Create<string>(), out _);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Descendant_Object_Array_Values(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(new[] { "Prop2", "Prop2a", "Prop2b" }, "Value", out var array);

                var arrayItems = array.ToList();

                var expected = _prop2.Concat(_prop2.Select(item => item * 2));

                arrayItems.Should().BeEquivalentTo(expected);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_False_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantObjectArrayValues<int>(new[] { "Prop0" }, "Value", out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Empty_Array_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(new[] { "Prop0" }, "Value", out var array);

                array.Should().BeEmpty();
            }
        }

        public class GetDescendantObjectArrayValues : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(null, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ArrayPropertyNames_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(new string[] { }, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(CreateMany<string>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("childPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(CreateMany<string>(), string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("childPropertyName");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Whitespace()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(CreateMany<string>(), "  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("childPropertyName");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Descendant_Object_Array_Property_Is_Not_An_Array(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(new[] { "Prop1" }, Create<string>());
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage("The property Prop1 is not an array type.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Descendant_Object_Array_Values(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var array = jsonHelper.GetDescendantObjectArrayValues<int>(new[] { "Prop2", "Prop2a", "Prop2b" }, "Value");

                var arrayItems = array.ToList();

                var expected = _prop2.Concat(_prop2.Select(item => item * 2));

                arrayItems.Should().BeEquivalentTo(expected);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Property_Not_Found(bool useObject)
            {
                var childPropName = Create<string>();

                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    var array = jsonHelper.GetDescendantObjectArrayValues<int>(new[] { "Prop0", "Prop1" }, childPropName);
                })
                   .Should()
                   .Throw<JsonHelperException>()
                   .WithMessage($"The property Prop0.Prop1.{childPropName} was not found.");
            }
        }

        private JsonHelper CreateJsonHelper(bool useObject)
        {
            if (useObject)
            {
                return new JsonHelper(_value);
            }

            var serializer = new SystemTextJsonSerializer();
            var strValue = serializer.SerializeObject(_value);

            return new JsonHelper(strValue);
        }
    }
}