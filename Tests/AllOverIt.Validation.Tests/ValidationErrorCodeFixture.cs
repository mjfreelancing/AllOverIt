using AllOverIt.Fixture;
using AllOverIt.Shouldly;

namespace AllOverIt.Validation.Tests
{
    public class ValidationErrorCodeFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Expected_Error_Codes()
        {
            var expected = new[]
            {
                nameof(ValidationErrorCode.Required),
                nameof(ValidationErrorCode.NotEmpty),
                nameof(ValidationErrorCode.OutOfRange),
                nameof(ValidationErrorCode.Duplicate)
            };

            // If this test fails then other tests may need to be added to check all error codes are returned
            expected
                .ShouldBe(typeof(ValidationErrorCode).GetEnumNames());
        }
    }
}









