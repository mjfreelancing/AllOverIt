namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter option where elements are compared to the value of its
    /// <see cref="IFilterOperationType{TProperty}.Value"/>. The comparison returns true when the
    /// element is less than or equal to the value of this <see cref="IFilterOperationType{TProperty}.Value"/>.</summary>
    public sealed class LessThanOrEqual<TType> : ILessThanOrEqual<TType>
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element is less than the value of this property.</summary>
        public TType Value { get; set; }

        /// <summary>Default constructor.</summary>
        public LessThanOrEqual()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public LessThanOrEqual(TType value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="LessThanOrEqual{TProperty}"/> instance as a <typeparamref name="TProperty"/>.</summary>
        /// <param name="value">The <see cref="LessThanOrEqual{TProperty}"/> instance.</param>
        public static explicit operator TType(LessThanOrEqual<TType> value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided <typeparamref name="TProperty"/> as a <see cref="LessThanOrEqual{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator LessThanOrEqual<TType>(TType value)
        {
            return new LessThanOrEqual<TType>(value);
        }
    }
}