using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNot : ThrowFixture
        {
            public class NoArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception>.WhenNot(true))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    Invoking(() => Throw<Exception>.WhenNot(false))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception1>.WhenNot(true, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNot(false, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception2>.WhenNot(true, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNot(false, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception3>.WhenNot(true, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNot(false, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception4>.WhenNot(true, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNot(false, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNot_Func : ThrowFixture
        {
            public class One_Arg : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception1>.WhenNot(true, () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNot(false, () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception2>.WhenNot(true, () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNot(false, () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception3>.WhenNot(true, () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNot(false, () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception4>.WhenNot(true, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNot(false, () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
