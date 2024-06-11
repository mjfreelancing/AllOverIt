#nullable enable

using AllOverIt.Extensions;
using AllOverIt.Fixture.Assertions;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Fixture.Tests
{
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

        private readonly ClassProperties<DummyClass> _notDeclaredOnlyProperties = new(false);
        private readonly ClassProperties<DummyClass> _declaredOnlyProperties = new(true);

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

        public class Including : ClassPropertiesFixture
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
                        _ = _declaredOnlyProperties.Including(null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including(null!);
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
                        _ = _declaredOnlyProperties.Including([]);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Including([]);
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
        }

        public class Excluding : ClassPropertiesFixture
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
                        _ = _declaredOnlyProperties.Excluding(null!);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding(null!);
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
                        _ = _declaredOnlyProperties.Excluding([]);
                    }
                    else
                    {
                        _ = _notDeclaredOnlyProperties.Excluding([]);
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