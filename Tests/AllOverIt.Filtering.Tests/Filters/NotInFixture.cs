using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class NotInFixture : FixtureBase
    {
        public class Constructor_Default : NotInFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new NotIn<int>();

                actual.Value.Should().BeNull();
            }
        }

        public class Constructor_Value : NotInFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = CreateMany<int>();

                var actual = new NotIn<int>(value);

                actual.Value.Should().BeSameAs(value);
            }
        }

        public class Explicit_Operator : NotInFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new NotIn<int>(CreateMany<int>());

                var actual = (List<int>) value;

                actual.Should().BeSameAs(value.Value);
            }
        }

        public class Implicit_Operator : NotInFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = CreateMany<int>().ToList();

                var actual = (NotIn<int>) value;

                actual.Value.Should().BeSameAs(value);
            }
        }
    }
}
