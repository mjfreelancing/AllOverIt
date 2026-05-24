using AllOverIt.Fixture.Extensions;
using Shouldly;

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
                    Should.Throw<Exception>(() => Throw<Exception>.WhenNotNullOrEmpty(CreateMany<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception>.WhenNotNullOrEmpty((IEnumerable<string>)null));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception>.WhenNotNullOrEmpty(Array.Empty<string>()));
                }
            }

            public class One_Arg : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNotNullOrEmpty(CreateMany<string>(), arg1))
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>()));
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNotNullOrEmpty(CreateMany<string>(), arg1, arg2))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>(), Create<bool>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>(), Create<bool>()));
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

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNotNullOrEmpty(CreateMany<string>(), arg1, arg2, arg3))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>(), Create<bool>(), Create<int>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>(), Create<bool>(), Create<int>()));
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

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNotNullOrEmpty(CreateMany<string>(), arg1, arg2, arg3, arg4))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty((IEnumerable<string>)null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty(Array.Empty<string>(), Create<string>(), Create<bool>(), Create<int>(), Create<string>()));
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

                    Should.Throw<Exception1>(() => Throw<Exception1>.WhenNotNullOrEmpty(CreateMany<string>(), () => arg1))
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => Create<string>()));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception1>.WhenNotNullOrEmpty(Array.Empty<string>(), () => Create<string>()));
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_Enumerable
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Should.Throw<Exception2>(() => Throw<Exception2>.WhenNotNullOrEmpty(CreateMany<string>(), () => (arg1, arg2)))
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => (Create<string>(), Create<bool>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception2>.WhenNotNullOrEmpty(Array.Empty<string>(), () => (Create<string>(), Create<bool>())));
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

                    Should.Throw<Exception3>(() => Throw<Exception3>.WhenNotNullOrEmpty(CreateMany<string>(), () => (arg1, arg2, arg3)))
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => (Create<string>(), Create<bool>(), Create<int>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception3>.WhenNotNullOrEmpty(Array.Empty<string>(), () => (Create<string>(), Create<bool>(), Create<int>())));
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

                    Should.Throw<Exception4>(() => Throw<Exception4>.WhenNotNullOrEmpty(CreateMany<string>(), () => (arg1, arg2, arg3, arg4)))
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty((IEnumerable<string>)null, () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Should.NotThrow(() => Throw<Exception4>.WhenNotNullOrEmpty(Array.Empty<string>(), () => (Create<string>(), Create<bool>(), Create<int>(), Create<string>())));
                }
            }
        }
    }
}
