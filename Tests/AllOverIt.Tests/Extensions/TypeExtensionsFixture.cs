using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Collections;
using System.Runtime.CompilerServices;

namespace AllOverIt.Tests.Extensions
{
    public partial class TypeExtensionsFixture : FixtureBase
    {
        private class PropertyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            public void Method1()
            {
            }

            private void Method2(int _)
            {
            }
        }

        private class PropertySuperClass : PropertyBaseClass
        {
            private readonly int _value;
            public override double Prop3 { get; set; }
            private long Prop4 { get; set; }
            private long Prop5 { set { } }

            public PropertySuperClass()
            {
            }

            public PropertySuperClass(int value)
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

            private void Method4(bool _)
            {
            }

            private int Method4(int arg1)
            {
                return arg1;
            }
        }

        private class FieldBaseClass
        {
            public int Field1;
            public string Field2;
        }

        private class FieldSuperClass : FieldBaseClass
        {
            private long Field3;
            internal static double Field4;
            public string Field5;
        }

        private class DummyComposite<T1, T2>
        {
        }

        private abstract class DummyEnrichedEnum : EnrichedEnum<DummyEnrichedEnum>
        {
            protected DummyEnrichedEnum(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class SuperEnrichedEnumDummy : DummyEnrichedEnum
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
                var actual = (object) AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(PropertySuperClass), "Prop3");

                var expected = new { Name = "Prop3", PropertyType = typeof(double) };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var actual = (object) AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(PropertySuperClass), "Prop1");

                var expected = new { Name = "Prop1", PropertyType = typeof(int) };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Property()
            {
                var actual = (object) AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(PropertySuperClass), "PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_Bindings : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                    .GetPropertyInfo(typeof(PropertySuperClass))
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
                    .GetPropertyInfo(typeof(PropertySuperClass), BindingOptions.Default, true)
                    .Select(item => new { item.Name, item.PropertyType });

                var expected = new[] { new { Name = "Prop3", PropertyType = typeof(double) } };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Include_Private_Property()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(PropertySuperClass), binding, false);

                actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
            }

            [Fact]
            public void Should_Include_All_Properties()
            {
                var binding = BindingOptions.All;

                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(PropertySuperClass), binding, false);

                var expected = new[] { "Prop1", "Prop2", "Prop3", "Prop4", "Prop5" };

                expected
                  .Should()
                  .BeEquivalentTo(actual.Select(item => item.Name));
            }

            [Fact]
            public void Should_Include_Public_Set_Properties_only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(typeof(PropertySuperClass), BindingOptions.SetMethod, false);

                var expected = new[] { "Prop1", "Prop2", "Prop3" };

                expected
                  .Should()
                  .BeEquivalentTo(actual.Select(item => item.Name));
            }

            [Fact]
            public void Should_Include_Private_Set_Properties_only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(
                    typeof(PropertySuperClass), BindingOptions.Private | BindingOptions.SetMethod, false);

                var expected = new[] { "Prop4", "Prop5" };

                expected
                  .Should()
                  .BeEquivalentTo(actual.Select(item => item.Name));
            }
        }

        public class GetFieldInfo_Field : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Get_Field_In_Super()
            {
                var actual = (object) AllOverIt.Extensions.TypeExtensions.GetFieldInfo(typeof(FieldSuperClass), "Field4");

                var expected = new { Name = "Field4", FieldType = typeof(double) };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Field_In_Base()
            {
                var actual = (object) AllOverIt.Extensions.TypeExtensions.GetFieldInfo(typeof(FieldSuperClass), "Field1");

                var expected = new { Name = "Field1", FieldType = typeof(int) };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Field()
            {
                var actual = (object) AllOverIt.Extensions.TypeExtensions.GetFieldInfo(typeof(FieldSuperClass), "PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetFieldInfo_Bindings : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                    .GetFieldInfo(typeof(FieldSuperClass))
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                    new {Name = "Field1", FieldType = typeof(int)},
                    new {Name = "Field2", FieldType = typeof(string)},
                    new {Name = "Field5", FieldType = typeof(string)}
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                    .GetFieldInfo(typeof(FieldSuperClass), BindingOptions.Default, true)
                    .Select(item => new { item.Name, item.FieldType });

                var expected = new[]
                {
                    new {Name = "Field5", FieldType = typeof(string)}
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Include_Private_Field()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = AllOverIt.Extensions.TypeExtensions.GetFieldInfo(typeof(FieldSuperClass), binding, false);

                actual.Single(item => item.Name == "Field3").Should().NotBeNull();
            }

            [Fact]
            public void Should_Include_All_Fields()
            {
                var binding = BindingOptions.All;

                var actual = AllOverIt.Extensions.TypeExtensions.GetFieldInfo(typeof(FieldSuperClass), binding, false);

                var expected = new[] { "Field1", "Field2", "Field3", "Field4", "Field5" };

                expected
                  .Should()
                  .BeEquivalentTo(actual.Select(item => item.Name));
            }
        }

        public class GetMethodInfo : TypeExtensionsFixture
        {
            private readonly string[] _knownMethods = { "Method1", "Method2", "Method3", "Method4" };

            // GetMethod() returns methods of object as well as property get/set methods, so these tests filter down to expected (non-property) methods in the dummy classes

            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(PropertySuperClass))
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new {Name = "Method1", DeclaringType = typeof(PropertyBaseClass)},
                    new {Name = "Method3", DeclaringType = typeof(PropertySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(PropertySuperClass), BindingOptions.Default, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new {Name = "Method3", DeclaringType = typeof(PropertySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Get_All_Base_Methods_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(PropertyBaseClass), BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                var expected = new[]
                {
                    new {Name = "Method1", DeclaringType = typeof(PropertyBaseClass)},
                    new {Name = "Method2", DeclaringType = typeof(PropertyBaseClass)}
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Get_All_Super_Methods_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(PropertySuperClass), BindingOptions.All, true)
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  }).ToList();

                // there are 3 overloads of Method4
                var expected = new[]
                {
                    new {Name = "Method3", DeclaringType = typeof(PropertySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(PropertySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(PropertySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(PropertySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Get_Private_Methods_Only()
            {
                var actual = AllOverIt.Extensions.TypeExtensions
                  .GetMethodInfo(typeof(PropertySuperClass), BindingOptions.Private, false)   // default scope and visibility is implied
                  .Where(item => _knownMethods.Contains(item.Name))
                  .Select(item => new
                  {
                      item.Name,
                      item.DeclaringType
                  });

                // there are 3 overloads of Method4
                var expected = new[]
                {
                    new {Name = "Method2", DeclaringType = typeof(PropertyBaseClass)},
                    new {Name = "Method4", DeclaringType = typeof(PropertySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(PropertySuperClass)},
                    new {Name = "Method4", DeclaringType = typeof(PropertySuperClass)}
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class GetMethodInfo_Named : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(PropertySuperClass), Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(PropertySuperClass), "Method4");

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new PropertySuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }
        }

        public class GetMethodInfo_Named_And_Args : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Not_Find_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(PropertySuperClass), Create<string>(), Type.EmptyTypes);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Find_Method_With_No_Arguments()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(PropertySuperClass), "Method4", Type.EmptyTypes);

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new PropertySuperClass(expected);

                var value = actual.Invoke(dummy, null);

                value.Should().Be(expected);
            }

            [Fact]
            public void Should_Find_Method_With_Specific_Arguments()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetMethodInfo(typeof(PropertySuperClass), "Method4", new[] { typeof(int) });

                actual.Should().NotBeNull();

                // make sure the correct overload was chosen
                var expected = Create<int>();
                var dummy = new PropertySuperClass();

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
            [InlineData(typeof(PropertySuperClass), false)]
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
            [InlineData(typeof(PropertySuperClass), true)]
            [InlineData(typeof(IEnumerable<int>), false)]
            public void Should_Determine_If_Is_Class_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsClassType(type);

                actual.Should().Be(expected);
            }
        }

        public class IsValueType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(DummyEnum), true)]
            [InlineData(typeof(int), true)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(bool), true)]
            [InlineData(typeof(char), true)]
            [InlineData(typeof(PropertySuperClass), false)]
            [InlineData(typeof(IEnumerable<int>), false)]
            public void Should_Determine_If_Is_Value_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsValueType(type);

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
            [InlineData(typeof(PropertySuperClass), false)]
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
            [InlineData(typeof(PropertySuperClass), false)]
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
            [InlineData(typeof(PropertySuperClass), false)]
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

        public class GetEnumerableElementType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(int[]), typeof(int))]
            [InlineData(typeof(IEnumerable<string>), typeof(string))]
            [InlineData(typeof(IList<double>), typeof(double))]
            [InlineData(typeof(ArrayList), typeof(object))]                 // ArrayList is a IEnumerable
            public void Should_Get_Element_Type(Type enumerableType, Type expectedType)
            {
                var actual = enumerableType.GetEnumerableElementType();

                expectedType.Should().BeSameAs(actual);
            }

            [Theory]
            [InlineData(typeof(DummyType))]
            public void Should_Throw_If_Not_Enumerable(Type type)
            {
                Invoking(() =>
                {
                    _ = type.GetEnumerableElementType();
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage($"{type.GetFriendlyName()} is not an {nameof(IEnumerable)}.");
            }

            [Theory]
            [InlineData(typeof(IDictionary<string, object>))]
            public void Should_Throw_If_Enumerable_With_More_Than_One_Generic_Argument(Type type)
            {
                Invoking(() =>
                {
                    _ = type.GetEnumerableElementType();
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage($"{type.GetFriendlyName()} is not an {nameof(IEnumerable)} with one generic argument.");
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

        public class IsSubClassOfRawGeneric : TypeExtensionsFixture
        {
            private interface IDerived { }
            private interface IDerived2<TType1, TType2> : IDerived { }
            private class Derived<TType1, TType2> : IDerived2<TType1, TType2> { }
            private class Derived2 : IDerived2<int, double> { }
            private class Derived3 : Derived<int, double> { }

            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                    {
                        AllOverIt.Extensions.TypeExtensions.IsSubClassOfRawGeneric(null, typeof(object));
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_FromType_Null()
            {
                Invoking(() =>
                    {
                        AllOverIt.Extensions.TypeExtensions.IsSubClassOfRawGeneric(typeof(object), null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fromType");
            }

            [Theory]
            [InlineData(typeof(Derived2), typeof(IDerived2<,>), false)]             // Testing against an interface, not a type
            [InlineData(typeof(IDerived), typeof(IDerived), true)]                  // Non-generic
            [InlineData(typeof(Derived2), typeof(Derived2), true)]                  // Non-generic
            [InlineData(typeof(Derived3), typeof(Derived<,>), true)]                // Unbound generic
            [InlineData(typeof(Derived3), typeof(Derived<int, double>), false)]     // Bound generic
            public void Should_Return_Expected_Result(Type type, Type generic, bool expected)
            {
                type.IsSubClassOfRawGeneric(generic).Should().Be(expected);
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

            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                    {
                        AllOverIt.Extensions.TypeExtensions.IsDerivedFrom(null, typeof(object));
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_FromType_Null()
            {
                Invoking(() =>
                    {
                        AllOverIt.Extensions.TypeExtensions.IsDerivedFrom(typeof(object), null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fromType");
            }

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
            [InlineData(typeof(Derived2), typeof(IDerived2<,>), true)]
            [InlineData(typeof(IDerived3<,>), typeof(IDerived2<,>), true)]
            [InlineData(typeof(Derived<>), typeof(Derived), true)]
            [InlineData(typeof(Derived3), typeof(Derived<>), true)]
            [InlineData(typeof(Derived3), typeof(Derived<bool>), false)]
            public void Should_Return_Expected_Result(Type derivedType, Type baseType, bool expected)
            {
                AllOverIt.Extensions.TypeExtensions.IsDerivedFrom(derivedType, baseType)
                    .Should()
                    .Be(expected);
            }
        }

        public class IsNullableType : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(int?), true)]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(PropertySuperClass), false)]
            [InlineData(typeof(DummyEnum?), true)]
            [InlineData(typeof(IEnumerable<int?>), false)]
            [InlineData(typeof(IEnumerable<int>), false)]
            public void Should_Return_If_Is_Generic_Nullable_Type(Type type, bool expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.IsNullableType(type);

                actual.Should().Be(expected);
            }
        }

        public class GetBaseGenericTypeDefinition : TypeExtensionsFixture
        {
            private class DummyList : List<int>
            {
            }

            private class DummyParentList : DummyList
            {
            }

            private class DummyDictionary : Dictionary<int, DummyList>
            {
            }

            [Fact]
            public void Should_Find_IEnumerable_From_List()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(List<int>), typeof(IEnumerable<>));

                actual.Should().BeSameAs(typeof(IEnumerable<int>));
            }

            [Fact]
            public void Should_Find_List_Class()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(DummyList), typeof(List<>));

                actual.Should().BeSameAs(typeof(List<int>));
            }

            [Fact]
            public void Should_Find_List_Class_From_Parent()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(DummyParentList), typeof(List<>));

                actual.Should().BeSameAs(typeof(List<int>));
            }

            [Fact]
            public void Should_Find_IEnumerable()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(DummyList), typeof(IEnumerable<>));

                actual.Should().BeSameAs(typeof(IEnumerable<int>));
            }

            [Fact]
            public void Should_Find_IEnumerable_From_Parent()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(DummyParentList), typeof(IEnumerable<>));

                actual.Should().BeSameAs(typeof(IEnumerable<int>));
            }

            [Fact]
            public void Should_Find_Dictionary()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(DummyDictionary), typeof(Dictionary<,>));

                actual.Should().BeSameAs(typeof(Dictionary<int, DummyList>));
            }

            [Fact]
            public void Should_Find_IDictionary()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(DummyDictionary), typeof(IDictionary<,>));

                actual.Should().BeSameAs(typeof(IDictionary<int, DummyList>));
            }

            [Fact]
            public void Should_Find_IEnumerable_In_Dictionary()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetBaseGenericTypeDefinition(typeof(DummyDictionary), typeof(IEnumerable<>));

                actual.Should().BeSameAs(typeof(IEnumerable<KeyValuePair<int, DummyList>>));
            }
        }

        public class GetFriendlyName : TypeExtensionsFixture
        {
            [Theory]
            [InlineData(typeof(object), "Object")]
            [InlineData(typeof(int), "Int32")]
            [InlineData(typeof(int?), "Int32?")]
            [InlineData(typeof(Dictionary<int, string>.KeyCollection), "KeyCollection<Int32, String>")]     // has no backticks in the name
            [InlineData(typeof(DummyEnum), "DummyEnum")]
            [InlineData(typeof(DummyEnum?), "DummyEnum?")]
            [InlineData(typeof(IEnumerable<DummyEnum>), "IEnumerable<DummyEnum>")]
            [InlineData(typeof(IEnumerable<DummyEnum?>), "IEnumerable<DummyEnum?>")]
            [InlineData(typeof(IDictionary<DummyEnum, PropertySuperClass>), "IDictionary<DummyEnum, PropertySuperClass>")]
            [InlineData(typeof(KeyValuePair<DummyEnum?, PropertyBaseClass>), "KeyValuePair<DummyEnum?, PropertyBaseClass>")]
            [InlineData(typeof(IEnumerable<IDictionary<DummyEnum, PropertySuperClass>>), "IEnumerable<IDictionary<DummyEnum, PropertySuperClass>>")]
            public void Should_Create_Friendly_Type_Name(Type type, string expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetFriendlyName(type);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(typeof(object), "System.Object")]
            [InlineData(typeof(int), "System.Int32")]
            [InlineData(typeof(int?), "System.Int32?")]
            [InlineData(typeof(Dictionary<int, string>.KeyCollection), "System.Collections.Generic.KeyCollection<Int32, String>")]     // has no backticks in the name and the inner types do not have a namespace
            [InlineData(typeof(DummyEnum), "AllOverIt.Tests.Extensions.TypeExtensionsFixture.DummyEnum")]
            [InlineData(typeof(DummyEnum?), "AllOverIt.Tests.Extensions.TypeExtensionsFixture.DummyEnum?")]
            [InlineData(typeof(IEnumerable<DummyEnum>), "System.Collections.Generic.IEnumerable<DummyEnum>")]
            [InlineData(typeof(IEnumerable<DummyEnum?>), "System.Collections.Generic.IEnumerable<DummyEnum?>")]
            [InlineData(typeof(IDictionary<DummyEnum, PropertySuperClass>), "System.Collections.Generic.IDictionary<DummyEnum, PropertySuperClass>")]
            [InlineData(typeof(KeyValuePair<DummyEnum?, PropertyBaseClass>), "System.Collections.Generic.KeyValuePair<DummyEnum?, PropertyBaseClass>")]
            [InlineData(typeof(IEnumerable<IDictionary<DummyEnum, PropertySuperClass>>), "System.Collections.Generic.IEnumerable<IDictionary<DummyEnum, PropertySuperClass>>")]
            public void Should_Create_Friendly_Type_Name_With_Namespace(Type type, string expected)
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetFriendlyName(type, true);

                actual.Should().Be(expected);
            }
        }

        public class IsEnrichedEnum : TypeExtensionsFixture
        {
            [Fact]
            public void Should_Return_False()
            {
                var actual = typeof(PropertySuperClass).IsEnrichedEnum();

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True()
            {
                var actual = typeof(DummyEnrichedEnum).IsEnrichedEnum();

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_For_Derived_Enum()
            {
                var actual = typeof(SuperEnrichedEnumDummy).IsEnrichedEnum();

                actual.Should().BeTrue();
            }
        }

        public class GetStaticMethod : TypeExtensionsFixture
        {
            private class StaticMethodClass
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
                public static void Method1()
#pragma warning restore CA1822 // Mark members as static
                {
                }

                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
#pragma warning disable CA1822 // Mark members as static
                private static void Method2()
#pragma warning restore CA1822 // Mark members as static
                {
                }

                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
#pragma warning disable CA1822 // Mark members as static
                internal static void Method3()
#pragma warning restore CA1822 // Mark members as static
                {
                }
            }

            [Fact]
            public void Should_Not_Get_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetStaticMethod(typeof(StaticMethodClass), Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Get_Public_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetStaticMethod(typeof(StaticMethodClass), nameof(StaticMethodClass.Method1));

                actual.Name.Should().Be(nameof(StaticMethodClass.Method1));
            }

            [Fact]
            public void Should_Get_Private_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetStaticMethod(typeof(StaticMethodClass), "Method2");

                actual.Name.Should().Be("Method2");
            }

            [Fact]
            public void Should_Get_Internal_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetStaticMethod(typeof(StaticMethodClass), nameof(StaticMethodClass.Method3));

                actual.Name.Should().Be(nameof(StaticMethodClass.Method3));
            }
        }

        public class GetInstanceMethod : TypeExtensionsFixture
        {
            private class InstanceMethodClass
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
                public void Method1()
#pragma warning restore CA1822 // Mark members as static
                {
                }

                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
#pragma warning disable CA1822 // Mark members as static
                private void Method2()
#pragma warning restore CA1822 // Mark members as static
                {
                }

                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
#pragma warning disable CA1822 // Mark members as static
                internal void Method3()
#pragma warning restore CA1822 // Mark members as static
                {
                }
            }

            [Fact]
            public void Should_Not_Get_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetInstanceMethod(typeof(InstanceMethodClass), Create<string>());

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Get_Public_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetInstanceMethod(typeof(InstanceMethodClass), nameof(InstanceMethodClass.Method1));

                actual.Name.Should().Be(nameof(InstanceMethodClass.Method1));
            }

            [Fact]
            public void Should_Get_Private_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetInstanceMethod(typeof(InstanceMethodClass), "Method2");

                actual.Name.Should().Be("Method2");
            }

            [Fact]
            public void Should_Get_Internal_Method()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.GetInstanceMethod(typeof(InstanceMethodClass), nameof(InstanceMethodClass.Method3));

                actual.Name.Should().Be(nameof(InstanceMethodClass.Method3));
            }
        }

        public class CreateListOf : TypeExtensionsFixture
        {

            [Fact]
            public void Should_Create_ListOf()
            {
                var actual = AllOverIt.Extensions.TypeExtensions.CreateListOf(typeof(string));

                actual.Should().BeOfType<List<string>>();
            }
        }
    }
}