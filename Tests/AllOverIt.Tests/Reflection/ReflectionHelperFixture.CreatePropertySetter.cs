using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class ReflectionHelperFixture : FixtureBase
    {
        public class CreatePropertySetter_Object : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertySetter(null);
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
                var setter = ReflectionHelper.CreatePropertySetter(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }
        }

        public class CreatePropertySetter_Typed : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertySetter<DummyBaseClass>((PropertyInfo) null);
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
                var setter = ReflectionHelper.CreatePropertySetter<DummyBaseClass>(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }
        }

        public class CreatePropertySetter_Typed_PropertyName : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertySetter<DummyBaseClass>((string) null);
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

                var setter = ReflectionHelper.CreatePropertySetter<DummyBaseClass>(nameof(DummyBaseClass.Prop1));

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }
        }

        public class CreatePropertySetter_Typed_PropertyInfo : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertySetter<DummyBaseClass>((PropertyInfo) null);
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
                var setter = ReflectionHelper.CreatePropertySetter<DummyBaseClass>(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }
        }
    }
}
