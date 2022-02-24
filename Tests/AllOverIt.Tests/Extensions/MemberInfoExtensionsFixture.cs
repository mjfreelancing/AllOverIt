using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class MemberInfoExtensionsFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }
        }

        private class DummySuperClass : DummyBaseClass
        {
            public override double Prop3 { get; set; }
            public int Field5;

            public DummySuperClass()
            {
            }

            public static double Method6()
            {
                return default;
            }
        }

        private class DummyParentClass
        {
            public DummySuperClass SuperClass { get; set; }
        }

        public class SetValue : MemberInfoExtensionsFixture
        {
            [Fact]
            public void Should_Set_Property_Value()
            {
                var superClass = new DummySuperClass();

                Expression<Func<double>> expression = () => superClass.Prop3;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var value = CreateExcluding<double>(0);

                MemberInfoExtensions.SetValue(memberInfo, superClass, value);

                name.Should().Be(nameof(DummySuperClass.Prop3));
                value.Should().Be(superClass.Prop3);
            }

            [Fact]
            public void Should_Set_Field_Value()
            {
                var superClass = new DummySuperClass();

                Expression<Func<int>> expression = () => superClass.Field5;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var value = CreateExcluding<int>(0);

                MemberInfoExtensions.SetValue(memberInfo, superClass, value);

                name.Should().Be(nameof(DummySuperClass.Field5));
                value.Should().Be(superClass.Field5);
            }
        }

        public class GetValue : MemberInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Value()
            {
                var superClass = Create<DummySuperClass>();

                Expression<Func<double>> expression = () => superClass.Prop3;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var value = MemberInfoExtensions.GetValue(memberInfo, superClass);

                name.Should().Be(nameof(DummySuperClass.Prop3));
                value.Should().Be(superClass.Prop3);
            }

            [Fact]
            public void Should_Get_Field_Value()
            {
                var superClass = Create<DummySuperClass>();

                Expression<Func<int>> expression = () => superClass.Field5;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var value = MemberInfoExtensions.GetValue(memberInfo, superClass);

                name.Should().Be(nameof(DummySuperClass.Field5));
                value.Should().Be(superClass.Field5);
            }

            [Fact]
            public void Should_Get_Child_Property_Value()
            {
                var parentClass = Create<DummyParentClass>();

                Expression<Func<string>> expression = () => parentClass.SuperClass.Prop2;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var value = MemberInfoExtensions.GetValue(memberInfo, parentClass.SuperClass);

                name.Should().Be(nameof(DummySuperClass.Prop2));
                value.Should().Be(parentClass.SuperClass.Prop2);
            }
        }

        public class GetMemberType : MemberInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Type()
            {
                var parentClass = Create<DummyParentClass>();

                Expression<Func<string>> expression = () => parentClass.SuperClass.Prop2;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var propType = MemberInfoExtensions.GetMemberType(memberInfo);

                name.Should().Be(nameof(DummySuperClass.Prop2));
                propType.Should().Be(parentClass.SuperClass.Prop2.GetType());
            }

            [Fact]
            public void Should_Get_Field_Type()
            {
                var parentClass = Create<DummyParentClass>();

                Expression<Func<int>> expression = () => parentClass.SuperClass.Field5;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var fieldType = MemberInfoExtensions.GetMemberType(memberInfo);

                name.Should().Be(nameof(DummySuperClass.Field5));
                fieldType.Should().Be(parentClass.SuperClass.Field5.GetType());
            }

            [Fact]
            public void Should_Get_Method_Return_Type()
            {
                var methodInfo = typeof(DummySuperClass).GetMethodInfo(nameof(DummySuperClass.Method6));

                var returnType = MemberInfoExtensions.GetMemberType(methodInfo);

                returnType.Should().Be(typeof(double));
            }
        }
    }
}
