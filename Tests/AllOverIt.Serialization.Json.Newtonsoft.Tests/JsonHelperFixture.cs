using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using FluentAssertions;
using Newtonsoft.Json;
using System.Data;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests
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
        private readonly IReadOnlyCollection<int> _prop11;
        private readonly IReadOnlyCollection<string> _prop12;
        private readonly IReadOnlyCollection<Guid> _prop13;
        private readonly string[] _prop14;
        private readonly string[] _prop15;
        private readonly object _value;

        protected JsonHelperFixture()
        {
            _prop1 = Guid.NewGuid();
            _prop2 = CreateMany<int>(3);
            _prop3 = CreateMany<string>().ToArray();
            _prop6 = CreateMany<string>(3);
            _prop7 = $"{Create<double>()}";
            _prop8 = Create<double>();
            _prop9 = DateTime.Now;
            _prop11 = CreateMany<int>(3);
            _prop12 = _prop11.SelectToReadOnlyCollection(item => $"{item}");
            _prop13 = CreateMany<Guid>(3);
            _prop14 = [Create<string>(), null, Create<string>()];
            _prop15 = null;

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
                },
                Prop11 = _prop11,
                Prop12 = _prop12,
                Prop13 = _prop13,
                Prop14 = _prop14,
                Prop15 = _prop15,
                Prop16 = new
                {
                    Prop17 = new
                    {
                        Prop18 = new
                        {
                            Prop19 = _prop9
                        }
                    }
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
            public void Should_Throw_When_Value_Null_Empty_Whitespace()
            {
                var value = "{768:\"0000\",\"evt\":{\"handle\":\"000e\",\"type\":\"charValue\",\"uuid\":\"65333333-a115-11e2-9e9a-0800200ca101\"}}";

                var helper = new JsonHelper(value);

                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = new JsonHelper(stringValue);
                    }, "value");
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
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetValue(stringValue, out _);
                    }, "propertyName");
            }

            [Theory]
            [InlineData(false, "Prop1", false)]
            [InlineData(false, "Prop1", true)]
            [InlineData(true, "Prop1", false)]
            [InlineData(true, "Prop1", true)]
            [InlineData(false, "Prop7", false)]
            [InlineData(false, "Prop7", true)]
            [InlineData(true, "Prop7", false)]
            [InlineData(true, "Prop7", true)]
            [InlineData(false, "Prop8", false)]
            [InlineData(false, "Prop8", true)]
            [InlineData(true, "Prop8", false)]
            [InlineData(true, "Prop8", true)]
            [InlineData(false, "Prop9", false)]
            [InlineData(false, "Prop9", true)]
            [InlineData(true, "Prop9", false)]
            [InlineData(true, "Prop9", true)]
            [InlineData(false, "Prop14", false)]
            [InlineData(false, "Prop14", true)]
            [InlineData(true, "Prop14", false)]
            [InlineData(true, "Prop14", true)]
            [InlineData(false, "Prop15", false)]
            [InlineData(false, "Prop15", true)]
            [InlineData(true, "Prop15", false)]
            [InlineData(true, "Prop15", true)]
            public void Should_Get_Value(bool useObject, string propName, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue(caseSensitive ? propName : propName.ToLower(), out var value);

                var expected = propName switch
                {
                    "Prop1" => (object) $"{_prop1}",        // Guid type - will be interpreted as a string
                    "Prop7" => _prop7,                      // string type - looks like a double but will be interpreted as a string
                    "Prop8" => _prop8,                      // double type
                    "Prop9" => _prop9,                      // DateTime
                    "Prop14" => _prop14,                    // string[]
                    "Prop15" => _prop15,                    // (string[])null
                    _ => throw new InvalidExpressionException($"Unexpected property name {propName}")
                };

                actual.Should().BeTrue();
                value.Should().BeEquivalentTo(expected);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue(Create<string>(), out _);

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
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.GetValue(stringValue);
                    }, "propertyName");
            }

            [Theory]
            [InlineData(false, "Prop1", false)]
            [InlineData(false, "Prop1", true)]
            [InlineData(true, "Prop1", false)]
            [InlineData(true, "Prop1", true)]
            [InlineData(false, "Prop7", false)]
            [InlineData(false, "Prop7", true)]
            [InlineData(true, "Prop7", false)]
            [InlineData(true, "Prop7", true)]
            [InlineData(false, "Prop8", false)]
            [InlineData(false, "Prop8", true)]
            [InlineData(true, "Prop8", false)]
            [InlineData(true, "Prop8", true)]
            public void Should_Get_Value(bool useObject, string propName, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetValue(caseSensitive ? propName : propName.ToLower());

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
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetValue<int>(stringValue, out _);
                    }, "propertyName");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValue<string>("Prop1", out var value)
                    : jsonHelper.TryGetValue("Prop1", out value);

                actual.Should().BeTrue();
                value.Should().Be($"{_prop1}");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_Guid(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValue<Guid>("Prop1", out var value)
                    : jsonHelper.TryGetValue("prop1", out value);

                actual.Should().BeTrue();
                value.Should().Be(_prop1);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_String_Looking_As_Double_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValue<string>("Prop7", out var value)
                    : jsonHelper.TryGetValue("prop7", out value);

                actual.Should().BeTrue();
                value.Should().Be(_prop7);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Double(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValue<double>("Prop8", out var value)
                    : jsonHelper.TryGetValue("prop8", out value);

                actual.Should().BeTrue();
                value.Should().Be(_prop8);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_DateTime(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValue<DateTime>("Prop9", out var value)
                    : jsonHelper.TryGetValue("prop9", out value);

                actual.Should().BeTrue();
                value.Should().Be(_prop9);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValue<int>(Create<string>(), out _);

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
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.GetValue<int>(stringValue);
                    }, "propertyName");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValue<string>("Prop1")
                    : jsonHelper.GetValue<string>("prop1");

                actual.Should().Be($"{_prop1}");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_Guid(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValue<Guid>("Prop1")
                    : jsonHelper.GetValue<Guid>("prop1");

                actual.Should().Be(_prop1);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_String_Looking_As_Double_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValue<string>("Prop7")
                    : jsonHelper.GetValue<string>("prop7");

                actual.Should().Be(_prop7);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Double(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValue<double>("Prop8")
                    : jsonHelper.GetValue<double>("prop8");

                actual.Should().Be(_prop8);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_DateTime(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValue<DateTime>("Prop9")
                    : jsonHelper.GetValue<DateTime>("prop9");

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

        public class TryGetValues_Typed : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetValues<int>(stringValue, out _);
                    }, "propertyName");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValues<string>("Prop13", out var values)
                    : jsonHelper.TryGetValues("Prop13", out values);

                actual.Should().BeTrue();
                values.Should().BeEquivalentTo(_prop13.Select(item => $"{item}"));
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_Guid(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValues<Guid>("Prop13", out var value)
                    : jsonHelper.TryGetValues("prop13", out value);

                actual.Should().BeTrue();
                value.Should().BeEquivalentTo(_prop13);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_String_Looking_As_Double_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValues<string>("Prop12", out var values)
                    : jsonHelper.TryGetValues("prop12", out values);

                actual.Should().BeTrue();
                values.Should().BeEquivalentTo(_prop12);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Int(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetValues<int>("Prop11", out var value)
                    : jsonHelper.TryGetValues("prop11", out value);

                actual.Should().BeTrue();
                value.Should().BeEquivalentTo(_prop11);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetValues<int>(Create<string>(), out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Default_When_Cannot_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetValues<int>(Create<string>(), out var value);

                value.Should().BeNull();
            }
        }

        public class GetValues_Typed : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.GetValues<int>(stringValue);
                    }, "propertyName");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValues<string>("Prop13")
                    : jsonHelper.GetValues<string>("prop13");

                actual.Should().BeEquivalentTo(_prop13.Select(item => $"{item}"));
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Guid_As_Guid(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValues<Guid>("Prop13")
                    : jsonHelper.GetValues<Guid>("prop13");

                actual.Should().BeEquivalentTo(_prop13);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_String_Looking_As_Double_As_String(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValues<string>("Prop12")
                    : jsonHelper.GetValues<string>("prop12");

                actual.Should().BeEquivalentTo(_prop12);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Int(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.GetValues<int>("Prop11")
                    : jsonHelper.GetValues<int>("prop11");

                actual.Should().BeEquivalentTo(_prop11);
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
                    _ = jsonHelper.GetValues<int>(propName);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property {propName} was not found.");
            }
        }

        public class TryGetDescendantElement : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantElement(null!, out _);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_PropertyNames_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantElement([], out _);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Element_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantElement(["Prop16", "Prop17", "Prop18", "Prop18"], out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Element_When_Not_Element(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                // Prop19 is a leaf node
                var actual = jsonHelper.TryGetDescendantElement(["Prop16", "Prop17", "Prop18", "Prop19"], out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Null_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                // Prop19 is a leaf node
                _ = jsonHelper.TryGetDescendantElement(["Prop16", "Prop17", "Prop18", "Prop19"], out var element);

                element.Should().BeNull();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Element(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                // Prop19 is a leaf node
                var actual = jsonHelper.TryGetDescendantElement(["Prop16", "Prop17", "Prop18"], out var element);

                actual.Should().BeTrue();

                element.Should().BeEquivalentTo(new Dictionary<string, object>()
                {
                    { "Prop19", _prop9 }
                });
            }
        }




        public class GetDescendantElement : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantElement(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_PropertyNames_Empty()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantElement([]);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Element_Not_Found(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);
                    _ = jsonHelper.GetDescendantElement(["Prop16", "Prop17", "Prop18", "Prop18"]);
                })
                   .Should()
                   .Throw<JsonHelperException>()
                   .WithMessage($"The element Prop16.Prop17.Prop18.Prop18 was not found.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Element_Not_Object(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    // Prop19 is a leaf node
                    _ = jsonHelper.GetDescendantElement(["Prop16", "Prop17", "Prop18", "Prop19"]);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The element Prop16.Prop17.Prop18.Prop19 was not found.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Element(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                // Prop19 is a leaf node
                var actual = jsonHelper.GetDescendantElement(["Prop16", "Prop17", "Prop18"]);

                actual.Should().BeEquivalentTo(new Dictionary<string, object>()
                {
                    { "Prop19", _prop9 }
                });
            }
        }

        public class TryGetDescendantValue : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantValue(null!, out _);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_One_Property_Name()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantValue([Create<string>()], out _);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Expected at least two property names. (Parameter 'propertyNames')");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Value_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantValue([Create<string>(), Create<string>()], out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Default_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantValue([Create<string>(), Create<string>()], out var element);

                element.Should().BeNull();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Element(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantValue(["Prop16", "Prop17", "Prop18", "Prop19"], out var element);

                actual.Should().BeTrue();

                element.Should().Be(_prop9);
            }
        }

        public class GetDescendantValue : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantValue(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_One_Property_Name()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantValue([Create<string>()]);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Expected at least two property names. (Parameter 'propertyNames')");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Property_Not_Found(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.GetDescendantValue(["Prop16", "Prop17", "Prop18", "Prop18"]);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property Prop16.Prop17.Prop18.Prop18 was not found.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetDescendantValue(["Prop16", "Prop17", "Prop18", "Prop19"]);

                actual.Should().Be(_prop9);
            }
        }

        public class TryGetDescendantValue_Typed : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantValue<DateTime>(null!, out _);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_One_Property_Name()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.TryGetDescendantValue<DateTime>([Create<string>()], out _);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Expected at least two property names. (Parameter 'propertyNames')");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Get_Value_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantValue<DateTime>([Create<string>(), Create<string>()], out _);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Default_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantValue<DateTime?>([Create<string>(), Create<string>()], out var element);

                element.Should().BeNull();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Element(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.TryGetDescendantValue<DateTime>(["Prop16", "Prop17", "Prop18", "Prop19"], out var element);

                actual.Should().BeTrue();

                element.Should().Be(_prop9);
            }
        }

        public class GetDescendantValue_Typed : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyNames_Null()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantValue<DateTime>(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_One_Property_Name()
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantValue<DateTime>([Create<string>()]);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Expected at least two property names. (Parameter 'propertyNames')");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Property_Not_Found(bool useObject)
            {
                Invoking(() =>
                {
                    var jsonHelper = CreateJsonHelper(useObject);

                    _ = jsonHelper.GetDescendantValue<DateTime>(["Prop16", "Prop17", "Prop18", "Prop18"]);
                })
                    .Should()
                    .Throw<JsonHelperException>()
                    .WithMessage($"The property Prop16.Prop17.Prop18.Prop18 was not found.");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Value(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = jsonHelper.GetDescendantValue<DateTime>(["Prop16", "Prop17", "Prop18", "Prop19"]);

                actual.Should().Be(_prop9);
            }
        }

        public class TryGetObjectArray : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetObjectArray(stringValue, out _);
                    }, "arrayPropertyName");
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
            public void Should_Return_Null_When_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetObjectArray(Create<string>(), out var array);

                array.Should().BeNull();
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
                    .WithMessage("The property Prop3 is not an array of objects.");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Array(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetObjectArray("Prop10", out var array)
                    : jsonHelper.TryGetObjectArray("prop10", out array);

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
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.GetObjectArray(stringValue);
                    }, "arrayPropertyName");
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
                    .WithMessage("The property Prop3 is not an array of objects.");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Array(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var array = caseSensitive
                    ? jsonHelper.GetObjectArray("Prop10")
                    : jsonHelper.GetObjectArray("prop10");

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
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetObjectArrayValues<int>(stringValue, Create<string>(), out _);
                    }, "arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetObjectArrayValues<int>(Create<string>(), stringValue, out _);
                    }, "propertyName");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Find_Object_Array_Property(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetObjectArrayValues<string>("Prop10", "Prop11", out _)
                    : jsonHelper.TryGetObjectArrayValues<string>("prop10", "prop11", out _);

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
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Object_Array_Property_Value(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = caseSensitive
                    ? jsonHelper.TryGetObjectArrayValues<string>("Prop10", "Prop11", out var arrayValues)
                    : jsonHelper.TryGetObjectArrayValues("prop10", "prop11", out arrayValues);

                arrayValues.Should().BeEquivalentTo(_prop6);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Return_Null_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetObjectArrayValues<string>("Prop10", "Prop1", out var arrayValues);

                arrayValues.Should().BeNull();
            }
        }

        public class GetObjectArrayValues : JsonHelperFixture
        {
            [Fact]
            public void Should_Throw_When_ArrayPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.GetObjectArrayValues<int>(stringValue, Create<string>());
                    }, "arrayPropertyName");
            }

            [Fact]
            public void Should_Throw_When_PropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.GetObjectArrayValues<int>(Create<string>(), stringValue);
                    }, "propertyName");
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
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Object_Array_Property_Value(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var arrayValues = caseSensitive
                    ? jsonHelper.GetObjectArrayValues<string>("Prop10", "Prop11")
                    : jsonHelper.GetObjectArrayValues<string>("prop10", "prop11");

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
                    _ = jsonHelper.TryGetDescendantObjectArray(Array.Empty<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyNames");
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Find_Descendant_Object_Array_Property(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var actual = caseSensitive
                    ? jsonHelper.TryGetDescendantObjectArray(new[] { "Prop10" }, out _)
                    : jsonHelper.TryGetDescendantObjectArray(new[] { "prop10" }, out _);

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
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Descendant_Object_Array_Property(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = caseSensitive
                    ? jsonHelper.TryGetDescendantObjectArray(new[] { "Prop2", "Prop2a" }, out var array)
                    : jsonHelper.TryGetDescendantObjectArray(new[] { "prop2", "prop2a" }, out array);

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
            public void Should_Return_Null_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantObjectArray(new[] { "Prop0" }, out var array);

                array.Should().BeNull();
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
                    _ = jsonHelper.GetDescendantObjectArray(Array.Empty<string>());
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
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Descendant_Object_Array_Property(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var array = caseSensitive
                    ? jsonHelper.GetDescendantObjectArray(new[] { "Prop2", "Prop2a" })
                    : jsonHelper.GetDescendantObjectArray(new[] { "prop2", "prop2a" });

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
                    _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(Array.Empty<string>(), Create<string>(), out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var jsonHelper = CreateJsonHelper(true);
                        _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(CreateMany<string>(), stringValue, out _);
                    }, "childPropertyName");
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
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Descendant_Object_Array_Values(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = caseSensitive
                    ? jsonHelper.TryGetDescendantObjectArrayValues<int>(new[] { "Prop2", "Prop2a", "Prop2b" }, "Value", out var array)
                    : jsonHelper.TryGetDescendantObjectArrayValues(new[] { "prop2", "prop2a", "prop2b" }, "value", out array);

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
            public void Should_Return_Null_When_Property_Not_Found(bool useObject)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                _ = jsonHelper.TryGetDescendantObjectArrayValues<int>(new[] { "Prop0" }, "Value", out var array);

                array.Should().BeNull();
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
                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(Array.Empty<string>(), Create<string>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("arrayPropertyNames");
            }

            [Fact]
            public void Should_Throw_When_ChildPropertyName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                {
                    var jsonHelper = CreateJsonHelper(true);
                    _ = jsonHelper.GetDescendantObjectArrayValues<int>(CreateMany<string>(), stringValue);
                }, "childPropertyName");
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
            [InlineData(true, true)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void Should_Get_Descendant_Object_Array_Values(bool useObject, bool caseSensitive)
            {
                var jsonHelper = CreateJsonHelper(useObject);

                var array = caseSensitive
                    ? jsonHelper.GetDescendantObjectArrayValues<int>(new[] { "Prop2", "Prop2a", "Prop2b" }, "Value")
                    : jsonHelper.GetDescendantObjectArrayValues<int>(new[] { "prop2", "prop2a", "prop2b" }, "value");

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
            var converter = new NestedDictionaryConverter();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);

            if (useObject)
            {
                return new JsonHelper(_value, settings);
            }

            var serializer = new NewtonsoftJsonSerializer();
            var strValue = serializer.SerializeObject(_value);

            return new JsonHelper(strValue, settings);
        }
    }
}