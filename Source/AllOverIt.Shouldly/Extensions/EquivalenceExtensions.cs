using AllOverIt.Reflection;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AllOverIt.Shouldly.Extensions;

/// <summary>
/// Provides recursive member-by-name matching assertions for use with Shouldly.
///
/// FEATURES:
/// - Recursive member matching: Compares actual and expected objects member-by-member by name
/// - Asymmetric comparison: Actual must contain all members found in expected (extra members on actual are allowed)
/// - Sequence support: Handles IEnumerable collections with strict or any-order matching
/// - Dictionary support: Compares IDictionary instances by key-value pairs
/// - Custom comparers: Path-based and type-based custom comparison functions for specific members or types
/// - Numeric tolerance: Configurable tolerance for float, double, and decimal comparisons
/// - Member exclusion: Exclude specific members from comparison by name or dotted member path
/// - Non-public member support: Optionally include private, protected, and internal members in comparisons
/// - Cycle detection: Prevents infinite loops when matching objects with circular references
/// - Configurable behavior: Via EquivalenceOptions for ordering, exclusions, comparers, and tolerances
/// </summary>
public static class EquivalenceExtensions
{
    /// <summary>
    /// Asserts that the actual object matches the expected object member-by-member.
    /// </summary>
    /// <typeparam name="TActual">The type of the actual object.</typeparam>
    /// <typeparam name="TExpected">The type of the expected object.</typeparam>
    /// <param name="actualModel">The actual object to match.</param>
    /// <param name="expectedModel">The expected object to match against.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the objects do not match.</exception>
    public static void ShouldBeEquivalentTo<TActual, TExpected>(this TActual actualModel, TExpected expectedModel, string? customMessage = null)
    {
        ShouldMatchCore(actualModel, expectedModel, new EquivalenceOptions(), customMessage);
    }

    /// <summary>
    /// Asserts that the actual object matches the expected object member-by-member using configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of the actual object.</typeparam>
    /// <typeparam name="TExpected">The type of the expected object.</typeparam>
    /// <param name="actualModel">The actual object to match.</param>
    /// <param name="expectedModel">The expected object to match against.</param>
    /// <param name="configure">An action to configure matching options (exclusions, custom comparers, tolerances, etc.).</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the objects do not match.</exception>
    public static void ShouldBeEquivalentTo<TActual, TExpected>(this TActual actualModel, TExpected expectedModel, Action<EquivalenceOptions> configure, string? customMessage = null)
    {
        var options = new EquivalenceOptions();
        configure(options);

        ShouldMatchCore(actualModel, expectedModel, options, customMessage);
    }

    /// <summary>
    /// Asserts that the actual object matches the expected object member-by-member using pre-configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of the actual object.</typeparam>
    /// <typeparam name="TExpected">The type of the expected object.</typeparam>
    /// <param name="actualModel">The actual object to match.</param>
    /// <param name="expectedModel">The expected object to match against.</param>
    /// <param name="options">Pre-configured matching options.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the objects do not match.</exception>
    public static void ShouldBeEquivalentTo<TActual, TExpected>(this TActual actualModel, TExpected expectedModel, EquivalenceOptions options, string? customMessage = null)
    {
        ShouldMatchCore(actualModel, expectedModel, options, customMessage);
    }

    /// <summary>
    /// Asserts that the actual sequence matches the expected sequence, comparing elements member-by-member in order.
    /// </summary>
    /// <typeparam name="TActual">The type of elements in the actual sequence.</typeparam>
    /// <typeparam name="TExpected">The type of elements in the expected sequence.</typeparam>
    /// <param name="actualModels">The actual sequence to match.</param>
    /// <param name="expectedModels">The expected sequence to match against.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the sequences do not match.</exception>
    public static void ShouldBeEquivalentTo<TActual, TExpected>(this IEnumerable<TActual> actualModels, IEnumerable<TExpected> expectedModels, string? customMessage = null)
    {
        MatchSequences(actualModels, expectedModels, new EquivalenceOptions(), customMessage);
    }

