#nullable enable

using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Result
{
    public class AggregateEnrichedError : EnrichedError
    {
        public EnrichedError[] Errors { get; }

        /// <summary>Constructs an error with no <see cref="Code"/>, <see cref="Type"/>,
        /// or <see cref="Description"/>.</summary>
        /// <param name="errors">The errors to compose as an aggregate.</param>
        public AggregateEnrichedError(params EnrichedError[] errors)
        {
            _ = errors.WhenNotNullOrEmpty(nameof(errors));

            Errors = [.. errors];
        }

        /// <summary>Constructs an error with no <see cref="Code"/> or <see cref="Type"/>.</summary>
        /// <param name="description">The description for the error.</param>
        /// <param name="errors">The errors to compose as an aggregate.</param>
        public AggregateEnrichedError(string description, params EnrichedError[] errors)
            : base(description)
        {
            _ = errors.WhenNotNullOrEmpty(nameof(errors));

            Errors = [.. errors];
        }

        /// <summary>Constructs an error with no <see cref="Code"/>.</summary>
        /// <param name="type">A string representation of the error type, such as "Unexpected", "NotFound",
        /// "Unauthorized", "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application
        /// specific. See <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as
        /// a numerical or Enum value.</param>
        /// <param name="description">The description for the error.</param>
        /// <param name="errors">The errors to compose as an aggregate.</param>
        public AggregateEnrichedError(string type, string description, params EnrichedError[] errors)
            : base(type, description)
        {
            _ = errors.WhenNotNullOrEmpty(nameof(errors));

            Errors = [.. errors];
        }

        /// <summary>Constructs an error with a <see cref="Type"/>, <see cref="Code"/>, and <see cref="Description"/>.</summary>
        /// <param name="type">A string representation of the error type, such as "Unexpected", "NotFound",
        /// "Unauthorized", "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application
        /// specific. See <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as
        /// a numerical or Enum value.</param>
        /// <param name="code">A code for the error.</param>
        /// <param name="description">The description for the error.</param>
        /// <param name="errors">The errors to compose as an aggregate.</param>
        public AggregateEnrichedError(string? type, string? code, string? description, params EnrichedError[] errors)
            : base(type, code, description)
        {
            _ = errors.WhenNotNullOrEmpty(nameof(errors));

            Errors = [.. errors];
        }
    }
}
