using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification.Types
{
    // Implements a specification that determines if a candidate contains a specified value.
    public sealed class Contains : Specification<string>
    {
        private string Value { get; }

        public Contains(string value)
        {
            Value = value.WhenNotNullOrEmpty(nameof(value));
        }

        protected override bool DoIsSatisfiedBy(string candidate)
        {
            return candidate.Contains(Value);
        }
    }
}