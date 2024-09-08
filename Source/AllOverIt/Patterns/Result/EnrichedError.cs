namespace AllOverIt.Patterns.Result;

/// <summary>Represents an error reported by a failed <see cref="EnrichedResult"/>.</summary>
public class EnrichedError
{
    /// <summary>A string representation of the error type, such as "Unexpected", "NotFound", "Unauthorized",
    /// "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application specific. See
    /// <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as a numerical
    /// or Enum value.</summary>
    public string? Type { get; }

    /// <summary>An optional error code.</summary>
    public string? Code { get; }

    /// <summary>An optional description for the error.</summary>
    public string? Description { get; }

    /// <summary>Constructs an error with no <see cref="Code"/>, <see cref="Type"/>,
    /// or <see cref="Description"/>.</summary>
    public EnrichedError()
    {
    }

    /// <summary>Constructs an error with no <see cref="Code"/> or <see cref="Type"/>.</summary>
    /// <param name="description">The description for the error.</param>
    public EnrichedError(string description)
        : this(null, null, description)
    {
    }

    /// <summary>Constructs an error with no <see cref="Code"/>.</summary>
    /// <param name="type">A string representation of the error type, such as "Unexpected", "NotFound",
    /// "Unauthorized", "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application
    /// specific. See <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as
    /// a numerical or Enum value.</param>
    /// <param name="description">The description for the error.</param>
    public EnrichedError(string type, string description)
        : this(type, null, description)
    {
    }

    /// <summary>Constructs an error with a <see cref="Type"/>, <see cref="Code"/>, and <see cref="Description"/>.</summary>
    /// <param name="type">A string representation of the error type, such as "Unexpected", "NotFound",
    /// "Unauthorized", "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application
    /// specific. See <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as
    /// a numerical or Enum value.</param>
    /// <param name="code">A code for the error.</param>
    /// <param name="description">The description for the error.</param>
    public EnrichedError(string? type, string? code, string? description)
    {
        Type = type;
        Code = code;
        Description = description;
    }

    /// <summary>Aggregates one or more <see cref="EnrichedError"/> instances into an <see cref="AggregateEnrichedError"/>.</summary>
    /// <param name="errors">The errors to compose as an aggregate.</param>
    /// <returns>An <see cref="AggregateEnrichedError"/> containing each of the provided <paramref name="errors"/>.</returns>
    public static AggregateEnrichedError Aggregate(params EnrichedError[] errors)
    {
        return new(errors);
    }

    /// <summary>Aggregates one or more <see cref="EnrichedError"/> instances into an <see cref="AggregateEnrichedError"/>.</summary>
    /// <param name="description">The description for the error.</param>
    /// <param name="errors">The errors to compose as an aggregate.</param>
    /// <returns>An <see cref="AggregateEnrichedError"/> containing each of the provided <paramref name="errors"/>.</returns>
    public static AggregateEnrichedError Aggregate(string description, params EnrichedError[] errors)
    {
        return new(description, errors);
    }

    /// <summary>Aggregates one or more <see cref="EnrichedError"/> instances into an <see cref="AggregateEnrichedError"/>.</summary>
    /// <param name="type">A string representation of the error type, such as "Unexpected", "NotFound",
    /// "Unauthorized", "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application
    /// specific. See <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as
    /// a numerical or Enum value.</param>
    /// <param name="description">The description for the error.</param>
    /// <param name="errors">The errors to compose as an aggregate.</param>
    /// <returns>An <see cref="AggregateEnrichedError"/> containing each of the provided <paramref name="errors"/>.</returns>
    public static AggregateEnrichedError Aggregate(string type, string description, params EnrichedError[] errors)
    {
        return new(type, description, errors);
    }

    /// <summary>Aggregates one or more <see cref="EnrichedError"/> instances into an <see cref="AggregateEnrichedError"/>.</summary>
    /// <param name="type">A string representation of the error type, such as "Unexpected", "NotFound",
    /// "Unauthorized", "Validation", "Conflict", "BadRequest", "Forbidden", and so on. This is application
    /// specific. See <see cref="EnrichedError{TErrorType}"/> if you need to store some other type, such as
    /// a numerical or Enum value.</param>
    /// <param name="code">A code for the error.</param>
    /// <param name="description">The description for the error.</param>
    /// <param name="errors">The errors to compose as an aggregate.</param>
    /// <returns>An <see cref="AggregateEnrichedError"/> containing each of the provided <paramref name="errors"/>.</returns>
    public static AggregateEnrichedError Aggregate(string type, string code, string description, params EnrichedError[] errors)
    {
        return new(type, code, description, errors);
    }
}
