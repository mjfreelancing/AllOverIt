using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Reflection.Exceptions;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class PropertyHelperFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
            public void Method1()
#pragma warning restore CA1822 // Mark members as static
            {
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "It's part of the test")]
#pragma warning disable CA1822 // Mark members as static
            private void Method2()
#pragma warning restore CA1822 // Mark members as static
            {
            }
        }

        private class DummySuperClass : DummyBaseClass
        {
            private readonly int _value;
            public override double Prop3 { get; set; }

            private long Prop4 { get; set; }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static
            public bool Prop5 { set { _ = value; } }    // automatic properties must have a getter
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression

            public bool Prop6 { get; }

            public int Field1;

            public DummySuperClass()
            {
            }

            public DummySuperClass(int value)
            {
                _value = value;
            }

            public void Method3()
            {
                // Both to prevent IDE0051 (member unused)
                Prop4 = 1;
                Method4();
            }

            private void Method4()
            {
                Field1 = 1;     // Prevent CS0649 (Field is never assigned)
                _ = Prop4;      // Prevent IDE0052 (Prop4 access never used)
            }

            public int Method5()
            {
                return _value;
            }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static
            public int Method6(int arg)
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression
            {
                return arg;
            }
        }

        public class CreatePropertyGetter_Object : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertyGetter(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Create_Getter()
            {
                var expected = new DummyBaseClass
                {
                    Prop1 = Create<int>()
                };

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var getter = PropertyHelper.CreatePropertyGetter(propInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }

            [Fact]
            public void Should_Create_Getter_For_Private_Property()
            {
                var model = new DummySuperClass();
                model.Method3();     // Sets Prop4 to 1

                // can't use nameof() since it is private
                var propInfo = typeof(DummySuperClass).GetProperty("Prop4", BindingFlags.Instance | BindingFlags.NonPublic);
                var getter = PropertyHelper.CreatePropertyGetter(propInfo);

                var actual = getter.Invoke(model);

                actual.Should().Be(1);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Getter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop5));
                    _ = PropertyHelper.CreatePropertyGetter(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop5)} on type {nameof(DummySuperClass)} does not have a getter.");
            }
        }

        public class CreatePropertyGetter_Typed_PropertyInfo : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertyGetter<DummyBaseClass>((PropertyInfo) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Create_Getter()
            {
                var expected = new DummyBaseClass
                {
                    Prop1 = Create<int>()
                };

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var getter = PropertyHelper.CreatePropertyGetter<DummyBaseClass>(propInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }

            [Fact]
            public void Should_Create_Getter_For_Private_Property()
            {
                var model = new DummySuperClass();
                model.Method3();     // Sets Prop4 to 1

                // can't use nameof() since it is private
                var propInfo = typeof(DummySuperClass).GetProperty("Prop4", BindingFlags.Instance | BindingFlags.NonPublic);
                var getter = PropertyHelper.CreatePropertyGetter<DummyBaseClass>(propInfo);

                var actual = getter.Invoke(model);

                actual.Should().Be(1);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Getter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop5));
                    _ = PropertyHelper.CreatePropertyGetter<DummySuperClass>(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop5)} on type {nameof(DummySuperClass)} does not have a getter.");
            }
        }

        public class CreatePropertyGetter_Typed_PropertyName : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertyGetter<DummyBaseClass>((string)null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyName");
            }

            [Fact]
            public void Should_Create_Getter()
            {
                var expected = new DummyBaseClass
                {
                    Prop1 = Create<int>()
                };

                var getter = PropertyHelper.CreatePropertyGetter<DummyBaseClass>(nameof(DummyBaseClass.Prop1));

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }

            [Fact]
            public void Should_Throw_When_Property_Does_Not_Exist()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertyGetter<DummyBaseClass>(propertyName);
                })
                   .Should()
                   .Throw<ReflectionException>()
                   .WithMessage($"The property {propertyName} on type {typeof(DummyBaseClass).GetFriendlyName()} does not exist.");
            }
        }
    }
}
