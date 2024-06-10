using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

using static AllOverIt.Mapping.Tests.ObjectMapperTypes;

namespace AllOverIt.Mapping.Tests
{
    public class ObjectMapperOptionsFixture : FixtureBase
    {
        private readonly ObjectMapperTypeFactory _objectMapperTypeFactory = new();
        private readonly ObjectMapperOptions _objectMapperOptions;

        public ObjectMapperOptionsFixture()
        {
            _objectMapperOptions = new(_objectMapperTypeFactory);
        }

        public class Register : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Register_Type()
            {
                _objectMapperOptions.Register<DummySource1>();

                var factory = _objectMapperTypeFactory.GetOrAdd(typeof(DummySource1), () =>
                {
                    return null;
                });

                var actual = factory.Invoke();

                actual.Should().BeOfType<DummySource1>();
            }
        }

        public class Register_Factory : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Factory_Null()
            {
                Invoking(() =>
                {
                    _objectMapperOptions.Register<DummySource1>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("factory");
            }

            [Fact]
            public void Should_Register_Type()
            {
                var expected = new DummySource1();

                _objectMapperOptions.Register<DummySource1>(() => expected);

                var factory = _objectMapperTypeFactory.GetOrAdd(typeof(DummySource1), () =>
                {
                    return null;
                });

                var actual = factory.Invoke();

                actual.Should().BeSameAs(expected);
            }
        }
    }
}