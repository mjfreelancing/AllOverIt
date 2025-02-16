using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        public class CheckNotNull_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                DummyClass dummy = null;

                Invoking(() =>
                    {
                        Guard.CheckNotNull(dummy);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        DummyClass dummy = null;

                        Guard.CheckNotNull(dummy, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
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

                        Guard.CheckNotNull(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new DummyClass();

                        Guard.CheckNotNull(dummy, Create<string>());
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class CheckIsNull_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var dummy = Create<DummyClass>();

                Invoking(() =>
                    {
                        Guard.CheckIsNull(dummy);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNotNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Not_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        var dummy = Create<DummyClass>();

                        Guard.CheckIsNull(dummy, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNotNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Not_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Invoking(() =>
                    {
                        var dummy = Create<DummyClass>();

                        Guard.CheckIsNull(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNotNull(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        Guard.CheckIsNull((DummyClass)null, Create<string>());
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class CheckNotNullOrEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                IEnumerable<DummyClass> dummy = null;

                Invoking(() =>
                    {
                        Guard.CheckNotNullOrEmpty(dummy);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.CheckNotNullOrEmpty(dummy, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
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

                        Guard.CheckNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.CheckNotNullOrEmpty(expected, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
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
                        Guard.CheckNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>());
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
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class CheckNotEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.CheckNotEmpty(dummy, name);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.CheckNotEmpty(expected);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.CheckNotEmpty(expected, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
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
                        Guard.CheckNotEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotEmpty(dummy, Create<string>());
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
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.CheckNotEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class CheckNotNullOrEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                string dummy = null;

                Invoking(() =>
                    {
                        Guard.CheckNotNullOrEmpty(dummy);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        string dummy = null;

                        Guard.CheckNotNullOrEmpty(dummy, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
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

                        Guard.CheckNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        Guard.CheckNotNullOrEmpty(expected, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
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
                        Guard.CheckNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = Create<string>();

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>());
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

                        Guard.CheckNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }
        }

        public class CheckNotEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        string dummy = null;

                        Guard.CheckNotEmpty(dummy, name);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var expected = string.Empty;

                Invoking(() =>
                    {
                        Guard.CheckNotEmpty(expected);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Invoking(() =>
                    {
                        Guard.CheckNotEmpty(expected, name);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
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
                        Guard.CheckNotEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = Create<string>();

                        Guard.CheckNotEmpty(dummy, Create<string>());
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

                        Guard.CheckNotEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }
        }
    }
}