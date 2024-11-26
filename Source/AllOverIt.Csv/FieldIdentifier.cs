using AllOverIt.Assertion;

namespace AllOverIt.Csv
{
    /// <summary>For complex fields that export multiple columns, this type is used to uniquely identify each set of column names.</summary>
    /// <typeparam name="TFieldId">The field identifier type that uniquely identifies each set of columns header names.</typeparam>
    public sealed class FieldIdentifier<TFieldId> where TFieldId : notnull
    {
        private sealed class FieldIdentifierComparer : IEqualityComparer<FieldIdentifier<TFieldId>>
        {
            public bool Equals(FieldIdentifier<TFieldId>? lhs, FieldIdentifier<TFieldId>? rhs)
            {
                Throw<InvalidOperationException>.When(lhs is null || rhs is null, "Field identifiers must not be null.");

                if (ReferenceEquals(lhs, rhs))
                {
                    return true;
                }

                // Only considering the 'Id' since the names should be unique per 'Id' for the CSV export to be of use
                return lhs!.Id.Equals(rhs!.Id);
            }

            public int GetHashCode(FieldIdentifier<TFieldId> obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        /// <summary>Returns a comparer that uniquely identifies each field identifier based on the <see cref="Id"/> property.</summary>
        public static IEqualityComparer<FieldIdentifier<TFieldId>> Comparer { get; } = new FieldIdentifierComparer();

        /// <summary>A unique field identifier. This value is used to determine each set of unique header column names. This value
        /// can also be used during value resolution to identify the values to return.</summary>
        /// <remarks>Examples of suitable values could be the item's index within a collection, a key in a dictionary, or a custom
        /// concatenation of field values.</remarks>
        public required TFieldId Id { get; init; }

        /// <summary>The column header names to be exported. These names should be unique across all identifiers.</summary>
        public required string[] Names { get; init; }
    }
}
