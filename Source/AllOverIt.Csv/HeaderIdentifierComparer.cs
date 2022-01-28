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

            if (ReferenceEquals(lhs, null))
            {
                return false;
            }
            
            if (ReferenceEquals(rhs, null))
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
            var objects = obj.Names
                .Select(item => (object)item)
                .Concat(new []{(object)obj.Id})
                .ToArray();

            return AggregateHashCode(objects);
        }

        // TODO: Add something that can be re-used
        private static int AggregateHashCode(IEnumerable<object> properties)
        {
            return properties.Aggregate(17, (current, property) => current * 23 + (property?.GetHashCode() ?? 0));
        }
    }
}