    /// <summary>
    /// Asserts that the actual sequence matches the expected sequence using configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of elements in the actual sequence.</typeparam>
    /// <typeparam name="TExpected">The type of elements in the expected sequence.</typeparam>
    /// <param name="actualModels">The actual sequence to match.</param>
    /// <param name="expectedModels">The expected sequence to match against.</param>
    /// <param name="configure">An action to configure matching options (e.g., SequenceOrdering.AnyOrder for unordered matching).</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the sequences do not match.</exception>
    public static void ShouldBeEquivalentTo<TActual, TExpected>(this IEnumerable<TActual> actualModels, IEnumerable<TExpected> expectedModels, Action<EquivalenceOptions> configure, string? customMessage = null)
    {
        var options = new EquivalenceOptions();
        configure(options);

        MatchSequences(actualModels, expectedModels, options, customMessage);
    }

    /// <summary>
    /// Asserts that the actual sequence matches the expected sequence using pre-configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of elements in the actual sequence.</typeparam>
    /// <typeparam name="TExpected">The type of elements in the expected sequence.</typeparam>
    /// <param name="actualModels">The actual sequence to match.</param>
    /// <param name="expectedModels">The expected sequence to match against.</param>
    /// <param name="options">Pre-configured matching options.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the sequences do not match.</exception>
    public static void ShouldBeEquivalentTo<TActual, TExpected>(this IEnumerable<TActual> actualModels, IEnumerable<TExpected> expectedModels, EquivalenceOptions options, string? customMessage = null)
    {
        MatchSequences(actualModels, expectedModels, options, customMessage);
    }

    /// <summary>
    /// Asserts that the actual object does not match the expected object member-by-member.
    /// </summary>
    /// <typeparam name="TActual">The type of the actual object.</typeparam>
    /// <typeparam name="TExpected">The type of the expected object.</typeparam>
    /// <param name="actualModel">The actual object.</param>
    /// <param name="expectedModel">The expected object to compare against.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the objects match.</exception>
    public static void ShouldNotBeEquivalentTo<TActual, TExpected>(this TActual actualModel, TExpected expectedModel, string? customMessage = null)
    {
        ShouldNotMatchCore(actualModel, expectedModel, new EquivalenceOptions(), customMessage);
    }

    /// <summary>
    /// Asserts that the actual object does not match the expected object member-by-member using configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of the actual object.</typeparam>
    /// <typeparam name="TExpected">The type of the expected object.</typeparam>
    /// <param name="actualModel">The actual object.</param>
    /// <param name="expectedModel">The expected object to compare against.</param>
    /// <param name="configure">An action to configure matching options.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the objects match.</exception>
    public static void ShouldNotBeEquivalentTo<TActual, TExpected>(this TActual actualModel, TExpected expectedModel, Action<EquivalenceOptions> configure, string? customMessage = null)
    {
        var options = new EquivalenceOptions();
        configure(options);

        ShouldNotMatchCore(actualModel, expectedModel, options, customMessage);
    }

    /// <summary>
    /// Asserts that the actual object does not match the expected object member-by-member using pre-configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of the actual object.</typeparam>
    /// <typeparam name="TExpected">The type of the expected object.</typeparam>
    /// <param name="actualModel">The actual object.</param>
    /// <param name="expectedModel">The expected object to compare against.</param>
    /// <param name="options">Pre-configured matching options.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the objects match.</exception>
    public static void ShouldNotBeEquivalentTo<TActual, TExpected>(this TActual actualModel, TExpected expectedModel, EquivalenceOptions options, string? customMessage = null)
    {
        ShouldNotMatchCore(actualModel, expectedModel, options, customMessage);
    }

    /// <summary>
    /// Asserts that the actual sequence does not match the expected sequence member-by-member.
    /// </summary>
    /// <typeparam name="TActual">The type of elements in the actual sequence.</typeparam>
    /// <typeparam name="TExpected">The type of elements in the expected sequence.</typeparam>
    /// <param name="actualModels">The actual sequence.</param>
    /// <param name="expectedModels">The expected sequence to compare against.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the sequences match.</exception>
    public static void ShouldNotBeEquivalentTo<TActual, TExpected>(this IEnumerable<TActual> actualModels, IEnumerable<TExpected> expectedModels, string? customMessage = null)
    {
        ShouldNotMatchSequencesCore(actualModels, expectedModels, new EquivalenceOptions(), customMessage);
    }

