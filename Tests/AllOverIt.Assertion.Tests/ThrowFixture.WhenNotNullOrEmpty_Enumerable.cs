using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNotNullOrEmpty_Enumerable : ThrowFixture
        {
            public class NoArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(CreateMany<string>()))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty((IEnumerable<string>)null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(Array.Empty<string>()))
                        .Should()
                        .NotThrow();
                }
            }

            public class One_Arg : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(CreateMany<string>(), arg1))
                        .Should()
                        .Throw<Exception1>()
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(CreateMany<string>(), arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }
            }

            public class ThreeArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(CreateMany<string>(), arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }
            }

            public class FourArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(CreateMany<string>(), arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }
            }
        }


        public class WhenNotNullOrEmpty_Enumerable_Func : ThrowFixture
        {
            public class One_Arg : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(CreateMany<string>(), () => arg1))
                        .Should()
                        .Throw<Exception1>()
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(Array.Empty<string>(), () => Create<string>()))
                        .Should()
                        .NotThrow();
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(CreateMany<string>(), () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(Array.Empty<string>(), () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }
            }

            public class ThreeArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(CreateMany<string>(), () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(Array.Empty<string>(), () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }
            }

            public class FourArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(CreateMany<string>(), () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(Array.Empty<string>(), () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
                        .Should()
                        .NotThrow();
                }
            }
        }
    }
}
