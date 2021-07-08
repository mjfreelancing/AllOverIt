using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class TypeExtensionsFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            public void Method1()
            {
            }

            private void Method2(int arg1)
            {
            }
        }

        private class DummySuperClass : DummyBaseClass
        {
            private readonly int _value;
            public override double Prop3 { get; set; }
            private long Prop4 { get; set; }

            public DummySuperClass()
            {
            }

            public DummySuperClass(int value)
            {
                _value = value;
            }

            public void Method3()
            {
            }

            private int Method4()
            {
                return _value;
            }

            private void Method4(bool arg1)
            {
            }

            private int Method4(int arg1)
            {
                return arg1;
            }
        }

        private class DummyComposite<T1, T2>
        {
        }

        public enum DummyEnum { One, Two, Three }

        public class GetPropertyInfo_Property : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var actual = (object)AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), "Prop3");

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop3", PropertyType = typeof(double)}
                );
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var actual = (object)AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), "Prop1");

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop1", PropertyType = typeof(int)}
                );
            }

            [Fact]
            public void Should_Not_Find_Property()
            {
                var actual = (object)AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), "PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_Bindings : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass));

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop1", PropertyType = typeof(int)},
                    new {Name = "Prop2", PropertyType = typeof(string)},
                    new {Name = "Prop3", PropertyType = typeof(double)}
                );
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), BindingOptions.Default, true);

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop3", PropertyType = typeof(double)}
                );
            }

            [Fact]
            public void Should_Include_Private_Property()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), binding, false);

                actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
            }

            [Fact]
            public void Should_Include_All_Properties()
            {
                var binding = BindingOptions.All;

                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), binding, false);

                actual.Select(item => item.Name)
                  .Should()
                  .BeEquivalentTo("Prop1", "Prop2", "Prop3", "Prop4");
            }
        }

        public class GetMethods : TypeExtensionsFixture
        {
            private readonly string[] _knownMethods = { "Method1", "Method2", "Method3", "Method4" };

            // GetMethod() returns methods of object as well as property get/set methods, so these tests filter down to expected (non-property) methods in the dummy classes

            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(DummySuperClass))
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                actual.Should().BeEquivalentTo(
                    new {Name = "Method1", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)}
                );
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(DummySuperClass), BindingOptions.Default, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                actual.Should().BeEquivalentTo(
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)}
                );
            }

            [Fact]
            public void Should_Get_All_Base_Methods_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(DummyBaseClass), BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                actual.Should().BeEquivalentTo(
                    new {Name = "Method1", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method2", DeclaringType = typeof(DummyBaseClass)}
                );
            }

            [Fact]
            public void Should_Get_All_Super_Methods_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(DummySuperClass), BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  }).ToList();

                // there are 3 overloads of Method4
                actual.Should().BeEquivalentTo(
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)}
                );
            }

            [Fact]
            public void Should_Get_Private_Methods_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(DummySuperClass), BindingOptions.Private, false)   // default scope and visibility is implied
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                // there are 3 overloads of Method4
                actual.Should().BeEquivalentTo(
                    new {Name = "Method2", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)}
                );
            }
        }

        public class GetMethodInfo_Named : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(DummySuperClass), Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(DummySuperClass), "Method4");

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }
        }

        public class GetMethodInfo_Named_And_Args : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(DummySuperClass), Create<string>(), Type.EmptyTypes);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(DummySuperClass), "Method4", Type.EmptyTypes);

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
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(DummySuperClass), "Method4", new[] { typeof(int) });

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new DummySuperClass();

                var value = actual.Invoke(dummy, new object[] { expected });

                value.Should().Be(expected);
            }
        }

        public class IsEnumType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(DummyEnum), true)]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(bool), false)]
            [InlineData(typeof(char), false)]
            [InlineData(typeof(DummySuperClass), false)]
            public void Should_Determine_If_Is_Enum_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsEnumType(type);

                actual.Should().Be(expected);
            }
        }

        public class IsClassType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(DummyEnum), false)]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(string), true)]
            [InlineData(typeof(bool), false)]
            [InlineData(typeof(char), false)]
            [InlineData(typeof(DummySuperClass), true)]
            [InlineData(typeof(IEnumerable<int>), false)]
            public void Should_Determine_If_Is_Class_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsClassType(type);

                actual.Should().Be(expected);
            }
        }

        public class IsPrimitiveType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(DummyEnum), false)]
            [InlineData(typeof(int), true)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(bool), true)]
            [InlineData(typeof(char), true)]
            [InlineData(typeof(DummySuperClass), false)]
            public void Should_Determine_If_Is_Primitive_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsPrimitiveType(type);

                actual.Should().Be(expected);
            }
        }

        public class IsIntegralType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(DummyEnum), false)]
            [InlineData(typeof(int), true)]
            [InlineData(typeof(double), false)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(bool), false)]
            [InlineData(typeof(char), false)]
            [InlineData(typeof(DummySuperClass), false)]
            public void Should_Determine_If_Is_Integral_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsIntegralType(type);

                actual.Should().Be(expected);
            }
        }

        public class IsFloatingType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(DummyEnum), false)]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(double), true)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(bool), false)]
            [InlineData(typeof(char), false)]
            [InlineData(typeof(DummySuperClass), false)]
            public void Should_Determine_If_Is_Integral_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsFloatingType(type);

                actual.Should().Be(expected);
            }
        }

        public class IsEnumerableType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(string), true)]
            [InlineData(typeof(IList<int>), true)]
            [InlineData(typeof(IEnumerable<int>), true)]
            public void Should_Return_If_Is_Enumerable_Type_Include_String(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsEnumerableType(type, true);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(IList<int>), true)]
            [InlineData(typeof(IEnumerable<int>), true)]
            public void Should_Return_If_Is_Enumerable_Type_Exclude_String(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsEnumerableType(type, false);

                actual.Should().Be(expected);
            }
        }

        public class IsGenericEnumerableType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(Lazy<int>), false)]
            [InlineData(typeof(IList<int>), true)]
            [InlineData(typeof(IEnumerable<int>), true)]
            public void Should_Return_If_Is_Generic_Enumerable_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsGenericEnumerableType(type);

                actual.Should().Be(expected);
            }
        }

        public class IsGenericType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(Lazy<int>), true)]
            [InlineData(typeof(IList<int>), true)]
            [InlineData(typeof(IEnumerable<int>), true)]
            public void Should_Return_If_Is_Generic_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsGenericType(type);

                actual.Should().Be(expected);
            }
        }

        public class GetGenericArguments : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(Lazy<int>), new[] { typeof(int) })]
            [InlineData(typeof(IEnumerable<IList<IDictionary<int, string>>>), new[] { typeof(IList<IDictionary<int, string>>) })]
            [InlineData(typeof(DummyComposite<int, IDictionary<int, string>>), new[] { typeof(int), typeof(IDictionary<int, string>) })]
            public void Should_Get_Generic_Argument_Types(Type type, IEnumerable<Type> expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetGenericArguments(type);

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class IsAssignableFromType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(DummySuperClass), typeof(DummySuperClass), true)]
            [InlineData(typeof(DummySuperClass), typeof(DummyBaseClass), false)]
            [InlineData(typeof(DummyBaseClass), typeof(DummySuperClass), true)]
            public void Should_Return_If_Is_Assignable_From_Type(Type type, Type fromType, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsAssignableFromType(type, fromType);

                actual.Should().Be(expected);
            }
        }

        public class IsGenericNullableType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(int?), true)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(DummySuperClass), false)]
            [InlineData(typeof(DummyEnum?), true)]
            [InlineData(typeof(IEnumerable<int?>), false)]
            [InlineData(typeof(IEnumerable<int>), false)]
            public void Should_Return_If_Is_Generic_Nullable_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsGenericNullableType(type);

                actual.Should().Be(expected);
            }
        }

        public class GetFriendlyName : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(object), "Object")]
            [InlineData(typeof(int), "Int32")]
            [InlineData(typeof(int?), "Int32?")]
            [InlineData(typeof(Dictionary<int,string>.KeyCollection), "KeyCollection<Int32, String>")]     // has no backticks in the name
            [InlineData(typeof(DummyEnum), "DummyEnum")]
            [InlineData(typeof(DummyEnum?), "DummyEnum?")]
            [InlineData(typeof(IEnumerable<DummyEnum>), "IEnumerable<DummyEnum>")]
            [InlineData(typeof(IEnumerable<DummyEnum?>), "IEnumerable<DummyEnum?>")]
            [InlineData(typeof(IDictionary<DummyEnum, DummySuperClass>), "IDictionary<DummyEnum, DummySuperClass>")]
            [InlineData(typeof(KeyValuePair<DummyEnum?, DummyBaseClass>), "KeyValuePair<DummyEnum?, DummyBaseClass>")]
            [InlineData(typeof(IEnumerable<IDictionary<DummyEnum, DummySuperClass>>), "IEnumerable<IDictionary<DummyEnum, DummySuperClass>>")]
            public void Should_Create_Friendly_Type_Name(Type type, string expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetFriendlyName(type);

                actual.Should().Be(expected);
            }
        }
    }
}