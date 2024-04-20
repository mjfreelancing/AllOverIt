#nullable enable

using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.Result;

/// <summary>Enhances <see cref="EnrichedResult"/> by storing a result value.</summary>
/// <typeparam name="TResult">The result type.</typeparam>
public class EnrichedResult<TResult> : EnrichedResult
{
    private TResult? _value;

    /// <summary>An optional result value. This property will throw an <see cref="InvalidOperationException"/>
    /// if the result is not in a successful state.</summary>
    public TResult? Value
    {
        get => GetValue();
        set => _value = value;
    }

    internal EnrichedResult()
        : base(true, null)
    {
    }

    internal EnrichedResult(TResult value)
        : base(true, null)
    {
        _value = value;
    }

    internal EnrichedResult(EnrichedError? error)
        : base(false, error)
    {
    }

    private TResult? GetValue()
    {
        Throw<InvalidOperationException>.When(IsError, $"The result has no value. More detail can be found on the {nameof(Fail)} property.");

        return _value;
    }
}
