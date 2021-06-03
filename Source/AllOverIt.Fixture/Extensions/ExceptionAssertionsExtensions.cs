using FluentAssertions.Specialized;
using System;

namespace AllOverIt.Fixture.Extensions
{
    public static class ExceptionAssertionsExtensions
    {
        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be null");
        }

        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenEmpty(
            this ExceptionAssertions<InvalidOperationException> assertion, string errorMessage = default)
        {
            return WithMessageWhenNull(assertion, errorMessage ?? "Value cannot be empty");
        }

        public static ExceptionAssertions<InvalidOperationException> WithNamedMessageWhenNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string name, string errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be null";
            return WithMessageWhenNull(assertion, $"{message} ({name})");
        }

        public static ExceptionAssertions<InvalidOperationException> WithNamedMessageWhenEmpty(
            this ExceptionAssertions<InvalidOperationException> assertion, string name, string errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be empty";
            return WithMessageWhenNull(assertion, $"{message} ({name})");
        }

        public static ExceptionAssertions<ArgumentNullException> WithNamedMessageWhenNull(
            this ExceptionAssertions<ArgumentNullException> assertion, string name, string errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be null.";
            return assertion.WithMessage($"{message} (Parameter '{name}')");
        }

        public static ExceptionAssertions<ArgumentException> WithNamedMessageWhenEmpty(
            this ExceptionAssertions<ArgumentException> assertion, string name, string errorMessage = default)
        {
            var message = errorMessage ?? "The argument cannot be empty";
            return assertion.WithMessage($"{message} (Parameter '{name}')");
        }
    }
}