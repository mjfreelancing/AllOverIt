using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.Extensions
{
    public class CardinalityExtensionsFixture : FixtureBase
    {
        private ErdOptions.CardinalityOptions _cardinality = new();

        public class SetLabelStyle_Action : CardinalityExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Cardinality_Null()
            {
                Invoking(() =>
                {
                    CardinalityExtensions.SetLabelStyle(null, _ => { });
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cardinality");
            }

            [Fact]
            public void Should_Throw_When_Configure_Null()
            {
                Invoking(() =>
                {
                    CardinalityExtensions.SetLabelStyle(_cardinality, (Action<LabelStyle>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("configure");
            }

            [Fact]
            public void Should_Update_LabelStyle()
            {
                var labelStyle = Create<LabelStyle>();

                CardinalityExtensions.SetLabelStyle(_cardinality, style =>
                {
                    style.IsVisible = labelStyle.IsVisible;
                    style.FontColor = labelStyle.FontColor;
                    style.FontSize = labelStyle.FontSize;
                    style.Italic = labelStyle.Italic;
                    style.Bold = labelStyle.Bold;
                    style.Underline = labelStyle.Underline;
                });

                labelStyle.Should().BeEquivalentTo(_cardinality.LabelStyle);
            }
        }

        public class SetLabelStyle_Style : CardinalityExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Cardinality_Null()
            {
                Invoking(() =>
                {
                    CardinalityExtensions.SetLabelStyle(null, new LabelStyle());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cardinality");
            }

            [Fact]
            public void Should_Throw_When_LabelStyle_Null()
            {
                Invoking(() =>
                {
                    CardinalityExtensions.SetLabelStyle(_cardinality, (LabelStyle) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("labelStyle");
            }

            [Fact]
            public void Should_Set_LabelStyle_Instance()
            {
                var labelStyle = new LabelStyle();

                CardinalityExtensions.SetLabelStyle(_cardinality, labelStyle);

                _cardinality.LabelStyle.Should().BeSameAs(labelStyle);
            }
        }
    }
}