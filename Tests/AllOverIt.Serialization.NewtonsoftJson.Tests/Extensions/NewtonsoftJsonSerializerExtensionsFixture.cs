using System.Linq;
using AllOverIt.Fixture;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using AllOverIt.Serialization.NewtonsoftJson.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.NewtonsoftJson.Tests.Extensions
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

                serializer.Settings.Converters
                    .Should()
                    .BeEquivalentTo(new[] {converter1, converter2});
            }
        }

        public class AddInterfaceConverter : NewtonsoftJsonSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Add_Interface_Converter()
            {
                var serializer = new NewtonsoftJsonSerializer();

                serializer.AddInterfaceConverter<IDummyType, DummyType>();

                serializer.Settings.Converters.Single()
                    .Should()
                    .BeOfType<InterfaceConverter<IDummyType, DummyType>>();
            }

            [Fact]
            public void Should_Add_Interface_And_Enumerable_Converter()
            {
                var serializer = new NewtonsoftJsonSerializer();

                serializer.AddInterfaceConverter<IDummyType, DummyType>(true);

                serializer.Settings.Converters.ElementAt(0)
                    .Should()
                    .BeOfType<InterfaceConverter<IDummyType, DummyType>>();

                serializer.Settings.Converters.ElementAt(1)
                    .Should()
                    .BeOfType<EnumerableInterfaceConverter<IDummyType, DummyType>>();
            }
        }

        public class AddEnumerableInterfaceConverter : NewtonsoftJsonSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Add_Enumerable_Interface_Converter()
            {
                var serializer = new NewtonsoftJsonSerializer();

                serializer.AddEnumerableInterfaceConverter<IDummyType, DummyType>();

                serializer.Settings.Converters.Single()
                    .Should()
                    .BeOfType<EnumerableInterfaceConverter<IDummyType, DummyType>>();
            }
        }
    }
}