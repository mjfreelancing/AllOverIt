using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Expressions
{
    public class ExpressionUtilsFixture : FixtureBase
    {
        public class CreateParameterizedValue_Typed : ExpressionUtilsFixture
        {
            [Fact]
            public void Should_Return_Expression_For_Value()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue<int>(expected);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Nullable_Value()
            {
                var actual = ExpressionUtils.CreateParameterizedValue<int?>(default);

                actual.GetValue()
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void Should_Return_Expression_For_String()
            {
                var expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue<string>(expected);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Null_String()
            {
                var actual = ExpressionUtils.CreateParameterizedValue<string>(default);

                actual.GetValue()
                    .Should()
                    .BeNull();
            }
        }

        public class CreateParameterizedValue_Object : ExpressionUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Not_Provided()
            {
                Invoking(() =>
                {
                    _ = ExpressionUtils.CreateParameterizedValue((string) default, null);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("The value type must be provided when creating a parameterized value expression.");
            }

            [Fact]
            public void Should_Determine_Value_Type()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, null);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Determine_String_Type()
            {
                object expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, null);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Value()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, typeof(int));

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Nullable_Value()
            {
                var actual = ExpressionUtils.CreateParameterizedValue(null, typeof(int?));

                actual.GetValue()
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void Should_Return_Expression_For_String()
            {
                var expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, typeof(string));

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Null_String()
            {
                var actual = ExpressionUtils.CreateParameterizedValue(null, typeof(string));

                actual.GetValue()
                    .Should()
                    .BeNull();
            }
        }

        public class CreateParameterExpressions : ExpressionUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Params_Null()
            {
                Invoking(() =>
                {
                    ExpressionUtils.CreateParameterExpressions(null).ToList();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("parameterTypes");
            }

            [Fact]
            public void Should_Throw_When_Params_Empty()
            {
                Invoking(() =>
                {
                    ExpressionUtils.CreateParameterExpressions(Type.EmptyTypes).ToList();
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("parameterTypes");
            }

            [Fact]
            public void Should_Return_Parameters()
            {
                var actual = ExpressionUtils
                    .CreateParameterExpressions(new[] { typeof(int), typeof(double), typeof(string) })
                    .ToList();

                actual.Should().BeEquivalentTo(new[]
                {
                    Expression.Parameter(typeof(int), "t1"),
                    Expression.Parameter(typeof(double), "t2"),
                    Expression.Parameter(typeof(string), "t3")
                });
            }
        }

        public class GetConstructorWithParameters : ExpressionUtilsFixture
        {
            private class DummyType
            {
                public DummyType(int val1, string val2)
                {
                }
            }

            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParameters(null, [typeof(double)]);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Params_Null()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParameters(typeof(DummyType), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("paramTypes");
            }

            [Fact]
            public void Should_Throw_When_Params_Empty()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParameters(typeof(DummyType), Type.EmptyTypes);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("paramTypes");
            }

            [Fact]
            public void Should_Return_NewExpression_And_Parameters()
            {
                var actual = ExpressionUtils.GetConstructorWithParameters(typeof(DummyType), new[] { typeof(int), typeof(string) });

                var expectedParameters = new[]
                {
                    Expression.Parameter(typeof(int), "t1"),
                    Expression.Parameter(typeof(string), "t2")
                };

                actual.NewExpression.Should().BeOfType<NewExpression>();
                actual.NewExpression.Type.Should().Be(typeof(DummyType));
                actual.NewExpression.Arguments.Should().BeEquivalentTo(expectedParameters);

                actual.ParameterExpressions.Should().BeEquivalentTo(expectedParameters);
            }
        }

        public class GetConstructorWithParametersAsObjects : ExpressionUtilsFixture
        {
            private class DummyType
            {
                public DummyType(int val1, string val2)
                {
                }
            }

            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParametersAsObjects(null, [typeof(double)]);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Params_Null()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(DummyType), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("paramTypes");
            }

            [Fact]
            public void Should_Throw_When_Params_Empty()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(DummyType), Type.EmptyTypes);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("paramTypes");
            }

            [Fact]
            public void Should_Return_NewExpression_And_Parameters()
            {
                var actual = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(DummyType), new[] { typeof(int), typeof(string) });

                var expectedParameters = new[]
                {
                    Expression.Parameter(typeof(object), "t1"),
                    Expression.Parameter(typeof(object), "t2")
                };

                var expectedConstructorParameters = new[]
                {
                    Expression.Convert(expectedParameters[0], typeof(int)),     // Convert
                    Expression.TypeAs(expectedParameters[1], typeof(string))    // Cast
                };

                actual.NewExpression.Should().BeOfType<NewExpression>();
                actual.NewExpression.Type.Should().Be(typeof(DummyType));
                actual.NewExpression.Arguments.Should().BeEquivalentTo(expectedConstructorParameters);

                actual.ParameterExpressions.Should().BeEquivalentTo(expectedParameters);
            }
        }
    }
}