using AllOverIt.Fixture;
using AllOverIt.Serialization.SystemTextJson.Converters;
using FluentAssertions;
using System;
using System.Text.Json;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests.Converters
{
    public class DateTimeAsUtcConverterFixture : FixtureBase
    {
        private class DummyDateTime
        {
            public DateTime Prop1 { get; set; }
        }

        private readonly SystemTextJsonSerializer _serializer;

        protected DateTimeAsUtcConverterFixture()
        {
            var converter = new DateTimeAsUtcConverter();

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            _serializer = new SystemTextJsonSerializer(options);
        }

        public class ReadJson : DateTimeAsUtcConverterFixture
        {
            [Fact]
            public void Should_Read_DateTime_As_UTC()
            {
                var dateTime = DateTime.Now;
                var value = $@"{{""Prop1"":""{dateTime:o}""}}";

                var actual = _serializer.DeserializeObject<DummyDateTime>(value);

                actual.Prop1.Kind.Should().Be(DateTimeKind.Utc);
                actual.Prop1.Day.Should().Be(dateTime.Day);
                actual.Prop1.Month.Should().Be(dateTime.Month);
                actual.Prop1.Year.Should().Be(dateTime.Year);
                actual.Prop1.Hour.Should().Be(dateTime.Hour);
                actual.Prop1.Minute.Should().Be(dateTime.Minute);
                actual.Prop1.Second.Should().Be(dateTime.Second);
                actual.Prop1.Millisecond.Should().Be(dateTime.Millisecond);
            }
        }

        public class WriteJson : DateTimeAsUtcConverterFixture
        {
            [Fact]
            public void Should_Write_DateTime_As_Utc()
            {
                var dummyValue = Create<DummyDateTime>();

                var actual = _serializer.SerializeObject(dummyValue);

                // Default date handling: https://docs.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support
                // Adding the 'Z' on the end because the original value was 'unspecified' but the serialized version is now UTC
                var expected = $@"{{""Prop1"":""{dummyValue.Prop1:yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK}Z""}}";

                actual.Should().Be(expected);
            }
        }
    }
}