#nullable enable

using AllOverIt.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Diagnostics;

namespace AllOverIt.Fixture.Assertions
{
    /// <summary>Provides a wrapper around the specified <typeparamref name="TType"/> so its' properties can be asserted.</summary>
    /// <typeparam name="TType">The type containing the properties to be asserted.</typeparam>
    [DebuggerNonUserCode]
    public sealed class ClassPropertiesAssertions
    {
        private readonly ClassPropertiesBase _classProperties;

        /// <summary>Constructor.</summary>
        /// <param name="subject">A wrapper for the type containing the properties to be asserted.</param>
        internal ClassPropertiesAssertions(ClassPropertiesBase subject)
        {
            _classProperties = subject;
        }

        /// <summary>Asserts that the <typeparamref name="TType"/> contains property names matching the specified property names.</summary>
        /// <param name="propertyNames">The expected property names.</param>
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
        public AndConstraint<ClassPropertiesAssertions> MatchNames(string[] propertyNames, string because = "", params object[] becauseArgs)
        {
            using var _ = new AssertionScope();

            var success = Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(_classProperties.Properties.Length > 0)
                .FailWith("Expected to validate at least one property on type {0}{reason}, but found none.", _classProperties.ClassType.GetFriendlyName());

            if (success)
            {
                // Not using BeEquivalentTo() here since we want to customize the message rather than it pick up the expression.
                var actual = _classProperties.Properties
                    .Select(propertyInfo => propertyInfo.Name)
#if NET8_0_OR_GREATER
                    .Order();
#else
                    .OrderBy(item => item);
#endif

                var expected = propertyNames
#if NET8_0_OR_GREATER
                    .Order();
#else
                    .OrderBy(item => item);
#endif

                static string GetQuoted(IEnumerable<string> values) => "{{" + string.Join(",", values.Select(item => $"\"{item}\"")) + "}}";

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(actual.SequenceEqual(expected))
                    .FailWith($"Expected properties on type {_classProperties.ClassType.GetFriendlyName()} to be named {GetQuoted(expected)}{{reason}}, but found {GetQuoted(actual)}.");
            }

            return new AndConstraint<ClassPropertiesAssertions>(this);
        }

        /// <summary>Invokes an action to execute one or more property assertions.</summary>
        /// <param name="propertyAssertion">The action to execute.</param>
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
        public AndConstraint<ClassPropertiesAssertions> BeDefinedAs(Action<PropertyInfoAssertions> propertyAssertion, string because = "", params object[] becauseArgs)
        {
            using var _ = new AssertionScope();

            var success = Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(_classProperties.Properties.Length > 0)
                .FailWith("Expected to validate at least one property on type {0}{reason}, but found none.", _classProperties.ClassType.GetFriendlyName());

            if (success)
            {
                foreach (var propInfo in _classProperties.Properties)
                {
                    using var propertyScope = new AssertionScope(propInfo.Name);

                    var assertions = new PropertyInfoAssertions(propInfo);

                    propertyAssertion.Invoke(assertions);
                }
            }

            return new AndConstraint<ClassPropertiesAssertions>(this);
        }
    }
}
