using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNullOrEmpty_String : ThrowFixture
        {
            public class NoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(null))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(string.Empty))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty("  "))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(Create<string>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(null, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(string.Empty, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty("  ", arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(null, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(string.Empty, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty("  ", arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(null, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(string.Empty, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty("  ", arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(null, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(string.Empty, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty("  ", arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNullOrEmpty_String_Func : ThrowFixture
        {
            public class One_Arg : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(Create<string>(), () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(null, () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(string.Empty, () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty("  ", () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(Create<string>(), () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(null, () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(string.Empty, () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty("  ", () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(Create<string>(), () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(null, () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(string.Empty, () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty("  ", () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(Create<string>(), () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(null, () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(string.Empty, () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty("  ", () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
