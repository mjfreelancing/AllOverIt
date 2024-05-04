using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using FluentAssertions;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests.Converters
{
    public class DateTimeAsUtcConverterFixture : FixtureBase
    {
        private class DummyDateTime
        {
            public DateTime Prop1 { get; set; }
            public DateTime? Prop2 { get; set; }
        }

        private readonly DateTimeAsUtcConverter _converter;
        private readonly NewtonsoftJsonSerializer _serializer;

        protected DateTimeAsUtcConverterFixture()
        {
            _converter = new DateTimeAsUtcConverter();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(_converter);

            _serializer = new NewtonsoftJsonSerializer(settings);
        }

        public class CanConvert : DateTimeAsUtcConverterFixture
        {
            [Theory]
            [InlineData(typeof(string), false)]
            [InlineData(typeof(int), false)]
            [InlineData(typeof(DateTime), true)]
            [InlineData(typeof(DateTime?), true)]
            public void Should_Return_Expected_CanConvert_Result(Type type, bool expected)
            {
                var actual = _converter.CanConvert(type);

                actual.Should().Be(expected);
            }
        }

        public class ReadJson : DateTimeAsUtcConverterFixture
        {
            [Fact]
            public void Should_Read_DateTime_As_UTC()
            {
                var dateTime1 = DateTime.Now;
                var dateTime2 = dateTime1.AddMinutes(1);

                var value = $@"{{""Prop1"":""{dateTime1:o}"",""Prop2"":""{dateTime2:o}""}}";

                var actual = _serializer.DeserializeObject<DummyDateTime>(value);

                actual.Prop1.Kind.Should().Be(DateTimeKind.Utc);
                actual.Prop1.Day.Should().Be(dateTime1.Day);
                actual.Prop1.Month.Should().Be(dateTime1.Month);
                actual.Prop1.Year.Should().Be(dateTime1.Year);
                actual.Prop1.Hour.Should().Be(dateTime1.Hour);
                actual.Prop1.Minute.Should().Be(dateTime1.Minute);
                actual.Prop1.Second.Should().Be(dateTime1.Second);
                actual.Prop1.Millisecond.Should().Be(dateTime1.Millisecond);

                actual.Prop2.Value.Kind.Should().Be(DateTimeKind.Utc);
                actual.Prop2.Value.Day.Should().Be(dateTime2.Day);
                actual.Prop2.Value.Month.Should().Be(dateTime2.Month);
                actual.Prop2.Value.Year.Should().Be(dateTime2.Year);
                actual.Prop2.Value.Hour.Should().Be(dateTime2.Hour);
                actual.Prop2.Value.Minute.Should().Be(dateTime2.Minute);
                actual.Prop2.Value.Second.Should().Be(dateTime2.Second);
                actual.Prop2.Value.Millisecond.Should().Be(dateTime2.Millisecond);
            }

            [Fact]
            public void Should_Read_Nullable_DateTime_As_Null()
            {
                var dateTime1 = DateTime.Now;
                var dateTime2 = dateTime1.AddMinutes(1);

                var value = $@"{{""Prop1"":""{dateTime1:o}"",""Prop2"":null}}";

                var actual = _serializer.DeserializeObject<DummyDateTime>(value);

                actual.Prop1.Kind.Should().Be(DateTimeKind.Utc);
                actual.Prop1.Day.Should().Be(dateTime1.Day);
                actual.Prop1.Month.Should().Be(dateTime1.Month);
                actual.Prop1.Year.Should().Be(dateTime1.Year);
                actual.Prop1.Hour.Should().Be(dateTime1.Hour);
                actual.Prop1.Minute.Should().Be(dateTime1.Minute);
                actual.Prop1.Second.Should().Be(dateTime1.Second);
                actual.Prop1.Millisecond.Should().Be(dateTime1.Millisecond);

                actual.Prop2.Should().BeNull();
            }
        }

        public class WriteJson : DateTimeAsUtcConverterFixture
        {
            [Fact]
            public void Should_Write_DateTime_As_Utc()
            {
                var dummyValue = Create<DummyDateTime>();

                var actual = _serializer.SerializeObject(dummyValue);

                // Default date handling: https://www.newtonsoft.com/json/help/html/SerializationSettings.htm#DateFormatHandling
                // Adding the 'Z' on the end because the original value was 'unspecified' but the serialized version is now UTC
                var expected = $@"{{""Prop1"":""{dummyValue.Prop1:yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK}Z"",""Prop2"":""{dummyValue.Prop2:yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK}Z""}}";

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Write_Nullable_DateTime_As_Null()
            {
                var dummyValue = Create<DummyDateTime>();
                dummyValue.Prop2 = null;

                var actual = _serializer.SerializeObject(dummyValue);

                // Default date handling: https://www.newtonsoft.com/json/help/html/SerializationSettings.htm#DateFormatHandling
                // Adding the 'Z' on the end because the original value was 'unspecified' but the serialized version is now UTC
                var expected = $@"{{""Prop1"":""{dummyValue.Prop1:yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK}Z"",""Prop2"":null}}";

                actual.Should().Be(expected);
            }
        }
    }
}