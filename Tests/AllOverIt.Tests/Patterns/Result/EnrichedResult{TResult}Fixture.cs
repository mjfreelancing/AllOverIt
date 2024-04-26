#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Patterns.Result;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Result;

public partial class EnrichedResultFixture : FixtureBase
{
    public class Value : EnrichedResultFixture
    {
        [Fact]
        public void Should_Not_Throw_When_Success()
        {
            var actual = EnrichedResult.Success<int>();

            Invoking(() =>
            {
                _ = actual.Value;
            })
            .Should()
            .NotThrow();
        }

        [Fact]
        public void Should_Throw_When_Fail()
        {
            var actual = EnrichedResult.Fail<int>();

            Invoking(() =>
            {
                _ = actual.Value;
            })
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"The result has no value. More detail can be found on the {nameof(EnrichedResult.Fail)} property.");
        }
    }

    public class Constructor_Default : EnrichedResultFixture
    {
        [Fact]
        public void Should_Set_IsSuccess_Default_Int_Value()
        {
            var actual = new EnrichedResult<int>();

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().Be(default(int));
        }

        [Fact]
        public void Should_Set_IsSuccess_Default_Nullable_Int_Value()
        {
            var actual = new EnrichedResult<int?>();

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().Be(default(int?));
        }

        [Fact]
        public void Should_Set_IsSuccess_Default_String_Value()
        {
            var actual = new EnrichedResult<string>();

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().Be(default(string));
        }
    }

    public class Constructor_Result : EnrichedResultFixture
    {
        [Fact]
        public void Should_Set_IsSuccess_With_Int_Value()
        {
            var value = Create<int>();

            var actual = new EnrichedResult<int>(value);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void Should_Set_IsSuccess_With_Nullable_Int_Value()
        {
            var value = Create<int?>();

            var actual = new EnrichedResult<int?>(value);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void Should_Set_IsSuccess_With_String_Value()
        {
            var value = Create<string>();

            var actual = new EnrichedResult<string>(value);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().Be(value);
        }
    }

    public class Constructor_Error : EnrichedResultFixture
    {
        [Fact]
        public void Should_Set_IsError_With_No_Error()
        {
            var actual1 = new EnrichedResult<int>((EnrichedError?) null);
            var actual2 = new EnrichedResult<int?>((EnrichedError?) null);
            var actual3 = new EnrichedResult<string>((EnrichedError?) null);

            actual1.IsFail.Should().BeTrue();
            actual1.Error.Should().Be(default);

            actual2.IsFail.Should().BeTrue();
            actual2.Error.Should().Be(default);

            actual2.IsFail.Should().BeTrue();
            actual2.Error.Should().Be(default);
        }

        [Fact]
        public void Should_Set_IsError_With_Error()
        {
            var error = new EnrichedError();

            var actual1 = new EnrichedResult<int>(error);
            var actual2 = new EnrichedResult<int?>(error);
            var actual3 = new EnrichedResult<string>(error);

            actual1.IsFail.Should().BeTrue();
            actual1.Error.Should().Be(error);

            actual2.IsFail.Should().BeTrue();
            actual2.Error.Should().Be(error);

            actual3.IsFail.Should().BeTrue();
            actual3.Error.Should().Be(error);
        }
    }

    public class Implicit_Operator : EnrichedResultFixture
    {
        [Fact]
        public void Should_Implicit_Convert_From_EnrichedResult_Int()
        {
            var value = Create<int>();

            var result = EnrichedResult.Success(value);

            int actual = result;

            actual.Should().Be(value);
        }

        [Fact]
        public void Should_Implicit_Convert_From_EnrichedResult_Nullable_Int()
        {
            var value = Create<int?>();

            var result = EnrichedResult.Success(value);

            int? actual = result;

            actual.Should().Be(value);
        }

        [Fact]
        public void Should_Implicit_Convert_From_EnrichedResult_String()
        {
            var value = Create<string>();

            var result = EnrichedResult.Success(value);

            string actual = result!;

            actual.Should().Be(value);
        }
    }

    public class Explicit_Operator : EnrichedResultFixture
    {
        [Fact]
        public void Should_Explicit_Convert_To_EnrichedResult_Int()
        {
            var value = Create<int>();

            var actual = (EnrichedResult<int>) value;

            actual.Value.Should().Be(value);
        }

        [Fact]
        public void Should_Explicit_Convert_To_EnrichedResult_Nullable_Int()
        {
            var value = Create<int?>();

            var actual = (EnrichedResult<int?>) value;

            actual.Value.Should().Be(value);
        }

        [Fact]
        public void Should_Explicit_Convert_To_EnrichedResult_String()
        {
            var value = Create<string>();

            var actual = (EnrichedResult<string>) value;

            actual.Value.Should().Be(value);
        }
    }
}