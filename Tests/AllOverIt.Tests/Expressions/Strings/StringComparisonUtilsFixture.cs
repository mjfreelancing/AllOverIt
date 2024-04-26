using AllOverIt.Expressions.Strings;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Expressions.Strings
{
    public class StringComparisonUtilsFixture : FixtureBase
    {
        public class CreateCompareCallExpression_StringComparison : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Value1_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateCompareCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

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
                    _ = StringComparisonUtils.CreateCompareCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
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
                    _ = StringComparisonUtils.CreateCompareCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparison?) null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData(null, "b", default, -1)]
            [InlineData("a", null, default, 1)]
            [InlineData("a", "b", default, -1)]
            [InlineData("a", "a", default, 0)]
            [InlineData("b", "a", default, 1)]
            [InlineData(null, "b", StringComparison.OrdinalIgnoreCase, -1)]
            [InlineData("a", null, StringComparison.OrdinalIgnoreCase, 1)]
            [InlineData("A", "b", StringComparison.OrdinalIgnoreCase, -1)]
            [InlineData("A", "a", StringComparison.OrdinalIgnoreCase, 0)]
            [InlineData("B", "a", StringComparison.OrdinalIgnoreCase, 1)]
            public void Should_Compare(string value1, string value2, StringComparison? comparison, int expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringComparisonUtils.CreateCompareCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<int>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }
        }

        public class CreateCompareCallExpression_StringComparisonMode : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Value1_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateCompareCallExpression(null, Expression.Constant(Create<string>()), StringComparisonMode.None);

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
                    _ = StringComparisonUtils.CreateCompareCallExpression(Expression.Constant(Create<string>()), null, StringComparisonMode.None);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value2");
            }

            [Fact]
            public void Should_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateCompareCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparisonMode) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("stringComparisonMode");
            }

            [Theory]
            [InlineData(null, "b", false, -1)]
            [InlineData("a", null, false, 1)]
            [InlineData("a", "b", false, -1)]
            [InlineData("a", "a", false, 0)]
            [InlineData("b", "a", false, 1)]
            [InlineData(null, "b", true, -1)]
            [InlineData("a", null, true, 1)]
            [InlineData("A", "b", true, -1)]
            [InlineData("A", "a", true, 0)]
            [InlineData("B", "a", true, 1)]
            public void Should_Compare_When_Not_String_Modifier(string value1, string value2, bool useStringComparison, int expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                // Not a string modifier such as ToLower or ToUpper
                var comparisonMode = useStringComparison ? StringComparisonMode.InvariantCultureIgnoreCase : StringComparisonMode.None;

                var expression = StringComparisonUtils.CreateCompareCallExpression(exp1, exp2, comparisonMode);

                var actual = Expression.Lambda<Func<int>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, "b", "value1")]
            [InlineData("a", null, "value2")]
            public void Should_Throw_When_Null_Constant_Value_And_String_Modifier(string value1, string value2, string parameterName)
            {
                Invoking(() =>
                {
                    var exp1 = Expression.Constant(value1);
                    var exp2 = Expression.Constant(value2);

                    _ = StringComparisonUtils.CreateCompareCallExpression(exp1, exp2, StringComparisonMode.ToLower);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Expected a non-null expression value. (Parameter '{parameterName}')");
            }
        }

        public class CreateContainsCallExpression_StringComparison : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Value1_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateContainsCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("instance");
            }

            [Fact]
            public void Should_Throw_When_Value2_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateContainsCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
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
                    _ = StringComparisonUtils.CreateContainsCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparison?) null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData("a", "b", default, false)]
            [InlineData("a", "a", default, true)]
            [InlineData("A", "b", StringComparison.OrdinalIgnoreCase, false)]
            [InlineData("A", "a", StringComparison.OrdinalIgnoreCase, true)]
            public void Should_Compare(string value1, string value2, StringComparison? comparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringComparisonUtils.CreateContainsCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, "b", "instance")]
            [InlineData("a", null, "value")]
            public void Should_Throw_When_Null_Constant_Value(string value1, string value2, string parameterName)
            {
                Invoking(() =>
                {
                    var exp1 = Expression.Constant(value1);
                    var exp2 = Expression.Constant(value2);

                    _ = StringComparisonUtils.CreateContainsCallExpression(exp1, exp2, StringComparison.OrdinalIgnoreCase);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Expected a non-null expression value. (Parameter '{parameterName}')");
            }
        }

        public class CreateContainsCallExpression_StringComparisonMode : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateContainsCallExpression(null, Expression.Constant(Create<string>()), StringComparisonMode.None);

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
                    _ = StringComparisonUtils.CreateContainsCallExpression(Expression.Constant(Create<string>()), null, StringComparisonMode.None);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateContainsCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparisonMode) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("stringComparisonMode");
            }

            [Theory]
            [InlineData("a", "b", false, false)]
            [InlineData("a", "a", false, true)]
            [InlineData("A", "b", true, false)]
            [InlineData("A", "a", true, true)]
            public void Should_Compare(string value1, string value2, bool useStringComparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);
                var comparisonMode = useStringComparison ? StringComparisonMode.InvariantCultureIgnoreCase : StringComparisonMode.None;

                var expression = StringComparisonUtils.CreateContainsCallExpression(exp1, exp2, comparisonMode);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, "b", "instance")]
            [InlineData("a", null, "value")]
            public void Should_Throw_When_Null_Constant_Value(string value1, string value2, string parameterName)
            {
                Invoking(() =>
                {
                    var exp1 = Expression.Constant(value1);
                    var exp2 = Expression.Constant(value2);

                    _ = StringComparisonUtils.CreateContainsCallExpression(exp1, exp2, StringComparisonMode.InvariantCultureIgnoreCase);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Expected a non-null expression value. (Parameter '{parameterName}')");
            }
        }

        public class CreateStartsWithCallExpression_StringComparison : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateStartsWithCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

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
                    _ = StringComparisonUtils.CreateStartsWithCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
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
                    _ = StringComparisonUtils.CreateStartsWithCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparison?) null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData("ab", "b", default, false)]
            [InlineData("ab", "a", default, true)]
            [InlineData("AB", "b", StringComparison.OrdinalIgnoreCase, false)]
            [InlineData("AB", "a", StringComparison.OrdinalIgnoreCase, true)]
            public void Should_Compare(string value1, string value2, StringComparison? comparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringComparisonUtils.CreateStartsWithCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, "b", "instance")]
            [InlineData("a", null, "value")]
            public void Should_Throw_When_Null_Constant_Value(string value1, string value2, string parameterName)
            {
                Invoking(() =>
                {
                    var exp1 = Expression.Constant(value1);
                    var exp2 = Expression.Constant(value2);

                    _ = StringComparisonUtils.CreateStartsWithCallExpression(exp1, exp2, StringComparison.OrdinalIgnoreCase);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Expected a non-null expression value. (Parameter '{parameterName}')");
            }
        }

        public class CreateStartsWithCallExpression_StringComparisonMode : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateStartsWithCallExpression(null, Expression.Constant(Create<string>()), StringComparisonMode.None);

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
                    _ = StringComparisonUtils.CreateStartsWithCallExpression(Expression.Constant(Create<string>()), null, StringComparisonMode.None);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateStartsWithCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparisonMode) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("stringComparisonMode");
            }

            [Theory]
            [InlineData("ab", "b", false, false)]
            [InlineData("ab", "a", false, true)]
            [InlineData("AB", "b", true, false)]
            [InlineData("AB", "a", true, true)]
            public void Should_Compare(string value1, string value2, bool useStringComparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);
                var comparisonMode = useStringComparison ? StringComparisonMode.InvariantCultureIgnoreCase : StringComparisonMode.None;

                var expression = StringComparisonUtils.CreateStartsWithCallExpression(exp1, exp2, comparisonMode);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, "b", "instance")]
            [InlineData("a", null, "value")]
            public void Should_Throw_When_Null_Constant_Value(string value1, string value2, string parameterName)
            {
                Invoking(() =>
                {
                    var exp1 = Expression.Constant(value1);
                    var exp2 = Expression.Constant(value2);

                    _ = StringComparisonUtils.CreateStartsWithCallExpression(exp1, exp2, StringComparisonMode.InvariantCultureIgnoreCase);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Expected a non-null expression value. (Parameter '{parameterName}')");
            }
        }

        public class CreateEndsWithCallExpression_StringComparison : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateEndsWithCallExpression(null, Expression.Constant(Create<string>()), StringComparison.OrdinalIgnoreCase);

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
                    _ = StringComparisonUtils.CreateEndsWithCallExpression(Expression.Constant(Create<string>()), null, StringComparison.OrdinalIgnoreCase);
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
                    _ = StringComparisonUtils.CreateEndsWithCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparison?) null);
                })
                    .Should()
                    .NotThrow();
            }

            [Theory]
            [InlineData("ab", "b", default, true)]
            [InlineData("ab", "a", default, false)]
            [InlineData("AB", "b", StringComparison.OrdinalIgnoreCase, true)]
            [InlineData("AB", "a", StringComparison.OrdinalIgnoreCase, false)]
            public void Should_Compare(string value1, string value2, StringComparison? comparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);

                var expression = StringComparisonUtils.CreateEndsWithCallExpression(exp1, exp2, comparison);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, "b", "instance")]
            [InlineData("a", null, "value")]
            public void Should_Throw_When_Null_Constant_Value(string value1, string value2, string parameterName)
            {
                Invoking(() =>
                {
                    var exp1 = Expression.Constant(value1);
                    var exp2 = Expression.Constant(value2);

                    _ = StringComparisonUtils.CreateEndsWithCallExpression(exp1, exp2, StringComparison.OrdinalIgnoreCase);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Expected a non-null expression value. (Parameter '{parameterName}')");
            }
        }

        public class CreateEndsWithCallExpression_StringComparisonMode : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateEndsWithCallExpression(null, Expression.Constant(Create<string>()), StringComparisonMode.None);

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
                    _ = StringComparisonUtils.CreateEndsWithCallExpression(Expression.Constant(Create<string>()), null, StringComparisonMode.None);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Throw_When_Comparison_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateEndsWithCallExpression(Expression.Constant(Create<string>()), Expression.Constant(Create<string>()), (StringComparisonMode) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("stringComparisonMode");
            }

            [Theory]
            [InlineData("ab", "b", false, true)]
            [InlineData("ab", "a", false, false)]
            [InlineData("AB", "b", true, true)]
            [InlineData("AB", "a", true, false)]
            public void Should_Compare(string value1, string value2, bool useStringComparison, bool expected)
            {
                var exp1 = Expression.Constant(value1);
                var exp2 = Expression.Constant(value2);
                var comparisonMode = useStringComparison ? StringComparisonMode.InvariantCultureIgnoreCase : StringComparisonMode.None;

                var expression = StringComparisonUtils.CreateEndsWithCallExpression(exp1, exp2, comparisonMode);

                var actual = Expression.Lambda<Func<bool>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, "b", "instance")]
            [InlineData("a", null, "value")]
            public void Should_Throw_When_Null_Constant_Value(string value1, string value2, string parameterName)
            {
                Invoking(() =>
                {
                    var exp1 = Expression.Constant(value1);
                    var exp2 = Expression.Constant(value2);

                    _ = StringComparisonUtils.CreateEndsWithCallExpression(exp1, exp2, StringComparisonMode.InvariantCultureIgnoreCase);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Expected a non-null expression value. (Parameter '{parameterName}')");
            }
        }

        public class CreateToLowerCallExpression : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateToLowerCallExpression(null);

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_ToLower()
            {
                var value = Create<string>();
                var expected = value.ToLower();

                var methodCallExpression = StringComparisonUtils.CreateToLowerCallExpression(Expression.Constant(value));

                var actual = Expression.Lambda<Func<string>>(methodCallExpression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Null_Constant_Value()
            {
                Invoking(() =>
                {
                    var expression = Expression.Constant((string) null);

                    _ = StringComparisonUtils.CreateToLowerCallExpression(expression);
                })
               .Should()
               .Throw<ArgumentNullException>()
               .WithMessage($"Expected a non-null expression value. (Parameter 'value')");
            }
        }

        public class CreateToUpperCallExpression : StringComparisonUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                {
                    _ = StringComparisonUtils.CreateToLowerCallExpression(null);

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_ToUpper()
            {
                var value = Create<string>();
                var expected = value.ToUpper();

                var expression = StringComparisonUtils.CreateToUpperCallExpression(Expression.Constant(value));

                var actual = Expression.Lambda<Func<string>>(expression).Compile().Invoke();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Null_Constant_Value()
            {
                Invoking(() =>
                {
                    var expression = Expression.Constant((string) null);

                    _ = StringComparisonUtils.CreateToUpperCallExpression(expression);
                })
               .Should()
               .Throw<ArgumentNullException>()
               .WithMessage($"Expected a non-null expression value. (Parameter 'value')");
            }
        }
    }
}