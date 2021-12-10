using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AllOverIt.Fixture;
using AllOverIt.Serialization.SystemTextJson.Converters;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests.Converters
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

            var settings = new JsonSerializerOptions();
            settings.Converters.Add(converter);

            var serializer = new SystemTextJsonSerializer(settings);

            var value = Create<int>();
            var dummyValue = $@"[{{""Prop1"":{value}}},{{""Prop1"":{value + 1}}}]";

            var actual = serializer.DeserializeObject<IEnumerable<IDummyType>>(dummyValue);

            actual.Should().BeOfType<List<DummyType>>();
            actual.ElementAt(0).Prop1.Should().Be(value);
            actual.ElementAt(1).Prop1.Should().Be(value + 1);
        }
    }
}
