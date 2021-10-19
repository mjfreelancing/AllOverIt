using System;

namespace AllOverIt.Patterns.Specification.Types
{
    // Implements a specification that determines if a candidate is less than a known value.
    // TType must implement IComparable
    public sealed class IsLessThan<TType> : Specification<TType> where TType : IComparable
    {
        private TType Value { get; }

        public IsLessThan(TType value)
        {
            Value = value;
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return candidate.CompareTo(Value) < 0;
        }
    }
}