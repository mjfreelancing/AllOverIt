using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests.Converters
{
    public class EnumerableInterfaceConverterFixture : FixtureBase
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
        public void Should_Convert_Enumerable_Interface()
        {
            var converter = new EnumerableInterfaceConverter<IDummyType, DummyType>();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);

            var serializer = new NewtonsoftJsonSerializer(settings);

            var value = Create<int>();
            var dummyValue = $@"[{{""Prop1"":{value}}},{{""Prop1"":{value + 1}}}]";

            var actual = serializer.DeserializeObject<IEnumerable<IDummyType>>(dummyValue);

            actual.Should().BeOfType<List<DummyType>>();
            actual.ElementAt(0).Prop1.Should().Be(value);
            actual.ElementAt(1).Prop1.Should().Be(value + 1);
        }
    }
}