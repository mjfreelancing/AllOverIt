using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.SystemText.Converters;
using FluentAssertions;
using System.Text.Json;

namespace AllOverIt.Serialization.Json.SystemText.Tests.Converters
{
    public class InterfaceConverterFixture : FixtureBase
    {
        private interface IDummyType
        {
            int Prop1 { get; }
        }

        private class DummyType : IDummyType
        {
            public int Prop1 { get; set; }
        }

        [Fact]
        public void Should_Convert_Interface()
        {
            var converter = new InterfaceConverter<IDummyType, DummyType>();

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            var serializer = new SystemTextJsonSerializer(options);

            var value = Create<int>();
            var dummyValue = $@"{{""Prop1"":{value}}}";

            var actual = serializer.DeserializeObject<IDummyType>(dummyValue);

            actual.Should().BeOfType<DummyType>();
            actual.Prop1.Should().Be(value);
        }
    }
}