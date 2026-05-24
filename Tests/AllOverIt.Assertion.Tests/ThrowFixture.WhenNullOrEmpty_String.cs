using AllOverIt.Fixture.Extensions;
using Shouldly;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture
    {
        public class WhenNullOrEmpty_String : ThrowFixture
        {
            public class NoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception>.WhenNullOrEmpty(Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNullOrEmpty(null));
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNullOrEmpty(string.Empty));
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNullOrEmpty("  "));
                }
            }

            public class One_Arg : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNullOrEmpty(Create<string>(), Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNullOrEmpty(null, arg1))
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNullOrEmpty(string.Empty, arg1))
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNullOrEmpty("  ", arg1))
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNullOrEmpty(null, arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNullOrEmpty(string.Empty, arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNullOrEmpty("  ", arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNullOrEmpty(null, arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNullOrEmpty(string.Empty, arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNullOrEmpty("  ", arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNullOrEmpty(null, arg1, arg2, arg3, arg4))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNullOrEmpty(string.Empty, arg1, arg2, arg3, arg4))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNullOrEmpty("  ", arg1, arg2, arg3, arg4))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNullOrEmpty_String_Func : ThrowFixture
        {
            public class One_Arg : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNullOrEmpty(Create<string>(), () => Create<string>()));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNullOrEmpty(null, () => arg1))
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNullOrEmpty(string.Empty, () => arg1))
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNullOrEmpty("  ", () => arg1))
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNullOrEmpty(Create<string>(), () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNullOrEmpty(null, () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNullOrEmpty(string.Empty, () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNullOrEmpty("  ", () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNullOrEmpty(Create<string>(), () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNullOrEmpty(null, () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNullOrEmpty(string.Empty, () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNullOrEmpty("  ", () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNullOrEmpty(Create<string>(), () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNullOrEmpty(null, () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNullOrEmpty(string.Empty, () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNullOrEmpty("  ", () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }
    }
}
