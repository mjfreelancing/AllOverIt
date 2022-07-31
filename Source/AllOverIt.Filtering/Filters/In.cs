using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter that returns true when the element exists in this filter.</summary>
    public sealed class In<TProperty> : IIn<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element exists in the list of values provided by this property.</summary>
        public IList<TProperty> Value { get; set; }

        /// <summary>Default constructor.</summary>
        public In()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="values">The values to set on this filter option.</param>
        public In(IEnumerable<TProperty> values)
        {
            Value = values.AsList();
        }

        /// <summary>Explicit operator to return the provided <see cref="In{TProperty}"/> instance as a List&lt;<typeparamref name="TProperty"/>&gt;.</summary>
        /// <param name="value">The <see cref="In{TProperty}"/> instance.</param>
        public static explicit operator List<TProperty>(In<TProperty> value)
        {
            return (List<TProperty>) value.Value;
        }

        /// <summary>Implicit operator to return the provided List&lt;<typeparamref name="TProperty"/>&gt; as a <see cref="In{TProperty}"/> instance.</summary>
        /// <param name="values">The string value.</param>
        public static implicit operator In<TProperty>(List<TProperty> values)
        {
            return new In<TProperty>(values);
        }
    }
}