namespace AllOverIt.Shouldly;

/// <summary>
/// Provides options that control <c>ShouldBeEquivalentTo</c> comparison behavior.
/// </summary>
public sealed class EquivalenceOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether non-public members (private, protected, and internal) are included in comparisons.
    /// </summary>
    public bool IncludeNonPublicMembers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether missing members on the actual value are ignored.
    /// </summary>
    public bool ExcludeMissingMembers { get; set; }

    /// <summary>
    /// Gets or sets sequence ordering behavior.
    /// </summary>
    public SequenceOrdering SequenceOrdering { get; set; } = SequenceOrdering.Strict;

    /// <summary>
    /// Gets the set of excluded member paths.
    /// </summary>
    public ISet<string> ExcludedMembers { get; } = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>
    /// Gets type-based custom comparers keyed by member type.
    /// </summary>
    public IDictionary<Type, Func<object?, object?, bool>> TypeComparers { get; } =
        new Dictionary<Type, Func<object?, object?, bool>>();

    /// <summary>
    /// Gets path-based custom comparers keyed by full member path.
    /// </summary>
    public IDictionary<string, Func<object?, object?, bool>> PathComparers { get; } =
        new Dictionary<string, Func<object?, object?, bool>>(StringComparer.Ordinal);

    /// <summary>
    /// Gets the set of additional types treated as atomic leaf values.
    /// </summary>
    public ISet<Type> LeafTypes { get; } = new HashSet<Type>();

    /// <summary>
    /// Gets or sets the tolerance for <see cref="float"/> comparisons.
    /// </summary>
    public float? FloatTolerance { get; set; }

    /// <summary>
    /// Gets or sets the tolerance for <see cref="double"/> comparisons.
    /// </summary>
    public double? DoubleTolerance { get; set; }

    /// <summary>
    /// Gets or sets the tolerance for <see cref="decimal"/> comparisons.
    /// </summary>
    public decimal? DecimalTolerance { get; set; }

    /// <summary>
    /// Excludes a member path from comparison.
    /// </summary>
    /// <param name="memberPath">The member path to exclude.</param>
    /// <returns>The current options instance.</returns>
    public EquivalenceOptions ExcludeMember(string memberPath)
    {
        if (!string.IsNullOrWhiteSpace(memberPath))
        {
            ExcludedMembers.Add(memberPath.Trim());
        }

        return this;
    }

    /// <summary>
    /// Registers a custom comparer for a member type.
    /// </summary>
    /// <typeparam name="TType">The member type.</typeparam>
    /// <param name="comparer">The comparer function.</param>
    /// <returns>The current options instance.</returns>
    public EquivalenceOptions UseComparer<TType>(Func<TType?, TType?, bool> comparer)
    {
        TypeComparers[typeof(TType)] = (actual, expected) => comparer((TType?)actual, (TType?)expected);
        return this;
    }

    /// <summary>
    /// Registers a custom comparer for a specific member path.
    /// </summary>
    /// <param name="memberPath">The member path.</param>
    /// <param name="comparer">The comparer function.</param>
    /// <returns>The current options instance.</returns>
    public EquivalenceOptions UseComparer(string memberPath, Func<object?, object?, bool> comparer)
    {
        PathComparers[memberPath] = comparer;
        return this;
    }

    /// <summary>
    /// Registers a type to be treated as an atomic leaf value.
    /// </summary>
    /// <typeparam name="TType">The type to treat as a leaf.</typeparam>
    /// <returns>The current options instance.</returns>
    public EquivalenceOptions TreatAsLeaf<TType>()
    {
        return TreatAsLeaf(typeof(TType));
    }

    /// <summary>
    /// Registers a type to be treated as an atomic leaf value.
    /// </summary>
    /// <param name="type">The type to treat as a leaf.</param>
    /// <returns>The current options instance.</returns>
    public EquivalenceOptions TreatAsLeaf(Type type)
    {
        var effectiveType = Nullable.GetUnderlyingType(type) ?? type;
        LeafTypes.Add(effectiveType);
        return this;
    }
}
