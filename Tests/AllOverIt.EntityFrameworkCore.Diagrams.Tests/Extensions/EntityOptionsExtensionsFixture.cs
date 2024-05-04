using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

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
                Invoking(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(null, _ => { });
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Throw_When_Configure_Null()
            {
                Invoking(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(_options, (Action<ShapeStyle>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
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

                shapeStyle.Should().BeEquivalentTo(_options.ShapeStyle);
            }
        }

        public class SetShapeStyle_Style : EntityOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(null, new ShapeStyle());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Throw_When_LabelStyle_Null()
            {
                Invoking(() =>
                {
                    EntityOptionsExtensions.SetShapeStyle(_options, (ShapeStyle) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("shapeStyle");
            }

            [Fact]
            public void Should_Set_LabelStyle_Instance()
            {
                var shapeStyle = new ShapeStyle();

                EntityOptionsExtensions.SetShapeStyle(_options, shapeStyle);

                _options.ShapeStyle.Should().BeSameAs(shapeStyle);
            }
        }
    }
}