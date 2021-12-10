using System.Linq;
using AllOverIt.Fixture;
using AllOverIt.Serialization.SystemTextJson.Converters;
using AllOverIt.Serialization.SystemTextJson.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests.Extensions
{
    public class SystemTextJsonSerializerExtensionsFixture : FixtureBase
    {
        private interface IDummyType
        {
            int Prop1 { get; }
        }

        private class DummyType : IDummyType
        {
            public int Prop1 { get; set; }
        }

        public class AddConverters : SystemTextJsonSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Add_Converter()
            {
                var converter1 = new InterfaceConverter<IDummyType, DummyType>();
                var converter2 = new InterfaceConverter<IDummyType, DummyType>();

                var serializer = new SystemTextJsonSerializer();

                serializer.AddConverters(converter1, converter2);

                serializer.Options.Converters
                    .Should()
                    .BeEquivalentTo(new[] {converter1, converter2});
            }
        }

        public class AddInterfaceConverter : SystemTextJsonSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Add_Interface_Converter()
            {
                var serializer = new SystemTextJsonSerializer();

                serializer.AddInterfaceConverter<IDummyType, DummyType>();

                serializer.Options.Converters.Single()
                    .Should()
                    .BeOfType<InterfaceConverter<IDummyType, DummyType>>();
            }

            [Fact]
            public void Should_Add_Interface_And_Enumerable_Converter()
            {
                var serializer = new SystemTextJsonSerializer();

                serializer.AddInterfaceConverter<IDummyType, DummyType>(true);

                serializer.Options.Converters.ElementAt(0)
                    .Should()
                    .BeOfType<InterfaceConverter<IDummyType, DummyType>>();

                serializer.Options.Converters.ElementAt(1)
                    .Should()
                    .BeOfType<EnumerableInterfaceConverter<IDummyType, DummyType>>();
            }
        }

        public class AddEnumerableInterfaceConverter : SystemTextJsonSerializerExtensionsFixture
        {
            [Fact]
            public void Should_Add_Enumerable_Interface_Converter()
            {
                var serializer = new SystemTextJsonSerializer();

                serializer.AddEnumerableInterfaceConverter<IDummyType, DummyType>();

                serializer.Options.Converters.Single()
                    .Should()
                    .BeOfType<EnumerableInterfaceConverter<IDummyType, DummyType>>();
            }
        }
    }
}