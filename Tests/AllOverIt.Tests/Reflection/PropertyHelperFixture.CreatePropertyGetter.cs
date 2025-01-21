using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Reflection.Exceptions;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Reflection
{
    public partial class PropertyHelperFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            public void Method1()
            {
            }

            private void Method2()
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
                Prop4 = 1;
            }

            private void Method4()
            {
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

                var prop1Info = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var getter1 = PropertyHelper.CreatePropertyGetter(prop1Info);

                var prop2Info = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop2));       // testing null string
                var getter2 = PropertyHelper.CreatePropertyGetter(prop2Info);

                var actual1 = getter1.Invoke(expected);
                actual1.Should().Be(expected.Prop1);

                var actual2 = getter2.Invoke(expected);
                actual2.Should().BeNull();

                expected.Prop2 = Create<string>();
                actual2 = getter2.Invoke(expected);
                actual2.Should().Be(expected.Prop2);
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
                    _ = PropertyHelper.CreatePropertyGetter<DummyBaseClass>((PropertyInfo)null);
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
            public void Should_Create_Getter_For_Derived()
            {
                var expected = new DummySuperClass
                {
                    Prop1 = Create<int>()
                };

                var propInfo = typeof(DummyBaseClass).GetProperty(nameof(DummyBaseClass.Prop1));
                var getter = PropertyHelper.CreatePropertyGetter<DummySuperClass>(propInfo);

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
            public void Should_Throw_When_PropertyName_Null()
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
            public void Should_Create_Getter_For_Derived()
            {
                var expected = new DummySuperClass
                {
                    Prop1 = Create<int>()
                };

                var getter = PropertyHelper.CreatePropertyGetter<DummySuperClass>(nameof(DummyBaseClass.Prop1));

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

        public class CreatePropertyGetter_Typed_Property_PropertyName : PropertyHelperFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyName_Null()
            {
                Invoking(() =>
                {
                    _ = PropertyHelper.CreatePropertyGetter<DummyBaseClass, int>((string)null);
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

                var getter = PropertyHelper.CreatePropertyGetter<DummyBaseClass, int>(nameof(DummyBaseClass.Prop1));

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }

            [Fact]
            public void Should_Create_Getter_For_Derived()
            {
                var expected = new DummySuperClass
                {
                    Prop1 = Create<int>()
                };

                var getter = PropertyHelper.CreatePropertyGetter<DummySuperClass, int>(nameof(DummyBaseClass.Prop1));

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Prop1);
            }
        }
    }
}
