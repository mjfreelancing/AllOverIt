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
    public sealed class ClassPropertiesAssertions<TType>
    {
        private readonly ClassProperties<TType> _classProperties;

        /// <summary>Constructor.</summary>
        /// <param name="subject">A wrapper for the type containing the properties to be asserted.</param>
        internal ClassPropertiesAssertions(ClassProperties<TType> subject)
        {
            _classProperties = subject;
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
        public AndConstraint<ClassPropertiesAssertions<TType>> BeDefinedAs(Action<PropertyInfoAssertions> propertyAssertion, string because = "", params object[] becauseArgs)
        {
            using var _ = new AssertionScope();

            var success = Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(_classProperties.Properties.Length > 0)
                .FailWith("Expected to validate at least one property on type {0}{reason}, but found none.", typeof(TType).GetFriendlyName());

            if (success)
            {
                foreach (var propInfo in _classProperties.Properties)
                {
                    using var propertyScope = new AssertionScope(propInfo.Name);

                    var assertions = new PropertyInfoAssertions(propInfo);

                    propertyAssertion.Invoke(assertions);
                }
            }

            return new AndConstraint<ClassPropertiesAssertions<TType>>(this);
        }
    }
}
