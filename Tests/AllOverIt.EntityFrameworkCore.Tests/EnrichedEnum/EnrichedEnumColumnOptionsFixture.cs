using AllOverIt.EntityFrameworkCore.EnrichedEnum;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Assertions;
using AllOverIt.Types;
using FluentAssertions;

namespace AllOverIt.EntityFrameworkCore.Tests.EnrichedEnum
{
    public class EnrichedEnumColumnOptionsFixture : FixtureBase
    {
        [Fact]
        public void Should_Be_Record_Type()
        {
            typeof(EnrichedEnumColumnOptions).IsRecordType().Should().BeTrue();
        }

        [Fact]
        public void Should_Have_Expected_Property_Definition()
        {
            Properties
                .For<EnrichedEnumColumnOptions>()
                .Should()
                .MatchNames([nameof(EnrichedEnumColumnOptions.ColumnType)])
                .And
                .BeDefinedAs(propertyInfo =>
                {
                    // There's only 1 property at the moment. Update as required when the model changes.
                    propertyInfo
                        .IsPublic(PropertyAccessor.Get | PropertyAccessor.Init)
                        .IsNullable();
                });
        }
    }
}