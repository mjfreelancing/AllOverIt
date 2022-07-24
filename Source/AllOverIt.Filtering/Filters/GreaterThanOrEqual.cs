namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter option where elements are compared to the value of its
    /// <see cref="IFilterOperationType{TProperty}.Value"/>. The comparison returns true when the
    /// element is greater than or equal to the value of this <see cref="IFilterOperationType{TProperty}.Value"/>.</summary>
    public sealed class GreaterThanOrEqual<TProperty> : IGreaterThanOrEqual<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element is greater than the value of this property.</summary>
        public TProperty Value { get; set; }

        /// <summary>Default constructor.</summary>
        public GreaterThanOrEqual()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public GreaterThanOrEqual(TProperty value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="GreaterThanOrEqual{TProperty}"/> instance as a <typeparamref name="TProperty"/>.</summary>
        /// <param name="value">The <see cref="GreaterThanOrEqual{TProperty}"/> instance.</param>
        public static explicit operator TProperty(GreaterThanOrEqual<TProperty> value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided <typeparamref name="TProperty"/> as a <see cref="GreaterThanOrEqual{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator GreaterThanOrEqual<TProperty>(TProperty value)
        {
            return new GreaterThanOrEqual<TProperty>(value);
        }
    }
}