    /// <summary>
    /// Asserts that the actual sequence does not match the expected sequence member-by-member using configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of elements in the actual sequence.</typeparam>
    /// <typeparam name="TExpected">The type of elements in the expected sequence.</typeparam>
    /// <param name="actualModels">The actual sequence.</param>
    /// <param name="expectedModels">The expected sequence to compare against.</param>
    /// <param name="configure">An action to configure matching options.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the sequences match.</exception>
    public static void ShouldNotBeEquivalentTo<TActual, TExpected>(this IEnumerable<TActual> actualModels, IEnumerable<TExpected> expectedModels, Action<EquivalenceOptions> configure, string? customMessage = null)
    {
        var options = new EquivalenceOptions();
        configure(options);

        ShouldNotMatchSequencesCore(actualModels, expectedModels, options, customMessage);
    }

    /// <summary>
    /// Asserts that the actual sequence does not match the expected sequence member-by-member using pre-configured options.
    /// </summary>
    /// <typeparam name="TActual">The type of elements in the actual sequence.</typeparam>
    /// <typeparam name="TExpected">The type of elements in the expected sequence.</typeparam>
    /// <param name="actualModels">The actual sequence.</param>
    /// <param name="expectedModels">The expected sequence to compare against.</param>
    /// <param name="options">Pre-configured matching options.</param>
    /// <param name="customMessage">An optional message to prefix to the assertion failure output.</param>
    /// <exception cref="ShouldAssertException">Thrown when the sequences match.</exception>
    public static void ShouldNotBeEquivalentTo<TActual, TExpected>(this IEnumerable<TActual> actualModels, IEnumerable<TExpected> expectedModels, EquivalenceOptions options, string? customMessage = null)
    {
        ShouldNotMatchSequencesCore(actualModels, expectedModels, options, customMessage);
    }

    /// <summary>
    /// Core matching logic that handles single objects and automatically delegates to sequence matching when needed.
    /// </summary>
    private static void ShouldMatchCore<TExpected>(object? actual, TExpected expected, EquivalenceOptions options, string? customMessage = null)
    {
        if (actual is IEnumerable actualEnumerable && expected is IEnumerable expectedEnumerable && actual is not string && expected is not string)
        {
            MatchSequences(actualEnumerable, expectedEnumerable, options, customMessage);
            return;
        }

        var state = new MatchState(options, customMessage);
        MatchByPublicMembers(actual, expected, "$", state);
    }

    /// <summary>
    /// Negation core: asserts the objects do NOT match. Passes when <see cref="ShouldMatchCore"/> throws,
    /// fails when it succeeds.
    /// </summary>
    private static void ShouldNotMatchCore<TExpected>(object? actual, TExpected expected, EquivalenceOptions options, string? customMessage = null)
    {
        var matched = false;

        try
        {
            ShouldMatchCore(actual, expected, options, customMessage);
            matched = true;
        }
        catch (global::Shouldly.ShouldAssertException)
        {
        }

        if (matched)
        {
            throw new global::Shouldly.ShouldAssertException(FailMessage(customMessage, "ShouldNotBeEquivalentTo failed: the actual value matched the expected but was expected to differ."));
        }
    }

    /// <summary>
    /// Negation core for sequences: asserts that sequences do NOT match. Passes when <see cref="MatchSequences{TActual,TExpected}"/> throws,
    /// fails when it succeeds.
    /// </summary>
    private static void ShouldNotMatchSequencesCore<TActual, TExpected>(IEnumerable<TActual>? actual, IEnumerable<TExpected>? expected, EquivalenceOptions options, string? customMessage)
    {
        var matched = false;

        try
        {
            MatchSequences(actual, expected, options, customMessage);
            matched = true;
        }
        catch (global::Shouldly.ShouldAssertException)
        {
        }

        if (matched)
        {
            throw new global::Shouldly.ShouldAssertException(FailMessage(customMessage, "ShouldNotBeEquivalentTo failed: the actual sequence matched the expected but was expected to differ."));
        }
    }

