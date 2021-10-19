using AllOverIt.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Patterns.Specification.Types
{
    // A specification that determines if a specified collection is empty.
    public sealed class IsEmpty<TType> : ISpecification<IEnumerable<TType>>
    {
        public bool IsSatisfiedBy(IEnumerable<TType> candidate)
        {
            return !candidate
                .WhenNotNull(nameof(candidate))
                .Any();
        }
    }
}