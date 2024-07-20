#nullable enable

using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Result;

/// <summary>Represents an aggregate, or collection, of <see cref="EnrichedError"/> instances.</summary>
public class AggregateEnrichedError : EnrichedError
{
    /// <summary>Contains the aggregated collection of errors.</summary>
    public EnrichedError[] Errors { get; }

    /// <summary>Constructs an error with no <see cref="EnrichedError.Code"/>, <see cref="EnrichedError.Type"/>,
    /// or <see cref="EnrichedError.Description"/>.</summary>
    /// <param name="errors">The errors to compose as an aggregate.</param>
    public AggregateEnrichedError(params EnrichedError[] errors)
    {
        _ = errors.WhenNotNullOrEmpty();

        Errors = [.. errors];
    }

    /// <summary>Constructs an error with no <see cref="EnrichedError.Code"/> or <see cref="EnrichedError.Type"/>.</summary>
    /// <param name="description">The description for the error.</param>
    /// <param name="errors">The errors to compose as an aggregate.</param>
    public AggregateEnrichedError(string description, params EnrichedError[] errors)
        : base(description)
    {
        _ = errors.WhenNotNullOrEmpty();

        Errors = [.. errors];
    }

    /// <summary>Constructs an error with no <see cref="EnrichedError.Code"/>.</summary>
    /// <param name="type">A string representation of the error type, such as "Unexpected", "NotFound",
    /// "Unauthorized", "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application
    /// specific. See <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as
    /// a numerical or Enum value.</param>
    /// <param name="description">The description for the error.</param>
    /// <param name="errors">The errors to compose as an aggregate.</param>
    public AggregateEnrichedError(string type, string description, params EnrichedError[] errors)
        : base(type, description)
    {
        _ = errors.WhenNotNullOrEmpty();

        Errors = [.. errors];
    }

    /// <summary>Constructs an error with a <see cref="EnrichedError.Type"/>, <see cref="EnrichedError.Code"/>,
    /// and <see cref="EnrichedError.Description"/>.</summary>
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
        _ = errors.WhenNotNullOrEmpty();

        Errors = [.. errors];
    }
}
