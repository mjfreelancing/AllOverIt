using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class InFixture : FixtureBase
    {
        public class Constructor_Default : InFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new In<int>();

                actual.Value.Should().BeNull();
            }
        }

        public class Constructor_Value : InFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = CreateMany<int>();

                var actual = new In<int>(value);

                actual.Value.Should().BeSameAs(value);
            }
        }

        public class Explicit_Operator : InFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new In<int>(CreateMany<int>());

                var actual = (List<int>) value;

                actual.Should().BeSameAs(value.Value);
            }
        }

        public class Implicit_Operator : InFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = CreateMany<int>().ToList();

                var actual = (In<int>) value;

                actual.Value.Should().BeSameAs(value);
            }
        }
    }
}
