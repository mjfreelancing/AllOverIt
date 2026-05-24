using AllOverIt.Fixture.Extensions;
using Shouldly;

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
                    Should.NotThrow(() => Throw<Exception>.WhenNull(new { }));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNull((string)null));
                }
            }

            public class One_Arg : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNull(new { }, Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNull((string)null, arg1))
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNull(new { }, Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNull((string)null, arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNull(new { }, Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNull((string)null, arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNull(new { }, Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNull((string)null, arg1, arg2, arg3, arg4))
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
                    Should.NotThrow(() => Throw<Exception1>.WhenNull(new { }, () => Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNull((string)null, () => arg1))
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNull(new { }, () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNull((string)null, () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNull(new { }, () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNull((string)null, () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNull(new { }, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNull((string)null, () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
