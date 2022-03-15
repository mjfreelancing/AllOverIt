﻿using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AllOverIt.Patterns.Enumeration;
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

        private abstract class EnrichedEnumDummy : EnrichedEnum<EnrichedEnumDummy>
        {
            protected EnrichedEnumDummy(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class SuperEnrichedEnumDummy : EnrichedEnumDummy
        {
            public static readonly SuperEnrichedEnumDummy Value1 = new(1);
            public static readonly SuperEnrichedEnumDummy Value2 = new(2);

            private SuperEnrichedEnumDummy(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        public enum DummyEnum { One, Two, Three }

        public class GetPropertyInfo_Property : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var actual = (object) AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), "Prop3");

                var expected = new {Name = "Prop3", PropertyType = typeof(double)};

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var actual = (object)AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(DummySuperClass), "Prop1");

                var expected = new {Name = "Prop1", PropertyType = typeof(int)};

                actual.Should().BeEquivalentTo(expected);
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
                var actual = AllOverIt.Extensions.TypeExtensions
                    .GetPropertyInfo(typeof(DummySuperClass))
                    .Select(item => new { item.Name, item.PropertyType });

                var expected = new[]
                {
                    new {Name = "Prop1", PropertyType = typeof(int)},
                    new {Name = "Prop2", PropertyType = typeof(string)},
                    new {Name = "Prop3", PropertyType = typeof(double)}
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                    .GetPropertyInfo(typeof(DummySuperClass), BindingOptions.Default, true)
                    .Select(item => new {item.Name, item.PropertyType});

                var expected = new[] {new {Name = "Prop3", PropertyType = typeof(double)}};

                expected.Should().BeEquivalentTo(actual);
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

                var expected = new[]{ "Prop1", "Prop2", "Prop3", "Prop4" };

                expected
                  .Should()
                  .BeEquivalentTo(actual.Select(item => item.Name));
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

                var expected = new[]
                {
                    new {Name = "Method1", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
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

                var expected = new[]
                {
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
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

                var expected = new[]
                {
                    new {Name = "Method1", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method2", DeclaringType = typeof(DummyBaseClass)}
                };

                expected.Should().BeEquivalentTo(actual);
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
                var expected = new[]
                {
                    new {Name = "Method3", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
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
                var expected = new[]
                {
                    new {Name = "Method2", DeclaringType = typeof(DummyBaseClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(DummySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
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

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class IsDerivedFrom : TypeExtensionsFixture
        {
            private interface IBase { }
            private interface IDerived : IBase { }
            private interface IDerived2<TType1, TType2> : IDerived { }
            private interface IDerived3<TType1, TType2> : IDerived2<TType1, TType2> { }
            private interface IDerived4<TType1, TType2> : IDerived2<TType2, TType1> { }
            private interface IDerived5<TType1, TType2> : IDerived2<TType1, IDerived3<TType1, TType2>> { }
            private class Base : IBase { }
            private class Derived : Base { }
            private class Derived2 : Derived, IDerived2<string, double> { }
            private class Derived<TType> : Derived { }
            private class Derived3 : Derived<Derived3> { }

            [Theory]
            [InlineData(typeof(IDerived2<,>), typeof(IBase), true)]
            [InlineData(typeof(IDerived2<int, double>), typeof(IBase), true)]
            [InlineData(typeof(IDerived2<int, double>), typeof(IDerived2<int, int>), false)]
            [InlineData(typeof(IDerived3<int, double>), typeof(IDerived2<int, double>), true)]
            [InlineData(typeof(IDerived4<int, double>), typeof(IDerived2<int, double>), false)]
            [InlineData(typeof(IDerived4<int, double>), typeof(IDerived2<double, int>), true)]
            [InlineData(typeof(IDerived5<int, double>), typeof(IDerived2<double, int>), false)]
            [InlineData(typeof(IDerived5<int, double>), typeof(IDerived), true)]
            [InlineData(typeof(IDerived5<int, double>), typeof(IDerived2<int, IDerived3<int, double>>), true)]
            [InlineData(typeof(IDerived5<int, double>), typeof(IDerived2<int, IDerived3<double, int>>), false)]
            [InlineData(typeof(Derived), typeof(IBase), true)]
            [InlineData(typeof(Derived2), typeof(IDerived2<string, double>), true)]
            [InlineData(typeof(IDerived2<string, double>), typeof(Derived2), false)]
            [InlineData(typeof(Derived2), typeof(IDerived2<double, string>), false)]
            [InlineData(typeof(Derived2), typeof(IDerived2<,>), false)]
            [InlineData(typeof(Derived<>), typeof(Derived), true)]
            [InlineData(typeof(Derived3), typeof(Derived<>), true)]
            [InlineData(typeof(Derived3), typeof(Derived<bool>), false)]
            public void Should_Return_Expected_Result(Type derivedType, Type baseType, bool expected)
            {
                derivedType.IsDerivedFrom(baseType).Should().Be(expected);
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

        public class IsEnrichedEnum : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Return_False()
            {
                var actual = typeof(DummySuperClass).IsEnrichedEnum();

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True()
            {
                var actual = typeof(EnrichedEnumDummy).IsEnrichedEnum();

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_For_Derived_Enum()
            {
                var actual = typeof(SuperEnrichedEnumDummy).IsEnrichedEnum();

                actual.Should().BeTrue();
            }
        }
    }
}