    /// <summary>    /// Matches two sequences element-by-element, with support for strict ordering or any-order matching.
    /// Throws if sequences differ in length or if matching fails.
    /// </summary>
    private static void MatchSequences<TActual, TExpected>(IEnumerable<TActual>? actual, IEnumerable<TExpected>? expected, EquivalenceOptions options, string? customMessage = null)
    {
        if (actual is null && expected is null)
        {
            return;
        }

        if (actual is null)
        {
            throw new global::Shouldly.ShouldAssertException(FailMessage(customMessage, "ShouldBeEquivalentTo failed at '$': actual is null but expected sequence has values."));
        }

        if (expected is null)
        {
            throw new global::Shouldly.ShouldAssertException(FailMessage(customMessage, "ShouldBeEquivalentTo failed at '$': expected sequence is null but actual has values."));
        }

        var actualValues = actual.Cast<object?>().ToArray();
        var expectedValues = expected.Cast<object?>().ToArray();

        if (actualValues.Length != expectedValues.Length)
        {
            throw new global::Shouldly.ShouldAssertException(FailMessage(customMessage, $"ShouldBeEquivalentTo failed at '$': sequence count mismatch. Expected {expectedValues.Length} but found {actualValues.Length}."));
        }

        if (options.SequenceOrdering == SequenceOrdering.AnyOrder)
        {
            MatchAnyOrder(actualValues, expectedValues, options, customMessage);
            return;
        }

        for (var index = 0; index < expectedValues.Length; index++)
        {
            var state = new MatchState(options, customMessage);
            MatchByPublicMembers(actualValues[index], expectedValues[index], $"$[{index}]", state);
        }
    }

    /// <summary>
    /// Matches sequences where element order doesn't matter. Each expected element must find a corresponding match in actual.
    /// Uses a greedy matching algorithm to pair elements.
    /// </summary>
    private static void MatchAnyOrder(object?[] actualValues, object?[] expectedValues, EquivalenceOptions options, string? customMessage)
    {
        var used = new bool[actualValues.Length];

        for (var expectedIndex = 0; expectedIndex < expectedValues.Length; expectedIndex++)
        {
            var matched = false;
            string? lastError = null;

            for (var actualIndex = 0; actualIndex < actualValues.Length; actualIndex++)
            {
                if (used[actualIndex])
                {
                    continue;
                }

                try
                {
                    var state = new MatchState(options, customMessage);
                    MatchByPublicMembers(actualValues[actualIndex], expectedValues[expectedIndex], $"$[{expectedIndex}]", state);
                    used[actualIndex] = true;
                    matched = true;
                    break;
                }
                catch (global::Shouldly.ShouldAssertException ex)
                {
                    lastError = ex.Message;
                }
            }

            if (!matched)
            {
                throw new global::Shouldly.ShouldAssertException(
                    FailMessage(customMessage, $"ShouldBeEquivalentTo failed at '$[{expectedIndex}]': no matching element found in actual sequence under AnyOrder mode." +
                    (string.IsNullOrWhiteSpace(lastError) ? string.Empty : $" Last mismatch: {lastError}")));
            }
        }
    }

    /// <summary>
    /// Adapter method that converts untyped IEnumerable sequences to typed enumerable for matching.
    /// </summary>
    private static void MatchSequences(IEnumerable actual, IEnumerable expected, EquivalenceOptions options, string? customMessage = null)
    {
        MatchSequences(actual.Cast<object?>(), expected.Cast<object?>(), options, customMessage);
    }

    /// <summary>
    /// Builds a failure message, optionally prefixing a custom user message.
    /// </summary>
    private static string FailMessage(string? customMessage, string detail) =>
        customMessage is null ? detail : $"{customMessage}\n\n{detail}";

