using AllOverIt.Extensions;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Reflection.Exceptions;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Reflection
{
    public partial class PropertyHelperFixture
    {
        private struct DummyStruct
        {
            public int Field1 { get; set; }
        }

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
            public void Should_Create_Setter_Class()
            {
                var model = new DummyBaseClass();

                var prop1Info = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var setter1 = PropertyHelper.CreatePropertySetter(prop1Info);

                var prop2Info = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop2));   // testing null string
                var setter2 = PropertyHelper.CreatePropertySetter(prop2Info);

                var expected1 = Create<int>();

                setter1.Invoke(model, expected1);
                model.Prop1.Should().Be(expected1);

                var expected2 = Create<string>();

                setter2.Invoke(model, expected2);
                model.Prop2.Should().Be(expected2);

                setter2.Invoke(model, null);
                model.Prop2.Should().BeNull();
            }

            [Fact]
            public void Should_Create_Setter_SuperClass()
            {
                var expected = Create<int>();
                var model = new DummySuperClass();

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var setter = PropertyHelper.CreatePropertySetter(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_Struct()
            {
                var expected = Create<int>();

                // Note: This overload of CreateFieldSetter() only works for structs when they are boxed.
                //       If you need to pass a strongly typed object then use CreateFieldSetterByRef().
                object model = new DummyStruct();

                var propInfo = typeof(DummyStruct).GetProperty(nameof(DummyStruct.Field1));
                var setter = PropertyHelper.CreatePropertySetter(propInfo);

                setter.Invoke(model, expected);

                ((DummyStruct) model).Field1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_For_Private_Property()
            {
                var model = new DummySuperClass();
                model.Method3();     // Sets Prop4 to 1

                // can't use nameof() since it is private
                var propInfo = typeof(DummySuperClass).GetProperty("Prop4", BindingFlags.Instance | BindingFlags.NonPublic);
                var getter = PropertyHelper.CreatePropertyGetter(propInfo);

                var actual = getter.Invoke(model);

                actual.Should().Be(1);

                // now change it
                var expected = Create<long>();

                var setter = PropertyHelper.CreatePropertySetter(propInfo);
                setter.Invoke(model, expected);

                actual = getter.Invoke(model);

                actual.Should().Be(expected);
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
            public void Should_Create_Setter_Class()
            {
                var expected = Create<int>();
                var model = new DummyBaseClass();

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var setter = PropertyHelper.CreatePropertySetter<DummyBaseClass>(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_SuperClass()
            {
                var expected = Create<int>();
                var model = new DummySuperClass();

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var setter = PropertyHelper.CreatePropertySetter<DummySuperClass>(propInfo);

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            // This typed version of CreatePropertySetter() will not work with structs. You can use
            // the overload accepting an object (the instance must be declared as object and not using var).

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
            public void Should_Create_Setter_Class()
            {
                var expected = Create<int>();
                var model = new DummyBaseClass();

                var setter = PropertyHelper.CreatePropertySetter<DummyBaseClass>(nameof(DummyBaseClass.Prop1));

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Setter_SuperClass()
            {
                var expected = Create<int>();
                var model = new DummySuperClass();

                var setter = PropertyHelper.CreatePropertySetter<DummySuperClass>(nameof(DummyBaseClass.Prop1));

                setter.Invoke(model, expected);

                model.Prop1.Should().Be(expected);
            }

            // This typed version of CreatePropertySetter() will not work with structs. You can use
            // the overload accepting an object (the instance must be declared as object and not using var).

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
