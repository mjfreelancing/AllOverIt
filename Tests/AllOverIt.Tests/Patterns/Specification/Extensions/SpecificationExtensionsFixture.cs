using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Extensions
{
    public class SpecificationExtensionsFixture : SpecificationFixtureBase
    {
        public class And : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, false)]
            [InlineData(2, true)]
            [InlineData(-3, false)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.And(IsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class AndNot : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, false)]
            [InlineData(-3, false)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.AndNot(IsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class Or : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, true)]
            [InlineData(-3, false)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.Or(IsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class OrNot : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, true)]
            [InlineData(-3, true)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.OrNot(IsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class Not : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(2, false)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.Not();

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }
    }
}