using AllOverIt.Expressions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions
{
    public class StringExpressionUtilsFixture : FixtureBase
    {
        public class CreateCompareCallExpression : StringExpressionUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Value1_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateCompareCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value1");
            }

            [Fact]
            public void Should_Throw_When_Value2_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateCompareCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value2");
            }

            [Fact]
            public void Should_Not_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateCompareCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData("a", "b", default, -1)]
            [InlineData("a", "a", default, 0)]
            [InlineData("b", "a", default, 1)]
            [InlineData("A", "b", StringComparison.OrdinalIgnoreCase, -1)]
            [InlineData("A", "a", StringComparison.OrdinalIgnoreCase, 0)]
            [InlineData("B", "a", StringComparison.OrdinalIgnoreCase, 1)]
            public void Should_Compare_Than(string value1, string value2, StringComparison? comparison, int expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringExpressionUtils.CreateCompareCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<int>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }
        }

        public class CreateContainsCallExpression : StringExpressionUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateContainsCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("instance");
            }

            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateContainsCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateContainsCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData("a", "b", default, false)]
            [InlineData("a", "a", default, true)]
            [InlineData("A", "b", StringComparison.OrdinalIgnoreCase, false)]
            [InlineData("A", "a", StringComparison.OrdinalIgnoreCase, true)]
            public void Should_Compare_Than(string value1, string value2, StringComparison? comparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringExpressionUtils.CreateContainsCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }
        }

        public class CreateStartsWithCallExpression : StringExpressionUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateStartsWithCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("instance");
            }

            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateStartsWithCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateStartsWithCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData("ab", "b", default, false)]
            [InlineData("ab", "a", default, true)]
            [InlineData("AB", "b", StringComparison.OrdinalIgnoreCase, false)]
            [InlineData("AB", "a", StringComparison.OrdinalIgnoreCase, true)]
            public void Should_Compare_Than(string value1, string value2, StringComparison? comparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringExpressionUtils.CreateStartsWithCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }
        }

        public class CreateEndsWithCallExpression : StringExpressionUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateEndsWithCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("instance");
            }

            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateEndsWithCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringExpressionUtils.CreateEndsWithCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData("ab", "b", default, true)]
            [InlineData("ab", "a", default, false)]
            [InlineData("AB", "b", StringComparison.OrdinalIgnoreCase, true)]
            [InlineData("AB", "a", StringComparison.OrdinalIgnoreCase, false)]
            public void Should_Compare_Than(string value1, string value2, StringComparison? comparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringExpressionUtils.CreateEndsWithCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }
        }
    }
}