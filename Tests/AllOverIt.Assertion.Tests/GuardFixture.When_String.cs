using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using Shouldly;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        public class WhenNotNullOrEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        string dummy = null;

                        _ = Guard.WhenNotNullOrEmpty(dummy, name);
                    })
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        string dummy = null;

                        _ = Guard.WhenNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Empty()
            {
                var name = Create<string>();
                var expected = Create<string>();

                Should.NotThrow(() =>
                    {
                        var actual = Guard.WhenNotNullOrEmpty(expected, name);

                        actual.ShouldBe(expected);
                    });
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        _ = Guard.WhenNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    });
            }

            [Fact]
            public void Should_Return_String()
            {
                var expected = Create<string>();

                var actual = Guard.WhenNotNullOrEmpty(expected, Create<string>());

                actual.ShouldBeSameAs(expected);
            }
        }

        public class WhenNotEmpty_String : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Should.NotThrow(() =>
                    {
                        string dummy = null;

                        _ = Guard.WhenNotEmpty(dummy, name);
                    });
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = string.Empty;

                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        Guard.WhenNotEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = Create<string>();

                        Guard.WhenNotEmpty(dummy, Create<string>(), errorMessage);
                    });
            }

            [Fact]
            public void Should_Return_String()
            {
                var expected = Create<string>();

                var actual = Guard.WhenNotEmpty(expected, Create<string>());

                actual.ShouldBeSameAs(expected);
            }
        }
    }
}
