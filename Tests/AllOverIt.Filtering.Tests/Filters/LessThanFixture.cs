using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class LessThanFixture : FixtureBase
    {
        public class Constructor_Default : LessThanFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new LessThan<int>();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : LessThanFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<int>();

                var actual = new LessThan<int>(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : LessThanFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new LessThan<int>(Create<int>());

                var actual = (int) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : LessThanFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<int>();

                var actual = (LessThan<int>) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
