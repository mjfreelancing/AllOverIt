namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter option where elements are compared to the value of its
    /// <see cref="IFilterOperationType{TProperty}.Value"/>. The comparison returns true when the
    /// element is not equal to the value of this <see cref="IFilterOperationType{TProperty}.Value"/>.</summary>
    public sealed class NotEqualTo<TProperty> : INotEqualTo<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element is not equal to the value of this property.</summary>
        public TProperty Value { get; set; }

        /// <summary>Default constructor.</summary>
        public NotEqualTo()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public NotEqualTo(TProperty value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="NotEqualTo{TProperty}"/> instance as a <typeparamref name="TProperty"/>.</summary>
        /// <param name="value">The <see cref="NotEqualTo{TProperty}"/> instance.</param>
        public static explicit operator TProperty(NotEqualTo<TProperty> value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided <typeparamref name="TProperty"/> as a <see cref="NotEqualTo{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator NotEqualTo<TProperty>(TProperty value)
        {
            return new NotEqualTo<TProperty>(value);
        }
    }
}