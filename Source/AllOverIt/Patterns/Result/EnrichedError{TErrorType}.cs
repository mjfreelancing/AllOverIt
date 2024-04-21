#nullable enable

namespace AllOverIt.Patterns.Result
{
    /// <summary>Enhances <see cref="EnrichedError"/> by providing the ability to store the error type
    /// as another type that is more suitable for comparisons, such as an <see langword="int"/> or <see langword="enum"/>.</summary>
    /// <typeparam name="TErrorType">The strongly-typed error type.</typeparam>
    public class EnrichedError<TErrorType> : EnrichedError where TErrorType : notnull
    {
        /// <summary>The strongly-typed error type.</summary>
        public TErrorType ErrorType { get; }

        /// <summary>Constructor.</summary>
        /// <param name="errorType">The error type.</param>
        public EnrichedError(TErrorType errorType)
            : this(errorType, null, null)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="errorType">The error type.</param>
        /// <param name="description">An optional description for the error.</param>
        public EnrichedError(TErrorType errorType, string description)
            : this(errorType, null, description)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="errorType">The error type.</param>
        /// <param name="code">An optional error code.</param>
        /// <param name="description">An optional description for the error.</param>
        public EnrichedError(TErrorType errorType, string? code, string? description)
            : base(errorType.ToString(), code, description)
        {
            ErrorType = errorType;
        }
    }
}
