using System.Text.RegularExpressions;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class StringExtensions
    {
        private static readonly Regex SplitWordsRegex = new("((?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]))", RegexOptions.Compiled);

        public static string GetGraphqlName(this string name)
        {
            return string.Concat(name[..1].ToLower(), name[1..]);
        }

        public static string ToUpperSnakeCase(this string value)
        {
            return SplitWordsRegex.Replace(value, "_$1").ToUpper();
        }
    }
}