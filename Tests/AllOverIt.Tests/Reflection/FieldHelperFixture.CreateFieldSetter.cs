using AllOverIt.Extensions;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Reflection.Exceptions;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Reflection
{
    public partial class FieldHelperFixture
    {
        private struct DummyStruct
        {
            public int Field1;
        }

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
            public void Should_Create_Setter_Class()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetter(fieldInfo);

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_ParentClass()
            {
                var expected = Create<int>();
                var model = new DummyParentClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetter(fieldInfo);

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_Struct()
            {
                var expected = Create<int>();

                // Note: This overload of CreateFieldSetter() only works for structs when they are boxed.
                //       If you need to pass a strongly typed object then use CreateFieldSetterByRef().
                object model = new DummyStruct();

                var fieldInfo = typeof(DummyStruct).GetField(nameof(DummyStruct.Field1));
                var setter = FieldHelper.CreateFieldSetter(fieldInfo);

                setter(model, expected);

                ((DummyStruct) model).Field1.Should().Be(expected);
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
            public void Should_Create_Setter_Class()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetter<DummyClass>(fieldInfo);

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_ParentClass()
            {
                var expected = Create<int>();
                var model = new DummyParentClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetter<DummyParentClass>(fieldInfo);

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            // This typed version of CreateFieldSetter() will not work with structs. You can use
            // the overload accepting an object (the instance must be declared as object and not using var).
            // Alternatively, use the CreateFieldSetterByRef() method.
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
            public void Should_Create_Setter_Class()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var setter = FieldHelper.CreateFieldSetter<DummyClass>(nameof(DummyClass.Field1));

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_ParentClass()
            {
                var expected = Create<int>();
                var model = new DummyParentClass();

                var setter = FieldHelper.CreateFieldSetter<DummyParentClass>(nameof(DummyClass.Field1));

                setter.Invoke(model, expected);

                model.Field1.Should().Be(expected);
            }

            // This typed version of CreateFieldSetter() will not work with structs. You can use
            // the overload accepting an object (the instance must be declared as object and not using var).
            // Alternatively, use the CreateFieldSetterByRef() method.

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

        public class CreateFieldSetter_Typed_FieldInfo_ByRef : FieldHelperFixture
        {
            [Fact]
            public void Should_Throw_When_FieldInfo_Null()
            {
                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldSetterByRef<DummyClass>((FieldInfo) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldInfo");
            }

            [Fact]
            public void Should_Create_Setter_Class()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetterByRef<DummyClass>(fieldInfo);

                setter.Invoke(ref model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_ParentClass()
            {
                var expected = Create<int>();
                var model = new DummyParentClass();

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var setter = FieldHelper.CreateFieldSetterByRef<DummyParentClass>(fieldInfo);

                setter.Invoke(ref model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_Struct()
            {
                var expected = Create<int>();
                var model = new DummyStruct();

                var fieldInfo = typeof(DummyStruct).GetField(nameof(DummyStruct.Field1));
                var setter = FieldHelper.CreateFieldSetterByRef<DummyStruct>(fieldInfo);

                setter.Invoke(ref model, expected);

                model.Field1.Should().Be(expected);
            }
        }

        public class CreateFieldSetter_Typed_FieldName_ByRef : FieldHelperFixture
        {
            [Fact]
            public void Should_Throw_When_FieldInfo_Null()
            {
                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldSetterByRef<DummyClass>((string) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldName");
            }

            [Fact]
            public void Should_Create_Setter_Class()
            {
                var expected = Create<int>();
                var model = new DummyClass();

                var setter = FieldHelper.CreateFieldSetterByRef<DummyClass>(nameof(DummyClass.Field1));

                setter.Invoke(ref model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_ParentClass()
            {
                var expected = Create<int>();
                var model = new DummyParentClass();

                var setter = FieldHelper.CreateFieldSetterByRef<DummyParentClass>(nameof(DummyClass.Field1));

                setter.Invoke(ref model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_Struct()
            {
                var expected = Create<int>();
                var model = new DummyStruct();

                var setter = FieldHelper.CreateFieldSetterByRef<DummyStruct>(nameof(DummyStruct.Field1));

                setter.Invoke(ref model, expected);

                model.Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Field_Does_Not_Exist()
            {
                var fieldName = Create<string>();

                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldSetterByRef<DummyClass>(fieldName);
                })
                   .Should()
                   .Throw<ReflectionException>()
                   .WithMessage($"The field {fieldName} on type {typeof(DummyClass).GetFriendlyName()} does not exist.");
            }
        }
    }
}
