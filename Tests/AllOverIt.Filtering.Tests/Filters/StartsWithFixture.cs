using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class StartsWithFixture : FixtureBase
    {
        public class Constructor_Default : StartsWithFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new StartsWith();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : StartsWithFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<string>();

                var actual = new StartsWith(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : StartsWithFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new StartsWith(Create<string>());

                var actual = (string) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : StartsWithFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<string>();

                var actual = (StartsWith) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
