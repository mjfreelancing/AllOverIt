using AllOverIt.Fixture;
using Shouldly;

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

                actual.IsDefault().ShouldBeTrue();
            }

            [Fact]
            public void Should_Not_Be_Default()
            {
                var actual = Create<LabelStyle>();

                actual.IsDefault().ShouldBeFalse();
            }
        }
    }
}