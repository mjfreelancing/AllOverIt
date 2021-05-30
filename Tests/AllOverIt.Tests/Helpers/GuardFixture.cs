using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class GuardFixture : AoiFixtureBase
    {
        private class DummyClass
        {
        }

        public class WhenNotNull_Expression : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                    {
                        Guard.WhenNotNull((Expression<Func<DummyClass>>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression", errorMessage));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("subject"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("subject", errorMessage));
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

        public class WhenNotNullOrEmpty_Expression_Type : GuardFixture
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression", errorMessage));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("subject"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("subject", errorMessage));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"The argument cannot be empty (Parameter '{nameof(expected)}')");
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected, errorMessage);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentExceptionMessage(nameof(expected), errorMessage));
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

        public class WhenNotEmpty_Expression_Type : GuardFixture
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression", errorMessage));
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
                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"The argument cannot be empty (Parameter '{nameof(expected)}')");
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotEmpty(() => expected, errorMessage);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentExceptionMessage(nameof(expected), errorMessage));

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

        public class WhenNotNullOrEmpty_Expression_String : GuardFixture
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression", errorMessage));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("subject"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("subject", errorMessage));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = string.Empty;

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"The argument cannot be empty (Parameter '{nameof(expected)}')");
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = string.Empty;

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected, errorMessage);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentExceptionMessage(nameof(expected), errorMessage));
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

        public class WhenNotEmpty_Expression_String : GuardFixture
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expression", errorMessage));
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
                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"The argument cannot be empty (Parameter '{nameof(expected)}')");
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = string.Empty;

                Invoking(() =>
                    {
                        var actual = Guard.WhenNotEmpty(() => expected, errorMessage);

                        actual.Should().BeSameAs(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentExceptionMessage(nameof(expected), errorMessage));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name, errorMessage));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name));
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name, errorMessage));
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
                    .WithMessage($"The argument cannot be empty (Parameter '{name}')");
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
                    .WithMessage(GetExpectedArgumentExceptionMessage(name, errorMessage));
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
                    .WithMessage($"The argument cannot be empty (Parameter '{name}')");
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
                    .WithMessage(GetExpectedArgumentExceptionMessage(name, errorMessage));
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

        public class WhenNotNullOrEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        string dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name));
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Invoking(() =>
                    {
                        string dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name, errorMessage));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"The argument cannot be empty (Parameter '{name}')");
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name, errorMessage));
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = Create<string>();

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
                        var dummy = Create<string>();

                        Guard.WhenNotNullOrEmpty(dummy, Create<string>(), errorMessage);
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

                        Guard.WhenNotEmpty(dummy, name);
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
                    .WithMessage($"The argument cannot be empty (Parameter '{name}')");
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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage(name, errorMessage));
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
