using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class LinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        private class DummySpecification : LinqSpecification<int>
        {
            public DummySpecification(Func<Expression<Func<int, bool>>> expressionResolver)
                : base(expressionResolver)
            {
            }
        }

        public class Constructor : LinqSpecificationFixture
        {
            [Fact]
            public void Should_Set_Expression()
            {
                var value = Create<int>();
                var expected1 = value % 2 == 0;
                var expected2 = (value + 1) % 2 == 0;

                var expression = GetExpression(value => value % 2 == 0);
                var specification = new DummySpecification(() => expression);
                var compiled = specification.Expression.Compile();

                compiled.Invoke(value).Should().Be(expected1);
                compiled.Invoke(value + 1).Should().Be(expected2);
            }
        }

        public class Create : LinqSpecificationFixture
        {
            [Fact]
            public void Should_Create_Expression()
            {
                var value = Create<int>();
                var expected1 = value % 2 == 0;
                var expected2 = (value + 1) % 2 == 0;

                var specification = LinqSpecification<int>.Create(value => value % 2 == 0);
                var compiled = specification.Expression.Compile();

                compiled.Invoke(value).Should().Be(expected1);
                compiled.Invoke(value + 1).Should().Be(expected2);
            }
        }

        public class Implicit_Expression : LinqSpecificationFixture
        {
            [Fact]
            public void Should_Apply_To_Queryable()
            {
                // When using the Create() factory method, the syntax will be:
                // Expression<Func<int, bool>> specification = LinqSpecification<int>.Create(value => value % 2 == 0) as LinqSpecification<int>;
                Expression<Func<int, bool>> isEvenSpecification = new LinqIsEven();

                var actual = Enumerable
                    .Range(1, 4)
                    .AsQueryable()
                    .Where(isEvenSpecification)
                    .ToList();

                actual.Should().BeEquivalentTo(new[] { 2, 4 });
            }
        }

        public class Explicit_Func : LinqSpecificationFixture
        {
            [Fact]
            public void Should_Apply_To_Enumerable()
            {
                // When using the Create() factory method, the syntax will be:
                // var specification = (Func<int, bool>) (LinqSpecification<int>.Create(value => value % 2 == 0) as LinqSpecification<int>);
                var isEvenSpecification = (Func<int, bool>) (new LinqIsEven());                

                var actual = Enumerable
                    .Range(1, 4)
                    .Where(isEvenSpecification)
                    .ToList();

                actual.Should().BeEquivalentTo(new[] { 2, 4 });
            }
        }

        public class Operator_And : LinqSpecificationFixture
        {
            [Theory]
            [InlineData(2, true)]
            [InlineData(-2, false)]
            [InlineData(3, false)]
            [InlineData(-3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var isEven = new LinqIsEven();
                var isPositive = new LinqIsPositive();
                var combined = isEven && isPositive;

                var actual = combined.IsSatisfiedBy(value);
                actual.Should().Be(expected);
            }
        }

        public class Operator_Or : LinqSpecificationFixture
        {
            [Theory]
            [InlineData(2, true)]
            [InlineData(-2, true)]
            [InlineData(3, true)]
            [InlineData(-3, false)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var isEven = new LinqIsEven();
                var isPositive = new LinqIsPositive();
                var combined = isEven || isPositive;

                var actual = combined.IsSatisfiedBy(value);
                actual.Should().Be(expected);
            }
        }

        public class Operator_Not : LinqSpecificationFixture
        {
            [Theory]
            [InlineData(2, false)]
            [InlineData(3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var isEven = new LinqIsEven();
                var isNotEven = !isEven;

                var actual = isNotEven.IsSatisfiedBy(value);
                actual.Should().Be(expected);
            }
        }

        public class Operator_Mixed_Not_Or : LinqSpecificationFixture
        {
            [Theory]
            [InlineData(2, false)]
            [InlineData(-2, true)]
            [InlineData(3, true)]
            [InlineData(-3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var isEven = new LinqIsEven();
                var isPositive = new LinqIsPositive();
                var combined = !isEven || !isPositive ;

                var actual = combined.IsSatisfiedBy(value);
                actual.Should().Be(expected);
            }
        }

        public class Operator_Mixed_Not_And : LinqSpecificationFixture
        {
            [Theory]
            [InlineData(2, false)]
            [InlineData(-2, false)]
            [InlineData(3, false)]
            [InlineData(-3, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var isEven = new LinqIsEven();
                var isPositive = new LinqIsPositive();
                var combined = !isEven && !isPositive;

                var actual = combined.IsSatisfiedBy(value);
                actual.Should().Be(expected);
            }
        }

        public class IsSatisfiedBy : LinqSpecificationFixture
        {
            [Theory]
            [InlineData(1, false)]
            [InlineData(2, true)]
            public void Should_Return_Expected_Result(int value, bool expected)
            {
                var expression = GetExpression(value => value % 2 == 0);
                var specification = new DummySpecification(() => expression);

                var actual = specification.IsSatisfiedBy(value);
                actual.Should().Be(expected);
            }
        }

        private static Expression<Func<int, bool>> GetExpression(Expression<Func<int, bool>> expression)
        {
            return expression;
        }
    }
}