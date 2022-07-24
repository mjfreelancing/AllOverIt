namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter option where elements are compared to the value of its
    /// <see cref="IFilterOperationType{TProperty}.Value"/>. The comparison returns true when the
    /// element is equal to the value of this <see cref="IFilterOperationType{TProperty}.Value"/>.</summary>
    public sealed class EqualTo<TProperty> : IEqualTo<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element is equal to the value of this property.</summary>
        public TProperty Value { get; set; }

        /// <summary>Default constructor.</summary>
        public EqualTo()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public EqualTo(TProperty value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="EqualTo{TProperty}"/> instance as a <typeparamref name="TProperty"/>.</summary>
        /// <param name="value">The <see cref="EqualTo{TProperty}"/> instance.</param>
        public static explicit operator TProperty(EqualTo<TProperty> value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided <typeparamref name="TProperty"/> as a <see cref="EqualTo{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator EqualTo<TProperty>(TProperty value)
        {
            return new EqualTo<TProperty>(value);
        }
    }
}