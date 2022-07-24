using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter option where elements are compared to the value of its
    /// <see cref="IArrayFilterOperation{TProperty}.Value"/>. The comparison returns true when the
    /// element does not exist in this <see cref="IFilterOperationType{TProperty}.Value"/>.</summary>
    public sealed class NotIn<TProperty> : INotIn<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element does not exist in the list of values provided by this property.</summary>
        public IList<TProperty> Value { get; set; }

        /// <summary>Default constructor.</summary>
        public NotIn()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The values to set on this filter option.</param>
        public NotIn(IEnumerable<TProperty> values)
        {
            Value = values.AsList();
        }

        /// <summary>Explicit operator to return the provided <see cref="NotIn{TProperty}"/> instance as a List&lt;<typeparamref name="TProperty"/>&gt;.</summary>
        /// <param name="value">The <see cref="NotIn{TProperty}"/> instance.</param>
        public static explicit operator List<TProperty>(NotIn<TProperty> value)
        {
            return (List<TProperty>) value.Value;
        }

        /// <summary>Implicit operator to return the provided List&lt;<typeparamref name="TProperty"/>&gt; as a <see cref="NotIn{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator NotIn<TProperty>(List<TProperty> values)
        {
            return new NotIn<TProperty>(values);
        }
    }
}