using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Extensions
{
    public class EnumerableExtensionsFixture : FixtureBase
    {
        public class Specifications : SpecificationExtensionsFixture
        {
            private readonly ISpecification<int> _isEven;

            protected Specifications()
            {
                _isEven = new IsEven();
            }

            public class Where : Specifications
            {
                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var expected = new[] { 2, 4, 6, 8, 10 };

                    var actual = Enumerable.Range(1, 10).Where(_isEven);

                    expected.Should().BeEquivalentTo(actual);
                }
            }

            public class Any : Specifications
            {
                [Theory]
                [InlineData(new[] { 1, 2 }, true)]
                [InlineData(new[] { 1, 3 }, false)]
                public void Should_Return_Expected_Result(int[] values, bool expected)
                {
                    var actual = values.Any(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class All : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 6 }, true)]
                [InlineData(new[] { 1, 2, 4 }, false)]
                public void Should_Return_Expected_Result(int[] values, bool expected)
                {
                    var actual = values.All(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class Count : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 6 }, 3)]
                [InlineData(new[] { 1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.Count(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class First : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4 }, 2)]
                [InlineData(new[] { -1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.First(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class FirstOrDefault : Specifications
            {
                [Theory]
                [InlineData(new int[] { }, 0)]
                [InlineData(new[] { -1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.FirstOrDefault(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class Last : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 1 }, 4)]
                [InlineData(new[] { -1, 2, -4 }, -4)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.Last(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class LastOrDefault : Specifications
            {
                [Theory]
                [InlineData(new int[] { }, 0)]
                [InlineData(new[] { -1, 2, 3 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.LastOrDefault(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class SkipWhile : Specifications
            {
                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var values = Enumerable.Range(1, 10);

                    var specification = Specification<int>.Create(value => value < 5);
                    var actual = values.SkipWhile(specification).ToList();

                    var expected = new[] { 5, 6, 7, 8, 9, 10 };

                    expected.Should().BeEquivalentTo(actual);
                }
            }

            public class TakeWhile : Specifications
            {
                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var values = Enumerable.Range(1, 10);

                    var specification = Specification<int>.Create(value => value < 5);
                    var actual = values.TakeWhile(specification).ToList();

                    var expected = new[] {1, 2, 3, 4};

                    expected.Should().BeEquivalentTo(actual);
                }
            }
        }
    }
}