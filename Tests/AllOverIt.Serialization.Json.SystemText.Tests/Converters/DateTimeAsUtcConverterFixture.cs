using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.SystemText.Converters;
using Shouldly;
using System.Text.Json;

namespace AllOverIt.Serialization.Json.SystemText.Tests.Converters
{
    public class DateTimeAsUtcConverterFixture : FixtureBase
    {
        private class DummyDateTime
        {
            public DateTime Prop1 { get; set; }
            public DateTime? Prop2 { get; set; }
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
                var dateTime1 = DateTime.Now;
                var dateTime2 = dateTime1.AddMinutes(1);

                var value = $@"{{""Prop1"":""{dateTime1:o}"",""Prop2"":""{dateTime2:o}""}}";

                var actual = _serializer.DeserializeObject<DummyDateTime>(value);

                actual.Prop1.Kind.ShouldBe(DateTimeKind.Utc);
                actual.Prop1.Day.ShouldBe(dateTime1.Day);
                actual.Prop1.Month.ShouldBe(dateTime1.Month);
                actual.Prop1.Year.ShouldBe(dateTime1.Year);
                actual.Prop1.Hour.ShouldBe(dateTime1.Hour);
                actual.Prop1.Minute.ShouldBe(dateTime1.Minute);
                actual.Prop1.Second.ShouldBe(dateTime1.Second);
                actual.Prop1.Millisecond.ShouldBe(dateTime1.Millisecond);

                actual.Prop2.Value.Kind.ShouldBe(DateTimeKind.Utc);
                actual.Prop2.Value.Day.ShouldBe(dateTime2.Day);
                actual.Prop2.Value.Month.ShouldBe(dateTime2.Month);
                actual.Prop2.Value.Year.ShouldBe(dateTime2.Year);
                actual.Prop2.Value.Hour.ShouldBe(dateTime2.Hour);
                actual.Prop2.Value.Minute.ShouldBe(dateTime2.Minute);
                actual.Prop2.Value.Second.ShouldBe(dateTime2.Second);
                actual.Prop2.Value.Millisecond.ShouldBe(dateTime2.Millisecond);
            }

            [Fact]
            public void Should_Read_Nullable_DateTime_As_Null()
            {
                var dateTime1 = DateTime.Now;
                var dateTime2 = dateTime1.AddMinutes(1);

                var value = $@"{{""Prop1"":""{dateTime1:o}"",""Prop2"":null}}";

                var actual = _serializer.DeserializeObject<DummyDateTime>(value);

                actual.Prop1.Kind.ShouldBe(DateTimeKind.Utc);
                actual.Prop1.Day.ShouldBe(dateTime1.Day);
                actual.Prop1.Month.ShouldBe(dateTime1.Month);
                actual.Prop1.Year.ShouldBe(dateTime1.Year);
                actual.Prop1.Hour.ShouldBe(dateTime1.Hour);
                actual.Prop1.Minute.ShouldBe(dateTime1.Minute);
                actual.Prop1.Second.ShouldBe(dateTime1.Second);
                actual.Prop1.Millisecond.ShouldBe(dateTime1.Millisecond);

                actual.Prop2.ShouldBeNull();
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
                var expected = $@"{{""Prop1"":""{dummyValue.Prop1:yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK}Z"",""Prop2"":""{dummyValue.Prop2:yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK}Z""}}";

                actual.ShouldBe(expected);
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

                actual.ShouldBe(expected);
            }
        }
    }
}




