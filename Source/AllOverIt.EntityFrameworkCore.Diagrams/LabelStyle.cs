namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Provides styling options for a label on the generated diagram.</summary>
    public sealed record LabelStyle     // A record so they can be compared - see IsDefault()
    {
        private static readonly LabelStyle Default = new();

        /// <summary>Indicates if the label is visible.</summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>When not <see langword="null"/>, specifies the font size of the label.
        /// When <see langword="null"/>, the diagram's default will be used.</summary>
        public int? FontSize { get; set; }

        /// <summary>When not <see langword="null"/>, specifies the font color of the label.
        /// When <see langword="null"/>, the diagram's default will be used.</summary>
        public string? FontColor { get; set; }

        /// <summary>When not <see langword="null"/>, indicates if the font weight of the label will be bold or not.
        /// When <see langword="null"/>, the diagram's default will be used.</summary>
        public bool? Bold { get; set; }

        /// <summary>When not <see langword="null"/>, indicates if the font of the label will be underline or not.
        /// When <see langword="null"/>, the diagram's default will be used.</summary>
        public bool? Underline { get; set; }

        /// <summary>When not <see langword="null"/>, indicates if the font of the label will be italic or not.
        /// When <see langword="null"/>, the diagram's default will be used.</summary>
        public bool? Italic { get; set; }

        /// <summary>Indicates if the label style matches the default.</summary>
        public bool IsDefault()
        {
            // Value equality
            return this == Default;
        }
    }
}