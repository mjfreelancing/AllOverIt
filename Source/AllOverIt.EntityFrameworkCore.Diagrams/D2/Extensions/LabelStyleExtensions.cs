namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    internal static class LabelStyleExtensions
    {
        public static string AsText(this LabelStyle labelStyle, int indent)
        {
            return StyleStringBuilder.Create(indent, styler =>
            {
                if (labelStyle.FontSize != default)
                {
                    styler.Invoke("font-size", $"{labelStyle.FontSize}");
                }

                if (labelStyle.FontColor != default)
                {
                    styler.Invoke("font-color", labelStyle.FontColor);
                }

                if (labelStyle.Bold.HasValue)
                {
                    styler.Invoke("bold", $"{GetBoolString(labelStyle.Bold.Value)}");
                }

                if (labelStyle.Underline.HasValue != default)
                {
                    styler.Invoke("underline", $"{GetBoolString(labelStyle.Underline.Value)}");
                }

                if (labelStyle.Italic.HasValue != default)
                {
                    styler.Invoke("italic", $"{GetBoolString(labelStyle.Italic.Value)}");
                }
            });
        }

        private static string GetBoolString(bool value)
        {
            return value ? "true" : "false";
        }
    }
}