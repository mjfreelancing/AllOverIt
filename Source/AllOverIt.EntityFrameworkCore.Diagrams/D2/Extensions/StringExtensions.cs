namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    internal static class StringExtensions
    {
        public static string D2EscapeString(this string value)
        {
            return value
                .Replace("#", "\\#")
                .Replace("[", "\\[")
                .Replace("]", "\\]");
        }
    }
}