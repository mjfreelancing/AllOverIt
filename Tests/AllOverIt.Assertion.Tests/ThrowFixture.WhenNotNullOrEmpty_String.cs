using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNotNullOrEmpty_String : ThrowFixture
        {
            public class NoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(Create<string>()))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(string.Empty))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty("  "))
                        .Should()
                        .NotThrow();
                }
            }

            public class One_Arg : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(Create<string>(), arg1))
                        .Should()
                        .Throw<Exception1>()
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(null, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(string.Empty, arg1))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty("  ", arg1))
                        .Should()
                        .NotThrow();
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }
            }

            public class ThreeArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }
            }

            public class FourArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }
            }
        }

        public class WhenNotNullOrEmpty_String_Func : ThrowFixture
        {
            public class One_Arg : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(Create<string>(), () => arg1))
                        .Should()
                        .Throw<Exception1>()
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(null, () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(string.Empty, () => arg1))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty("  ", () => arg1))
                        .Should()
                        .NotThrow();
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(Create<string>(), () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(null, () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(string.Empty, () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty("  ", () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }
            }

            public class ThreeArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(Create<string>(), () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(null, () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(string.Empty, () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty("  ", () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }
            }

            public class FourArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(Create<string>(), () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(null, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(string.Empty, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty("  ", () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }
            }
        }
    }
}
