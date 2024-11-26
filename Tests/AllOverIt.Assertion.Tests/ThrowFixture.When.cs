using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class When : ThrowFixture
        {
            public class NoArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception>.When(false))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    Invoking(() => Throw<Exception>.When(true))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception1>.When(false, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.When(true, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception2>.When(false, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.When(true, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception3>.When(false, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.When(true, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception4>.When(false, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.When(true, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class When_Func : ThrowFixture
        {
            public class One_Arg : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception1>.When(false, () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.When(true, () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception2>.When(false, () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.When(true, () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception3>.When(false, () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.When(true, () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception4>.When(false, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.When(true, () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
