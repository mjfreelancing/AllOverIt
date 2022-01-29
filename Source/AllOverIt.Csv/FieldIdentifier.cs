using System.Collections.Generic;

namespace AllOverIt.Csv
{
    public sealed class FieldIdentifier<TFieldId>
    {
        private sealed class FieldIdentifierComparer<TFieldId> : IEqualityComparer<FieldIdentifier<TFieldId>>
        {
            // Only considering the 'Id' since the names should be unique per 'Id' for the CSV export to be of use
            public bool Equals(FieldIdentifier<TFieldId> lhs, FieldIdentifier<TFieldId> rhs)
            {
                if (ReferenceEquals(lhs, rhs))
                {
                    return true;
                }

                if (lhs == null || rhs == null)
                {
                    return false;
                }

                return lhs.Id.Equals(rhs.Id);
            }

            public int GetHashCode(FieldIdentifier<TFieldId> obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public static IEqualityComparer<FieldIdentifier<TFieldId>> Comparer { get; } = new FieldIdentifierComparer<TFieldId>();

        public TFieldId Id { get; init; }      // Could be the item's index within a collection or a key in a dictionary or a custom concat of field values
        public IReadOnlyCollection<string> Names { get; init; }
    }
}
