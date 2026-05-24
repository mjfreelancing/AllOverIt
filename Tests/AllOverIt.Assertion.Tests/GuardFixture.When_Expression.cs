using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using Shouldly;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        public class WhenNotNull_Expression : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNull((Expression<Func<DummyClass>>) null, null);
                    })
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNull((Expression<Func<DummyClass>>) null, errorMessage);
                    })
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        DummyClass subject = null;

                        Guard.WhenNotNull(() => subject);
                    })
                    .WithNamedMessageWhenNull("subject");
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        DummyClass subject = null;

                        Guard.WhenNotNull(() => subject, errorMessage);
                    })
                    .WithNamedMessageWhenNull("subject", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotNull(() => (DummyClass) null);
                    })
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Should.NotThrow(() =>
                    {
                        var expected = new DummyClass();

                        var actual = Guard.WhenNotNull(() => expected);

                        actual.ShouldBeSameAs(expected);
                    });
            }
        }

        public class WhenNotNullOrEmpty_Expression_Type : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<IEnumerable<DummyClass>>>) null);
                    })
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<IEnumerable<DummyClass>>>) null, errorMessage);
                    })
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        IEnumerable<DummyClass> subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject);
                    })
                    .WithNamedMessageWhenNull("subject");
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        IEnumerable<DummyClass> subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject, errorMessage);
                    })
                    .WithNamedMessageWhenNull("subject", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var expected = new List<DummyClass> { new() };

                Should.NotThrow(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.ShouldBeSameAs(expected);
                    });
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty(() => (IEnumerable<DummyClass>) null);
                    })
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Should.NotThrow(() =>
                    {
                        var expected = new List<DummyClass> { new DummyClass() };

                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.ShouldBeSameAs(expected);
                    });
            }
        }

        public class WhenNotEmpty_Expression_Type : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<IEnumerable<DummyClass>>>) null);
                    })
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<IEnumerable<DummyClass>>>) null, errorMessage);
                    })
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                Should.NotThrow(() =>
                    {
                        IEnumerable<DummyClass> subject = null;

                        Guard.WhenNotEmpty(() => subject);
                    });
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotEmpty(() => expected);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var expected = new List<DummyClass> { new() };

                Should.NotThrow(() =>
                    {
                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.ShouldBeSameAs(expected);
                    });
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotEmpty(() => expected, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);

            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotEmpty(() => (IEnumerable<DummyClass>) null);
                    })
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Should.NotThrow(() =>
                    {
                        var expected = new List<DummyClass> { new DummyClass() };

                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.ShouldBeSameAs(expected);
                    });
            }
        }

        public class WhenNotNullOrEmpty_Expression_String : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<string>>) null);
                    })
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty((Expression<Func<string>>) null, errorMessage);
                    })
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        string subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject);
                    })
                    .WithNamedMessageWhenNull("subject");
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        string subject = null;

                        Guard.WhenNotNullOrEmpty(() => subject, errorMessage);
                    })
                    .WithNamedMessageWhenNull("subject", errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var expected = Create<string>();

                Should.NotThrow(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.ShouldBeSameAs(expected);
                    });
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(() => expected, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty(() => (string) null);
                    })
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Should.NotThrow(() =>
                    {
                        var expected = Create<string>();

                        var actual = Guard.WhenNotNullOrEmpty(() => expected);

                        actual.ShouldBe(expected);
                    });
            }
        }

        public class WhenNotEmpty_Expression_String : Assertion.GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<string>>) null);
                    })
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Throw_Message_When_Expression_Null()
            {
                var errorMessage = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotEmpty((Expression<Func<string>>) null, errorMessage);
                    })
                    .WithNamedMessageWhenNull("expression", errorMessage);
            }

            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                Should.NotThrow(() =>
                    {
                        string subject = null;

                        Guard.WhenNotEmpty(() => subject);
                    });
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotEmpty(() => expected);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotEmpty(() => expected, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected), errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Not_MemberExpression()
            {
                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotEmpty(() => (string) null);
                    })
                    .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
            }

            [Fact]
            public void Should_Return_Expression_Value()
            {
                Should.NotThrow(() =>
                    {
                        var expected = Create<string>();

                        var actual = Guard.WhenNotEmpty(() => expected);

                        actual.ShouldBe(expected);
                    });
            }
        }
    }
}
