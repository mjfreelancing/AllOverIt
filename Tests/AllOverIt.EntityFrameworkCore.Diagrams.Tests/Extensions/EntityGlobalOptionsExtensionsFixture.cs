using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.Extensions
{
    public class EntityGlobalOptionsExtensionsFixture : FixtureBase
    {
        private ErdOptions.EntityGlobalOptions _options = new();

        public class SetShapeStyle_Action : EntityGlobalOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityGlobalOptionsExtensions.SetShapeStyle(null, _ => { });
                })
                .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Throw_When_Configure_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityGlobalOptionsExtensions.SetShapeStyle(_options, (Action<ShapeStyle>) null);
                })
                .WithNamedMessageWhenNull("configure");
            }

            [Fact]
            public void Should_Update_ShapeStyle()
            {
                var shapeStyle = Create<ShapeStyle>();

                EntityGlobalOptionsExtensions.SetShapeStyle(_options, style =>
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

        public class SetShapeStyle_Style : EntityGlobalOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityGlobalOptionsExtensions.SetShapeStyle(null, new ShapeStyle());
                })
                .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Throw_When_LabelStyle_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    EntityGlobalOptionsExtensions.SetShapeStyle(_options, (ShapeStyle) null);
                })
                .WithNamedMessageWhenNull("shapeStyle");
            }

            [Fact]
            public void Should_Set_LabelStyle_Instance()
            {
                var shapeStyle = new ShapeStyle();

                EntityGlobalOptionsExtensions.SetShapeStyle(_options, shapeStyle);

                _options.ShapeStyle.ShouldBeSameAs(shapeStyle);
            }
        }
    }
}