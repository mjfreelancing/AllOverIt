#nullable enable

using AllOverIt.Assertion;

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

    /// <summary>Implicit operator to convert an <see cref="EnrichedResult{TResult}"/> to its underlying result type.</summary>
    /// <param name="result">The result value to implicitly convert to.</param>
    public static implicit operator TResult?(EnrichedResult<TResult> result) => result.Value;

    /// <summary>Explicit operator to convert a result value to its <see cref="EnrichedResult{TResult}"/> equivalent.</summary>
    /// <param name="result">The result value to explicitly convert to.</param>
    public static explicit operator EnrichedResult<TResult>(TResult result) => new(result);

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
        Throw<InvalidOperationException>.When(IsFail, $"The result has no value. More detail can be found on the {nameof(Fail)} property.");

        return _value;
    }
}
