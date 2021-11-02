using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        public class InvalidWhenNull_Type : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        DummyClass dummy = null;

                        Guard.InvalidWhenNull(dummy);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenNull();
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        DummyClass dummy = null;

                        Guard.InvalidWhenNull(dummy, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenNull(errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new DummyClass();

                        Guard.InvalidWhenNull(dummy, Create<string>());
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
                        var dummy = new DummyClass();

                        Guard.InvalidWhenNull(dummy, errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new DummyClass();

                var actual = Guard.InvalidWhenNull(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }

        public class InvalidWhenNullOrEmpty_Type : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.InvalidWhenNullOrEmpty(dummy);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenNull();
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.InvalidWhenNullOrEmpty(dummy, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenNull(errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.InvalidWhenNullOrEmpty(expected);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenEmpty();
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.InvalidWhenNullOrEmpty(expected, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenEmpty(errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> {new DummyClass()};

                        Guard.InvalidWhenNullOrEmpty(dummy, Create<string>());
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
                        var dummy = new List<DummyClass> {new DummyClass()};

                        Guard.InvalidWhenNullOrEmpty(dummy, errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new List<DummyClass> { new DummyClass() };

                var actual = Guard.WhenNotNullOrEmpty(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }

        public class InvalidWhenEmpty_Type : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.InvalidWhenEmpty(dummy);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.InvalidWhenEmpty(expected);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenEmpty();
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.InvalidWhenEmpty(expected, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessageWhenEmpty(errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> {new DummyClass()};

                        Guard.InvalidWhenEmpty(dummy, Create<string>());
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
                        var dummy = new List<DummyClass> {new DummyClass()};

                        Guard.InvalidWhenEmpty(dummy, errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new List<DummyClass> { new DummyClass() };

                var actual = Guard.InvalidWhenEmpty(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }
    }
}