    /// <summary>
    /// Recursively matches two objects member-by-member by name. Handles null values, custom comparers, 
    /// numeric tolerances, dictionaries, sequences, and leaf types (primitives, strings, dates, etc.).
    /// </summary>
    private static void MatchByPublicMembers(object? actual, object? expected, string path, MatchState state)
    {
        if (IsExcluded(path, state.Options))
        {
            return;
        }

        if (actual is null && expected is null)
        {
            return;
        }

        if (actual is null)
        {
            throw new global::Shouldly.ShouldAssertException(FailMessage(state.CustomMessage, $"ShouldBeEquivalentTo failed at '{path}': actual is null but expected is '{expected}'."));
        }

        if (expected is null)
        {
            throw new global::Shouldly.ShouldAssertException(FailMessage(state.CustomMessage, $"ShouldBeEquivalentTo failed at '{path}': expected is null but actual is '{actual}'."));
        }

        if (TryCompareUsingCustomRules(actual, expected, path, state.Options, out var handled, out var matchedByRule))
        {
            if (!matchedByRule)
            {
                throw new global::Shouldly.ShouldAssertException(FailMessage(state.CustomMessage, $"ShouldBeEquivalentTo failed at '{path}': custom comparer reported mismatch."));
            }

            return;
        }

        if (!IsLeafType(expected.GetType(), state.Options) && !IsLeafType(actual.GetType(), state.Options))
        {
            if (!state.Visited.Add((actual, expected)))
            {
                return;
            }
        }

        if (IsLeafType(expected.GetType(), state.Options) || IsLeafType(actual.GetType(), state.Options))
        {
            if (!Equals(actual, expected))
            {
                throw new global::Shouldly.ShouldAssertException(FailMessage(state.CustomMessage, $"ShouldBeEquivalentTo failed at '{path}': value mismatch, expected '{expected}' ({expected.GetType().FullName}) but found '{actual}' ({actual.GetType().FullName})."));
            }

            return;
        }

        if (expected is IDictionary expectedDictionary && actual is IDictionary actualDictionary)
        {
            MatchDictionaries(actualDictionary, expectedDictionary, path, state);
            return;
        }

        if (expected is IEnumerable expectedEnumerable && actual is IEnumerable actualEnumerable && expected is not string)
        {
            MatchSequences(actualEnumerable, expectedEnumerable, state.Options, state.CustomMessage);
            return;
        }

        var expectedMembers = GetReadableMembers(expected.GetType(), state.Options).ToArray();

        foreach (var expectedMember in expectedMembers)
        {
            var memberPath = $"{path}.{expectedMember.Name}";

            if (IsExcluded(memberPath, state.Options))
            {
                continue;
            }

            var actualMember = GetReadableMember(actual.GetType(), expectedMember.Name, state.Options);

            if (actualMember is null)
            {
                if (state.Options.ExcludeMissingMembers)
                {
                    continue;
                }

                throw new global::Shouldly.ShouldAssertException(FailMessage(state.CustomMessage, $"ShouldBeEquivalentTo failed at '{memberPath}': missing member '{expectedMember.Name}' on actual type '{actual.GetType().FullName}'."));
            }

            var expectedValue = expectedMember.GetValue(expected);
            var actualValue = actualMember.GetValue(actual);

            MatchByPublicMembers(actualValue, expectedValue, memberPath, state);
        }
    }

    /// <summary>
    /// Matches two dictionaries by comparing keys and their associated values recursively.
    /// </summary>
    private static void MatchDictionaries(IDictionary actual, IDictionary expected, string path, MatchState state)
    {
        foreach (DictionaryEntry expectedEntry in expected)
        {
            var keyPath = $"{path}[{expectedEntry.Key}]";

            if (!actual.Contains(expectedEntry.Key))
            {
                if (state.Options.ExcludeMissingMembers)
                {
                    continue;
                }

                throw new global::Shouldly.ShouldAssertException(FailMessage(state.CustomMessage, $"ShouldBeEquivalentTo failed at '{keyPath}': missing dictionary key '{expectedEntry.Key}'."));
            }

            var actualValue = actual[expectedEntry.Key];
            MatchByPublicMembers(actualValue, expectedEntry.Value, keyPath, state);
        }
    }

