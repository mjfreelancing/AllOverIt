namespace AllOverIt.Formatters.Strings.Extensions
{
    /// <summary>Provides extensions for formatting strings in a known format.</summary>
    public static class StringFormatExtensions
    {
        /// <summary>Returns a beautified version of the provided string, assumed to be in a JSON format.</summary>
        /// <param name="jsonValue">The string to be beautified.</param>
        /// <param name="indentSize">The number of spaces to use for indentation.</param>
        /// <returns>A beautified version of the provided string, assumed to be in a JSON format.</returns>
        /// <remarks>This method does not validate the string is well-formed.</remarks>
        public static string FormatJsonString(this string jsonValue, int indentSize = 2)
        {
            return JsonString.Format(jsonValue, indentSize);
        }
    }
}