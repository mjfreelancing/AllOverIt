using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class FieldHelperFixture : FixtureBase
    {
        private class DummyClass
        {
            public int Field1;
            private int Field2;

            public DummyClass()
            {
            }

            public DummyClass(int field2)
            {
                Field2 = field2;
            }
        }

        public class CreateFieldGetter_Object : FieldHelperFixture
        {
            [Fact]
            public void Should_Throw_When_FieldInfo_Null()
            {
                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldGetter(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldInfo");
            }

            [Fact]
            public void Should_Create_Getter()
            {
                var expected = new DummyClass
                {
                    Field1 = Create<int>()
                };

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var getter = FieldHelper.CreateFieldGetter(fieldInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Field1);
            }

            [Fact]
            public void Should_Create_Getter_For_Private_Field()
            {
                var field2 = Create<int>();
                var model = new DummyClass(field2);

                // can't use nameof() since it is private
                var fieldInfo = typeof(DummyClass).GetField("Field2", BindingFlags.Instance | BindingFlags.NonPublic);
                var getter = FieldHelper.CreateFieldGetter(fieldInfo);

                var actual = getter.Invoke(model);

                actual.Should().Be(field2);
            }
        }

        public class CreateFieldGetter_Typed : FieldHelperFixture
        {
            [Fact]
            public void Should_Throw_When_FieldInfo_Null()
            {
                Invoking(() =>
                {
                    _ = FieldHelper.CreateFieldGetter<DummyClass>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("fieldInfo");
            }

            [Fact]
            public void Should_Create_Getter()
            {
                var expected = new DummyClass
                {
                    Field1 = Create<int>()
                };

                var fieldInfo = typeof(DummyClass).GetField(nameof(DummyClass.Field1));
                var getter = FieldHelper.CreateFieldGetter<DummyClass>(fieldInfo);

                var actual = getter.Invoke(expected);

                actual.Should().Be(expected.Field1);
            }

            [Fact]
            public void Should_Create_Getter_For_Private_Field()
            {
                var field2 = Create<int>();
                var model = new DummyClass(field2);

                // can't use nameof() since it is private
                var fieldInfo = typeof(DummyClass).GetField("Field2", BindingFlags.Instance | BindingFlags.NonPublic);
                var getter = FieldHelper.CreateFieldGetter<DummyClass>(fieldInfo);

                var actual = getter.Invoke(model);

                actual.Should().Be(field2);
            }
        }
    }
}
