using AllOverIt.Fixture;
using FluentAssertions;

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

                actual.IsDefault().Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Default()
            {
                var actual = Create<ShapeStyle>();

                actual.IsDefault().Should().BeFalse();
            }

            [Fact]
            public void Should_Copy()
            {
                var shapeStyle = Create<ShapeStyle>();

                var actual = new ShapeStyle();

                actual.Should().NotBeEquivalentTo(shapeStyle);

                actual.CopyFrom(shapeStyle);

                actual.Should().BeEquivalentTo(shapeStyle);
            }
        }
    }
}