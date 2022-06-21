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
        public class CreatePropertyGetter_Object : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertyGetter(null);
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
                var getter = ReflectionHelper.CreatePropertyGetter(propInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }
        }

        public class CreatePropertyGetter_Typed : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertyGetter<DummyBaseClass>((PropertyInfo)null);
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
                var getter = ReflectionHelper.CreatePropertyGetter<DummyBaseClass>(propInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }
        }

        public class CreatePropertyGetter_Typed_PropertyName : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertyGetter<DummyBaseClass>((string)null);
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

                var getter = ReflectionHelper.CreatePropertyGetter<DummyBaseClass>(nameof(DummyBaseClass.Prop1));

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }
        }

        public class CreatePropertyGetter_Typed_PropertyInfo : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = ReflectionHelper.CreatePropertyGetter<DummyBaseClass>((PropertyInfo) null);
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
                var getter = ReflectionHelper.CreatePropertyGetter<DummyBaseClass>(propInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }
        }
    }
}
