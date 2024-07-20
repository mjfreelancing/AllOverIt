using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using FluentAssertions;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests.Converters
{
    public class InterfaceConverterFixture : FixtureBase
    {
        private interface IDummyType
        {
            int Prop1 { get; }
        }

        private sealed class DummyType : IDummyType
        {
            public int Prop1 { get; set; }
        }

        private sealed class DummyCollection
        {
            public IDummyType Prop1 { get; set; }
            public IDummyType[] Prop2 { get; set; }
            public IList<IDummyType> Prop3 { get; set; }
        }

        [Fact]
        public void Should_Have_CanWrite_False()
        {
            var converter = new InterfaceConverter<IDummyType, DummyType>();

            converter.CanWrite.Should().BeFalse();
        }

        [Fact]
        public void Should_Convert_Interface()
        {
            var converter = new InterfaceConverter<IDummyType, DummyType>();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);

            var serializer = new NewtonsoftJsonSerializer(settings);

            var value = Create<int>();
            var dummyValue = $@"{{""Prop1"":{value}}}";

            var actual = serializer.DeserializeObject<IDummyType>(dummyValue);

            actual.Should().BeOfType<DummyType>();
            actual.Prop1.Should().Be(value);
        }

        [Fact]
        public void Should_Convert_Interface_Collections()
        {
            var converter = new InterfaceConverter<IDummyType, DummyType>();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);

            var serializer = new NewtonsoftJsonSerializer(settings);

            var expected = new DummyCollection
            {
                Prop1 = Create<DummyType>(),
                Prop2 = CreateMany<DummyType>().ToArray(),
                Prop3 = CreateMany<DummyType>().Cast<IDummyType>().AsList()
            };

            var dummyValues = serializer.SerializeObject(expected);

            var actual = serializer.DeserializeObject<DummyCollection>(dummyValues);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}