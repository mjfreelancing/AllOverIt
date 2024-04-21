#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Patterns.Result;
using FluentAssertions;
using System;

namespace AllOverIt.Tests.Patterns.Result;

public partial class EnrichedResultFixture : FixtureBase
{
    private enum DummyErrorType
    {
        Dummy1,
        Dummy2,
        Dummy3,
        Dummy4,
        Dummy5
    }

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

    public class Fail_EnrichedError : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Fail_No_Error()
        {
            var actual = EnrichedResult.Fail((EnrichedError?) null);

            actual.Should().BeOfType<EnrichedResult>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = (EnrichedError?) null
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.Should().BeEquivalentTo(actual);
        }

        [Fact]
        public void Should_Create_EnrichedResult_Fail_With_Error()
        {
            var error = new EnrichedError();
            var actual = EnrichedResult.Fail(error);

            actual.Should().BeOfType<EnrichedResult>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_TResult_No_Error : EnrichedResultFixture
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
                //Value =
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
                //Value =
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
                //Value =
                Error = (EnrichedError?) null

            };

            // Have to exclude Error since it will throw when there is no error.
            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }

    public class Fail_TResult_With_Error : EnrichedResultFixture
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

    public class Fail_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail(description);

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_Type_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error_With_Type_Description()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail(type, description);

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_Type_Code_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error_With_Type_Code_Description()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail(type, code, description);

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_TResult_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int>(description);

            actual.Should().BeOfType<EnrichedResult<int>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?>(description);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string>(description);

            actual.Should().BeOfType<EnrichedResult<string>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }

    public class Fail_TResult_Type_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_Error_With_Type_Description()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int>(type, description);

            actual.Should().BeOfType<EnrichedResult<int>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Type_Description()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?>(type, description);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Type_Description()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string>(type, description);

            actual.Should().BeOfType<EnrichedResult<string>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }

    public class Fail_TResult_Type_Code_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_Error_With_Code_Description()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int>(type, code, description);

            actual.Should().BeOfType<EnrichedResult<int>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Code_Description()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?>(type, code, description);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Code_Description()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string>(type, code, description);

            actual.Should().BeOfType<EnrichedResult<string>>();

            actual.Error.Should().BeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }

    public class Fail_TErrorType : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<DummyErrorType>(errorType);

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_TErrorType_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error_Description()
        {
            var errorType = Create<DummyErrorType>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<DummyErrorType>(errorType, description);

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_TErrorType_Code_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error_Code_Description()
        {
            var errorType = Create<DummyErrorType>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<DummyErrorType>(errorType, code, description);

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Fail_TResult_TErrorType : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_With_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<int, DummyErrorType>(errorType);

            actual.Should().BeOfType<EnrichedResult<int>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_With_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<int?, DummyErrorType>(errorType);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_With_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<string, DummyErrorType>(errorType);

            actual.Should().BeOfType<EnrichedResult<string>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }

    public class Fail_TResult_TErrorType_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_Error_With_Description()
        {
            var errorType = Create<DummyErrorType>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int, DummyErrorType>(errorType, description);

            actual.Should().BeOfType<EnrichedResult<int>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Description()
        {
            var errorType = Create<DummyErrorType>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?, DummyErrorType>(errorType, description);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Description()
        {
            var errorType = Create<DummyErrorType>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string, DummyErrorType>(errorType, description);

            actual.Should().BeOfType<EnrichedResult<string>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }

    public class Fail_TResult_TErrorType_Code_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_Error_With_Code_Description()
        {
            var errorType = Create<DummyErrorType>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int, DummyErrorType>(errorType, code, description);

            actual.Should().BeOfType<EnrichedResult<int>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Code_Description()
        {
            var errorType = Create<DummyErrorType>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?, DummyErrorType>(errorType, code, description);

            actual.Should().BeOfType<EnrichedResult<int?>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Code_Description()
        {
            var errorType = Create<DummyErrorType>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string, DummyErrorType>(errorType, code, description);

            actual.Should().BeOfType<EnrichedResult<string>>();

            actual.Error.Should().BeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsError = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.Should().BeEquivalentTo(actual, options => options.Excluding(result => result.Value));
        }
    }
}