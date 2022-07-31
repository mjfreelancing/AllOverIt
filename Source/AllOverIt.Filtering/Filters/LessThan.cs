namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter that returns true when the element is less than the value of this filter.</summary>
    public sealed class LessThan<TProperty> : ILessThan<TProperty>
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element is less than the value of this property.</summary>
        public TProperty Value { get; set; }

        /// <summary>Default constructor.</summary>
        public LessThan()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public LessThan(TProperty value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="LessThan{TProperty}"/> instance as a <typeparamref name="TProperty"/>.</summary>
        /// <param name="value">The <see cref="LessThan{TProperty}"/> instance.</param>
        public static explicit operator TProperty(LessThan<TProperty> value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided <typeparamref name="TProperty"/> as a <see cref="LessThan{TProperty}"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator LessThan<TProperty>(TProperty value)
        {
            return new LessThan<TProperty>
            {
                Value = value
            };
        }
    }


}