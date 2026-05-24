using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;

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
                Should.Throw<ArgumentNullException>(() =>
                {
                    CardinalityExtensions.SetLabelStyle(null, _ => { });
                })
                .WithNamedMessageWhenNull("cardinality");
            }

            [Fact]
            public void Should_Throw_When_Configure_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    CardinalityExtensions.SetLabelStyle(_cardinality, (Action<LabelStyle>) null);
                })
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

                _cardinality.LabelStyle.ShouldBeEquivalentTo(labelStyle);
            }
        }

        public class SetLabelStyle_Style : CardinalityExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Cardinality_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    CardinalityExtensions.SetLabelStyle(null, new LabelStyle());
                })
                .WithNamedMessageWhenNull("cardinality");
            }

            [Fact]
            public void Should_Throw_When_LabelStyle_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    CardinalityExtensions.SetLabelStyle(_cardinality, (LabelStyle) null);
                })
                .WithNamedMessageWhenNull("labelStyle");
            }

            [Fact]
            public void Should_Set_LabelStyle_Instance()
            {
                var labelStyle = new LabelStyle();

                CardinalityExtensions.SetLabelStyle(_cardinality, labelStyle);

                _cardinality.LabelStyle.ShouldBeSameAs(labelStyle);
            }
        }
    }
}