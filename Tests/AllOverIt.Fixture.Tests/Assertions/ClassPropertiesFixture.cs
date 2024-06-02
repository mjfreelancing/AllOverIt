#nullable enable

using AllOverIt.Extensions;
using AllOverIt.Fixture.Assertions;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Fixture.Tests
{
    public class ClassPropertiesFixture : FixtureBase
    {
        private abstract class DummyClass
        {
            public int Prop1 { set { } }
            public int Prop2 { get; private set; }
            public int Prop3 { get; internal set; }
            public int Prop4 { get; init; }
            protected abstract int Prop5 { get; init; }
            private int Prop6 { get; init; }
        }

        private readonly ClassProperties<DummyClass> _classProperties = new();

        public class Constructor : ClassPropertiesFixture
        {
            [Fact]
            public void Should_Get_All_Properties()
            {
                _classProperties.Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop1", "Prop2", "Prop3", "Prop4", "Prop5", "Prop6"]);
            }
        }

        public class Including : ClassPropertiesFixture
        {
            [Fact]
            public void Should_Throw_When_Property_Names_Null()
            {
                Invoking(() =>
                {
                    _ = _classProperties.Including(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_Property_Names_Empty()
            {
                Invoking(() =>
                {
                    _ = _classProperties.Including([]);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Fact]
            public void Should_Filter_Properties()
            {
                _classProperties
                    .Including("Prop2", "Prop4")
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop2", "Prop4"]);
            }
        }

        public class Excluding : ClassPropertiesFixture
        {
            [Fact]
            public void Should_Throw_When_Property_Names_Null()
            {
                Invoking(() =>
                {
                    _ = _classProperties.Excluding(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyNames");
            }

            [Fact]
            public void Should_Throw_When_Property_Names_Empty()
            {
                Invoking(() =>
                {
                    _ = _classProperties.Excluding([]);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("propertyNames");
            }

            [Fact]
            public void Should_Filter_Properties()
            {
                _classProperties
                    .Excluding("Prop2", "Prop4")
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop1", "Prop3", "Prop5", "Prop6"]);
            }
        }

        public class Where : ClassPropertiesFixture
        {
            [Fact]
            public void Should_Throw_When_Predicate_Null()
            {
                Invoking(() =>
                {
                    _ = _classProperties.Where(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("predicate");
            }

            [Fact]
            public void Should_Filter_Properties()
            {
                _classProperties
                    .Where(propInfo => propInfo.IsAbstract() || propInfo.IsInitOnly())
                    .Properties
                    .Select(propInfo => propInfo.Name)
                    .Should()
                    .BeEquivalentTo(["Prop4", "Prop5", "Prop6"]);
            }
        }
    }
}