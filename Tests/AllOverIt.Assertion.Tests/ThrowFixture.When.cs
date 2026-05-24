using AllOverIt.Fixture.Extensions;
using Shouldly;

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
                    Should.NotThrow(() => Throw<Exception>.When(false));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.When(true));
                }
            }

            public class One_Arg : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Should.NotThrow(() => Throw<Exception1>.When(false, Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.When(true, arg1))
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Should.NotThrow(() => Throw<Exception2>.When(false, Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.When(true, arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Should.NotThrow(() => Throw<Exception3>.When(false, Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.When(true, arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Should.NotThrow(() => Throw<Exception4>.When(false, Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.When(true, arg1, arg2, arg3, arg4))
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
                    Should.NotThrow(() => Throw<Exception1>.When(false, () => Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.When(true, () => arg1))
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Should.NotThrow(() => Throw<Exception2>.When(false, () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.When(true, () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Should.NotThrow(() => Throw<Exception3>.When(false, () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.When(true, () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Should.NotThrow(() => Throw<Exception4>.When(false, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.When(true, () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
