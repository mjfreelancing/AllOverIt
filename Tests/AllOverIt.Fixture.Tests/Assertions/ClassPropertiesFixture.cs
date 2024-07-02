#nullable enable

using AllOverIt.Extensions;
using AllOverIt.Fixture.Assertions;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Fixture.Tests
{
    public class ClassProperties_Generic_Fixture : FixtureBase
    {
        private abstract class DummyClassBase
        {
            public int Prop1 { set { } }
            public int Prop2 { get; private set; }
            public int Prop3 { get; internal set; }
            public int Prop4 { get; init; }
            protected abstract int Prop5 { get; init; }
            private int Prop6 { get; init; }
        }

        private class DummyClass : DummyClassBase
        {
            protected override int Prop5 { get; init; }
            public int Prop7 { set { } }
            public int Prop8 { get; private set; }
            public int Prop9 { get; internal set; }
            public int Prop10 { get; init; }
            private int Prop11 { get; init; }
        }

        private readonly ClassProperties<DummyClass> _notDeclaredOnlyProperties = new(false);
        private readonly ClassProperties<DummyClass> _declaredOnlyProperties = new(true);

        public class Constructor : ClassProperties_Generic_Fixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Get_All_Properties(bool declaredOnly)
            {
                if (declaredOnly)
                {
                    _declaredOnlyProperties.Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop5", "Prop7", "Prop8", "Prop9", "Prop10", "Prop11"]);
                }
                else
                {
                    _notDeclaredOnlyProperties.Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop1", "Prop2", "Prop3", "Prop4", "Prop5", "Prop6", "Prop7", "Prop8", "Prop9", "Prop10", "Prop11"]);
                }
            }
        }

        public class Including_Names : ClassProperties_Generic_Fixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Including((string[]) null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including((string[]) null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Empty(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Including(Array.Empty<string>());
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including(Array.Empty<string>());
                    }
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                // Unlike the expression based approach, this string based overload works with properties that do not have getters.
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                        .Including("Prop1", "Prop7")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop7"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Including("Prop1", "Prop7")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop1", "Prop7"]);
                }
            }

            [Fact]
            public void Should_Filter_Previous_Calls()
            {
                _notDeclaredOnlyProperties
                    .Including("Prop1", "Prop7")
                    .Including("Prop1")
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop1"]);
            }

            [Fact]
            public void Should_Ignore_Unknown_Properties()
            {
                _notDeclaredOnlyProperties
                    .Including("Prop1", "Prop7")
                    .Including("Prop1", Create<string>())
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop1"]);
            }
        }

        public class Including_Expressions : ClassProperties_Generic_Fixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Including((Expression<Func<DummyClass, object>>[]) null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including((Expression<Func<DummyClass, object>>[]) null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("properties");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Empty(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Including(Array.Empty<Expression<Func<DummyClass, object>>>());
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including(Array.Empty<Expression<Func<DummyClass, object>>>());
                    }
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("properties");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                // The expression based approach only works with properties that have getters.
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                        .Including(model => model.Prop4, model => model.Prop8)
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop8"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Including(model => model.Prop4, model => model.Prop8)
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop4", "Prop8"]);
                }
            }

            [Fact]
            public void Should_Filter_Previous_Calls()
            {
                _notDeclaredOnlyProperties
                    .Including(model => model.Prop4, model => model.Prop8)
                    .Including(model => model.Prop4)
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop4"]);
            }
        }

        public class Excluding_Expressions : ClassProperties_Generic_Fixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Excluding((Expression<Func<DummyClass, object>>[]) null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding((Expression<Func<DummyClass, object>>[]) null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("properties");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Empty(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Excluding(Array.Empty<Expression<Func<DummyClass, object>>>());
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding(Array.Empty<Expression<Func<DummyClass, object>>>());
                    }
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("properties");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                         .Excluding(model => model.Prop2, model => model.Prop3, model => model.Prop8)
                         .Properties
                         .Select(propInfo => propInfo.Name)
                         .Should()
                         .BeEquivalentTo(["Prop5", "Prop7", "Prop9", "Prop10", "Prop11"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Excluding(model => model.Prop2, model => model.Prop3, model => model.Prop8)
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop1", "Prop4", "Prop5", "Prop6", "Prop7", "Prop9", "Prop10", "Prop11"]);
                }
            }

            [Fact]
            public void Should_Filter_Previous_Calls()
            {
                _notDeclaredOnlyProperties
                        .Excluding(model => model.Prop2, model => model.Prop3, model => model.Prop8)
                        .Excluding(model => model.Prop4)
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop1", "Prop5", "Prop6", "Prop7", "Prop9", "Prop10", "Prop11"]);
            }
        }

        public class Excluding_Names : ClassProperties_Generic_Fixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Excluding((string[]) null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding((string[]) null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Empty(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Excluding(Array.Empty<string>());
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding(Array.Empty<string>());
                    }
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                        .Excluding("Prop1", "Prop7", "Prop9")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop5", "Prop8", "Prop10", "Prop11"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Excluding("Prop1", "Prop7", "Prop9")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop2", "Prop3", "Prop4", "Prop5", "Prop6", "Prop8", "Prop10", "Prop11"]);
                }
            }

            [Fact]
            public void Should_Filter_Previous_Calls()
            {
                _notDeclaredOnlyProperties
                        .Excluding("Prop1", "Prop7", "Prop9")
                        .Excluding("Prop5")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop2", "Prop3", "Prop4", "Prop6", "Prop8", "Prop10", "Prop11"]);
            }
        }

        public class Where : ClassProperties_Generic_Fixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Predicate_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Where(null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Where(null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("predicate");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                        .Where(propInfo => propInfo.IsAbstract() || propInfo.IsInitOnly())
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop5", "Prop10", "Prop11"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Where(propInfo => propInfo.IsAbstract() || propInfo.IsInitOnly())
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop4", "Prop5", "Prop6", "Prop10", "Prop11"]);
                }
            }
        }
    }

    public class ClassPropertiesFixture : FixtureBase
    {
        private abstract class DummyClassBase
        {
            public int Prop1 { set { } }
            public int Prop2 { get; private set; }
            public int Prop3 { get; internal set; }
            public int Prop4 { get; init; }
            protected abstract int Prop5 { get; init; }
            private int Prop6 { get; init; }
        }

        private class DummyClass : DummyClassBase
        {
            protected override int Prop5 { get; init; }
            public int Prop7 { set { } }
            public int Prop8 { get; private set; }
            public int Prop9 { get; internal set; }
            public int Prop10 { get; init; }
            private int Prop11 { get; init; }
        }

        private readonly ClassProperties _notDeclaredOnlyProperties = new(typeof(DummyClass), false);
        private readonly ClassProperties _declaredOnlyProperties = new(typeof(DummyClass), true);

        public class Constructor : ClassPropertiesFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Get_All_Properties(bool declaredOnly)
            {
                if (declaredOnly)
                {
                    _declaredOnlyProperties.Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop5", "Prop7", "Prop8", "Prop9", "Prop10", "Prop11"]);
                }
                else
                {
                    _notDeclaredOnlyProperties.Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop1", "Prop2", "Prop3", "Prop4", "Prop5", "Prop6", "Prop7", "Prop8", "Prop9", "Prop10", "Prop11"]);
                }
            }
        }

        public class Including_Names : ClassPropertiesFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Including((string[]) null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including((string[]) null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Empty(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Including(Array.Empty<string>());
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including(Array.Empty<string>());
                    }
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                // Unlike the expression based approach, this string based overload works with properties that do not have getters.
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                        .Including("Prop1", "Prop7")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop7"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Including("Prop1", "Prop7")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop1", "Prop7"]);
                }
            }

            [Fact]
            public void Should_Filter_Previous_Calls()
            {
                _notDeclaredOnlyProperties
                    .Including("Prop1", "Prop7")
                    .Including("Prop1")
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop1"]);
            }

            [Fact]
            public void Should_Ignore_Unknown_Properties()
            {
                _notDeclaredOnlyProperties
                    .Including("Prop1", "Prop7")
                    .Including("Prop1", Create<string>())
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop1"]);
            }
        }

        public class Excluding_Names : ClassPropertiesFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Excluding((string[]) null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding((string[]) null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Property_Names_Empty(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Excluding(Array.Empty<string>());
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding(Array.Empty<string>());
                    }
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                        .Excluding("Prop1", "Prop7", "Prop9")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop5", "Prop8", "Prop10", "Prop11"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Excluding("Prop1", "Prop7", "Prop9")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop2", "Prop3", "Prop4", "Prop5", "Prop6", "Prop8", "Prop10", "Prop11"]);
                }
            }

            [Fact]
            public void Should_Filter_Previous_Calls()
            {
                _notDeclaredOnlyProperties
                        .Excluding("Prop1", "Prop7", "Prop9")
                        .Excluding("Prop5")
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop2", "Prop3", "Prop4", "Prop6", "Prop8", "Prop10", "Prop11"]);
            }
        }

        public class Where : ClassPropertiesFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Predicate_Null(bool declaredOnly)
            {
                Invoking(() =>
                {
                    if (declaredOnly)
                    {
                        _ = _declaredOnlyProperties.Where(null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Where(null!);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("predicate");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Filter_Properties(bool declaredOnly)
            {
                if (declaredOnly)
                {
                    _declaredOnlyProperties
                        .Where(propInfo => propInfo.IsAbstract() || propInfo.IsInitOnly())
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop5", "Prop10", "Prop11"]);
                }
                else
                {
                    _notDeclaredOnlyProperties
                        .Where(propInfo => propInfo.IsAbstract() || propInfo.IsInitOnly())
                        .Properties
                        .Select(propInfo => propInfo.Name)
                        .Should()
                        .BeEquivalentTo(["Prop4", "Prop5", "Prop6", "Prop10", "Prop11"]);
                }
            }
        }
    }
}