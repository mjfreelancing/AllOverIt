using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using AllOverIt.Shouldly.Extensions;

namespace AllOverIt.Tests.Extensions
{
    public class QueryableExtensionsFixture : FixtureBase
    {
        public class Specifications : QueryableExtensionsFixture
        {
            private readonly ILinqSpecification<int> _isEven;

            protected Specifications()
            {
                _isEven = new LinqIsEven();
            }

            public class Where : Specifications
            {
                [Fact]
                public void Should_Return_IQueryable()
                {
                    var actual = Enumerable.Range(1, 10).AsQueryable().Where(_isEven);

                    actual.ShouldBeAssignableTo<IQueryable<int>>();
                }

                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var expected = new[] { 2, 4, 6, 8, 10 };

                    var actual = Enumerable.Range(1, 10).AsQueryable().Where(_isEven);

                    actual.ShouldBeEquivalentTo(expected);
                }
            }

            public class Any : Specifications
            {
                [Theory]
                [InlineData(new[] { 1, 2 }, true)]
                [InlineData(new[] { 1, 3 }, false)]
                public void Should_Return_Expected_Result(int[] values, bool expected)
                {
                    var actual = values.AsQueryable().Any(_isEven);

                    actual.ShouldBe(expected);
                }
            }

            public class All : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 6 }, true)]
                [InlineData(new[] { 1, 2, 4 }, false)]
                public void Should_Return_Expected_Result(int[] values, bool expected)
                {
                    var actual = values.AsQueryable().All(_isEven);

                    actual.ShouldBe(expected);
                }
            }

            public class Count : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 6 }, 3)]
                [InlineData(new[] { 1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.AsQueryable().Count(_isEven);

                    actual.ShouldBe(expected);
                }
            }

            public class First : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4 }, 2)]
                [InlineData(new[] { -1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.AsQueryable().First(_isEven);

                    actual.ShouldBe(expected);
                }
            }

            public class FirstOrDefault : Specifications
            {
                [Theory]
                [InlineData(new int[] { }, 0)]
                [InlineData(new[] { -1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.AsQueryable().FirstOrDefault(_isEven);

                    actual.ShouldBe(expected);
                }
            }

            public class Last : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 1 }, 4)]
                [InlineData(new[] { -1, 2, -4 }, -4)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.AsQueryable().Last(_isEven);

                    actual.ShouldBe(expected);
                }
            }

            public class LastOrDefault : Specifications
            {
                [Theory]
                [InlineData(new int[] { }, 0)]
                [InlineData(new[] { -1, 2, 3 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.AsQueryable().LastOrDefault(_isEven);

                    actual.ShouldBe(expected);
                }
            }

            public class SkipWhile : Specifications
            {
                [Fact]
                public void Should_Return_IQueryable()
                {
                    var values = Enumerable.Range(1, 10).AsQueryable();

                    var specification = LinqSpecification<int>.Create(value => value < 5);
                    var actual = values.SkipWhile(specification);

                    actual.ShouldBeAssignableTo<IQueryable<int>>();
                }

                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var values = Enumerable.Range(1, 10).AsQueryable();

                    var specification = LinqSpecification<int>.Create(value => value < 5);
                    var actual = values.SkipWhile(specification).ToList();

                    var expected = new[] { 5, 6, 7, 8, 9, 10 };

                    actual.ShouldBeEquivalentTo(expected);
                }
            }

            public class TakeWhile : Specifications
            {
                [Fact]
                public void Should_Return_IQueryable()
                {
                    var values = Enumerable.Range(1, 10).AsQueryable();

                    var specification = LinqSpecification<int>.Create(value => value < 5);
                    var actual = values.TakeWhile(specification);

                    actual.ShouldBeAssignableTo<IQueryable<int>>();
                }

                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var values = Enumerable.Range(1, 10).AsQueryable();

                    var specification = LinqSpecification<int>.Create(value => value < 5);
                    var actual = values.TakeWhile(specification).ToList();

                    var expected = new[] { 1, 2, 3, 4 };

                    actual.ShouldBeEquivalentTo(expected);
                }
            }
        }
    }
}







