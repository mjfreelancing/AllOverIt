namespace AllOverIt.Shouldly.Extensions;

/// <summary>
/// Fluent helpers for configuring <see cref="EquivalenceOptions"/>.
/// </summary>
public static class EquivalenceOptionsExtensions
{
    /// <summary>
    /// Includes non-public members (private, protected, and internal) in comparisons.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    /// <returns>The same options instance.</returns>
    public static EquivalenceOptions IncludeNonPublicMembers(this EquivalenceOptions options)
    {
        options.IncludeNonPublicMembers = true;
        return options;
    }

    /// <summary>
    /// Ignores missing members on the actual object.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    /// <returns>The same options instance.</returns>
    public static EquivalenceOptions ExcludeMissingMembers(this EquivalenceOptions options)
    {
        options.ExcludeMissingMembers = true;
        return options;
    }

    /// <summary>
    /// Excludes the selected member from comparison.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TMember">The member type.</typeparam>
    /// <param name="options">The options to configure.</param>
    /// <param name="memberSelector">The member selector expression.</param>
    /// <returns>The same options instance.</returns>
    public static EquivalenceOptions Excluding<TModel, TMember>(this EquivalenceOptions options, System.Linq.Expressions.Expression<Func<TModel, TMember>> memberSelector)
    {
        var body = memberSelector.Body;

        if (body is System.Linq.Expressions.UnaryExpression unary && unary.Operand is System.Linq.Expressions.MemberExpression unaryMember)
        {
            return options.ExcludeMember(unaryMember.Member.Name);
        }

        if (body is System.Linq.Expressions.MemberExpression member)
        {
            return options.ExcludeMember(member.Member.Name);
        }

        return options;
    }
}
