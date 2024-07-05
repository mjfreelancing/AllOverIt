namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter option where elements are compared to the value of its
    /// <see cref="IStringFilterOperation.Value"/>. The comparison returns <see langword="True"/> when the
    /// element starts with the value of this <see cref="IStringFilterOperation.Value"/>.</summary>
    public sealed class StartsWith : IStartsWith
    {
        /// <summary>The filter value used for comparison. The comparison returns <see langword="True"/> when the
        /// element starts with the value of this property.</summary>
        public string? Value { get; set; }

        /// <summary>Default constructor.</summary>
        public StartsWith()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public StartsWith(string value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="StartsWith"/> instance as a string.</summary>
        /// <param name="value">The <see cref="StartsWith"/> instance.</param>
        public static explicit operator string?(StartsWith value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided string as a <see cref="StartsWith"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator StartsWith(string? value)
        {
            return new StartsWith
            {
                Value = value
            };
        }
    }
}