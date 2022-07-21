using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Reflection.Exceptions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using System;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class FieldHelperFixture
    {
        public class CreateFieldSetter_Object : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_FieldInfo_Null()
            {
                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldSetter(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldInfo");
            }

            [Fact]
            public void Should_Create_Setter()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetter(fieldInfo);

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_For_Private_Field()
            {
                var value = Create<int>();
                var model = new DummyClass(value);

                // can't use nameof() since it is private
                var fieldInfo = typeof(DummyClass).GetField("Field2", BindingFlags.Instance | BindingFlags.NonPublic);
                var getter = FieldHelper.CreateFieldGetter(fieldInfo);

                var actual = getter.Invoke(model);

                actual.Should().Be(value);

                // now change it
                var expected = Create<int>();

                var setter = FieldHelper.CreateFieldSetter(fieldInfo);
                setter.Invoke(model, expected);

                actual = getter.Invoke(model);

                actual.Should().Be(expected);
            }
        }

        public class CreateFieldSetter_Typed_FieldInfo : FieldHelperFixture
        {
            [Fact]
            public void Should_Throw_When_FieldInfo_Null()
            {
                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldSetter<DummyClass>((FieldInfo) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldInfo");
            }

            [Fact]
            public void Should_Create_Setter()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetter<DummyClass>(fieldInfo);

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }
        }

        public class CreateFieldSetter_Typed_FieldName : FieldHelperFixture
        {
            [Fact]
            public void Should_Throw_When_FieldInfo_Null()
            {
                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldSetter<DummyClass>((string) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldName");
            }

            [Fact]
            public void Should_Create_Setter()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var setter = FieldHelper.CreateFieldSetter<DummyClass>(nameof(DummyClass.Field1));

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Field_Does_Not_Exist()
            {
                var fieldName = Create<string>();

                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldSetter<DummyClass>(fieldName);
                })
                   .Should()
                   .Throw<ReflectionException>()
                   .WithMessage($"The field {fieldName} on type {typeof(DummyClass).GetFriendlyName()} does not exist.");
            }
        }
    }
}
