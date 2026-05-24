using AllOverIt.Fixture.Extensions;
using Shouldly;

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
                    Should.NotThrow(() => Throw<Exception>.WhenNotNull((string)null));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNotNull(new { }));
                }
            }

            public class One_Arg : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNull((string)null, Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNotNull(new { }, arg1))
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNull((string)null, Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNotNull(new { }, arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNull((string)null, Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNotNull(new { }, arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNull((string)null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNotNull(new { }, arg1, arg2, arg3, arg4))
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
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNull((string)null, () => Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNotNull(new { }, () => arg1))
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNull((string)null, () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNotNull(new { }, () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNull((string)null, () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNotNull(new { }, () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNull((string)null, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNotNull(new { }, () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
