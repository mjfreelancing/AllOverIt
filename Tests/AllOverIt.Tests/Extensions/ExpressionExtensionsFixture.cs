using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class ExpressionExtensionsFixture : FixtureBase
    {
        private class DummyPropertyClass
        {
            public int Value { get; set; }
        }

        private class DummyNestedClass : DummyPropertyClass
        {
            public DummyPropertyClass Child { get; set; }
        }

        public class GetMemberExpressions : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => ExpressionExtensions.GetMemberExpressions(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Get_Member_Single_From_Lambda_Expression()
            {
                var subject = Create<DummyPropertyClass>();

                Expression<Func<int>> expression = () => subject.Value;

                var actual = ExpressionExtensions.GetMemberExpressions(expression);

                var expected = new[]{ "Value", "subject" };

                expected.Should().BeEquivalentTo(actual.Select(item => item.Member.Name));
            }

            [Fact]
            public void Should_Get_Member_Nested_From_Lambda_Expression()
            {
                // can't use Create<> due to cyclic object graph
                var subject = Create<DummyNestedClass>();

                Expression<Func<int>> expression = () => subject.Child.Value;

                var actual = ExpressionExtensions.GetMemberExpressions(expression).AsReadOnlyList();

                ExpressionExtensions.GetValue(actual[0]).Should().Be(subject);
                ExpressionExtensions.GetValue(actual[1]).Should().Be(subject.Child);
                ExpressionExtensions.GetValue(actual[2]).Should().Be(subject.Child.Value);
            }
        }

        public class UnwrapMemberExpression : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => ExpressionExtensions.UnwrapMemberExpression(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Return_Same_Reference()
            {
                var subject = Create<DummyPropertyClass>();

                Expression<Func<int>> expression = () => subject.Value;

                var actual = ExpressionExtensions.UnwrapMemberExpression(expression.Body);
                var expected = expression.Body;

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Unwrap_LambaExpression()
            {
                var subject = Create<DummyPropertyClass>();

                Expression<Func<int>> expression = () => subject.Value;

                var actual = ExpressionExtensions.UnwrapMemberExpression(expression);
                var expected = expression.Body;

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Unwrap_UnaryExpression()
            {
                var subject = Create<DummyPropertyClass>();

                Expression<Func<int>> expression = () => -subject.Value;

                expression.Body.Should().BeOfType<UnaryExpression>();

                var actual = ExpressionExtensions.UnwrapMemberExpression(expression);
                var expected = (expression.Body as UnaryExpression).Operand;

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Null_When_Not_MemberExpression()
            {
                Expression<Func<int>> expression = () => 0;

                expression.Body.Should().BeOfType<ConstantExpression>();

                var actual = ExpressionExtensions.UnwrapMemberExpression(expression);

                actual.Should().BeNull();
            }
        }

        public class GetFieldOrProperty : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => ExpressionExtensions.GetFieldOrProperty(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Unwrap_To_MemberExpression()
            {
                var subject = Create<DummyPropertyClass>();

                Expression<Func<int>> expression = () => -subject.Value;

                expression.Body.Should().BeOfType<UnaryExpression>();

                var actual = ExpressionExtensions.GetFieldOrProperty(expression);

                var expected = ((expression.Body as UnaryExpression).Operand as MemberExpression).Member;

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Throw_When_Not_Property_Or_Field()
            {
                Expression<Func<int>> expression = () => 0;

                Invoking(() => ExpressionExtensions.GetFieldOrProperty(expression))
                  .Should()
                  .Throw<InvalidOperationException>()
                  .WithMessage("Expected a property or field access expression.");
            }
        }

        public class GetValue : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Return_Null_For_Null_Expression()
            {
                var actual = ExpressionExtensions.GetValue(null);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Get_Constant_Expression_Value()
            {
                Expression<Func<int>> expression = () => 10;

                var actual = expression.Body.GetValue();

                actual.Should().Be(10);
            }

            [Fact]
            public void Should_Get_Member_Field_Expression_Value()
            {
                var value = Create<int>();
                Expression<Func<int>> expression = () => value;

                var actual = expression.Body.GetValue();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Get_Member_Property_Expression_Value()
            {
                var dummy = Create<DummyPropertyClass>();
                Expression<Func<int>> expression = () => dummy.Value;

                var actual = expression.Body.GetValue();

                actual.Should().Be(dummy.Value);
            }

            [Fact]
            public void Should_Get_Member_Expression_Array_Value()
            {
                var dummy = CreateMany<DummyPropertyClass>(2);
                Expression<Func<int>> expression = () => dummy[1].Value;

                var actual = expression.Body.GetValue();

                actual.Should().Be(dummy[1].Value);
            }

            [Fact]
            public void Should_Get_MethodCall_Expression_Value()
            {
                var val1 = Create<int>();
                var val2 = Create<int>();

                Expression<Func<int>> expression = () => GetSum(val1, val2);

                var actual = expression.Body.GetValue();
                var expected = val1 + val2;

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Get_Lambda_Expression_Value()
            {
                var value = Create<int>();

                Expression<Func<int>> expression = () => value;

                var actual = ExpressionExtensions.GetValue(expression);

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Throw_For_ParameterExpression()
            {
                Expression<Action<int, int>> expression = (a, b) => GetSum(a, b);

                var actual = Invoking(() =>
                {
                    expression.Body.GetValue();
                });

                actual
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("A ParameterExpression does not have a value (Parameter 'expression')");
            }

            [Fact]
            public void Should_Throw_Inner_TargetInvocationException()
            {
                var message = Create<string>();
                Expression<Func<int>> expression = () => GetException(message);

                var actual = Invoking(() => expression.Body.GetValue());

                actual.Should().Throw<Exception>().WithMessage(message);
            }

            [Fact]
            public void Should_Get_Dynamic_Invocation_Expression_Value()
            {
                var val1 = Create<int>();
                var val2 = Create<int>();

                Expression<Func<int[]>> expression = () => new[] { val1, val2 };

                var actual = expression.Body.GetValue();
                var expected = new[] { val1, val2 };

                actual.Should().BeEquivalentTo(expected);
            }

            private static int GetSum(int val1, int val2)
            {
                return val1 + val2;
            }

            private static int GetException(string message)
            {
                throw new Exception(message);
            }
        }
    }
}