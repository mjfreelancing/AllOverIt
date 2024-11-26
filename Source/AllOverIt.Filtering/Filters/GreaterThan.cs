namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter that returns <see langword="True"/> when the element is greater than the value of this filter.</summary>
    public sealed class GreaterThan<TProperty> : IGreaterThan<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns <see langword="True"/> when the
        /// element is greater than the value of this property.</summary>
        public TProperty? Value { get; set; }

        /// <summary>Default constructor.</summary>
        public GreaterThan()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public GreaterThan(TProperty? value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="GreaterThan{TProperty}"/> instance as a <typeparamref name="TProperty"/>.</summary>
        /// <param name="value">The <see cref="GreaterThan{TProperty}"/> instance.</param>
        public static explicit operator TProperty?(GreaterThan<TProperty> value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided <typeparamref name="TProperty"/> as a <see cref="GreaterThan{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator GreaterThan<TProperty>(TProperty? value)
        {
            return new GreaterThan<TProperty>(value);
        }
    }
}