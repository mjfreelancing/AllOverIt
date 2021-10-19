namespace AllOverIt.Patterns.Specification.Types
{
    // A specification that determines if a string is not null, empty or comprised of whitespace.
    public sealed class IsNotNullOrWhitespace : ISpecification<string>
    {
        public bool IsSatisfiedBy(string candidate)
        {
            return !string.IsNullOrWhiteSpace(candidate);
        }
    }
}