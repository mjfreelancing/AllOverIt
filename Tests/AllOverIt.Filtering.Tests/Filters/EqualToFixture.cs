using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class EqualToFixture : FixtureBase
    {
        public class Constructor_Default : EqualToFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new EqualTo<int>();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : EqualToFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<int>();

                var actual = new EqualTo<int>(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : EqualToFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new EqualTo<int>(Create<int>());

                var actual = (int) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : EqualToFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<int>();

                var actual = (EqualTo<int>) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
