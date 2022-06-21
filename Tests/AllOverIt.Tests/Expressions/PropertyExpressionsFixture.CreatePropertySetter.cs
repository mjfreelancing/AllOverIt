using AllOverIt.Exceptions;
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
        public class CreatePropertySetter_Object : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertySetter(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Create_Setter()
            {
                var expected = Create<int>();
                var model = new DummyBaseClass();

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var setter = PropertyExpressions.CreatePropertySetter(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Setter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop6));
                    _ = PropertyExpressions.CreatePropertySetter(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop6)} on type {nameof(DummySuperClass)} does not have a setter.");
            }
        }

        public class CreatePropertySetter_Typed_PropertyInfo : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertySetter<DummyBaseClass>((PropertyInfo) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Create_Getter()
            {
                var expected = Create<int>();
                var model = new DummyBaseClass();

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var setter = PropertyExpressions.CreatePropertySetter<DummyBaseClass>(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Setter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop6));
                    _ = PropertyExpressions.CreatePropertySetter<DummySuperClass>(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop6)} on type {nameof(DummySuperClass)} does not have a setter.");
            }
        }

        public class CreatePropertySetter_Typed_PropertyName : PropertyExpressionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertySetter<DummyBaseClass>((string) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyName");
            }

            [Fact]
            public void Should_Create_Getter()
            {
                var expected = Create<int>();
                var model = new DummyBaseClass();

                var setter = PropertyExpressions.CreatePropertySetter<DummyBaseClass>(nameof(DummyBaseClass.Prop1));

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Property_Does_Not_Exist()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _ = PropertyExpressions.CreatePropertySetter<DummyBaseClass>(propertyName);
                })
                   .Should()
                   .Throw<ReflectionException>()
                   .WithMessage($"The property {propertyName} on type {typeof(DummyBaseClass).GetFriendlyName()} does not exist.");
            }
        }
    }
}
