#nullable enable

using AllOverIt.Assertion;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Patterns.Result;

/// <summary>A discriminated union of a void result, or an error.</summary>
public class EnrichedResult
{
    private EnrichedError? _error;

    /// <summary>Indicates if the result has a successful state.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates if the result has an error state.</summary>
    public bool IsError => !IsSuccess;

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
        Error = error;
    }

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a successful state.</summary>
    /// <returns>A new <see cref="EnrichedResult"/> with a successful state.</returns>
    [return: NotNull]
    public static EnrichedResult Success() => new(true, null);

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a successful state, with a default value.</summary>
    /// <typeparam name="TResult">The result's <see cref="EnrichedResult{TResult}.Value"/> type.</typeparam>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> with a successful state, and a default value.</returns>
    [return: NotNull]
    public static EnrichedResult<TResult> Success<TResult>() => new();

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a successful state, with a value.</summary>
    /// <typeparam name="TResult">The result's <see cref="EnrichedResult{TResult}.Value"/> type.</typeparam>
    /// <param name="result">The value of the successful result.</param>
    /// <returns>A new <see cref="EnrichedResult{TResult}"/> with a successful state, and a value.</returns>
    [return: NotNull]
    public static EnrichedResult<TResult> Success<TResult>(TResult result) => new(result);

    /// <summary>Constructs a new <see cref="EnrichedResult"/> in a failed state.</summary>
    /// <param name="error">An optional <see cref="EnrichedError"/> to associate with the failed result.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state.</returns>
    [return: NotNull]
    public static EnrichedResult Fail(EnrichedError? error = default) => new(false, error);

    /// <summary>Constructs a new <see cref="EnrichedResult{TResult}"/> in a failed state.</summary>
    /// <typeparam name="TResult">The result type associated with the result.</typeparam>
    /// <param name="error">An optional <see cref="EnrichedError{TResult}"/> to associate with the failed result.</param>
    /// <returns>A new <see cref="EnrichedResult"/> in a failed state.</returns>
    [return: NotNull]
    public static EnrichedResult<TResult> Fail<TResult>(EnrichedError? error = default) => new(error);

    private EnrichedError? GetError()
    {
        Throw<InvalidOperationException>.When(IsSuccess, "The result has no error.");

        return _error;
    }
}
