using AllOverIt.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Csv
{
    public sealed class HeaderIdentifierComparer<THeaderId> : IEqualityComparer<HeaderIdentifier<THeaderId>>
    {
        public bool Equals(HeaderIdentifier<THeaderId> lhs, HeaderIdentifier<THeaderId> rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs == null || rhs == null)
            {
                return false;
            }

            if (!lhs.Id.Equals(rhs.Id))
            {
                return false;
            }

            if (lhs.Names.Count != rhs.Names.Count)
            {
                return false;
            }

            return lhs.Names
                .Zip(rhs.Names, (thisName, otherName) => thisName.Equals(otherName))
                .All(item => true);
        }

        public int GetHashCode(HeaderIdentifier<THeaderId> obj)
        {
            var hash = HashCodeHelper.CalculateHashCode(obj.Id);
            return HashCodeHelper.CalculateHashCode(hash, obj.Names);
        }
    }
}