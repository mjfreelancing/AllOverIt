using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class GreaterThanOrEqualFixture : FixtureBase
    {
        public class Constructor_Default : GreaterThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new GreaterThanOrEqual<int>();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : GreaterThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<int>();

                var actual = new GreaterThanOrEqual<int>(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : GreaterThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new GreaterThanOrEqual<int>(Create<int>());

                var actual = (int) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : GreaterThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<int>();

                var actual = (GreaterThanOrEqual<int>) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
