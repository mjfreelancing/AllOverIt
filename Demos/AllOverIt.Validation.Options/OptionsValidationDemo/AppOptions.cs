namespace OptionsValidationDemo
{
    public sealed class AppOptions
    {
        public static readonly string SectionName = "Options";

        public int MinLevel { get; init; }
        public string? AppName { get; init; }
    }
}