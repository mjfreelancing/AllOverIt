using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Specification.Extensions
{
    public class LinqSpecificationExtensionsFixture : LinqSpecificationFixtureBase
    {
        public class And : LinqSpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, false)]
            [InlineData(2, true)]
            [InlineData(-3, false)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = LinqIsEven.And(LinqIsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class AndNot : LinqSpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, false)]
            [InlineData(-3, false)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = LinqIsEven.AndNot(LinqIsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class Or : LinqSpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, true)]
            [InlineData(-3, false)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = LinqIsEven.Or(LinqIsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class OrNot : LinqSpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, true)]
            [InlineData(-3, true)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = LinqIsEven.OrNot(LinqIsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class Not : LinqSpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(2, false)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = LinqIsEven.Not();

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }
    }
}