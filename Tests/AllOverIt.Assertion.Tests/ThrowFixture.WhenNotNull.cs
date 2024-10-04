using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNotNull : ThrowFixture
        {
            public class NoArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNull((string)null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNull(new { }))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNull((string)null, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNull(new { }, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNull((string)null, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNotNull(new { }, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNull((string)null, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNotNull(new { }, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNull((string)null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNotNull(new { }, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNotNull_Func : ThrowFixture
        {
            public class One_Arg : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNull((string)null, () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNull(new { }, () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNull((string)null, () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNotNull(new { }, () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNull((string)null, () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNotNull(new { }, () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNull((string)null, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNotNull(new { }, () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
