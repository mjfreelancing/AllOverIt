namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    internal static class ShapeStyleExtensions
    {
        public static string AsText(this ShapeStyle shapeStyle, int indent)
        {
            return StyleStringBuilder.Create(indent, styler =>
            {
                if (shapeStyle.Fill != default)
                {
                    styler.Invoke("fill", shapeStyle.Fill);
                }

                if (shapeStyle.Stroke != default)
                {
                    styler.Invoke("stroke", shapeStyle.Stroke);
                }

                if (shapeStyle.StrokeWidth != default)
                {
                    styler.Invoke("stroke-width", $"{shapeStyle.StrokeWidth}");
                }

                if (shapeStyle.StrokeDash != default)
                {
                    styler.Invoke("stroke-dash", $"{shapeStyle.StrokeDash}");
                }

                if (shapeStyle.Opacity != default)
                {
                    styler.Invoke("opacity", $"{shapeStyle.Opacity}");
                }
            });
        }
    }
}