using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Fixture;
using Shouldly;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2.Extensions
{
    public class ShapeStyleExtensionsFixture : FixtureBase
    {
        public class AsText : ShapeStyleExtensionsFixture
        {
            [Fact]
            public void Should_Return_Empty_String_When_Default_Style()
            {
                var style = new ShapeStyle();
                var indent = GetWithinRange(0, 3);

                var actual = ShapeStyleExtensions.AsText(style, indent);

                actual.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Add_Fill()
            {
                var style = new ShapeStyle
                {
                    Fill = Create<string>()
                };

                var indent = GetWithinRange(0, 4);

                var actual = ShapeStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "fill", style.Fill);

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Add_Stroke()
            {
                var style = new ShapeStyle
                {
                    Stroke = Create<string>()
                };

                var indent = GetWithinRange(0, 4);

                var actual = ShapeStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "stroke", style.Stroke);

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Add_Stroke_Width()
            {
                var style = new ShapeStyle
                {
                    StrokeWidth = Create<int>()
                };

                var indent = GetWithinRange(0, 4);

                var actual = ShapeStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "stroke-width", style.StrokeWidth.ToString());

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Add_Stroke_Dash()
            {
                var style = new ShapeStyle
                {
                    StrokeDash = Create<int>()
                };

                var indent = GetWithinRange(0, 4);

                var actual = ShapeStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "stroke-dash", style.StrokeDash.ToString());

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Add_Opacity()
            {
                var style = new ShapeStyle
                {
                    Opacity = Create<int>()
                };

                var indent = GetWithinRange(0, 4);

                var actual = ShapeStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "opacity", style.Opacity.ToString());

                actual.ShouldBe(expected);
            }

            private static string BuildExpectedStyle(int indent, string attribute, string value)
            {
                var indenting = new string(' ', indent);
                var styleText = $"{indenting}{indenting}{attribute}: {value}";

                return $$"""
                   {{indenting}}style: {
                   {{styleText.D2EscapeString()}}
                   {{indenting}}}
                   """;
            }
        }
    }
}
