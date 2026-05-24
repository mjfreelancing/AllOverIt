using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using System.Linq.Expressions;
using AllOverIt.Shouldly.Extensions;

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
                    .ShouldBe(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Nullable_Value()
            {
                var actual = ExpressionUtils.CreateParameterizedValue<int?>(default);

                actual.GetValue()
                    .ShouldBeNull();
            }

            [Fact]
            public void Should_Return_Expression_For_String()
            {
                var expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue<string>(expected);

                actual.GetValue()
                    .ShouldBe(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Null_String()
            {
                var actual = ExpressionUtils.CreateParameterizedValue<string>(default);

                actual.GetValue()
                    .ShouldBeNull();
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
                    .ShouldThrow<ArgumentException>()
                    .WithMessage("The value type must be provided when creating a parameterized value expression.");
            }

            [Fact]
            public void Should_Determine_Value_Type()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, null);

                actual.GetValue()
                    .ShouldBe(expected);
            }

            [Fact]
            public void Should_Determine_String_Type()
            {
                object expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, null);

                actual.GetValue()
                    .ShouldBe(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Value()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, typeof(int));

                actual.GetValue()
                    .ShouldBe(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Nullable_Value()
            {
                var actual = ExpressionUtils.CreateParameterizedValue(null, typeof(int?));

                actual.GetValue()
                    .ShouldBeNull();
            }

            [Fact]
            public void Should_Return_Expression_For_String()
            {
                var expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, typeof(string));

                actual.GetValue()
                    .ShouldBe(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Null_String()
            {
                var actual = ExpressionUtils.CreateParameterizedValue(null, typeof(string));

                actual.GetValue()
                    .ShouldBeNull();
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("parameterTypes");
            }

            [Fact]
            public void Should_Throw_When_Params_Empty()
            {
                Invoking(() =>
                {
                    ExpressionUtils.CreateParameterExpressions(Type.EmptyTypes).ToList();
                })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("parameterTypes");
            }

            [Fact]
            public void Should_Return_Parameters()
            {
                var actual = ExpressionUtils
                    .CreateParameterExpressions(new[] { typeof(int), typeof(double), typeof(string) })
                    .ToList();

                actual.ShouldBeEquivalentTo(new[]
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Params_Null()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParameters(typeof(DummyType), null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("paramTypes");
            }

            [Fact]
            public void Should_Throw_When_Params_Empty()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParameters(typeof(DummyType), Type.EmptyTypes);
                })
                    .ShouldThrow<ArgumentException>()
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

                actual.NewExpression.ShouldBeOfType<NewExpression>();
                actual.NewExpression.Type.ShouldBe(typeof(DummyType));
                actual.NewExpression.Arguments.ShouldBeEquivalentTo(expectedParameters);

                actual.ParameterExpressions.ShouldBeEquivalentTo(expectedParameters);
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Params_Null()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(DummyType), null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("paramTypes");
            }

            [Fact]
            public void Should_Throw_When_Params_Empty()
            {
                Invoking(() =>
                {
                    ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(DummyType), Type.EmptyTypes);
                })
                    .ShouldThrow<ArgumentException>()
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

                actual.NewExpression.ShouldBeOfType<NewExpression>();
                actual.NewExpression.Type.ShouldBe(typeof(DummyType));
                actual.NewExpression.Arguments.ShouldBeEquivalentTo(expectedConstructorParameters);

                actual.ParameterExpressions.ShouldBeEquivalentTo(expectedParameters);
            }
        }
    }
}







