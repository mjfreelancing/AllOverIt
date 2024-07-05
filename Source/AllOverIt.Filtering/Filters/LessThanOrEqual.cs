namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter that returns <see langword="True"/> when the element is less than or equal to the value of this filter.</summary>
    public sealed class LessThanOrEqual<TProperty> : ILessThanOrEqual<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns <see langword="True"/> when the
        /// element is less than the value of this property.</summary>
        public TProperty? Value { get; set; }

        /// <summary>Default constructor.</summary>
        public LessThanOrEqual()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public LessThanOrEqual(TProperty? value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="LessThanOrEqual{TProperty}"/> instance as a <typeparamref name="TProperty"/>.</summary>
        /// <param name="value">The <see cref="LessThanOrEqual{TProperty}"/> instance.</param>
        public static explicit operator TProperty?(LessThanOrEqual<TProperty> value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided <typeparamref name="TProperty"/> as a <see cref="LessThanOrEqual{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator LessThanOrEqual<TProperty>(TProperty? value)
        {
            return new LessThanOrEqual<TProperty>(value);
        }
    }
}