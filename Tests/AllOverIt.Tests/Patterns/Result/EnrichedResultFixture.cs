#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Patterns.Result;
using AllOverIt.Shouldly.Extensions;
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

    public class IsSuccess : EnrichedResultFixture
    {
        [Fact]
        public void Should_Return_True_When_Success()
        {
            var result = EnrichedResult.Success();

            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public void Should_Return_False_When_Fail()
        {
            var result = EnrichedResult.Fail();

            result.IsSuccess.ShouldBeFalse();
        }
    }

    public class IsFail : EnrichedResultFixture
    {
        [Fact]
        public void Should_Return_True_When_Fail()
        {
            var result = EnrichedResult.Fail();

            result.IsFail.ShouldBeTrue();
        }

        [Fact]
        public void Should_Return_False_When_Success()
        {
            var result = EnrichedResult.Success();

            result.IsFail.ShouldBeFalse();
        }
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
            .ShouldThrow<InvalidOperationException>()
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
            .ShouldNotThrow();
        }
    }

    public class Success : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Success()
        {
            var actual = EnrichedResult.Success();

            actual.ShouldBeOfType<EnrichedResult>();

            var expected = new
            {
                IsSuccess = true,
                IsFail = false
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Error"));
        }
    }

    public class Success_Typed_No_Result : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Success_Int()
        {
            var actual = EnrichedResult.Success<int>();

            actual.ShouldBeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = true,
                IsFail = false,
                Value = default(int)
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Error"));
        }

        [Fact]
        public void Should_Create_Success_Nullable_Int()
        {
            var actual = EnrichedResult.Success<int?>();

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = true,
                IsFail = false,
                Value = default(int?)
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Error"));
        }

        [Fact]
        public void Should_Create_Success_String()
        {
            var actual = EnrichedResult.Success<string>();

            actual.ShouldBeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = true,
                IsFail = false,
                Value = default(string)
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Error"));
        }
    }

    public class Success_Typed_With_Result : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Success_Int()
        {
            var value = Create<int>();

            var actual = EnrichedResult.Success(value);

            actual.ShouldBeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = true,
                IsFail = false,
                Value = value
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Error"));
        }

        [Fact]
        public void Should_Create_Success_Nullable_Int()
        {
            var value = Create<int?>();

            var actual = EnrichedResult.Success(value);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = true,
                IsFail = false,
                Value = value
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Error"));
        }

        [Fact]
        public void Should_Create_Success_String()
        {
            var value = Create<string>();

            var actual = EnrichedResult.Success(value);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = true,
                IsFail = false,
                Value = value
                //Error = (EnrichedError) null
            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Error"));
        }

        [Fact]
        public void Should_Set_Value()
        {
            var value = Create<string>();

            var actual = EnrichedResult.Success(value);

            actual.Value.ShouldBe(value);

            value = Create<string>();

            actual.Value.ShouldNotBe(value);

            actual.Value = value;

            actual.Value.ShouldBe(value);
        }
    }

    public class Fail_EnrichedError : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Fail_No_Error()
        {
            var actual = EnrichedResult.Fail((EnrichedError?) null);

            actual.ShouldBeOfType<EnrichedResult>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = (EnrichedError?) null
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.ShouldBeEquivalentTo(actual);
        }

        [Fact]
        public void Should_Create_EnrichedResult_Fail_With_Error()
        {
            var error = new EnrichedError();
            var actual = EnrichedResult.Fail(error);

            actual.ShouldBeOfType<EnrichedResult>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.ShouldBeEquivalentTo(actual);
        }
    }

    public class Fail_TResult_No_Error : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Error_Int()
        {
            var actual = EnrichedResult.Fail<int>();

            actual.ShouldBeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                //Value =
                Error = (EnrichedError?) null
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_Error_Nullable_Int()
        {
            var actual = EnrichedResult.Fail<int?>();

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                //Value =
                Error = (EnrichedError?) null

            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_Error_String()
        {
            var actual = EnrichedResult.Fail<string>();

            actual.ShouldBeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                //Value =
                Error = (EnrichedError?) null

            };

            // Have to exclude Error since it will throw when there is no error.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }
    }

    public class Fail_TResult_With_Error : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_Error_Int()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<int>(error);

            actual.ShouldBeOfType<EnrichedResult<int>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                //Value = 
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_Error_Nullable_Int()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<int?>(error);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                //Value = 
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_Error_String()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<string>(error);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                //Value = 
                Error = error
            };

            // Have to exclude Value since it will throw when there is no result.
            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Set_Error()
        {
            var error = new EnrichedError();

            var actual = EnrichedResult.Fail<string>(error);

            actual.Error.ShouldBe(error);

            error = new EnrichedError();

            actual.Error.ShouldNotBe(error);

            actual.Error = error;

            actual.Error.ShouldBe(error);
        }
    }

    public class Fail_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail(description);

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual);
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

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual);
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

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual);
        }
    }

    public class Fail_TResult_Description : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int>(description);

            actual.ShouldBeOfType<EnrichedResult<int>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?>(description);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Description()
        {
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string>(description);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = (string?) null,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
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

            actual.ShouldBeOfType<EnrichedResult<int>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Type_Description()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?>(type, description);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Type_Description()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string>(type, description);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = (string?) null,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
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

            actual.ShouldBeOfType<EnrichedResult<int>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Code_Description()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?>(type, code, description);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Code_Description()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string>(type, code, description);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            actual.Error.ShouldBeOfType<EnrichedError>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = type,
                    Code = code,
                    Description = description
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }
    }

    public class Fail_TErrorType : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<DummyErrorType>(errorType);

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual);
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

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual);
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

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual);
        }
    }

    public class Fail_TResult_TErrorType : EnrichedResultFixture
    {
        [Fact]
        public void Should_Create_EnrichedResult_Int_With_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<int, DummyErrorType>(errorType);

            actual.ShouldBeOfType<EnrichedResult<int>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_With_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<int?, DummyErrorType>(errorType);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_With_Error()
        {
            var errorType = Create<DummyErrorType>();

            var actual = EnrichedResult.Fail<string, DummyErrorType>(errorType);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = (string?) null,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
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

            actual.ShouldBeOfType<EnrichedResult<int>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Description()
        {
            var errorType = Create<DummyErrorType>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?, DummyErrorType>(errorType, description);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Description()
        {
            var errorType = Create<DummyErrorType>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string, DummyErrorType>(errorType, description);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = (string?) null,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
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

            actual.ShouldBeOfType<EnrichedResult<int>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_Nullable_Int_Error_With_Code_Description()
        {
            var errorType = Create<DummyErrorType>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<int?, DummyErrorType>(errorType, code, description);

            actual.ShouldBeOfType<EnrichedResult<int?>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }

        [Fact]
        public void Should_Create_EnrichedResult_String_Error_With_Code_Description()
        {
            var errorType = Create<DummyErrorType>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedResult.Fail<string, DummyErrorType>(errorType, code, description);

            actual.ShouldBeOfType<EnrichedResult<string>>();

            actual.Error.ShouldBeOfType<EnrichedError<DummyErrorType>>();

            var expected = new
            {
                IsSuccess = false,
                IsFail = true,
                Error = new
                {
                    Type = errorType.ToString(),
                    Code = code,
                    Description = description,
                    ErrorType = errorType
                }
            };

            expected.ShouldBeEquivalentTo(actual, opts => opts.ExcludeMember("Value"));
        }
    }
}




