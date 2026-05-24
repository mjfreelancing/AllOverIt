using AllOverIt.Fixture;
using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests
{
    public class ShapeStyleFixture : FixtureBase
    {
        public class IsDefault : ShapeStyleFixture
        {
            [Fact]
            public void Should_Be_Default()
            {
                var actual = new ShapeStyle();

                actual.IsDefault().ShouldBeTrue();
            }

            [Fact]
            public void Should_Not_Be_Default()
            {
                var actual = Create<ShapeStyle>();

                actual.IsDefault().ShouldBeFalse();
            }

            [Fact]
            public void Should_Copy()
            {
                var shapeStyle = Create<ShapeStyle>();

                var actual = new ShapeStyle();

                actual.Fill.ShouldNotBe(shapeStyle.Fill);

                actual.CopyFrom(shapeStyle);

                actual.ShouldBeEquivalentTo(shapeStyle);
            }
        }
    }
}