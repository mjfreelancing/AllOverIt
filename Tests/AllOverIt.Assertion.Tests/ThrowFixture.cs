using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Assertion.Tests
{
    public class ThrowFixture : FixtureBase
    {
        private class Exception1 : Exception
        {
            public string Arg1 { get; }

            public Exception1(string arg1)
                : base($"{arg1}")
            {
                Arg1 = arg1;
            }
        }

        private class Exception2 : Exception
        {
            public string Arg1 { get; }
            public bool Arg2 { get; }

            public Exception2(string arg1, bool arg2)
                : base($"{arg1}{arg2}")
            {
                Arg1 = arg1;
                Arg2 = arg2;
            }
        }

        private class Exception3 : Exception
        {
            public string Arg1 { get; }
            public bool Arg2 { get; }
            public int Arg3 { get; }

            public Exception3(string arg1, bool arg2, int arg3)
                : base($"{arg1}{arg2}{arg3}")
            {
                Arg1 = arg1;
                Arg2 = arg2;
                Arg3 = arg3;
            }
        }

        private class Exception4 : Exception
        {
            public string Arg1 { get; }
            public bool Arg2 { get; }
            public int Arg3 { get; }
            public string Arg4 { get; }

            public Exception4(string arg1, bool arg2, int arg3, string arg4)
                : base($"{arg1}{arg2}{arg3}{arg4}")
            {
                Arg1 = arg1;
                Arg2 = arg2;
                Arg3 = arg3;
                Arg4 = arg4;
            }
        }

        public class When : ThrowFixture
        {
            public class NoArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception>.When(false))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    Invoking(() => Throw<Exception>.When(true))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception1>.When(false, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.When(true, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception2>.When(false, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.When(true, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception3>.When(false, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.When(true, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : When
            {
                [Fact]
                public void Should_Not_Throw_When_False()
                {
                    Invoking(() => Throw<Exception4>.When(false, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_True()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.When(true, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNot : ThrowFixture
        {
            public class NoArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception>.WhenNot(true))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    Invoking(() => Throw<Exception>.WhenNot(false))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception1>.WhenNot(true, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNot(false, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage($"{arg1}");
                }
            }

            public class TwoArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception2>.WhenNot(true, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNot(false, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception3>.WhenNot(true, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNot(false, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNot
            {
                [Fact]
                public void Should_Not_Throw_When_True()
                {
                    Invoking(() => Throw<Exception4>.WhenNot(true, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_False()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNot(false, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNull : ThrowFixture
        {
            public class NoArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNull(new { }))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNull((string) null))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNull(new { }, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNull((string) null, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNull(new { }, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNull((string) null, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNull(new { }, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNull((string) null, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNull
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNull(new { }, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
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

                    Invoking(() => Throw<Exception4>.WhenNull((string) null, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNotNull : ThrowFixture
        {
            public class NoArgs : WhenNotNull
            {
                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNull((string) null))
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
                    Invoking(() => Throw<Exception1>.WhenNotNull((string) null, Create<string>()))
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
                    Invoking(() => Throw<Exception2>.WhenNotNull((string) null, Create<string>(), Create<bool>()))
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
                    Invoking(() => Throw<Exception3>.WhenNotNull((string) null, Create<string>(), Create<bool>(), Create<int>()))
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
                    Invoking(() => Throw<Exception4>.WhenNotNull((string) null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
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

        public class WhenNullOrEmpty_String : ThrowFixture
        {
            public class NoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(null))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty(string.Empty))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty("  "))
                        .Should()
                        .Throw<Exception>();
                }
            }

            public class One_Arg : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(Create<string>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(null, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty(string.Empty, arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty("  ", arg1))
                      .Should()
                      .Throw<Exception1>()
                      .WithMessage(arg1);
                }
            }

            public class TwoArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(null, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Empty()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty(string.Empty, arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty("  ", arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }
            }

            public class ThreeArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Throw_When_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(null, arg1, arg2, arg3))
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

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty(string.Empty, arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty("  ", arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }
            }

            public class FourArgs : WhenNullOrEmpty_String
            {
                [Fact]
                public void Should_Not_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(Create<string>(), Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
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

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(null, arg1, arg2, arg3, arg4))
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

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty(string.Empty, arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();
                    var arg3 = Create<int>();
                    var arg4 = Create<string>();

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty("  ", arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }
            }
        }

        public class WhenNotNullOrEmpty_String : ThrowFixture
        {
            public class NoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(Create<string>()))
                        .Should()
                        .Throw<Exception>();
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty(string.Empty))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty("  "))
                        .Should()
                        .NotThrow();
                }
            }

            public class One_Arg : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(Create<string>(), arg1))
                        .Should()
                        .Throw<Exception1>()
                        .WithMessage(arg1);
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(null, Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty(string.Empty, arg1))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    var arg1 = Create<string>();

                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty("  ", arg1))
                        .Should()
                        .NotThrow();
                }
            }

            public class TwoArgs : WhenNotNullOrEmpty_String
            {
                [Fact]
                public void Should_Throw_When_Not_Null()
                {
                    var arg1 = Create<string>();
                    var arg2 = Create<bool>();

                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2))
                        .Should()
                        .Throw<Exception2>()
                        .WithMessage($"{arg1}{arg2}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>()))
                        .Should()
                        .NotThrow();
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

                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2, arg3))
                        .Should()
                        .Throw<Exception3>()
                        .WithMessage($"{arg1}{arg2}{arg3}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>(), Create<int>()))
                        .Should()
                        .NotThrow();
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

                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(Create<string>(), arg1, arg2, arg3, arg4))
                        .Should()
                        .Throw<Exception4>()
                        .WithMessage($"{arg1}{arg2}{arg3}{arg4}");
                }

                [Fact]
                public void Should_Not_Throw_When_Null()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Empty()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty(string.Empty, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Whitespace()
                {
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty("  ", Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
                        .Should()
                        .NotThrow();
                }
            }
        }

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
                    Invoking(() => Throw<Exception>.WhenNullOrEmpty((IEnumerable<string>) null))
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

                    Invoking(() => Throw<Exception1>.WhenNullOrEmpty((IEnumerable<string>) null, arg1))
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

                    Invoking(() => Throw<Exception2>.WhenNullOrEmpty((IEnumerable<string>) null, arg1, arg2))
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

                    Invoking(() => Throw<Exception3>.WhenNullOrEmpty((IEnumerable<string>) null, arg1, arg2, arg3))
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

                    Invoking(() => Throw<Exception4>.WhenNullOrEmpty((IEnumerable<string>) null, arg1, arg2, arg3, arg4))
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
                    Invoking(() => Throw<Exception>.WhenNotNullOrEmpty((IEnumerable<string>) null))
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
                    Invoking(() => Throw<Exception1>.WhenNotNullOrEmpty((IEnumerable<string>) null, Create<string>()))
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
                    Invoking(() => Throw<Exception2>.WhenNotNullOrEmpty((IEnumerable<string>) null, Create<string>(), Create<bool>()))
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
                    Invoking(() => Throw<Exception3>.WhenNotNullOrEmpty((IEnumerable<string>) null, Create<string>(), Create<bool>(), Create<int>()))
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
                    Invoking(() => Throw<Exception4>.WhenNotNullOrEmpty((IEnumerable<string>) null, Create<string>(), Create<bool>(), Create<int>(), Create<string>()))
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
    }
}
