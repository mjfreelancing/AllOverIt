using AllOverIt.EntityFrameworkCore.EnrichedEnum;
using AllOverIt.EntityFrameworkCore.ValueConverters;
using AllOverIt.Fixture;
using Shouldly;
using System.Reflection;

namespace AllOverIt.EntityFrameworkCore.Tests.EnrichedEnum
{
    public class EnrichedEnumModelBuilderTypesFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Known_Converter_Types()
        {
            var fields = typeof(EnrichedEnumModelBuilderTypes).GetFields(BindingFlags.Static | BindingFlags.NonPublic);

            fields.Length.ShouldBe(2);

            EnrichedEnumModelBuilderTypes.AsNameConverter.ShouldBe(typeof(EnrichedEnumNameConverter<>));
            EnrichedEnumModelBuilderTypes.AsValueConverter.ShouldBe(typeof(EnrichedEnumValueConverter<>));
        }
    }
}


