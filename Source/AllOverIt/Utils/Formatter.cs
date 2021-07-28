using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllOverIt.Utils
{
    public static class Formatter
    {
        /// <summary>Returns a beautified version of the provided string, assumed to be in a JSON format.</summary>
        /// <param name="jsonValue">The string to be beautified.</param>
        /// <param name="indentSize">The number of spaces to use for indentation.</param>
        /// <returns>A beautified version of the provided string, assumed to be in a JSON format.</returns>
        /// <remarks>This method does not validate the string is well-formed.</remarks>
        public static string FormatJsonString(this string jsonValue, int indentSize = 2)
        {
            // This implementation is inspired by https://stackoverflow.com/questions/4580397/json-formatter-in-c) but not
            // using LINQ as the memory and speed performance is significantly poorer.

            _ = jsonValue.WhenNotNull(nameof(jsonValue));

            if (indentSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indentSize), "The indent size cannot be negtive.");
            }

            var indentation = 0;
            var indent = string.Concat(Enumerable.Repeat(" ", indentSize));

            var tokenProcessors = new List<Func<bool, char, StringBuilder, bool>>
            {
                // add a space after a colon
                (unquoted, ch, sb) =>
                {
                    if (unquoted && ch == ':')
                    {
                        sb.Append(": ");
                        return true;
                    }

                    return false;
                },

                // ignore whitespace
                (unquoted, ch, sb) =>
                {
                    return unquoted && char.IsWhiteSpace(ch);
                },

                // add a linebreak after a comma
                (unquoted, ch, sb) =>
                {
                    if (unquoted && ch == ',')
                    {
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(string.Concat(Enumerable.Repeat(indent, indentation)));
                        return true;
                    }

                    return false;
                },

                // start a new line after an opening bracket, and indent the next line
                (unquoted, ch, sb) =>
                {
                    if (unquoted && (ch == '{' || ch == '['))
                    {
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(string.Concat(Enumerable.Repeat(indent, ++indentation)));
                        return true;
                    }

                    return false;
                },

                // moving a closing bracket to the next line, with appropriate indentation
                (unquoted, ch, sb) =>
                {
                    if (unquoted && (ch == '}' || ch == ']'))
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append(string.Concat(Enumerable.Repeat(indent, --indentation)));
                        sb.Append(ch);
                    }
                    else
                    {
                        sb.Append(ch);
                    }

                    return true;
                }
            };

            var stringBuilder = new StringBuilder();
            var quoteCount = 0;
            var escapeCount = 0;

            foreach (var ch in jsonValue)
            {
                if (ch == '\\')
                {
                    escapeCount++;
                }
                else if (escapeCount > 0)
                {
                    escapeCount--;
                }

                var escaped = escapeCount > 0;
                var unquoted = quoteCount % 2 == 0;

                if (ch == '"' && !escaped)
                {
                    quoteCount++;
                }

                foreach (var processor in tokenProcessors)
                {
                    if (processor.Invoke(unquoted, ch, stringBuilder))
                    {
                        break;
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}