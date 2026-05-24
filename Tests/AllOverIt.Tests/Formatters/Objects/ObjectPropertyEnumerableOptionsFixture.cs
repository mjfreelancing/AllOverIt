using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;

namespace AllOverIt.Tests.Formatters.Objects
{
    public class ObjectPropertyEnumerableOptionsFixture : FixtureBase
    {
        private readonly ObjectPropertyEnumerableOptions _options = new();

        [Fact]
        public void Should_Default_To_No_Collation()
        {
            _options.CollateValues.ShouldBeFalse();
        }

        [Fact]
        public void Should_Default_To_Comma_Separator()
        {
            _options.Separator.ShouldBe(", ");
        }
    }
}


