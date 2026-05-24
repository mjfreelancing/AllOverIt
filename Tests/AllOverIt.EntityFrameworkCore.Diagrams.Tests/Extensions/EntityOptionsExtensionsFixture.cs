using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.Extensions
{
    public class EntityOptionsExtensionsFixture : FixtureBase
    {
        private ErdOptions.EntityOptions _options = new();

        public class SetShapeStyle_Action : EntityOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(null, _ => { });
                })
                .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Throw_When_Configure_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(_options, (Action<ShapeStyle>) null);
                })
                .WithNamedMessageWhenNull("configure");
            }

            [Fact]
            public void Should_Update_ShapeStyle()
            {
                var shapeStyle = Create<ShapeStyle>();

                EntityOptionsExtensions.SetShapeStyle(_options, style =>
                {
                    style.Fill = shapeStyle.Fill;
                    style.Stroke = shapeStyle.Stroke;
                    style.StrokeDash = shapeStyle.StrokeDash;
                    style.StrokeWidth = shapeStyle.StrokeWidth;
                    style.Opacity = shapeStyle.Opacity;
                });

                _options.ShapeStyle.ShouldBeEquivalentTo(shapeStyle);
            }
        }

        public class SetShapeStyle_Style : EntityOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(null, new ShapeStyle());
                })
                .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Throw_When_LabelStyle_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(_options, (ShapeStyle) null);
                })
                .WithNamedMessageWhenNull("shapeStyle");
            }

            [Fact]
            public void Should_Set_LabelStyle_Instance()
            {
                var shapeStyle = new ShapeStyle();

                EntityOptionsExtensions.SetShapeStyle(_options, shapeStyle);

                _options.ShapeStyle.ShouldBeSameAs(shapeStyle);
            }
        }
    }
}