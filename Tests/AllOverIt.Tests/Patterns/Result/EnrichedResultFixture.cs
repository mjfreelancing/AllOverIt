#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Patterns.Result;
using FluentAssertions;
using System;

namespace AllOverIt.Tests.Patterns.Result;

public partial class EnrichedResultFixture : FixtureBase
{
    public class Error : EnrichedResultFixture
    {
        [Fact]
        public void Should_Throw_When_Success()
        {
            var actual = EnrichedResult.Success();

            Invoking(() =>
            {
                _ = actual.Error;
            })
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The result has no error.");
        }

        [Fact]
        public void Should_Not_Throw_When_Error()
        {
            var actual = EnrichedResult.Fail();

            Invoking(() =>
            {
                _ = actual.Error;
            })
            .Should()
            .NotThrow();
        }
    }

    public class Success : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Success()
        {
            var actual = EnrichedResult.Success();

            actual.Should().BeOfType<EnrichedResult>();

            var expected = new
            {
                IsSuccess = true,
                IsError = false
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Error));
        }
    }

    public class Success_Typed_No_Result : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Success_Int()
        {
            var actual = EnrichedResult.Success<int>();

            actual.Should().BeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = true,
                IsError = false,
                Value = default(int)
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Error));
        }

        [Fact]
        public void Should_Create_Success_Nullable_Int()
        {
            var actual = EnrichedResult.Success<int?>();

            actual.Should().BeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = true,
                IsError = false,
                Value = default(int?)
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Error));
        }

        [Fact]
        public void Should_Create_Success_String()
        {
            var actual = EnrichedResult.Success<string>();

            actual.Should().BeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = true,
                IsError = false,
                Value = default(string)
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Error));
        }
    }

    public class Success_Typed_With_Result : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Success_Int()
        {
            var value = Create<int>();

            var actual = EnrichedResult.Success(value);

            actual.Should().BeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = true,
                IsError = false,
                Value = value
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Error));
        }

        [Fact]
        public void Should_Create_Success_Nullable_Int()
        {
            var value = Create<int?>();

            var actual = EnrichedResult.Success(value);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = true,
                IsError = false,
                Value = value
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Error));
        }

        [Fact]
        public void Should_Create_Success_String()
        {
            var value = Create<string>();

            var actual = EnrichedResult.Success(value);

            actual.Should().BeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = true,
                IsError = false,
                Value = value
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Error));
        }

        [Fact]
        public void Should_Set_Value()
        {
            var value = Create<string>();

            var actual = EnrichedResult.Success(value);

            actual.Value.Should().Be(value);

            value = Create<string>();

            actual.Value.Should().NotBe(value);

            actual.Value = value;

            actual.Value.Should().Be(value);
        }
    }

    public class Fail_Default : EnrichedResultFixture
    {
        [Fact]
        public void Should_Fail_No_Error()
        {
            var actual = EnrichedResult.Fail();

            actual.Should().BeOfType<EnrichedResult>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = (EnrichedError?) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_Typed_No_Error : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Error_Int()
        {
            var actual = EnrichedResult.Fail<int>();

            actual.Should().BeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                //Value = default(int)
                Error = (EnrichedError?) null
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_Error_Nullable_Int()
        {
            var actual = EnrichedResult.Fail<int?>();

            actual.Should().BeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                //Value = default(int?)
                Error = (EnrichedError?) null

            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_Error_String()
        {
            var actual = EnrichedResult.Fail<string>();

            actual.Should().BeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                //Value = default(string)
                Error = (EnrichedError?) null

            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }

    public class Fail_Typed_With_Error : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Error_Int()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<int>(error);

            actual.Should().BeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                //Value = 
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_Error_Nullable_Int()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<int?>(error);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                //Value = 
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_Error_String()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<string>(error);

            actual.Should().BeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                //Value = 
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Set_Error()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<string>(error);

            actual.Error.Should().Be(error);

            error = new EnrichedError();

            actual.Error.Should().NotBe(error);

            actual.Error = error;

            actual.Error.Should().Be(error);
        }
    }
}