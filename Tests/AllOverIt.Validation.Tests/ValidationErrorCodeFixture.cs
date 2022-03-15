using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

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
                nameof(ValidationErrorCode.OutOfRange)
            };

            // If this test fails then other tests may need to be added to check all error codes are returned
            expected
                .Should()
                .BeEquivalentTo(typeof(ValidationErrorCode).GetEnumNames());
        }
    }
}
