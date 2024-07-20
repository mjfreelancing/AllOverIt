using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using AllOverIt.Serialization.Json.Newtonsoft.Extensions;
using FluentAssertions;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests.Extensions
{
    public class NewtonsoftJsonSerializerExtensionsFixture : FixtureBase
    {
        private interface IDummyType
        {
            int Prop1 { get; }
        }

        private class DummyType : IDummyType
        {
            public int Prop1 { get; set; }
        }

        public class AddConverters : NewtonsoftJsonSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Add_Converter()
            {
                var converter1 = new InterfaceConverter<IDummyType, DummyType>();
                var converter2 = new InterfaceConverter<IDummyType, DummyType>();

                var serializer = new NewtonsoftJsonSerializer();

                serializer.AddConverters(converter1, converter2);

                var expected = new[] { converter1, converter2 };

                expected
                    .Should()
                    .BeEquivalentTo(serializer.Settings.Converters);
            }
        }

        public class AddInterfaceConverter : NewtonsoftJsonSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    NewtonsoftJsonSerializerExtensions.AddInterfaceConverter<IDummyType, DummyType>(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }

            [Fact]
            public void Should_Add_Interface_Converter()
            {
                var serializer = new NewtonsoftJsonSerializer();

                serializer.AddInterfaceConverter<IDummyType, DummyType>();

                serializer.Settings.Converters.Single()
                    .Should()
                    .BeOfType<InterfaceConverter<IDummyType, DummyType>>();
            }
        }
    }
}