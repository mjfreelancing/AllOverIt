using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllOverIt.Formatters
{
    internal static class JsonString
    {
        private class FormatterState
        {
            private readonly StringBuilder _stringBuilder = new();

            public char Char { get; set; }
            public bool Unquoted { get; set; }
            public bool PendingIndent { get; set; }

            public void Append(char value)
            {
                _stringBuilder.Append(value);
            }

            public void Append(string value)
            {
                _stringBuilder.Append(value);
            }

            public override string ToString()
            {
                return _stringBuilder.ToString();
            }
        }

        /// <summary>Formats a well-formed JSON string.</summary>
        /// <param name="jsonValue">The JSON string to format.</param>
        /// <param name="indentSize">The indent size. The default is 2 spaces.</param>
        /// <returns>The formatted string. The input string is expected to be well-formed so an invalid
        /// input will result in an invalid formatted output.</returns>
        public static string Format(string jsonValue, int indentSize = 2)
        {
            // Inspired by https://stackoverflow.com/questions/4580397/json-formatter-in-c) but not
            // using LINQ as the memory and speed performance is significantly poorer.

            _ = jsonValue.WhenNotNull(nameof(jsonValue));

            if (indentSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indentSize), "The indent size cannot be negative.");
            }

            var indentation = 0;

            string GetIndent()
            {
                return new string(' ', indentSize * indentation);
            }

            var tokenProcessors = new List<Func<FormatterState, bool>>
            {
                // add a space after a colon because we are ignoring whitespace
                state =>
                {
                    if (state.Unquoted && state.Char == ':')
                    {
                        state.Append(": ");

                        return true;
                    }

                    return false;
                },

                // ignore whitespace
                state => state.Unquoted && char.IsWhiteSpace(state.Char),

                // add a line break after a comma
                state =>
                {
                    if (state.Unquoted && state.Char == ',')
                    {
                        state.Append(state.Char);
                        state.Append(Environment.NewLine);
                        state.Append(GetIndent());

                        return true;
                    }

                    return false;
                },

                // start a new line after an opening bracket and note the next line requires indenting
                state =>
                {
                    if (state.Unquoted && state.Char is '{' or '[')
                    {
                        state.Append(state.Char);
                        state.PendingIndent = true;
                        indentation++;

                        return true;
                    }

                    return false;
                },

                // move a closing bracket to the next line with appropriate indentation
                state =>
                {
                    if (state.Unquoted && state.Char is '}' or ']')
                    {
                        state.Append(Environment.NewLine);
                        indentation--;
                        state.Append(GetIndent());
                        state.Append(state.Char);

                        return true;
                    }

                    return false;
                },

                // some other character
                state =>
                {
                    if (state.PendingIndent)
                    {
                        state.Append(Environment.NewLine);
                        state.Append(GetIndent());
                    }

                    state.Append(state.Char);
                    state.PendingIndent = false;

                    return true;
                }
            };

            var state = new FormatterState();
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
                state.Unquoted = quoteCount % 2 == 0;

                if (ch == '"' && !escaped)
                {
                    quoteCount++;
                }

                state.Char = ch;

                foreach (var processor in tokenProcessors)
                {
                    if (processor.Invoke(state))
                    {
                        break;
                    }
                }
            }

            return state.ToString();
        }
    }
}