#nullable enable

using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.Result.Extensions;

/// <summary>Provides a variety of extension methods for <see cref="EnrichedResult"/> and <see cref=EnrichedResult{TResult}"/>.</summary>
public static class EnrichedResultExtensions
{
    /// <summary>Matches a result for success or failure and invokes an associated action.</summary>
    /// <param name="result">The result to match success or failure.</param>
    /// <param name="onSuccess">The action to invoke if the result has a success status.</param>
    /// <param name="onFail">The action to invoke if the result has an error status.</param>
    /// <returns>The result of the invoked action.</returns>
    public static EnrichedResult Match(this EnrichedResult result, Func<EnrichedResult, EnrichedResult> onSuccess,
        Func<EnrichedResult, EnrichedResult> onFail)
    {
        _ = result.WhenNotNull(nameof(result));
        _ = onSuccess.WhenNotNull(nameof(onSuccess));
        _ = onFail.WhenNotNull(nameof(onFail));

        return result.IsSuccess
            ? onSuccess.Invoke(result)
            : onFail.Invoke(result);
    }

    /// <summary>Matches a result for success or failure and invokes an associated action.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result to match success or failure.</param>
    /// <param name="onSuccess">The action to invoke if the result has a success status.</param>
    /// <param name="onFail">The action to invoke if the result has an error status.</param>
    /// <returns>The result of the invoked action.</returns>
    public static EnrichedResult<TResult> Match<TResult>(this EnrichedResult<TResult> result,
        Func<EnrichedResult<TResult>, EnrichedResult<TResult>> onSuccess,
        Func<EnrichedResult<TResult>, EnrichedResult<TResult>> onFail)
    {
        return Match<TResult, TResult>(result, onSuccess, onFail);
    }

    /// <summary>Matches a result for success or failure and invokes an associated action. The returned result
    /// can be of a different type to the original result.</summary>
    /// <typeparam name="TInResult">The incoming result type.</typeparam>
    /// <typeparam name="TOutResult">The returned result type.</typeparam>
    /// <param name="result">The result to match success or failure.</param>
    /// <param name="onSuccess">The action to invoke if the result has a success status.</param>
    /// <param name="onFail">The action to invoke if the result has an error status.</param>
    /// <returns>The result of the invoked action.</returns>
    public static EnrichedResult<TOutResult> Match<TInResult, TOutResult>(this EnrichedResult<TInResult> result,
        Func<EnrichedResult<TInResult>, EnrichedResult<TOutResult>> onSuccess,
        Func<EnrichedResult<TInResult>, EnrichedResult<TOutResult>> onFail)
    {
        _ = result.WhenNotNull(nameof(result));
        _ = onSuccess.WhenNotNull(nameof(onSuccess));
        _ = onFail.WhenNotNull(nameof(onFail));

        return result.IsSuccess
            ? onSuccess.Invoke(result)
            : onFail.Invoke(result);
    }

    /// <summary>Invokes an action based on the success or failed state of a result.</summary>
    /// <param name="result">The result to match success or failure.</param>
    /// <param name="onSuccess">The action to invoke if the result has a success status.</param>
    /// <param name="onFail">The action to invoke if the result has an error status.</param>
    public static void Switch(this EnrichedResult result, Action<EnrichedResult> onSuccess, Action<EnrichedResult> onFail)
    {
        _ = result.WhenNotNull(nameof(result));
        _ = onSuccess.WhenNotNull(nameof(onSuccess));
        _ = onFail.WhenNotNull(nameof(onFail));

        if (result.IsSuccess)
        {
            onSuccess.Invoke(result);
        }
        else
        {
            onFail.Invoke(result);
        }
    }

    /// <summary>Invokes an action based on the success or failed state of a result.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result to match success or failure.</param>
    /// <param name="onSuccess">The action to invoke if the result has a success status.</param>
    /// <param name="onFail">The action to invoke if the result has an error status.</param>
    public static void Switch<TResult>(this EnrichedResult<TResult> result, Action<EnrichedResult<TResult>> onSuccess,
        Action<EnrichedResult<TResult>> onFail)
    {
        _ = result.WhenNotNull(nameof(result));
        _ = onSuccess.WhenNotNull(nameof(onSuccess));
        _ = onFail.WhenNotNull(nameof(onFail));

        if (result.IsSuccess)
        {
            onSuccess.Invoke(result);
        }
        else
        {
            onFail.Invoke(result);
        }
    }
}
