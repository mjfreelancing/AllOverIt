using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class ExpressionExtensionsFixture : FixtureBase
    {
        private class DummyPropertyClass
        {
            public int Value { get; set; }
            public double Field;
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

        public class GetPropertyOrFieldExpressionUsingParameter : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Property_Expression_Null()
{
                var parameter = Expression.Parameter(typeof(DummyPropertyClass), "item");

                Invoking(() => ExpressionExtensions.GetPropertyOrFieldExpressionUsingParameter<DummyPropertyClass, int>(null, parameter))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("propertyOrFieldExpression");
            }

            [Fact]
            public void Should_Throw_When_Parameter_Expression_Null()
            {
                var parameter = Expression.Parameter(typeof(DummyPropertyClass), "item");

                Invoking(() => ExpressionExtensions.GetPropertyOrFieldExpressionUsingParameter<DummyPropertyClass, int>(item => item.Value, null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("parameterExpression");
            }

            [Fact]
            public void Should_Get_Property_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyPropertyClass>();                

                var parameter = Expression.Parameter(typeof(DummyPropertyClass), "item");
                var memberExpression = ExpressionExtensions.GetPropertyOrFieldExpressionUsingParameter<DummyPropertyClass, int>(subject => subject.Value, parameter);

                memberExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Value));

                // Convert it to a Func<DummyPropertyClass, bool>
                var lambda = Expression.Lambda<Func<DummyPropertyClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Value++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
            }

            [Fact]
            public void Should_Get_Nested_Property_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyNestedClass>();

                var parameter = Expression.Parameter(typeof(DummyNestedClass), "item");
                var memberExpression = ExpressionExtensions.GetPropertyOrFieldExpressionUsingParameter<DummyNestedClass, int>(subject => subject.Child.Value, parameter);

                var parentExpression = memberExpression.Expression as MemberExpression;
                parentExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Child.Value));

                // Convert it to a Func<DummyNestedClass, bool>
                var lambda = Expression.Lambda<Func<DummyNestedClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Child.Value++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
            }

            [Fact]
            public void Should_Get_Field_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyPropertyClass>();

                var parameter = Expression.Parameter(typeof(DummyPropertyClass), "item");
                var memberExpression = ExpressionExtensions.GetPropertyOrFieldExpressionUsingParameter<DummyPropertyClass, double>(subject => subject.Field, parameter);

                memberExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Field));

                // Convert it to a Func<DummyPropertyClass, bool>
                var lambda = Expression.Lambda<Func<DummyPropertyClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Field++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
            }

            [Fact]
            public void Should_Get_Nested_Field_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyNestedClass>();

                var parameter = Expression.Parameter(typeof(DummyNestedClass), "item");
                var memberExpression = ExpressionExtensions.GetPropertyOrFieldExpressionUsingParameter<DummyNestedClass, double>(subject => subject.Child.Field, parameter);

                var parentExpression = memberExpression.Expression as MemberExpression;
                parentExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Child.Field));

                // Convert it to a Func<DummyNestedClass, bool>
                var lambda = Expression.Lambda<Func<DummyNestedClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Child.Field++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
            }
        }

        public class GetParameterPropertyOrFieldExpression : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Property_Expression_Null()
            {
                var parameter = Expression.Parameter(typeof(DummyPropertyClass), "item");

                Invoking(() => ExpressionExtensions.GetParameterPropertyOrFieldExpression<DummyPropertyClass, int>(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("propertyOrFieldExpression");
            }

            [Fact]
            public void Should_Get_Property_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyPropertyClass>();

                var memberExpression = ExpressionExtensions.GetParameterPropertyOrFieldExpression<DummyPropertyClass, int>(subject => subject.Value);

                memberExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                var parameter = memberExpression.Expression as ParameterExpression;

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Value));

                // Convert it to a Func<DummyPropertyClass, bool>
                var lambda = Expression.Lambda<Func<DummyPropertyClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Value++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
            }

            [Fact]
            public void Should_Get_Nested_Property_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyNestedClass>();

                var memberExpression = ExpressionExtensions.GetParameterPropertyOrFieldExpression<DummyNestedClass, int>(subject => subject.Child.Value);

                var parentExpression = memberExpression.Expression as MemberExpression;
                parentExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                var parameter = parentExpression.Expression as ParameterExpression;

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Child.Value));

                // Convert it to a Func<DummyNestedClass, bool>
                var lambda = Expression.Lambda<Func<DummyNestedClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Child.Value++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
            }

            [Fact]
            public void Should_Get_Field_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyPropertyClass>();

                var memberExpression = ExpressionExtensions.GetParameterPropertyOrFieldExpression<DummyPropertyClass, double>(subject => subject.Field);

                memberExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                var parameter = memberExpression.Expression as ParameterExpression;

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Field));

                // Convert it to a Func<DummyPropertyClass, bool>
                var lambda = Expression.Lambda<Func<DummyPropertyClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Field++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
            }

            [Fact]
            public void Should_Get_Nested_Field_As_Parameter_MemberExpression()
            {
                var subject = Create<DummyNestedClass>();

                var memberExpression = ExpressionExtensions.GetParameterPropertyOrFieldExpression<DummyNestedClass, double>(subject => subject.Child.Field);

                var parentExpression = memberExpression.Expression as MemberExpression;
                parentExpression.Expression.Should().BeAssignableTo<ParameterExpression>();

                var parameter = parentExpression.Expression as ParameterExpression;

                // Now test by building a predicate that compares against the expected value
                var predicate = Expression.Equal(memberExpression, Expression.Constant(subject.Child.Field));

                // Convert it to a Func<DummyNestedClass, bool>
                var lambda = Expression.Lambda<Func<DummyNestedClass, bool>>(predicate, parameter);
                var compiled = lambda.Compile();

                // And invoke
                compiled.Invoke(subject).Should().BeTrue();

                // And confirm it fails when the value on the subject is changed
                subject.Child.Field++;

                // And invoke
                compiled.Invoke(subject).Should().BeFalse();
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

        public class GetPropertyOrFieldMemberInfo : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => ExpressionExtensions.GetPropertyOrFieldMemberInfo(null))
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

                var actual = ExpressionExtensions.GetPropertyOrFieldMemberInfo(expression);

                var expected = ((expression.Body as UnaryExpression).Operand as MemberExpression).Member;

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Throw_When_Not_Property_Or_Field()
            {
                Expression<Func<int>> expression = () => 0;

                Invoking(() => ExpressionExtensions.GetPropertyOrFieldMemberInfo(expression))
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

        public class CastOrConvertTo : ExpressionExtensionsFixture
        {
            [Fact]
            public void Should_Return_Same_Expression_When_Object_Assignable_From_String()
            {
                var strValue = Create<string>();
                var valueExpression = Expression.Constant(strValue);

                var converted = valueExpression.CastOrConvertTo(typeof(object));

                converted.Should().BeSameAs(valueExpression);
            }

            [Fact]
            public void Should_Return_Same_Expression_When_IEnumerable_Assignable_From_IReadOnlyList()
            {
                var strValue = CreateMany<string>();
                var valueExpression = Expression.Constant(strValue);

                var converted = valueExpression.CastOrConvertTo(typeof(IEnumerable<string>));

                converted.Should().BeSameAs(valueExpression);
            }

            [Fact]
            public void Should_Convert_Value_Type()
            {
                var intValue = Create<int>();
                var valueExpression = Expression.Constant(intValue);

                var converted = valueExpression.CastOrConvertTo(typeof(double)) as UnaryExpression;

                converted.NodeType.Should().Be(ExpressionType.Convert);
                converted.Operand.Should().Be(valueExpression);
                converted.Type.Should().Be(typeof(double));
            }

            [Fact]
            public void Should_TypeAs_Bool_To_String()
            {
                var boolValue = Create<bool>();
                var valueExpression = Expression.Constant(boolValue);

                var converted = valueExpression.CastOrConvertTo(typeof(string)) as UnaryExpression;

                converted.NodeType.Should().Be(ExpressionType.TypeAs);
                converted.Operand.Should().Be(valueExpression);
                converted.Type.Should().Be(typeof(string));
            }

            [Fact]
            public void Should_Convert_Object_To_Value_Type()
            {
                var itemParam = Expression.Parameter(typeof(object), "item");

                var converted = itemParam.CastOrConvertTo(typeof(int)) as UnaryExpression;

                converted.NodeType.Should().Be(ExpressionType.Convert);
                converted.Operand.Should().Be(itemParam);
                converted.Type.Should().Be(typeof(int));
            }

            [Fact]
            public void Should_TypeAs_Object_To_Nullable_Type()
            {
                var itemParam = Expression.Parameter(typeof(object), "item");

                var converted = itemParam.CastOrConvertTo(typeof(int?)) as UnaryExpression;

                converted.NodeType.Should().Be(ExpressionType.TypeAs);
                converted.Operand.Should().Be(itemParam);
                converted.Type.Should().Be(typeof(int?));
            }
        }
    }
}