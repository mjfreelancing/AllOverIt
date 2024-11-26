using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNull : ThrowFixture
        {
            public class NoArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNull(new { }))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNull((string)null))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNull(new { }, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNull((string)null, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNull(new { }, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNull((string)null, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNull(new { }, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNull((string)null, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNull(new { }, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
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

                    Invoking(() => Throw<Exception4>.WhenNull((string)null, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNull_Func : ThrowFixture
        {
            public class One_Arg : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNull(new { }, () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNull((string)null, () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNull(new { }, () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNull((string)null, () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNull(new { }, () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNull((string)null, () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNull(new { }, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
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

                    Invoking(() => Throw<Exception4>.WhenNull((string)null, () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
