using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class GreaterThanFixture : FixtureBase
    {
        public class Constructor_Default : GreaterThanFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new GreaterThan<int>();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : GreaterThanFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<int>();

                var actual = new GreaterThan<int>(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : GreaterThanFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new GreaterThan<int>(Create<int>());

                var actual = (int) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : GreaterThanFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<int>();

                var actual = (GreaterThan<int>) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
