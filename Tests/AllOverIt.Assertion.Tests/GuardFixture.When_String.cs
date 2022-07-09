using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        public class WhenNotNullOrEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        string dummy = null;

                        _ = Guard.WhenNotNullOrEmpty(dummy, name);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Invoking(() =>
                    {
                        string dummy = null;

                        _ = Guard.WhenNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(expected, name);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var name = Create<string>();
                var expected = Create<string>();

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(expected, name);

                        actual.Should().Be(expected);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = Create<string>();

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>());
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        var dummy = Create<string>();

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_String()
            {
                var expected = Create<string>();

                var actual = Guard.WhenNotNullOrEmpty(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }

        public class WhenNotEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        string dummy = null;

                        _ = Guard.WhenNotEmpty(dummy, name);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        Guard.WhenNotEmpty(expected, name);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        Guard.WhenNotEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = Create<string>();

                        Guard.WhenNotEmpty(dummy, Create<string>());
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        var dummy = Create<string>();

                        Guard.WhenNotEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_String()
            {
                var expected = Create<string>();

                var actual = Guard.WhenNotEmpty(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }
    }
}