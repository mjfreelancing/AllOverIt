using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using FluentAssertions.Specialized;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Threading.Tasks;
using Xunit;
using ExceptionAssertionsExtensions = AllOverIt.Fixture.Extensions.ExceptionAssertionsExtensions;

namespace AllOverIt.Fixture.Tests.Extensions
{
    public class ExceptionAssertionsExtensionsFixture : FixtureBase
    {
        private const string ValueCannotBeNull = "Value cannot be null";
        private const string ValueMustBeNull = "Value must be null";
        private const string ValueCannotBeEmpty = "Value cannot be empty";
        private const string ArgumentCannotBeEmpty = "The argument cannot be empty.";

        public class WithMessageWhenNull_InvalidOperationException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException(message);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessageWhenNull(message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                Invoking(() =>
                {
                    throw new InvalidOperationException(ValueCannotBeNull);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessageWhenNull();
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException(message);
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessageWhenNull(message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException(ValueCannotBeNull);
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessageWhenNull();
            }
        }

        public class WithMessageWhenNotNull_InvalidOperationException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException(message);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessageWhenNotNull(message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                Invoking(() =>
                {
                    throw new InvalidOperationException(ValueMustBeNull);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessageWhenNotNull();
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException(message);
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessageWhenNotNull(message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException(ValueMustBeNull);
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessageWhenNotNull();
            }
        }

        public class WithMessageWhenEmpty_InvalidOperationException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException(message);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessageWhenEmpty(message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                Invoking(() =>
                {
                    throw new InvalidOperationException(ValueCannotBeEmpty);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessageWhenEmpty();
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException(message);
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessageWhenEmpty(message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException(ValueCannotBeEmpty);
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessageWhenEmpty();
            }
        }

        public class WithNamedMessageWhenNull_InvalidOperationException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var name = Create<string>();
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException($"{message} ({name})");
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithNamedMessageWhenNull(name, message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                var name = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException($"{ValueCannotBeNull} ({name})");
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var name = Create<string>();
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException($"{message} ({name})");
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithNamedMessageWhenNull(name, message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                var name = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException($"{ValueCannotBeNull} ({name})");
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithNamedMessageWhenNull(name);
            }
        }

        public class WithNamedMessageWhenNotNull_InvalidOperationException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var name = Create<string>();
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException($"{message} ({name})");
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithNamedMessageWhenNotNull(name, message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                var name = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException($"{ValueMustBeNull} ({name})");
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithNamedMessageWhenNotNull(name);
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var name = Create<string>();
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException($"{message} ({name})");
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithNamedMessageWhenNotNull(name, message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                var name = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException($"{ValueMustBeNull} ({name})");
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithNamedMessageWhenNotNull(name);
            }
        }

        public class WithNamedMessageWhenEmpty_InvalidOperationException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var name = Create<string>();
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException($"{message} ({name})");
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithNamedMessageWhenEmpty(name, message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                var name = Create<string>();

                Invoking(() =>
                {
                    throw new InvalidOperationException($"{ValueCannotBeEmpty} ({name})");
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var name = Create<string>();
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException($"{message} ({name})");
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithNamedMessageWhenEmpty(name, message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                var name = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new InvalidOperationException($"{ValueCannotBeEmpty} ({name})");
                })
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithNamedMessageWhenEmpty(name);
            }
        }

        public class WithNamedMessageWhenNull_ArgumentNullException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var name = Create<string>();
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new ArgumentNullException(name, message);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull(name, message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                var name = Create<string>();

                Invoking(() =>
                {
                    throw new ArgumentNullException(name);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var name = Create<string>();
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new ArgumentNullException(name, message);
                })
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithNamedMessageWhenNull(name, message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                var name = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new ArgumentNullException(name);
                })
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithNamedMessageWhenNull(name);
            }
        }

        public class WithNamedMessageWhenEmpty_ArgumentException : ExceptionAssertionsExtensionsFixture
        {
            [Fact]
            public void Should_Assert_Expected_Message()
            {
                var name = Create<string>();
                var message = Create<string>();

                Invoking(() =>
                {
                    throw new ArgumentException(message, name);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty(name, message);
            }

            [Fact]
            public void Should_Assert_Default_Message()
            {
                var name = Create<string>();

                Invoking(() =>
                {
                    throw new ArgumentException($"{ArgumentCannotBeEmpty} (Parameter '{name}')");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public async Task Should_Assert_Expected_Message_Async()
            {
                var name = Create<string>();
                var message = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new ArgumentException(message, name);
                })
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithNamedMessageWhenEmpty(name, message);
            }

            [Fact]
            public async Task Should_Assert_Default_Message_Async()
            {
                var name = Create<string>();

                await Invoking(async () =>
                {
                    await Task.Yield();

                    throw new ArgumentException($"{ArgumentCannotBeEmpty} (Parameter '{name}')");
                })
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithNamedMessageWhenEmpty(name);
            }
        }
    }
}