using AllOverIt.Fixture.Extensions;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public partial class GuardFixture
    {
        public class WhenNotNull_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        DummyClass dummy = null;

                        Guard.WhenNotNull(dummy, name);
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
                        DummyClass dummy = null;

                        Guard.WhenNotNull(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new DummyClass();

                        Guard.WhenNotNull(dummy, Create<string>());
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

                        Guard.WhenNotNull(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new DummyClass();

                var actual = Guard.WhenNotNull(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }

        public class WhenNotNullOrEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name);
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
                        IEnumerable<DummyClass> dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name);
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
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name, errorMessage);
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
                        var dummy = new List<DummyClass> {new DummyClass()};

                        Guard.WhenNotNullOrEmpty(dummy, Create<string>());
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

                        Guard.WhenNotNullOrEmpty(dummy, Create<string>(), errorMessage);
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

        public class WhenNotEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.WhenNotEmpty(dummy, name);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

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
                var expected = new List<DummyClass>();

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
                        var dummy = new List<DummyClass> {new DummyClass()};

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
                        var dummy = new List<DummyClass> {new DummyClass()};

                        Guard.WhenNotEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new List<DummyClass> { new DummyClass() };

                var actual = Guard.WhenNotEmpty(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }
    }
}