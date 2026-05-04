using FluentAssertions;
using FluentAssertions.Specialized;
using Shouldly;

namespace AllOverIt.Fixture.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ExceptionAssertions{TException}"/>.</summary>
    public static class ExceptionAssertionsExtensions
    {
        /// <summary>Asserts the message of the thrown exception matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="errorMessage">The expected exception message.</param>
        /// <returns>The original exception.</returns>
        public static TException WithMessage<TException>(this TException exception, string errorMessage)
            where TException : Exception
        {
            exception.Message.ToUpperInvariant().ShouldBe(errorMessage.ToUpperInvariant());

            return exception;
        }

        /// <summary>Asserts the message of the thrown exception matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="errorMessage">The expected exception message.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<TException> WithMessage<TException>(this Task<TException> exceptionTask, string errorMessage)
            where TException : Exception
        {
            var exception = await exceptionTask;

            exception.Message.ToUpperInvariant().ShouldBe(errorMessage.ToUpperInvariant());

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static TException WithMessageWhenNull<TException>(this TException exception, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            exception.Message.ShouldBe(errorMessage ?? "Value cannot be null");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<TException> WithMessageWhenNull<TException>(this Task<TException> exceptionTask, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var exception = await exceptionTask;

            exception.Message.ShouldBe(errorMessage ?? "Value cannot be null");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static TException WithMessageWhenNotNull<TException>(this TException exception, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            exception.Message.ShouldBe(errorMessage ?? "Value must be null");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<TException> WithMessageWhenNotNull<TException>(this Task<TException> exceptionTask, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var exception = await exceptionTask;

            exception.Message.ShouldBe(errorMessage ?? "Value must be null");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static TException WithMessageWhenEmpty<TException>(this TException exception, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            exception.Message.ShouldBe(errorMessage ?? "Value cannot be empty");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<TException> WithMessageWhenEmpty<TException>(this Task<TException> exceptionTask, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var exception = await exceptionTask;

            exception.Message.ShouldBe(errorMessage ?? "Value cannot be empty");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static TException WithNamedMessageWhenNull<TException>(this TException exception, string name, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var message = errorMessage ?? "Value cannot be null";

            exception.Message.ShouldBe($"{message} ({name})");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<TException> WithNamedMessageWhenNull<TException>(this Task<TException> exceptionTask, string name, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var exception = await exceptionTask;
            var message = errorMessage ?? "Value cannot be null";

            exception.Message.ShouldBe($"{message} ({name})");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static TException WithNamedMessageWhenNotNull<TException>(this TException exception, string name, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var message = errorMessage ?? "Value must be null";

            exception.Message.ShouldBe($"{message} ({name})");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null (<paramref name="name"/>)' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<TException> WithNamedMessageWhenNotNull<TException>(this Task<TException> exceptionTask, string name, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var exception = await exceptionTask;
            var message = errorMessage ?? "Value must be null";

            exception.Message.ShouldBe($"{message} ({name})");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty (<paramref name="name"/>)' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static TException WithNamedMessageWhenEmpty<TException>(this TException exception, string name, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var message = errorMessage ?? "Value cannot be empty";

            exception.Message.ShouldBe($"{message} ({name})");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty (<paramref name="name"/>)' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<TException> WithNamedMessageWhenEmpty<TException>(this Task<TException> exceptionTask, string name, string? errorMessage = default)
            where TException : InvalidOperationException
        {
            var exception = await exceptionTask;
            var message = errorMessage ?? "Value cannot be empty";

            exception.Message.ShouldBe($"{message} ({name})");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentNullException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static ArgumentNullException WithNamedMessageWhenNull(this ArgumentNullException exception, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "Value cannot be null.";

            exception.Message.ShouldBe($"{message} (Parameter '{name}')");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentNullException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<ArgumentNullException> WithNamedMessageWhenNull(this Task<ArgumentNullException> exceptionTask, string name, string? errorMessage = default)
        {
            var exception = await exceptionTask;
            var message = errorMessage ?? "Value cannot be null.";

            exception.Message.ShouldBe($"{message} (Parameter '{name}')");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'The argument cannot be empty (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>The original exception.</returns>
        public static ArgumentException WithNamedMessageWhenEmpty(this ArgumentException exception, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "The argument cannot be empty.";

            exception.Message.ShouldBe($"{message} (Parameter '{name}')");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="ArgumentException"/> matches <paramref name="errorMessage"/> and
        /// contains the named parameter.</summary>
        /// <param name="exceptionTask">A task returning the thrown exception.</param>
        /// <param name="name">The name of the parameter that caused the exception to be thrown.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'The argument cannot be empty (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>A task returning the original exception.</returns>
        public static async Task<ArgumentException> WithNamedMessageWhenEmpty(this Task<ArgumentException> exceptionTask, string name, string? errorMessage = default)
        {
            var exception = await exceptionTask;
            var message = errorMessage ?? "The argument cannot be empty.";

            exception.Message.ShouldBe($"{message} (Parameter '{name}')");

            return exception;
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithMessageWhenNull(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenNotNull(
            this ExceptionAssertions<InvalidOperationException> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value must be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<InvalidOperationException>> WithMessageWhenNotNull(
            this Task<ExceptionAssertions<InvalidOperationException>> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value must be null");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static ExceptionAssertions<InvalidOperationException> WithMessageWhenEmpty(
            this ExceptionAssertions<InvalidOperationException> assertion, string? errorMessage = default)
        {
            return assertion.WithMessage(errorMessage ?? "Value cannot be empty");
        }

        /// <summary>Asserts the message of the thrown <see cref="InvalidOperationException"/> matches <paramref name="errorMessage"/>.</summary>
        /// <param name="assertion">The exception assertion.</param>
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (<paramref name="name"/>)' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (<paramref name="name"/>)' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null (<paramref name="name"/>)' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value must be null (<paramref name="name"/>)' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty (<paramref name="name"/>)' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be empty (<paramref name="name"/>)' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (Parameter '<paramref name="name"/>')' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'Value cannot be null (Parameter '<paramref name="name"/>')' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'The argument cannot be empty (Parameter '<paramref name="name"/>')' is assumed.</param>
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
        /// <param name="errorMessage">The expected exception message. If <see langword="null"/> then 'The argument cannot be empty (Parameter '<paramref name="name"/>')' is assumed.</param>
        /// <returns>The original assertion.</returns>
        public static Task<ExceptionAssertions<ArgumentException>> WithNamedMessageWhenEmpty(
            this Task<ExceptionAssertions<ArgumentException>> assertion, string name, string? errorMessage = default)
        {
            var message = errorMessage ?? "The argument cannot be empty.";

            return assertion.WithMessage($"{message} (Parameter '{name}')");
        }
    }
}