    /// <summary>
    /// Attempts to apply custom comparison rules in order: path-specific comparers, type comparers, then numeric tolerances.
    /// Returns true if a rule was found, false otherwise.
    /// </summary>
    private static bool TryCompareUsingCustomRules(object actual, object expected, string path, EquivalenceOptions options, out bool handled, out bool matched)
    {
        if (options.PathComparers.TryGetValue(path, out var pathComparer))
        {
            handled = true;
            matched = pathComparer(actual, expected);
            return true;
        }

        if (options.TypeComparers.TryGetValue(expected.GetType(), out var typeComparer))
        {
            handled = true;
            matched = typeComparer(actual, expected);
            return true;
        }

        if (TryCompareWithTolerance(actual, expected, options, out matched))
        {
            handled = true;
            return true;
        }

        handled = false;
        matched = false;
        return false;
    }

    /// <summary>
    /// Compares floating-point and decimal types using configured tolerance values.
    /// Returns true if a tolerance check was applicable, false otherwise.
    /// </summary>
    private static bool TryCompareWithTolerance(object actual, object expected, EquivalenceOptions options, out bool matched)
    {
        if (expected is float expectedFloat && options.FloatTolerance.HasValue)
        {
            var actualFloat = Convert.ToSingle(actual);
            matched = Math.Abs(actualFloat - expectedFloat) <= options.FloatTolerance.Value;
            return true;
        }

        if (expected is double expectedDouble && options.DoubleTolerance.HasValue)
        {
            var actualDouble = Convert.ToDouble(actual);
            matched = Math.Abs(actualDouble - expectedDouble) <= options.DoubleTolerance.Value;
            return true;
        }

        if (expected is decimal expectedDecimal && options.DecimalTolerance.HasValue)
        {
            var actualDecimal = Convert.ToDecimal(actual);
            matched = Math.Abs(actualDecimal - expectedDecimal) <= options.DecimalTolerance.Value;
            return true;
        }

        matched = false;
        return false;
    }

    /// <summary>
    /// Retrieves all readable members (properties and optionally fields) from a type, respecting visibility options.
    /// Members are wrapped in ReadableMember instances for uniform access.
    /// </summary>
    private static IEnumerable<ReadableMember> GetReadableMembers(Type type, EquivalenceOptions options)
    {
        var members = new List<ReadableMember>();

        var propertyBindings = options.IncludeNonPublicMembers
            ? BindingOptions.Instance | BindingOptions.AllAccessor | BindingOptions.AllVisibility | BindingOptions.GetMethod
            : BindingOptions.Instance | BindingOptions.AllAccessor | BindingOptions.Public | BindingOptions.GetMethod;

        var properties = ReflectionCache.GetPropertyInfo(type, propertyBindings, false) ?? [];

        foreach (var property in properties)
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            if (!members.Any(item => item.Name == property.Name))
            {
                members.Add(new ReadableMember(property.Name, property.PropertyType, instance => property.GetValue(instance), ReadableMemberKind.Property));
            }
        }

        if (options.IncludeNonPublicMembers)
        {
            var fields = ReflectionCache.GetFieldInfo(type, BindingOptions.Instance | BindingOptions.AllAccessor | BindingOptions.AllVisibility, false) ?? [];

            foreach (var field in fields)
            {
                if (!members.Any(item => item.Name == field.Name))
                {
                    members.Add(new ReadableMember(field.Name, field.FieldType, instance => field.GetValue(instance), ReadableMemberKind.Field));
                }
            }
        }

