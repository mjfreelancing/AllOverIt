using AllOverIt.Fixture.Extensions;
using Shouldly;

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
                    Should.NotThrow(() => Throw<Exception>.WhenNot(true));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNot(false));
                }
            }

            public class One_Arg : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNot(true, Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNot(false, arg1))
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNot(true, Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNot(false, arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNot(true, Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNot(false, arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNot(true, Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNot(false, arg1, arg2, arg3, arg4))
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
                    Should.NotThrow(() => Throw<Exception1>.WhenNot(true, () => Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNot(false, () => arg1))
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNot(true, () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNot(false, () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNot(true, () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNot(false, () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNot(true, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNot(false, () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
