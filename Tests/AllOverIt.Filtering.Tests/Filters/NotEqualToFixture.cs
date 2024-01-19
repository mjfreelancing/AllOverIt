using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class NotEqualToFixture : FixtureBase
    {
        public class Constructor_Default : NotEqualToFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new NotEqualTo<int>();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : NotEqualToFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<int>();

                var actual = new NotEqualTo<int>(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : NotEqualToFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new NotEqualTo<int>(Create<int>());

                var actual = (int) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : NotEqualToFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<int>();

                var actual = (NotEqualTo<int>) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
