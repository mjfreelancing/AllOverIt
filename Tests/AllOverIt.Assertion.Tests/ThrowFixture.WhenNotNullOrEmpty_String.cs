using AllOverIt.Fixture.Extensions;
using Shouldly;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNotNullOrEmpty_String : ThrowFixture
        {
            public class NoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNotNullOrEmpty(Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception>.WhenNotNullOrEmpty(null));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception>.WhenNotNullOrEmpty(string.Empty));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Should.NotThrow(() => Throw<Exception>.WhenNotNullOrEmpty("  "));
                }
            }

            public class One_Arg : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNotNullOrEmpty(Create<string>(), arg1))
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty(null, Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty(string.Empty, arg1));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty("  ", arg1));
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>()));
                }
            }

            public class ThreeArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>(), Create<int>()));
                }
            }

            public class FourArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2, arg3, arg4))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }
            }
        }

        public class WhenNotNullOrEmpty_String_Func : ThrowFixture
        {
            public class One_Arg : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNotNullOrEmpty(Create<string>(), () => arg1))
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty(null, () => Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty(string.Empty, () => arg1));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty("  ", () => arg1));
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNotNullOrEmpty(Create<string>(), () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty(null, () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty(string.Empty, () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty("  ", () => (Create<string>(), Create<bool>())));
                }
            }

            public class ThreeArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNotNullOrEmpty(Create<string>(), () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty(null, () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty(string.Empty, () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty("  ", () => (Create<string>(), Create<bool>(), Create<int>())));
                }
            }

            public class FourArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNotNullOrEmpty(Create<string>(), () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty(null, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty(string.Empty, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty("  ", () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }
            }
        }
    }
}
