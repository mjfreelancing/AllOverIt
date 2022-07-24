namespace AllOverIt.Filtering.Filters
{
    /// <summary>Provides a filter option where elements are compared to the value of its
    /// <see cref="IStringFilterOperation.Value"/>. The comparison returns true when the
    /// element ends with the value of this <see cref="IStringFilterOperation.Value"/>.</summary>
    public sealed class EndsWith : IEndsWith
    {
        /// <summary>The filter value used for comparison. The comparison returns true when the
        /// element ends with the value of this property.</summary>
        public string Value { get; set; }

        /// <summary>Default constructor.</summary>
        public EndsWith()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The value to set on this filter option.</param>
        public EndsWith(string value)
        {
            Value = value;
        }

        /// <summary>Explicit operator to return the provided <see cref="EndsWith"/> instance as a string.</summary>
        /// <param name="value">The <see cref="EndsWith"/> instance.</param>
        public static explicit operator string(EndsWith value)
        {
            return value.Value;
        }

        /// <summary>Implicit operator to return the provided string as a <see cref="EndsWith"/> instance.</summary>
        /// <param name="value">The string value.</param>
        public static implicit operator EndsWith(string value)
        {
            return new EndsWith
            {
                Value = value
            };
        }
    }
}