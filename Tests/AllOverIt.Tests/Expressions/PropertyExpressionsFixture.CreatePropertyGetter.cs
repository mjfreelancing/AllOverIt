﻿using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class PropertyExpressionsFixture : FixtureBase
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

            public bool Prop5 { set { _ = value; } }    // automatic properties must have a getter

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

            public int Method6(int arg)
            {
                return arg;
            }
        }

        public class CreatePropertyGetter_Object : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertyGetter(null);
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
                var getter = PropertyExpressions.CreatePropertyGetter(propInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Getter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop5));
                    _ = PropertyExpressions.CreatePropertyGetter(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop5)} on type {nameof(DummySuperClass)} does not have a getter.");
            }
        }

        public class CreatePropertyGetter_Typed_PropertyInfo : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertyGetter<DummyBaseClass>((PropertyInfo) null);
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
                var getter = PropertyExpressions.CreatePropertyGetter<DummyBaseClass>(propInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Getter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop5));
                    _ = PropertyExpressions.CreatePropertyGetter<DummySuperClass>(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop5)} on type {nameof(DummySuperClass)} does not have a getter.");
            }
        }

        public class CreatePropertyGetter_Typed_PropertyName : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertyGetter<DummyBaseClass>((string)null);
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

                var getter = PropertyExpressions.CreatePropertyGetter<DummyBaseClass>(nameof(DummyBaseClass.Prop1));

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }

            [Fact]
            public void Should_Throw_When_Property_Does_Not_Exist()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertyGetter<DummyBaseClass>(propertyName);
                })
                   .Should()
                   .Throw<ReflectionException>()
                   .WithMessage($"The property {propertyName} on type {typeof(DummyBaseClass).GetFriendlyName()} does not exist.");
            }
        }
    }
}
