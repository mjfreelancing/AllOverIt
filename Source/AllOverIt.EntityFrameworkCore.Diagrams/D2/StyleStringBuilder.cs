using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    internal static class StyleStringBuilder
    {
        // <string, string> is style attribute, value
        public static string Create(int indent, Action<Action<string, string>> styler)
        {
            var indenting = new string(' ', indent);
            var builder = new StringBuilder();

            void AddStyleOption(string attribute, string value)
            {
                builder.AppendLine($"{indenting}{indenting}{attribute}: {value}");
            }

            styler.Invoke(AddStyleOption);

            if (builder.Length == 0)
            {
                return string.Empty;
            }

            var styleText = builder.ToString().TrimEnd();

            return $$"""
                   {{indenting}}style: {
                   {{styleText.D2EscapeString()}}
                   {{indenting}}}
                   """;
        }
    }
}