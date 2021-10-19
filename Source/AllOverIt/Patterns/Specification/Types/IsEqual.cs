using System;

namespace AllOverIt.Patterns.Specification.Types
{
    // Implements a specification that determines if a candidate is equal to a known value.
    // TType must implement IComparable
    public sealed class IsEqual<TType> : Specification<TType> where TType : IComparable
    {
        private TType Value { get; }

        public IsEqual(TType value)
        {
            Value = value;
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return candidate.CompareTo(Value) == 0;
        }
    }
}