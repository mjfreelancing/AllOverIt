using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Result;

/// <summary>A discriminated union of a void result, or an error.</summary>
public class EnrichedResult
{
    private EnrichedError? _error;

    /// <summary>Indicates if the result has a successful state.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates if the result has an error state.</summary>
    public bool IsFail => !IsSuccess;

    /// <summary>An optional <see cref="EnrichedError"/> that describes the error state.
    /// This property will throw an <see cref="InvalidOperationException"/> if the result
    /// is not in an error state.</summary>
    public EnrichedError? Error
    {
        get => GetError();
        set => _error = value;
    }

    internal EnrichedResult(bool success, EnrichedError? error)
    {
        IsSuccess = success;
        _error = error;
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a successful state.</summary>
    /// <returns>A new <see cref="EnrichedResult"/> with a successful state.</returns>
    public static EnrichedResult Success()
    {
        return new(true, null);
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a successful state, with a default value.</summary>
    /// <typeparam name="TResult">The result's <see cref="EnrichedResult{TResult}.Value"/> type.</typeparam>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> with a successful state, and a default value.</returns>
    public static EnrichedResult<TResult> Success<TResult>()
    {
        return new();
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a successful state, with a value.</summary>
    /// <typeparam name="TResult">The result's <see cref="EnrichedResult{TResult}.Value"/> type.</typeparam>
    /// <param name="result">The value of the successful result.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> with a successful state, and a value.</returns>
    public static EnrichedResult<TResult> Success<TResult>(TResult? result)
    {
        return new(result);
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state.</summary>
    /// <param name="error">An optional <see cref="EnrichedError"/> to associate with the failed result.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state.</returns>
    public static EnrichedResult Fail(EnrichedError? error = default)
    {
        return new(false, error);
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state.</summary>
    /// <typeparam name="TResult">The result type associated with the result.</typeparam>
    /// <param name="error">An optional <see cref="EnrichedError{TResult}"/> to associate with the failed result.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state.</returns>
    public static EnrichedResult<TResult> Fail<TResult>(EnrichedError? error = default)
    {
        return new(error);
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state, with a description.</summary>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state, with a description.</returns>
    public static EnrichedResult Fail(string description)
    {
        return new(false, new EnrichedError(description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state, with a type and description.</summary>
    /// <param name="type">Describes the error type.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state, with a type and description.</returns>
    public static EnrichedResult Fail(string type, string description)
    {
        return new(false, new EnrichedError(type, description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state, with a type, a code, and description.</summary>
    /// <param name="type">Describes the error type.</param>
    /// <param name="code">A code that identifies the error.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state, with a type, a code, and description.</returns>
    public static EnrichedResult Fail(string? type, string? code, string? description)
    {
        return new(false, new EnrichedError(type, code, description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state, with a description.</summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> in a failed state, with a description.</returns>
    public static EnrichedResult<TResult> Fail<TResult>(string description)
    {
        return new EnrichedResult<TResult>(new EnrichedError(description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state, with a type and description.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="type">Describes the error type.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> in a failed state, with a type and description.</returns>
    public static EnrichedResult<TResult> Fail<TResult>(string type, string description)
    {
        return new EnrichedResult<TResult>(new EnrichedError(type, description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state, with a type, a code, and a description.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="type">Describes the error type.</param>
    /// <param name="code">A code that identifies the error.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> in a failed state, with a type, a code, and a description.</returns>
    public static EnrichedResult<TResult> Fail<TResult>(string? type, string? code, string? description)
    {
        return new EnrichedResult<TResult>(new EnrichedError(type, code, description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state, with an error type.</summary>
    /// <typeparam name="TErrorType">The error type that describes the error.</typeparam>
    /// <param name="errorType">The value of the <typeparamref name="TErrorType"/>.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state, with an error type.</returns>
    public static EnrichedResult Fail<TErrorType>(TErrorType errorType)
        where TErrorType : notnull
    {
        return new(false, new EnrichedError<TErrorType>(errorType));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state, with an error type, and a description.</summary>
    /// <typeparam name="TErrorType">The error type that describes the error.</typeparam>
    /// <param name="errorType">The value of the <typeparamref name="TErrorType"/>.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state, with an error type, and a description.</returns>
    public static EnrichedResult Fail<TErrorType>(TErrorType errorType, string description)
        where TErrorType : notnull
    {
        return new(false, new EnrichedError<TErrorType>(errorType, description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state, with an error type, error code, and a description.</summary>
    /// <typeparam name="TErrorType">The error type that describes the error.</typeparam>
    /// <param name="errorType">The value of the <typeparamref name="TErrorType"/>.</param>
    /// <param name="code">A code that identifies the error.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state, with an error type, error code, and a description.</returns>
    public static EnrichedResult Fail<TErrorType>(TErrorType errorType, string? code, string? description)
        where TErrorType : notnull
    {
        return new(false, new EnrichedError<TErrorType>(errorType, code, description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state, with an error type.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TErrorType">The error type that describes the error.</typeparam>
    /// <param name="errorType">The value of the <typeparamref name="TErrorType"/>.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> in a failed state, with an error type.</returns>
    public static EnrichedResult<TResult> Fail<TResult, TErrorType>(TErrorType errorType)
        where TErrorType : notnull
    {
        return new EnrichedResult<TResult>(new EnrichedError<TErrorType>(errorType));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state, with an error type, and a description.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TErrorType">The error type that describes the error.</typeparam>
    /// <param name="errorType">The value of the <typeparamref name="TErrorType"/>.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> in a failed state, with an error type, and a description.</returns>
    public static EnrichedResult<TResult> Fail<TResult, TErrorType>(TErrorType errorType, string description)
        where TErrorType : notnull
    {
        return new EnrichedResult<TResult>(new EnrichedError<TErrorType>(errorType, description));
    }

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state, with an error type, an error code, and a description.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TErrorType">The error type that describes the error.</typeparam>
    /// <param name="errorType">The value of the <typeparamref name="TErrorType"/>.</param>
    /// <param name="code">A code that identifies the error.</param>
    /// <param name="description">A description for the error.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> in a failed state, with an error type, an error code, and a description.</returns>
    public static EnrichedResult<TResult> Fail<TResult, TErrorType>(TErrorType errorType, string? code, string? description)
        where TErrorType : notnull
    {
        return new EnrichedResult<TResult>(new EnrichedError<TErrorType>(errorType, code, description));
    }

    private EnrichedError? GetError()
    {
        Throw<InvalidOperationException>.When(IsSuccess, "The result has no error.");

        return _error;
    }
}
