namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter that returns <see langword="True"/> when the element does not contain the value of this filter.</summary>
    public sealed class NotContains : INotContains
    {
        /// <summary>The filter value used for comparison. The comparison returns <see langword="True"/> when the
        /// element does not contain the value of this property.</summary>
        public string? Value { get; set; }

        /// <summary>Default constructor.</summary>
        public NotContains()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public NotContains(string? value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="Contains"/> instance as a string.</summary>
        /// <param name="value">The <see cref="Contains"/> instance.</param>
        public static explicit operator string?(NotContains value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided string as a <see cref="Contains"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator NotContains(string? value)
        {
            return new NotContains
            {
                Value = value
            };
        }
    }
}