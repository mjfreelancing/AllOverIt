using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class ReflectionHelperFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            public void Method1()
            {
            }

            private void Method2()
            {
            }
        }

        private class DummySuperClass : DummyBaseClass
        {
            private readonly int _value;
            public override double Prop3 { get; set; }
            private long Prop4 { get; set; }
            public int Field5;

            public DummySuperClass()
            {
            }

            public DummySuperClass(int value)
            {
                _value = value;
            }

            public DummySuperClass(int value, int field5)
            {
                _value = value;
                Field5 = field5;
            }

            public void Method3()
            {
            }

            private int Method4()
            {
                return _value;
            }

            private static void Method4(bool arg1)
            {
            }

            private int Method4(int arg1)
            {
                return arg1;
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

        private class DummyComposite<T1, T2>
        {
        }

        public enum DummyEnum { One, Two, Three }

        public class GetPropertyInfo_Property : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop3");

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop3", PropertyType = typeof(double)}
                );
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop1");

                actual.Should().BeEquivalentTo(new {Name = "Prop1", PropertyType = typeof(int)});
            }

            [Fact]
            public void Should_Not_Find_Property()
            {
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_Bindings : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>();

                actual.Should().BeEquivalentTo(new[]
                {
                    new {Name = "Prop1", PropertyType = typeof(int)},
                    new {Name = "Prop2", PropertyType = typeof(string)},
                    new {Name = "Prop3", PropertyType = typeof(double)}
                });
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(BindingOptions.Default, true);

                actual.Should().BeEquivalentTo(new[]
                {
                    new {Name = "Prop3", PropertyType = typeof(double)}
                });
            }

            [Fact]
            public void Should_Include_Private_Property()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(binding, false);

                actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
            }
        }

        public class GetMethodInfo : ReflectionHelperFixture
        {
            private readonly string[] _knownMethods = { "Method1", "Method2", "Method3", "Method4" };

            // GetMethod() returns methods of object as well as property get/set methods, so these tests filter down to expected (non-property) methods in the dummy classes

            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>()
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                actual.Should().BeEquivalentTo(new[]
                {
                    new {Name = "Method1", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)}
                });
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.Default, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                actual.Should().BeEquivalentTo(new[]
                {
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)}
                });
            }

            [Fact]
            public void Should_Get_All_Base_Methods_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummyBaseClass>(BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                actual.Should().BeEquivalentTo(new[]
                {
                    new {Name = "Method1", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method2", DeclaringType = typeof(DummyBaseClass)}
                });
            }

            [Fact]
            public void Should_Get_All_Super_Methods_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                // there are 3 overloads of Method4
                actual.Should().BeEquivalentTo(new[]
                {
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)}
                });
            }

            [Fact]
            public void Should_Get_Private_Methods_Only()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.Private, false)   // default scope and visibility is implied
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                // there are 3 overloads of Method4
                actual.Should().BeEquivalentTo(new[]
                {
                    new {Name = "Method2", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)}
                });
            }
        }

        public class GetMethodInfo_Named : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>("Method4");

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }
        }

        public class GetMethodInfo_Named_And_Args : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(Create<string>(), Type.EmptyTypes);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>("Method4", Type.EmptyTypes);

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }

            [Fact]
            public void Should_Find_Method_With_Specific_Arguments()
            {
                var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>("Method4", new[] { typeof(int) });

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass();

                var value = actual.Invoke(dummy, new object[] { expected });

                value.Should().Be(expected);
            }
        }

        public class SetMemberValue : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Set_Property_Value()
            {
                var superClass = new DummySuperClass();

                Expression<Func<double>> expression = () => superClass.Prop3;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var value = CreateExcluding<double>(0);

                ReflectionHelper.SetMemberValue(memberInfo, superClass, value);

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

                ReflectionHelper.SetMemberValue(memberInfo, superClass, value);

                name.Should().Be(nameof(DummySuperClass.Field5));
                value.Should().Be(superClass.Field5);
            }
        }

        public class GetMemberValue : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Get_Property_Value()
            {
                var superClass = Create<DummySuperClass>();

                Expression<Func<double>> expression = () => superClass.Prop3;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var value = ReflectionHelper.GetMemberValue(memberInfo, superClass);

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
                var value = ReflectionHelper.GetMemberValue(memberInfo, superClass);

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
                var value = ReflectionHelper.GetMemberValue(memberInfo, parentClass.SuperClass);

                name.Should().Be(nameof(DummySuperClass.Prop2));
                value.Should().Be(parentClass.SuperClass.Prop2);
            }
        }

        public class GetMemberType : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Get_Property_Type()
            {
                var parentClass = Create<DummyParentClass>();

                Expression<Func<string>> expression = () => parentClass.SuperClass.Prop2;

                var memberInfo = expression.GetFieldOrProperty();

                var name = memberInfo.Name;
                var propType = ReflectionHelper.GetMemberType(memberInfo);

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
                var fieldType = ReflectionHelper.GetMemberType(memberInfo);

                name.Should().Be(nameof(DummySuperClass.Field5));
                fieldType.Should().Be(parentClass.SuperClass.Field5.GetType());
            }

            [Fact]
            public void Should_Get_Method_Return_Type()
            {
                var methodInfo = typeof(DummySuperClass).GetMethodInfo(nameof(DummySuperClass.Method6));

                var returnType = ReflectionHelper.GetMemberType(methodInfo);

                returnType.Should().Be(typeof(double));
            }
        }
    }
}
