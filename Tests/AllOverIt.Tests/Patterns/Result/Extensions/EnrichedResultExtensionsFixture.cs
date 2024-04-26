#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Result;
using AllOverIt.Patterns.Result.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Result.Extensions;

public class EnrichedResultExtensionsFixture : FixtureBase
{
    private readonly EnrichedResult _success = EnrichedResult.Success();
    private readonly EnrichedResult<int> _successInt = EnrichedResult.Success(0);
    private readonly EnrichedResult _fail = EnrichedResult.Fail();
    private readonly EnrichedResult<int> _failInt = EnrichedResult.Fail<int, string>("0");

    public class Match : EnrichedResultExtensionsFixture
    {
        [Fact]
        public void Should_Throw_When_Result_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match(
                    null!,
                    result => result,
                    result => result);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("result");
        }

        [Fact]
        public void Should_Throw_When_OnSuccess_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match(
                    _success,
                    null!,
                    result => result);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("onSuccess");
        }

        [Fact]
        public void Should_Throw_When_OnFail_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match(
                    _fail,
                    result => result,
                    null!);
            })
           .Should()
           .Throw<ArgumentNullException>()
           .WithNamedMessageWhenNull("onFail");
        }

        [Fact]
        public void Should_Return_On_Success_Result()
        {
            EnrichedResult? actual = null;

            var expected = EnrichedResult.Success();

            _ = EnrichedResultExtensions.Match(
                _success,
                result =>
                {
                    actual = expected;

                    return expected;
                },
                result => result);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Return_On_Fail_Result()
        {
            EnrichedResult? actual = null;

            var expected = EnrichedResult.Fail();

            _ = EnrichedResultExtensions.Match(
                _fail,
                result => result,
                result =>
                {
                    actual = expected;

                    return expected;
                });

            actual.Should().Be(expected);
        }
    }

    public class Match_TResult : EnrichedResultExtensionsFixture
    {
        [Fact]
        public void Should_Throw_When_Result_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match<int>(null!, result => result, result => result);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("result");
        }

        [Fact]
        public void Should_Throw_When_OnSuccess_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match<int>(_successInt, null!, result => result);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("onSuccess");
        }

        [Fact]
        public void Should_Throw_When_OnFail_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match<int>(_failInt, result => result, null!);
            })
           .Should()
           .Throw<ArgumentNullException>()
           .WithNamedMessageWhenNull("onFail");
        }

        [Fact]
        public void Should_Return_On_Success_Result()
        {
            var expected = EnrichedResult.Success(Create<int>());

            var actual = EnrichedResultExtensions.Match<int>(
                _successInt,
                result =>
                {
                    return expected;
                },
                result => result);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Return_On_Fail_Result()
        {
            var expected = EnrichedResult.Fail<int>();

            var actual = EnrichedResultExtensions.Match<int>(
                _failInt,
                result => result,
                result =>
                {
                    return expected;
                });

            actual.Should().Be(expected);
        }
    }

    public class Match_TInResult_TOutResult : EnrichedResultExtensionsFixture
    {
        [Fact]
        public void Should_Throw_When_Result_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match<int, double>(
                    null!,
                    result => EnrichedResult.Success<double>(result.Value),
                    result => EnrichedResult.Fail<double>());
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("result");
        }

        [Fact]
        public void Should_Throw_When_OnSuccess_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match<int, double>(
                    _successInt,
                    null!,
                    result => EnrichedResult.Fail<double>());
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("onSuccess");
        }

        [Fact]
        public void Should_Throw_When_OnFail_Null()
        {
            Invoking(() =>
            {
                _ = EnrichedResultExtensions.Match<int, double>(
                    _failInt,
                    result => EnrichedResult.Success<double>(result.Value),
                    null!);
            })
           .Should()
           .Throw<ArgumentNullException>()
           .WithNamedMessageWhenNull("onFail");
        }

        [Fact]
        public void Should_Return_On_Success_Result()
        {
            var expected = EnrichedResult.Success<double>();

            var actual = EnrichedResultExtensions.Match<int, double>(
                _successInt,
                result =>
                {
                    return expected;
                },
                result => EnrichedResult.Fail<double>());

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Return_On_Fail_Result()
        {
            var expected = EnrichedResult.Fail<double>();

            var actual = EnrichedResultExtensions.Match<int, double>(
                _failInt,
                result => EnrichedResult.Success<double>(),
                result =>
                {
                    return expected;
                });

            actual.Should().Be(expected);
        }
    }

    public class Switch : EnrichedResultExtensionsFixture
    {
        [Fact]
        public void Should_Throw_When_Result_Null()
        {
            Invoking(() =>
            {
                EnrichedResultExtensions.Switch(
                    null!,
                    result => { },
                    result => { });
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("result");
        }

        [Fact]
        public void Should_Throw_When_OnSuccess_Null()
        {
            Invoking(() =>
            {
                EnrichedResultExtensions.Switch(
                    _success,
                    null!,
                    result => { });
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("onSuccess");
        }

        [Fact]
        public void Should_Throw_When_OnFail_Null()
        {
            Invoking(() =>
            {
                EnrichedResultExtensions.Switch(
                    _success,
                    result => { },
                    null!);
            })
           .Should()
           .Throw<ArgumentNullException>()
           .WithNamedMessageWhenNull("onFail");
        }

        [Fact]
        public void Should_Invoke_On_Success()
        {
            var invoked = false;

            EnrichedResultExtensions.Switch(
                _success,
                result =>
                {
                    invoked = true;
                },
                result => { });

            invoked.Should().BeTrue();
        }

        [Fact]
        public void Should_Invoke_On_Fail()
        {
            var invoked = false;

            EnrichedResultExtensions.Switch(
                _fail,
                result => { },
                result =>
                {
                    invoked = true;
                });

            invoked.Should().BeTrue();
        }
    }

    public class Switch_TResult : EnrichedResultExtensionsFixture
    {
        [Fact]
        public void Should_Throw_When_Result_Null()
        {
            Invoking(() =>
            {
                EnrichedResultExtensions.Switch<int>(
                    null!,
                    result => { },
                    result => { });
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("result");
        }

        [Fact]
        public void Should_Throw_When_OnSuccess_Null()
        {
            Invoking(() =>
            {
                EnrichedResultExtensions.Switch<int>(
                    _successInt,
                    null!,
                    result => { });
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("onSuccess");
        }

        [Fact]
        public void Should_Throw_When_OnFail_Null()
        {
            Invoking(() =>
            {
                EnrichedResultExtensions.Switch<int>(
                    _successInt,
                    result => { },
                    null!);
            })
           .Should()
           .Throw<ArgumentNullException>()
           .WithNamedMessageWhenNull("onFail");
        }

        [Fact]
        public void Should_Invoke_On_Success()
        {
            var invoked = false;

            EnrichedResultExtensions.Switch<int>(
                _successInt,
                result =>
                {
                    invoked = true;
                },
                result => { });

            invoked.Should().BeTrue();
        }

        [Fact]
        public void Should_Invoke_On_Fail()
        {
            var invoked = false;

            EnrichedResultExtensions.Switch<int>(
                _failInt,
                result => { },
                result =>
                {
                    invoked = true;
                });

            invoked.Should().BeTrue();
        }
    }
}
