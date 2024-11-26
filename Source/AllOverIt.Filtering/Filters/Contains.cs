namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter that returns <see langword="True"/> when the element contains the value of this filter.</summary>
    public sealed class Contains : IContains
    {
        /// <summary>The filter value used for comparison. The comparison returns <see langword="True"/> when the
        /// element contains the value of this property.</summary>
        public string? Value { get; set; }

        /// <summary>Default constructor.</summary>
        public Contains()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public Contains(string value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="Contains"/> instance as a string.</summary>
        /// <param name="value">The <see cref="Contains"/> instance.</param>
        public static explicit operator string?(Contains value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided string as a <see cref="Contains"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator Contains(string? value)
        {
            return new Contains
            {
                Value = value
            };
        }
    }
}