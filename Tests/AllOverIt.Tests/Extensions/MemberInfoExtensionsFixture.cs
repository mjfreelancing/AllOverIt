using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Extensions
{
    public class MemberInfoExtensionsFixture : FixtureBase
    {
        private class DummyClassBase
        {
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }
        }

        private class DummySuperClass : DummyClassBase
        {
            public override double Prop3 { get; set; }

            public int Field5;

            public DummySuperClass()
            {
                Field5 = 1;         // Prevent CS0649 (Field is never assigned)
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

        private sealed class MemberInfoDummy : System.Reflection.MemberInfo
        {
            public override Type DeclaringType => throw new NotImplementedException();

            public override System.Reflection.MemberTypes MemberType => throw new NotImplementedException();

            public override string Name => throw new NotImplementedException();

            public override Type ReflectedType => throw new NotImplementedException();

            public override object[] GetCustomAttributes(bool inherit)
            {
                throw new NotImplementedException();
            }

            public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            public override bool IsDefined(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }
        }

        public class SetValue : MemberInfoExtensionsFixture
        {
            [Fact]
            public void Should_Set_Property_Value()
            {
                var superClass = new DummySuperClass();

                Expression<Func<double>> expression = () => superClass.Prop3;

                var memberInfo = expression.GetPropertyOrFieldMemberInfo();

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

                var memberInfo = expression.GetPropertyOrFieldMemberInfo();

                var name = memberInfo.Name;
                var value = CreateExcluding<int>(0);

                MemberInfoExtensions.SetValue(memberInfo, superClass, value);

                name.Should().Be(nameof(DummySuperClass.Field5));
                value.Should().Be(superClass.Field5);
            }

            [Fact]
            public void Should_Throw_When_Not_Property_Or_Field()
            {
                Invoking(() =>
                {
                    var memberInfo = new MemberInfoDummy();

                    MemberInfoExtensions.SetValue(memberInfo, new { }, null);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Expected memberInfo to be a property or field.");
            }
        }

        public class GetValue : MemberInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Value()
            {
                var superClass = Create<DummySuperClass>();

                Expression<Func<double>> expression = () => superClass.Prop3;

                var memberInfo = expression.GetPropertyOrFieldMemberInfo();

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

                var memberInfo = expression.GetPropertyOrFieldMemberInfo();

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

                var memberInfo = expression.GetPropertyOrFieldMemberInfo();

                var name = memberInfo.Name;
                var value = MemberInfoExtensions.GetValue(memberInfo, parentClass.SuperClass);

                name.Should().Be(nameof(DummySuperClass.Prop2));
                value.Should().Be(parentClass.SuperClass.Prop2);
            }

            [Fact]
            public void Should_Throw_When_Not_Property_Or_Field()
            {
                Invoking(() =>
                {
                    var memberInfo = new MemberInfoDummy();

                    MemberInfoExtensions.GetValue(memberInfo, new { });
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Expected memberInfo to be a property or field.");
            }
        }

        public class GetMemberType : MemberInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Type()
            {
                var parentClass = Create<DummyParentClass>();

                Expression<Func<string>> expression = () => parentClass.SuperClass.Prop2;

                var memberInfo = expression.GetPropertyOrFieldMemberInfo();

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

                var memberInfo = expression.GetPropertyOrFieldMemberInfo();

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

            [Fact]
            public void Should_Throw_When_Not_Property_Or_Field_Or_Method()
            {
                Invoking(() =>
                {
                    var memberInfo = new MemberInfoDummy();

                    MemberInfoExtensions.GetMemberType(memberInfo);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Expected memberInfo to be a property, field, or method.");
            }
        }
    }
}
