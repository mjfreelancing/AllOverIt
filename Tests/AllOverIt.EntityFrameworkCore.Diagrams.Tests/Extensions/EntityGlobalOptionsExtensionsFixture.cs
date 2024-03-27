using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;

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
                Invoking(() =>
                {
                    EntityGlobalOptionsExtensions.SetShapeStyle(null, _ => { });
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
                    EntityGlobalOptionsExtensions.SetShapeStyle(_options, (Action<ShapeStyle>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
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

                shapeStyle.Should().BeEquivalentTo(_options.ShapeStyle);
            }
        }

        public class SetShapeStyle_Style : EntityGlobalOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    EntityGlobalOptionsExtensions.SetShapeStyle(null, new ShapeStyle());
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
                    EntityGlobalOptionsExtensions.SetShapeStyle(_options, (ShapeStyle) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("shapeStyle");
            }

            [Fact]
            public void Should_Set_LabelStyle_Instance()
            {
                var shapeStyle = new ShapeStyle();

                EntityGlobalOptionsExtensions.SetShapeStyle(_options, shapeStyle);

                _options.ShapeStyle.Should().BeSameAs(shapeStyle);
            }
        }
    }
}