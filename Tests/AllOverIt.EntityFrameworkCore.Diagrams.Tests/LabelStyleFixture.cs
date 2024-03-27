using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests
{
    public class LabelStyleFixture : FixtureBase
    {
        public class IsDefault : LabelStyleFixture
        {
            [Fact]
            public void Should_Be_Default()
            {
                var actual = new LabelStyle();

                actual.IsDefault().Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Default()
            {
                var actual = Create<LabelStyle>();

                actual.IsDefault().Should().BeFalse();
            }
        }
    }
}