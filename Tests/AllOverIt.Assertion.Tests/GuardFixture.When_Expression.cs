using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        public class WhenNotNull_Expression : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotNull((Expression<Func<DummyClass>>)null, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        Guard.WhenNotNull((Expression<Func<DummyClass>>)null, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        DummyClass subject = null;

                        Guard.WhenNotNull(() => subject);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("subject");
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        DummyClass subject = null;

                        Guard.WhenNotNull(() => subject, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("subject", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotNull(() => (DummyClass)null);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Invoking(() =>
                    {
                        var expected = new DummyClass();

                        var actual = Guard.WhenNotNull(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class WhenNotNullOrEmpty_Expression_Type : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<IEnumerable<DummyClass>>>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<IEnumerable<DummyClass>>>)null, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        IEnumerable<DummyClass> subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("subject");
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("subject", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var expected = new List<DummyClass> {new()};

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(() => (IEnumerable<DummyClass>)null);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Invoking(() =>
                    {
                        var expected = new List<DummyClass> {new DummyClass()};

                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class WhenNotEmpty_Expression_Type : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<IEnumerable<DummyClass>>>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<IEnumerable<DummyClass>>>)null, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        IEnumerable<DummyClass> subject = null;

                        Guard.WhenNotEmpty(() => subject);
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
                        _ = Guard.WhenNotEmpty(() => expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var expected = new List<DummyClass> {new()};

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        _ = Guard.WhenNotEmpty(() => expected, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);

            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotEmpty(() => (IEnumerable<DummyClass>)null);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Invoking(() =>
                    {
                        var expected = new List<DummyClass> {new DummyClass()};

                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class WhenNotNullOrEmpty_Expression_String : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<string>>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<string>>)null, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        string subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("subject");
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        string subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("subject", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = string.Empty;

                Invoking(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var expected = Create<string>();

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = string.Empty;

                Invoking(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(() => (string)null);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Invoking(() =>
                    {
                        var expected = Create<string>();

                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.Should().Be(expected);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class WhenNotEmpty_Expression_String : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<string>>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<string>>)null, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        string subject = null;

                        Guard.WhenNotEmpty(() => subject);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = string.Empty;

                Invoking(() =>
                    {
                        _ = Guard.WhenNotEmpty(() => expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = string.Empty;

                Invoking(() =>
                    {
                        _ = Guard.WhenNotEmpty(() => expected, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotEmpty(() => (string)null);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Invoking(() =>
                    {
                        var expected = Create<string>();

                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.Should().Be(expected);
                    })
                    .Should()
                    .NotThrow();
            }
        }
    }
}