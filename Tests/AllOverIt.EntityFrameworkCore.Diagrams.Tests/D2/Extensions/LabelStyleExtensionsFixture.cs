using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2.Extensions
{
    public class LabelStyleExtensionsFixture : FixtureBase
    {
        public class AsText : LabelStyleExtensionsFixture
        {
            [Fact]
            public void Should_Return_Empty_String_When_Default_Style()
            {
                var style = new LabelStyle();
                var indent = GetWithinRange(0, 3);

                var actual = LabelStyleExtensions.AsText(style, indent);

                actual.Should().Be(string.Empty);
            }

            [Fact]
            public void Should_Add_Font_Size()
            {
                var style = new LabelStyle
                {
                    FontSize = Create<int>()
                };

                var indent = GetWithinRange(0, 4);

                var actual = LabelStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "font-size", style.FontSize.ToString());

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Add_Font_Color()
            {
                var style = new LabelStyle
                {
                    FontColor = Create<string>()
                };

                var indent = GetWithinRange(0, 4);

                var actual = LabelStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "font-color", style.FontColor);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Add_Bold(bool bold)
            {
                var style = new LabelStyle
                {
                    Bold = bold
                };

                var indent = GetWithinRange(0, 4);

                var actual = LabelStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "bold", GetBoolString(bold));

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Add_Underline(bool underline)
            {
                var style = new LabelStyle
                {
                    Underline = underline
                };

                var indent = GetWithinRange(0, 4);

                var actual = LabelStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "underline", GetBoolString(underline));

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Add_Italic(bool italic)
            {
                var style = new LabelStyle
                {
                    Italic = italic
                };

                var indent = GetWithinRange(0, 4);

                var actual = LabelStyleExtensions.AsText(style, indent);

                var expected = BuildExpectedStyle(indent, "italic", GetBoolString(italic));

                actual.Should().Be(expected);
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

            private static string GetBoolString(bool value)
            {
                return value ? "true" : "false";
            }
        }
    }
}
