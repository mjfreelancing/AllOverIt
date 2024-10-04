using FluentAssertions;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNullOrEmpty_Enumerable : ThrowFixture
        {
            public class NoArgs : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(CreateMany<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty((IEnumerable<string>)null))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(Array.Empty<string>()))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(CreateMany<string>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty((IEnumerable<string>)null, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(Array.Empty<string>(), arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(CreateMany<string>(), Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty((IEnumerable<string>)null, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(Array.Empty<string>(), arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(CreateMany<string>(), Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty((IEnumerable<string>)null, arg1, arg2, arg3))
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

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(Array.Empty<string>(), arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(CreateMany<string>(), Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
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

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty((IEnumerable<string>)null, arg1, arg2, arg3, arg4))
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

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(Array.Empty<string>(), arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }


        public class WhenNullOrEmpty_Enumerable_Func : ThrowFixture
        {
            public class One_Arg : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(CreateMany<string>(), () => Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty((IEnumerable<string>)null, () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(Array.Empty<string>(), () => arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(CreateMany<string>(), () => (Create<string>(), Create<bool>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty((IEnumerable<string>)null, () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(Array.Empty<string>(), () => (arg1, arg2)))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(CreateMany<string>(), () => (Create<string>(), Create<bool>(), Create<int>())))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty((IEnumerable<string>)null, () => (arg1, arg2, arg3)))
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

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(Array.Empty<string>(), () => (arg1, arg2, arg3)))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(CreateMany<string>(), () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())))
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

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty((IEnumerable<string>)null, () => (arg1, arg2, arg3, arg4)))
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

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(Array.Empty<string>(), () => (arg1, arg2, arg3, arg4)))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
