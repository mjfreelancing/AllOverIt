using AllOverIt.EntityFrameworkCore.EnrichedEnum;
using AllOverIt.EntityFrameworkCore.ValueConverters;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.EntityFrameworkCore.Tests.EnrichedEnum
{
    public class EnrichedEnumModelBuilderTypesFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Known_Converter_Types()
        {
            var fields = typeof(EnrichedEnumModelBuilderTypes).GetFields(BindingFlags.Static | BindingFlags.NonPublic);

            fields.Length.Should().Be(2);

            EnrichedEnumModelBuilderTypes.AsNameConverter.Should().Be(typeof(EnrichedEnumNameConverter<>));
            EnrichedEnumModelBuilderTypes.AsValueConverter.Should().Be(typeof(EnrichedEnumValueConverter<>));
        }
    }
}