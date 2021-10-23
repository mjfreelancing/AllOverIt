using AllOverIt.Patterns.Specification;

namespace SpecificationDemo
{
    internal sealed class IsMultipleOf : Specification<int>
    {
        private int Value { get; }

        public IsMultipleOf(int value)
        {
            Value = value;
        }

        public override bool IsSatisfiedBy(int candidate)
        {
            return candidate % Value == 0;
        }
    }
}