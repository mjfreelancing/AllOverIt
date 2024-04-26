using FluentAssertions;
using FluentAssertions.Specialized;

namespace AllOverIt.Fixture.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ExceptionAssertions{TException}"/>.</summary>
    public static class ExceptionAssertionsExtensions
    {
        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithMessageWhenNull(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value must be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenNotNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value must be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value must be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithMessageWhenNotNull(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value must be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be empty' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenEmpty(
            this ExceptionAssertions<InvalidOperationException> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be empty");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be empty' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithMessageWhenEmpty(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be empty");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithNamedMessageWhenNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be null";

            return assertion.WithMessage($"{message} ({name})");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithNamedMessageWhenNull(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be null";

            return assertion.WithMessage($"{message} ({name})");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value must be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithNamedMessageWhenNotNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value must be null";

            return assertion.WithMessage($"{message} ({name})");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value must be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithNamedMessageWhenNotNull(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value must be null";

            return assertion.WithMessage($"{message} ({name})");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be empty (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithNamedMessageWhenEmpty(
            this ExceptionAssertions<InvalidOperationException> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be empty";

            return assertion.WithMessage($"{message} ({name})");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be empty (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithNamedMessageWhenEmpty(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be empty";

            return assertion.WithMessage($"{message} ({name})");
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentNullException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be null (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<ArgumentNullException> WithNamedMessageWhenNull(
            this ExceptionAssertions<ArgumentNullException> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be null.";

            return assertion.WithMessage($"{message} (Parameter '{name}')");
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentNullException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'Value cannot be null (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<ArgumentNullException>> WithNamedMessageWhenNull(
            this Task<ExceptionAssertions<ArgumentNullException>> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be null.";

            return assertion.WithMessage($"{message} (Parameter '{name}')");
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'The argument cannot be empty (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<ArgumentException> WithNamedMessageWhenEmpty(
            this ExceptionAssertions<ArgumentException> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "The argument cannot be empty.";

            return assertion.WithMessage($"{message} (Parameter '{name}')");
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If null then 'The argument cannot be empty (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<ArgumentException>> WithNamedMessageWhenEmpty(
            this Task<ExceptionAssertions<ArgumentException>> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "The argument cannot be empty.";

            return assertion.WithMessage($"{message} (Parameter '{name}')");
        }
    }
}