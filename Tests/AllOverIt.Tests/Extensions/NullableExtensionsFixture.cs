using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Shouldly;

namespace AllOverIt.Tests.Extensions
{
    public class NullableExtensionsFixture : FixtureBase
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Deconstruct(bool expected)
        {
            int? actual = null;

            if (expected)
            {
                actual = Create<int>();
            }

            var (hasValue, value) = actual;

            hasValue.ShouldBe(expected);
            hasValue.ShouldBe(actual.HasValue);
            value.ShouldBe(expected ? actual.Value : default);
        }
    }
}








