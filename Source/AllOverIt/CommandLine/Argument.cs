using System.Text;

namespace AllOverIt.CommandLine
{
    /// <summary>Provides helper methods related to command line arguments.</summary>
    public static class Argument
    {
        private const char Quote = '\"';
        private const char Backslash = '\\';

        /// <summary>Escapes backslash and quote characters and quotes a string if it contains whitespace.</summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>An escaped version of the string if it contains backslash or quote characters, otherwise the original string.</returns>
        public static string Escape(string value)
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.environment.getcommandlineargs?redirectedfrom=MSDN&view=net-7.0#System_Environment_GetCommandLineArgs
            // TL;DR - If a double quotation mark follows two or an even number of backslashes, each proceeding backslash pair is replaced with one backslash and the double
            // quotation mark is removed. If a double quotation mark follows an odd number of backslashes, including just one, each preceding pair is replaced with one
            // backslash and the remaining backslash is removed; however, in this case the double quotation mark is not removed.

            // Code reference:
            // https://github.com/dotnet/runtime/blob/9a50493f9f1125fda5e2212b9d6718bc7cdbc5c0/src/libraries/System.Private.CoreLib/src/System/PasteArguments.cs#L10-L79
            // MIT License, .NET Foundation

            if (value.Length > 0 && !value.RequiresEscaping())
            {
                return value;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(Quote);            

            for (var idx = 0; idx < value.Length;)
            {
                var current = value[idx++];

                if (current == Backslash)
                {
                    var backslashCount = 1;

                    while (idx < value.Length && value[idx] == Backslash)
                    {
                        idx++;
                        backslashCount++;
                    }

                    if (idx == value.Length)
                    {
                        stringBuilder.Append(Backslash, backslashCount * 2);
                    }
                    else if (value[idx] == Quote)
                    {
                        stringBuilder.Append(Backslash, backslashCount * 2 + 1);
                        stringBuilder.Append(Quote);
                        idx++;
                    }
                    else
                    {
                        stringBuilder.Append(Backslash, backslashCount);
                    }

                    continue;
                }

                if (current == Quote)
                {
                    stringBuilder.Append(Backslash);
                    stringBuilder.Append(Quote);
                    continue;
                }

                stringBuilder.Append(current);
            }

            stringBuilder.Append(Quote);

            return stringBuilder.ToString();
        }

        private static bool RequiresEscaping(this string value)
        {
            // Code reference:
            // https://github.com/dotnet/runtime/blob/9a50493f9f1125fda5e2212b9d6718bc7cdbc5c0/src/libraries/System.Private.CoreLib/src/System/PasteArguments.cs#L81-L93
            // MIT License, .NET Foundation

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (char.IsWhiteSpace(c) || c == Quote)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
