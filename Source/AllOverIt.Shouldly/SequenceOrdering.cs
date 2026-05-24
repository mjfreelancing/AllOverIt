namespace AllOverIt.Shouldly;

/// <summary>
/// Defines sequence ordering behavior when comparing enumerable values.
/// </summary>
public enum SequenceOrdering
{
    /// <summary>
    /// Compare elements in the same order.
    /// </summary>
    Strict,

    /// <summary>
    /// Compare elements regardless of order.
    /// </summary>
    AnyOrder
}
