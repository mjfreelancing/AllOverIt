using System;
using System.Text;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    internal static class StyleStringBuilder
    {
        public static string Create(int indent, Action<Action<string, string>> styler)
        {
            var indenting = new string(' ', indent);
            var builder = new StringBuilder();

            void AddStyleOption(string key, string value)
            {
                builder.AppendLine($"{indenting}{indenting}{key}: {value}");
            }

            styler.Invoke(AddStyleOption);

            var styleText = builder.ToString().TrimEnd();

            return $$"""
                   {{indenting}}style: {
                   {{styleText.D2EscapeString()}}
                   {{indenting}}}
                   """;
        }
    }
}