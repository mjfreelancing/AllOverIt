using AllOverIt.Fixture;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace AllOverIt.Serialization.NewtonsoftJson.Tests.Converters
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
            
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);
            
            var serializer = new NewtonsoftJsonSerializer(settings);

            var value = Create<int>();
            var dummyValue = $@"{{""Prop1"":{value}}}";

            var actual = serializer.DeserializeObject<IDummyType>(dummyValue);

            actual.Should().BeOfType<DummyType>();
            actual.Prop1.Should().Be(value);
        }
    }
}