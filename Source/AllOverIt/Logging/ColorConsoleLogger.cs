using AllOverIt.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace AllOverIt.Logging
{
    /// <inheritdoc cref="IColorConsoleLogger" />
    [ExcludeFromCodeCoverage]
    public sealed class ColorConsoleLogger : IColorConsoleLogger
    {
        private const string ForegroundPrefix = "fore";
        private const string BackgroundPrefix = "back";

        // If the regex did not use the Singleline option, the regular expression would only match the first segment
        // ("{color:yellow}This text is yellow\nand continues on a new line") and would not match the second segment
        // ("{color:red}This text is red"), because the second segment contains a line break.
        //
        // Breakdown:
        // {(fore|back)color:  - Match the opening curly brace {, followed by either "forecolor" or "backcolor", captured
        //                       as a group, followed by a colon.
        // ([\s\S]*?)          - Match any character (including newline characters \r\n) zero or more times, non-greedily,
        //                       and capture it as a group. The \s matches any whitespace character and \S matches any
        //                       non-whitespace character.
        // \}                  - Match the closing curly brace }.
        // ([\s\S]*?)          - Match any character (including newline characters \r\n) zero or more times, non-greedily,
        //                       and capture it as a group. This will match the text between the color tags.
        // (?={(fore|back)color:|\z)
        //                     - Positive lookahead for either the start of another color tag (either "forecolor" or "backcolor"),
        //                       or the end of the string. This will allow the regex to match multiple color tags in a single string.
        //
        // Note that the use of [\s\S] instead of . is to match any character, including newline characters. If we were to use .
        // instead of [\s\S], it would not match newline characters, unless the RegexOptions.Singleline option is used.
        private static readonly Regex ColorMatchRegex = new($@"{{({ForegroundPrefix}|{BackgroundPrefix})color:([\s\S]*?)\}}([\s\S]*?)(?={{({ForegroundPrefix}|{BackgroundPrefix})color:|\z)");

        private readonly StringBuilder _stringBuilder = new();

        /// <inheritdoc />
        public IColorConsoleLogger Write(string formattedText)
        {
            LogToConsole(formattedText);

            return this;
        }

        /// <inheritdoc />
        public IColorConsoleLogger Write(ConsoleColor foreColor, string text)
        {
            return AddFragment(foreColor, null, text);
        }

        /// <inheritdoc />
        public IColorConsoleLogger Write(ConsoleColor foreColor, ConsoleColor backColor, string text)
        {
            return Write(foreColor, backColor, text);
        }

        /// <inheritdoc />
        public IColorConsoleLogger WriteLine(string formattedText)
        {
            LogToConsole(formattedText);

            return WriteLine();
        }

        /// <inheritdoc />
        public IColorConsoleLogger WriteLine(ConsoleColor foreColor, string text)
        {
            return AddFragment(foreColor, null, text).WriteLine();
        }

        /// <inheritdoc />
        public IColorConsoleLogger WriteLine(ConsoleColor foreColor, ConsoleColor backColor, string text)
        {
            return Write(foreColor, backColor, text).WriteLine();
        }

        /// <inheritdoc />
        public IColorConsoleLogger WriteLine()
        {
            Console.WriteLine();

            return this;
        }

        private ColorConsoleLogger AddFragment(ConsoleColor? foreColor, ConsoleColor? backColor, string text)
        {
            if (foreColor.HasValue)
            {
                _stringBuilder.Append($"{{forecolor:{foreColor}}}");
            }

            if (backColor.HasValue)
            {
                _stringBuilder.Append($"{{backcolor:{backColor}}}");
            }

            _stringBuilder.Append(text);

            LogToConsole(_stringBuilder.ToString());

            _stringBuilder.Clear();

            return this;
        }

        private static void LogToConsole(string message)
        {
            // Example formatted strings:
            //   "{forecolor:yellow}This text is yellow\nand continues on a new line\n{forecolor:red}This text is red",
            //   "{forecolor:yellow}This text has a yellow foreground{backcolor:green} and a green background{forecolor:red} and a red foreground{backcolor:blue} and a blue background"

            try
            {
                // Find fore/back color and text
                var matches = ColorMatchRegex
                    .Matches(message)
                    .Cast<Match>()
                    .AsReadOnlyCollection();

                if (matches.Count == 0)
                {
                    Console.Write(message);
                    return;
                }

                foreach (var match in matches)
                {
                    var colorType = match.Groups[1].Value;
                    var colorName = match.Groups[2].Value;
                    var text = match.Groups[3].Value;

                    if (colorType == ForegroundPrefix)
                    {
                        Console.ForegroundColor = Enum.Parse<ConsoleColor>(colorName, true);
                    }
                    else if (colorType == BackgroundPrefix)
                    {
                        Console.BackgroundColor = Enum.Parse<ConsoleColor>(colorName, true);
                    }

                    Console.Write(text);
                }
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}