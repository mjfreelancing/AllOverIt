namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Provides styling options for a shape on the generated diagram.</summary>
    public sealed class ShapeStyle
    {
        private static readonly ShapeStyle Default = new();

        /// <summary>Specifies the fill color of the shape. When not specified the diagram's
        /// default fill color is used.</summary>
        public string Fill { get; set; }

        /// <summary>Specifies the stroke color of the shape. When not specified the diagram's
        /// default stroke color is used.</summary>
        public string Stroke { get; set; }

        /// <summary>Specifies the stroke width of the shape. When not <see langword="null"/> the diagram's
        /// default stroke width is used.</summary>
        public int? StrokeWidth { get; set; }

        /// <summary>Specifies the stroke dash of the shape. When not <see langword="null"/> the diagram's
        /// default stroke dash is used.</summary>
        public int? StrokeDash { get; set; }

        /// <summary>Specifies the opacity of the shape. When not <see langword="null"/> the diagram's
        /// default opacity is used.</summary>
        public double? Opacity { get; set; }

        /// <summary>Indicates if the shape style matches the default.</summary>
        public bool IsDefault()
        {
            return this == Default;
        }
    }
}