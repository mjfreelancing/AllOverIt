namespace AllOverIt.Patterns.Specification.Types
{
    // A specification that determines if a string is null, empty or comprised of whitespace.
    public sealed class IsNullOrWhitespace : ISpecification<string>
    {
        public bool IsSatisfiedBy(string candidate)
        {
            return string.IsNullOrWhiteSpace(candidate);
        }
    }
}