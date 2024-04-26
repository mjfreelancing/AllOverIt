using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class NotContainsFixture : FixtureBase
    {
        public class Constructor_Default : NotContainsFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new NotContains();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : NotContainsFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<string>();

                var actual = new NotContains(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : NotContainsFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new NotContains(Create<string>());

                var actual = (string) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : NotContainsFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<string>();

                var actual = (NotContains) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
