using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class TypeFactoryFixture : FixtureBase
    {
        private class DummyType
        {
            public int Prop1 { get; }
            public int Prop2 { get; }
            public double Prop3 { get; }
            public double Prop4 { get; }
            public Guid Prop5 { get; }
            public Guid Prop6 { get; }
            public string Prop7 { get; }
            public string Prop8 { get; }

            public DummyType()
            {
            }

            public DummyType(int prop1)
            {
                Prop1 = prop1;
            }

            public DummyType(int prop1, int prop2)
                : this(prop1)
            {
                Prop2 = prop2;
            }

            public DummyType(int prop1, int prop2, double prop3)
                : this(prop1, prop2)
            {
                Prop3 = prop3;
            }

            public DummyType(int prop1, int prop2, double prop3, double prop4)
                : this(prop1, prop2, prop3)
            {
                Prop4 = prop4;
            }

            public DummyType(int prop1, int prop2, double prop3, double prop4, Guid prop5)
                : this(prop1, prop2, prop3, prop4)
            {
                Prop5 = prop5;
            }

            public DummyType(int prop1, int prop2, double prop3, double prop4, Guid prop5, Guid prop6)
                : this(prop1, prop2, prop3, prop4, prop5)
            {
                Prop6 = prop6;
            }

            public DummyType(int prop1, int prop2, double prop3, double prop4, Guid prop5, Guid prop6, string prop7)
                : this(prop1, prop2, prop3, prop4, prop5, prop6)
            {
                Prop7 = prop7;
            }

            public DummyType(int prop1, int prop2, double prop3, double prop4, Guid prop5, Guid prop6, string prop7, string prop8)
                : this(prop1, prop2, prop3, prop4, prop5, prop6, prop7)
            {
                Prop8 = prop8;
            }
        }

        [Fact]
        public void Should_Create_For_Default_Constructor()
        {
            var expected = new DummyType();

            var factory = TypeFactory<DummyType>.Create();

            var actual1 = factory.Invoke();
            var actual2 = factory.Invoke();

            actual1.Should().NotBeSameAs(actual2);

            expected.Should().BeEquivalentTo(actual1);
            expected.Should().BeEquivalentTo(actual2);
        }

        public class Typed : TypeFactoryFixture
        {
            [Fact]
            public void Should_Create_Using_Constructor_With_1_Arg()
            {
                var expected = new DummyType(Create<int>());

                var factory = TypeFactory<DummyType>.Create<int>();

                var actual1 = factory.Invoke(expected.Prop1);
                var actual2 = factory.Invoke(expected.Prop1);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_2_Args()
            {
                var expected = new DummyType(Create<int>(), Create<int>());

                var factory = TypeFactory<DummyType>.Create<int, int>();

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_3_Args()
            {
                var expected = new DummyType(Create<int>(), Create<int>(), Create<double>());

                var factory = TypeFactory<DummyType>.Create<int, int, double>();

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_4_Args()
            {
                var expected = new DummyType(Create<int>(), Create<int>(), Create<double>(), Create<double>());

                var factory = TypeFactory<DummyType>.Create<int, int, double, double>();

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_5_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>());

                var factory = TypeFactory<DummyType>.Create<int, int, double, double, Guid>();

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_6_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>(), Create<Guid>());

                var factory = TypeFactory<DummyType>.Create<int, int, double, double, Guid, Guid>();

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_7_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>(), Create<Guid>(), Create<string>());

                var factory = TypeFactory<DummyType>.Create<int, int, double, double, Guid, Guid, string>();

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_8_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>(), Create<Guid>(), Create<string>(), Create<string>());

                var factory = TypeFactory<DummyType>.Create<int, int, double, double, Guid, Guid, string, string>();

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7, expected.Prop8);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7, expected.Prop8);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }
        }

        public class AsObject : TypeFactoryFixture
        {
            [Fact]
            public void Should_Create_Using_Constructor_With_1_Arg()
            {
                var expected = new DummyType(Create<int>());

                var factory = TypeFactory<DummyType>.Create(typeof(int));

                var actual1 = factory.Invoke(expected.Prop1);
                var actual2 = factory.Invoke(expected.Prop1);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_2_Args()
            {
                var expected = new DummyType(Create<int>(), Create<int>());

                var factory = TypeFactory<DummyType>.Create(typeof(int), typeof(int));

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_3_Args()
            {
                var expected = new DummyType(Create<int>(), Create<int>(), Create<double>());

                var factory = TypeFactory<DummyType>.Create(typeof(int), typeof(int), typeof(double));

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_4_Args()
            {
                var expected = new DummyType(Create<int>(), Create<int>(), Create<double>(), Create<double>());

                var factory = TypeFactory<DummyType>.Create(typeof(int), typeof(int), typeof(double), typeof(double));

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_5_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>());

                var factory = TypeFactory<DummyType>.Create(typeof(int), typeof(int), typeof(double), typeof(double), typeof(Guid));

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_6_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>(), Create<Guid>());

                var factory = TypeFactory<DummyType>.Create(typeof(int), typeof(int), typeof(double), typeof(double), typeof(Guid), typeof(Guid));

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_7_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>(), Create<Guid>(), Create<string>());

                var factory = TypeFactory<DummyType>.Create(typeof(int), typeof(int), typeof(double), typeof(double), typeof(Guid), typeof(Guid), typeof(string));

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }

            [Fact]
            public void Should_Create_Using_Constructor_With_8_Args()
            {
                var expected = new DummyType(
                    Create<int>(), Create<int>(), Create<double>(), Create<double>(),
                    Create<Guid>(), Create<Guid>(), Create<string>(), Create<string>());

                var factory = TypeFactory<DummyType>.Create(typeof(int), typeof(int), typeof(double), typeof(double), typeof(Guid), typeof(Guid), typeof(string), typeof(string));

                var actual1 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7, expected.Prop8);
                var actual2 = factory.Invoke(expected.Prop1, expected.Prop2, expected.Prop3, expected.Prop4, expected.Prop5, expected.Prop6, expected.Prop7, expected.Prop8);

                actual1.Should().NotBeSameAs(actual2);

                expected.Should().BeEquivalentTo(actual1);
                expected.Should().BeEquivalentTo(actual2);
            }
        }
    }
}