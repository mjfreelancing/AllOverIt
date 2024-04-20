#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Patterns.Result;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Result;

public partial class EnrichedErrorFixture : FixtureBase
{
    private enum DummyErrorType
    {
        Dummy1,
        Dummy2,
        Dummy3,
        Dummy4,
        Dummy5
    }

    public class Constructor_ErrorType : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_ErrorType()
        {
            var errorType = Create<DummyErrorType>();

            var actual = new EnrichedError<DummyErrorType>(errorType);

            var expected = new
            {
                ErrorType = errorType,
                Type = errorType.ToString(),
                Code = (string?) null,
                Description = (string?) null
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Constructor_ErrorType_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_ErrorType_Description()
        {
            var errorType = Create<DummyErrorType>();
            var description = Create<string>();

            var actual = new EnrichedError<DummyErrorType>(errorType, description);

            var expected = new
            {
                ErrorType = errorType,
                Type = errorType.ToString(),
                Code = (string?) null,
                Description = description
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Constructor_ErrorType_Code_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_ErrorType_Description()
        {
            var errorType = Create<DummyErrorType>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = new EnrichedError<DummyErrorType>(errorType, code, description);

            var expected = new
            {
                ErrorType = errorType,
                Type = errorType.ToString(),
                Code = code,
                Description = description
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }
}