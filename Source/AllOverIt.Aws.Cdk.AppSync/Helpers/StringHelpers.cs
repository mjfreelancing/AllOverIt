using AllOverIt.Extensions;
using System.Linq;
using System.Text;
using SystemEnvironment = System.Environment;

namespace AllOverIt.Aws.Cdk.AppSync.Helpers
{
    public static class StringHelpers
    {
        public static string Prettify(string lines)
        {
            var allLines = lines
                .Split(SystemEnvironment.NewLine)
                .AsReadOnlyCollection();

            var minPadding = allLines
                .Where(item => !item.IsNullOrEmpty() && item != SystemEnvironment.NewLine)
                .Min(GetPaddingLength);

            var builder = new StringBuilder();

            foreach (var line in allLines)
            {
                builder.AppendLine(line.Length < minPadding ? string.Empty : line[minPadding..]);
            }

            return builder.ToString().Trim();
        }

        private static int GetPaddingLength(string value)
        {
            var index = 0;

            foreach (var item in value)
            {
                if (item is not (' ' or '\t'))
                {
                    break;
                }

                index++;
            }

            return index;
        }
    }
}