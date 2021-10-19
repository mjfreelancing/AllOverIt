using AllOverIt.Extensions;
using AllOverIt.Tests.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Extensions
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

        public class And_Specification_Type : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, false)]
            [InlineData(2, true)]
            [InlineData(-3, false)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.And<IsPositive, int>();

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class AndNot : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, false)]
            [InlineData(-3, true)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.AndNot(IsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class AndNot_Specification_Type : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, false)]
            [InlineData(-3, true)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.AndNot<IsPositive, int>();

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

        public class Or_Specification_Type : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, true)]
            [InlineData(2, true)]
            [InlineData(-3, false)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.Or<IsPositive, int>();

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class OrNot : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, false)]
            [InlineData(2, false)]
            [InlineData(-3, true)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.OrNot(IsPositive);

                var actual = combined.IsSatisfiedBy(value);

                actual.Should().Be(expected);
            }
        }

        public class OrNot_Specification_Type : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(-2, false)]
            [InlineData(2, false)]
            [InlineData(-3, true)]
            [InlineData(3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var combined = IsEven.OrNot<IsPositive, int>();

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

        public class WhereSatisfied : SpecificationExtensionsFixture
        {
            [Fact]
            public void Should_Return_Expected_Result()
            {
                var expected = new[] { 2, 4, 6, 8, 10 };

                var actual = Enumerable.Range(1, 10).WhereSatisfied(IsEven);

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class AnySatisfied : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(new[] { 1, 2 }, true)]
            [InlineData(new[] { 1, 3 }, false)]
            public void Should_Return_Expected_Result(int[] values, bool expected)
            {
                var actual = values.AnySatisfied(IsEven);

                actual.Should().Be(expected);
            }
        }

        public class AllSatisfied : SpecificationExtensionsFixture
        {
            [Theory]
            [InlineData(new[] { 2, 4, 6 }, true)]
            [InlineData(new[] { 1, 2, 4 }, false)]
            public void Should_Return_Expected_Result(int[] values, bool expected)
            {
                var actual = values.AllSatisfied(IsEven);

                actual.Should().Be(expected);
            }
        }
    }
}