        return members;
    }

    /// <summary>
    /// Retrieves a single readable member by name from a type, or null if not found.
    /// Falls back to public static fields (including consts) when no instance member with the given name exists.
    /// </summary>
    private static ReadableMember? GetReadableMember(Type type, string memberName, EquivalenceOptions options)
    {
        var instanceMember = GetReadableMembers(type, options)
            .FirstOrDefault(member => member.Name == memberName);

        if (instanceMember is not null)
        {
            return instanceMember;
        }

        // Also resolve public static fields so that 'const' and 'static readonly' members are
        // reachable when the expected shape (e.g. an anonymous type) names them explicitly.
        var staticField = type.GetField(memberName, BindingFlags.Public | BindingFlags.Static);

        if (staticField is not null)
        {
            return new ReadableMember(staticField.Name, staticField.FieldType, _ => staticField.GetValue(null), ReadableMemberKind.Field);
        }

        return null;
    }

    /// <summary>
    /// Checks if a member path is excluded from comparison using full path, simplified path, or simple member name matching.
    /// </summary>
    private static bool IsExcluded(string path, EquivalenceOptions options)
    {
        if (options.ExcludedMembers.Contains(path))
        {
            return true;
        }

        var simplifiedPath = path.StartsWith("$.", StringComparison.Ordinal)
            ? path[2..]
            : path;

        if (options.ExcludedMembers.Contains(simplifiedPath))
        {
            return true;
        }

        var lastSegmentIndex = simplifiedPath.LastIndexOf('.');

        if (lastSegmentIndex >= 0)
        {
            var segment = simplifiedPath[(lastSegmentIndex + 1)..];
            return options.ExcludedMembers.Contains(segment);
        }

        return false;
    }

    /// <summary>
    /// Determines if a type is a "leaf" type (non-complex) that should be compared by value rather than recursively.
    /// Includes primitives, enums, strings, DateTime, Guid, and other built-in value types.
    /// </summary>
    private static bool IsLeafType(Type type, EquivalenceOptions options)
    {
        var effectiveType = Nullable.GetUnderlyingType(type) ?? type;

        if (effectiveType.IsPrimitive || effectiveType.IsEnum)
        {
            return true;
        }

        if (options.LeafTypes.Contains(effectiveType))
        {
            return true;
        }

        return effectiveType == typeof(string)
            || effectiveType == typeof(decimal)
            || effectiveType == typeof(DateTime)
            || effectiveType == typeof(DateTimeOffset)
            || effectiveType == typeof(DateOnly)
            || effectiveType == typeof(TimeOnly)
            || effectiveType == typeof(Guid)
            || effectiveType == typeof(TimeSpan)
            || effectiveType == typeof(Uri)
            || effectiveType == typeof(Version)
            || typeof(Type).IsAssignableFrom(effectiveType)
            || typeof(MemberInfo).IsAssignableFrom(effectiveType)
            || effectiveType == typeof(Half);
    }

    /// <summary>
    /// Equality comparer for (object, object) tuples that uses reference equality for cycle detection.
    /// This allows accurate tracking of visited object pairs to prevent infinite loops in circular reference scenarios.
    /// </summary>
    private sealed class ReferencePairComparer : IEqualityComparer<(object Actual, object Expected)>
    {
        internal static readonly ReferencePairComparer Instance = new();

        public bool Equals((object Actual, object Expected) x, (object Actual, object Expected) y)
        {
            return ReferenceEquals(x.Actual, y.Actual) && ReferenceEquals(x.Expected, y.Expected);
        }

        public int GetHashCode((object Actual, object Expected) obj)
        {
            return HashCode.Combine(RuntimeHelpers.GetHashCode(obj.Actual), RuntimeHelpers.GetHashCode(obj.Expected));
        }
    }

    /// <summary>
    /// Encapsulates matching state during recursive member comparison, including options and visited object pairs for cycle detection.
    /// </summary>
    private sealed class MatchState
    {
        internal MatchState(EquivalenceOptions options, string? customMessage = null)
        {
            Options = options;
            CustomMessage = customMessage;
            Visited = new HashSet<(object Actual, object Expected)>(ReferencePairComparer.Instance);
        }

        internal EquivalenceOptions Options { get; }

        internal string? CustomMessage { get; }

        internal ISet<(object Actual, object Expected)> Visited { get; }
    }

    /// <summary>
    /// Discriminates between property and field members for type information purposes.
    /// </summary>
    private enum ReadableMemberKind
    {
        Property,
        Field
    }

    /// <summary>
    /// Provides a uniform interface for accessing properties or fields. Encapsulates member name, type, value accessor, and kind.
    /// </summary>
    private sealed record ReadableMember(string Name, Type MemberType, Func<object, object?> GetValue, ReadableMemberKind MemberKind);
}
