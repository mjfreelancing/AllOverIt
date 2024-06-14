#nullable enable

using AllOverIt.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Diagnostics;
using System.Reflection;

namespace AllOverIt.Fixture.Assertions
{
    /// <summary>Provides a number of assertions that can be applied against a property's nullability info.</summary>
    [DebuggerNonUserCode]
    public class NullabilityInfoAssertions
    {
        private readonly NullabilityInfo _subject;

        internal NullabilityInfoAssertions(NullabilityInfo subject)
        {
            _subject = subject;
        }

        /// <summary>Asserts the subject's nullability is <c>Nullable</c>.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public NullabilityInfoAssertions IsNullable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                var state = _subject.ReadState != NullabilityState.Unknown
                   ? _subject.ReadState
                   : _subject.WriteState;

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(state == NullabilityState.Nullable)
                    .FailWith("Expected {context} to be {0}{reason}, but it is {1}.", GetNullableString(false), GetNullableString(true));
            }

            return this;
        }

        /// <summary>Asserts the subject's nullability is <c>NotNull</c>.</summary>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public NullabilityInfoAssertions IsNotNullable(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                var state = _subject.ReadState != NullabilityState.Unknown
                   ? _subject.ReadState
                   : _subject.WriteState;

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(state == NullabilityState.NotNull)
                    .FailWith("Expected {context} to be {0}{reason}, but it is {1}.", GetNullableString(true), GetNullableString(false));
            }

            return this;
        }

        /// <summary>Asserts the subject's nullability for the element type of a collection.</summary>
        /// <typeparam name="TElementType">The expected element type of the collection.</typeparam>
        /// <param name="nullabilityAssertions">The assertions instance for the subject's element type.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public NullabilityInfoAssertions IsCollectionOf<TElementType>(Action<NullabilityInfoAssertions> nullabilityAssertions, string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                var elementType = typeof(TElementType);

                var success = _subject.ElementType?.Type == elementType;

                if (success)
                {
                    var assertions = new NullabilityInfoAssertions(_subject.ElementType!);

                    nullabilityAssertions.Invoke(assertions);
                }
                else
                {
                    Execute.Assertion
                        .BecauseOf(because, becauseArgs)
                        .FailWith($"Expected {{context}} to be a collection of type {elementType.GetFriendlyName()}{{reason}}, but it isn't.");
                }
            }

            return this;
        }

        /// <summary>Asserts the subject's nullability is for a specified type.</summary>
        /// <typeparam name="TType">The subject's type.</typeparam>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public NullabilityInfoAssertions IsOfType<TType>(string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                var expectedType = typeof(TType);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(_subject.Type == expectedType)
                    .FailWith(
                        "Expected {context} to have type {0}{reason}, but found {1}.",
                        expectedType.GetFriendlyName(true),
                        _subject.Type.GetFriendlyName(true));
            }

            return this;
        }

        /// <summary>Gets access to the <c>NullabilityInfo</c> assertions instance for the subject's generic type at a specified index.</summary>
        /// <param name="index">The index of the generic type argument to be asserted.</param>
        /// <param name="nullabilityAssertions">The assertions instance to use for validating the generic type argument.</param>
        /// <param name="because">
        /// A formatted phrase compatible with <see cref="string.Format(string,object[])"/> explaining why the condition should
        /// be satisfied. If the phrase does not start with the word <em>because</em>, it is prepended to the message.
        /// <para>
        /// If the format of <paramref name="because"/> or <paramref name="becauseArgs"/> is not compatible with
        /// <see cref="string.Format(string,object[])"/>, then a warning message is returned instead.
        /// </para>
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])"/> compatible placeholders.
        /// </param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        [CustomAssertion]
        public NullabilityInfoAssertions ForGenericArg(int index, Action<NullabilityInfoAssertions> nullabilityAssertions, string because = "", params object[] becauseArgs)
        {
            if (SubjectIsNotNull())
            {
                var success = index < _subject.GenericTypeArguments.Length;

                if (success)
                {
                    var genericNullabilityInfo = _subject.GenericTypeArguments[index];

                    using var _ = new AssertionScope($"{GetCurrentScopeGenericArgValue()}<{index}>");

                    var assertions = new NullabilityInfoAssertions(genericNullabilityInfo);

                    nullabilityAssertions.Invoke(assertions);
                }
                else
                {
                    Execute.Assertion
                           .BecauseOf(because, becauseArgs)
                           .FailWith($"Expected {{context}} to have a generic type argument at index {index}{{reason}}, but the index is out of range.");
                }
            }

            return this;
        }

        private bool SubjectIsNotNull()
        {
            return Execute.Assertion
               .ForCondition(_subject is not null)
               .FailWith("Cannot validate {context:property} when its <NullabilityInfo> is <null>.");
        }

        private static string GetCurrentScopeGenericArgValue()
        {
            return AssertionScope.Current.Context?.Value ?? "generic arg";
        }

        private static string GetNullableString(bool inverted)
        {
            return inverted ? "Not Nullable" : "Nullable";
        }
    }
}
