using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class ExpressionExtensionsFunctionalFixture : FixtureBase
    {
        private class ChildClass
        {
            public int Property { get; set; }

#pragma warning disable 649
            // Unassigned field
            public int Field;
#pragma warning restore 649

            public int GetPropertyValue()
            {
                return Property;
            }
        }

        private class ParentClass
        {
            public int ParentId { get; set; }
            public ChildClass Child { get; set; }
        }

        [Fact]
        public void Should_Get_Constant_Value()
        {
            var actual = GetValue(() => 4);

            actual.Should().Be(4);
        }

        [Fact]
        public void Should_Get_Property_Value()
        {
            var expected = Create<ChildClass>();

            var actual = GetValue(() => expected.Property);

            actual.Should().Be(expected.Property);
        }

        [Fact]
        public void Should_Get_Field_Value()
        {
            var expected = Create<ChildClass>();

            var actual = GetValue(() => expected.Field);

            actual.Should().Be(expected.Field);
        }

        [Fact]
        public void Should_Get_MethodCall_Value()
        {
            var expected = Create<ChildClass>();

            var actual = GetValue(() => expected.GetPropertyValue());

            actual.Should().Be(expected.Property);
        }

        [Fact]
        public void Should_Get_Parent_And_Child_Member_Values()
        {
            var parent = Create<ParentClass>();

            Expression<Func<int>> expression = () => parent.Child.Property;

            // unwrap the LambdaExpression
            var memberExpression = expression.UnwrapMemberExpression();

            var members = memberExpression
              .GetMemberExpressions()
              .Select(exp => exp.GetValue())
              .AsReadOnlyList();

            members[0].Should().Be(parent);
            members[1].Should().Be(parent.Child);
            members[2].Should().Be(parent.Child.Property);

            var value = expression.GetValue();

            value.Should().Be(parent.Child.Property);
        }

        [Fact]
        public void Get_Property_Of_Child()
        {
            var parent = Create<ParentClass>();

            Expression<Func<int>> expression = () => parent.Child.Property;

            var memberInfo = expression.GetFieldOrProperty();

            var name = memberInfo.Name;
            var value = ReflectionHelper.GetMemberValue(memberInfo, parent.Child);

            name.Should().Be(nameof(ChildClass.Property));
            value.Should().Be(parent.Child.Property);
        }

        [Fact]
        public void Get_Field_Of_Child()
        {
            var parent = Create<ParentClass>();

            Expression<Func<int>> expression = () => parent.Child.Field;

            var memberInfo = expression.GetFieldOrProperty();

            var name = memberInfo.Name;
            var value = ReflectionHelper.GetMemberValue(memberInfo, parent.Child);

            name.Should().Be(nameof(ChildClass.Field));
            value.Should().Be(parent.Child.Field);
        }

        private static int GetValue(Expression<Func<int>> expression)
        {
            var value = expression.GetValue();

            return value.As<int>();
        }
    }
}