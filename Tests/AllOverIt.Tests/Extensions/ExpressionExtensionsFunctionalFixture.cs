using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Shouldly;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Extensions
{
    public class ExpressionExtensionsFunctionalFixture : FixtureBase
    {
        private class ChildClass
        {
            public int Property { get; set; }

            public int Field;

            public int GetPropertyValue()
            {
                return Property;
            }

            public void DummyFieldSetter()
            {
                Field = 1;          // Prevent CS0649 (Field is never assigned)
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

            actual.ShouldBe(4);
        }

        [Fact]
        public void Should_Get_Property_Value()
        {
            var expected = Create<ChildClass>();

            var actual = GetValue(() => expected.Property);

            actual.ShouldBe(expected.Property);
        }

        [Fact]
        public void Should_Get_Field_Value()
        {
            var expected = Create<ChildClass>();

            var actual = GetValue(() => expected.Field);

            actual.ShouldBe(expected.Field);
        }

        [Fact]
        public void Should_Get_MethodCall_Value()
        {
            var expected = Create<ChildClass>();

            var actual = GetValue(() => expected.GetPropertyValue());

            actual.ShouldBe(expected.Property);
        }

        [Fact]
        public void Should_Get_Parent_And_Child_Member_Values()
        {
            var parent = Create<ParentClass>();

            Expression<Func<int>> expression = () => parent.Child.Property;

            var members = expression
              .GetMemberExpressions()               // unwraps the LambdaExpression
              .Select(exp => exp.GetValue())
              .AsReadOnlyList();

            members[0].ShouldBe(parent);
            members[1].ShouldBe(parent.Child);
            members[2].ShouldBe(parent.Child.Property);

            var value = expression.GetValue();

            value.ShouldBe(parent.Child.Property);
        }

        [Fact]
        public void Get_Property_Of_Child()
        {
            var parent = Create<ParentClass>();

            Expression<Func<int>> expression = () => parent.Child.Property;

            var memberInfo = expression.GetPropertyOrFieldMemberInfo();

            var name = memberInfo.Name;
            var value = memberInfo.GetValue(parent.Child);

            name.ShouldBe(nameof(ChildClass.Property));
            value.ShouldBe(parent.Child.Property);
        }

        [Fact]
        public void Get_Field_Of_Child()
        {
            var parent = Create<ParentClass>();

            Expression<Func<int>> expression = () => parent.Child.Field;

            var memberInfo = expression.GetPropertyOrFieldMemberInfo();

            var name = memberInfo.Name;
            var value = memberInfo.GetValue(parent.Child);

            name.ShouldBe(nameof(ChildClass.Field));
            value.ShouldBe(parent.Child.Field);
        }

        private static int GetValue(Expression<Func<int>> expression)
        {
            var value = expression.GetValue();

            return value.As<int>();
        }
    }
}







