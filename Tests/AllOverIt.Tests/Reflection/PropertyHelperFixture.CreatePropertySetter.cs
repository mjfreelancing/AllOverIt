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
    public partial class PropertyHelperFixture : FixtureBase
    {
        public class CreatePropertySetter_Object : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertySetter(null);
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
                var setter = PropertyHelper.CreatePropertySetter(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Setter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop6));
                    _ = PropertyHelper.CreatePropertySetter(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop6)} on type {nameof(DummySuperClass)} does not have a setter.");
            }
        }

        public class CreatePropertySetter_Typed_PropertyInfo : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertySetter<DummyBaseClass>((PropertyInfo) null);
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
                var setter = PropertyHelper.CreatePropertySetter<DummyBaseClass>(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Property_Has_No_Setter()
            {
                Invoking(() =>
                {
                    var propInfo = typeof(DummySuperClass).GetProperty(nameof(DummySuperClass.Prop6));
                    _ = PropertyHelper.CreatePropertySetter<DummySuperClass>(propInfo);
                })
                  .Should()
                  .Throw<ReflectionException>()
                  .WithMessage($"The property {nameof(DummySuperClass.Prop6)} on type {nameof(DummySuperClass)} does not have a setter.");
            }
        }

        public class CreatePropertySetter_Typed_PropertyName : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertySetter<DummyBaseClass>((string) null);
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

                var setter = PropertyHelper.CreatePropertySetter<DummyBaseClass>(nameof(DummyBaseClass.Prop1));

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Property_Does_Not_Exist()
            {
                var propertyName = Create<string>();

                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertySetter<DummyBaseClass>(propertyName);
                })
                   .Should()
                   .Throw<ReflectionException>()
                   .WithMessage($"The property {propertyName} on type {typeof(DummyBaseClass).GetFriendlyName()} does not exist.");
            }
        }
    }
}
