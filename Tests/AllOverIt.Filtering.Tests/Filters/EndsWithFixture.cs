using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class EndsWithFixture : FixtureBase
    {
        public class Constructor_Default : EndsWithFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new EndsWith();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : EndsWithFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<string>();

                var actual = new EndsWith(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : EndsWithFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new EndsWith(Create<string>());

                var actual = (string) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : EndsWithFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<string>();

                var actual = (EndsWith) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
