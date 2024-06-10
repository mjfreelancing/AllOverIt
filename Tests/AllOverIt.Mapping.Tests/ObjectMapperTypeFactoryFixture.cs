using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

using static AllOverIt.Mapping.Tests.ObjectMapperTypes;

namespace AllOverIt.Mapping.Tests
{
    public class ObjectMapperTypeFactoryFixture : FixtureBase
    {
        private readonly ObjectMapperTypeFactory _factory = new();

        public class Add_Source_Target_Type : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.Add(null, typeof(DummyTarget), (mapper, value) => new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.Add(typeof(DummySource2), null, (mapper, value) => new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Throw_When_Factory_Null()
            {
                Invoking(() =>
                {
                    _factory.Add(typeof(DummySource2), typeof(DummyTarget), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("factory");
            }

            [Fact]
            public void Should_Add_Factory()
            {
                Func<IObjectMapper, object, object> factory = (mapper, value) => new { };

                _factory.Add(typeof(DummySource2), typeof(DummyTarget), factory);

                var success = _factory.TryGet(typeof(DummySource2), typeof(DummyTarget), out var actual);

                success.Should().BeTrue();
                factory.Should().BeSameAs(actual);
            }
        }

        public class TryGet_Source_Target_Type : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() =>
                {
                    _ = _factory.TryGet(null, typeof(DummyTarget), out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() =>
                {
                    _ = _factory.TryGet(typeof(DummySource2), null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Get_Factory()
            {
                Func<IObjectMapper, object, object> factory = (mapper, value) => new { };

                _factory.Add(typeof(DummySource2), typeof(DummyTarget), factory);

                var success = _factory.TryGet(typeof(DummySource2), typeof(DummyTarget), out var actual);

                success.Should().BeTrue();
                factory.Should().BeSameAs(actual);
            }

            [Fact]
            public void Should_Not_Get_Factory()
            {
                var success = _factory.TryGet(typeof(DummySource2), typeof(DummyTarget), out var actual);

                success.Should().BeFalse();
                actual.Should().BeNull();
            }
        }

        public class GetOrAdd : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.GetOrAdd(null, () => new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Factory_Null()
            {
                Invoking(() =>
                {
                    _factory.GetOrAdd(typeof(DummyTarget), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("factory");
            }

            [Fact]
            public void Should_Add_Factory()
            {
                Func<object> factory = () => new { };

                var actual = _factory.GetOrAdd(typeof(DummyTarget), factory);

                factory.Should().BeSameAs(actual);
            }
        }

        public class GetOrLazilyAdd : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.GetOrLazilyAdd(null, () => () => new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_FactoryResolver_Null()
            {
                Invoking(() =>
                {
                    _factory.GetOrLazilyAdd(typeof(DummyTarget), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("factoryResolver");
            }

            [Fact]
            public void Should_Add_Factory()
            {
                object expected = new { };

                Func<Func<object>> factoryResolver = () => () => expected;

                var factory = _factory.GetOrLazilyAdd(typeof(DummyTarget), factoryResolver);

                var actual = factory.Invoke();

                actual.Should().BeSameAs(expected);
            }
        }
    }
}