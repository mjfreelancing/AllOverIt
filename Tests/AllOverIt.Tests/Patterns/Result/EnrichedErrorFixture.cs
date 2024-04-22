#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Patterns.Result;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Result;
public partial class EnrichedErrorFixture : FixtureBase
{
    private readonly EnrichedError[] EnrichedErrors =
    [
        EnrichedResult.Fail().Error!,
        EnrichedResult.Fail<int>().Error!,
        EnrichedResult.Fail<double>().Error!,
    ];

    public class Constructor : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Defaults()
        {
            var actual = new EnrichedError();

            var expected = new
            {
                Type = (string?) null,
                Code = (string?) null,
                Description = (string?) null
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Constructor_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Description()
        {
            var description = Create<string>();

            var actual = new EnrichedError(description);

            var expected = new
            {
                Type = (string?) null,
                Code = (string?) null,
                Description = description
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Constructor_Type_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Type_Description()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = new EnrichedError(type, description);

            var expected = new
            {
                Type = type,
                Code = (string?) null,
                Description = description
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Constructor_Type_Code_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Type_Code_Description()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = new EnrichedError(type, code, description);

            var expected = new
            {
                Type = type,
                Code = code,
                Description = description
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Aggregate : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Errors()
        {
            var actual = EnrichedError.Aggregate(EnrichedErrors);

            var expected = new
            {
                Type = (string?) null,
                Code = (string?) null,
                Description = (string?) null,
                Errors = EnrichedErrors
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Aggregate_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Description_Errors()
        {
            var description = Create<string>();

            var actual = EnrichedError.Aggregate(description, EnrichedErrors);

            var expected = new
            {
                Type = (string?) null,
                Code = (string?) null,
                Description = description,
                Errors = EnrichedErrors
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Aggregate_Type_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Type_Description_Errors()
        {
            var type = Create<string>();
            var description = Create<string>();

            var actual = EnrichedError.Aggregate(type, description, EnrichedErrors);

            var expected = new
            {
                Type = type,
                Code = (string?) null,
                Description = description,
                Errors = EnrichedErrors
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }

    public class Aggregate_Type_Code_Description : EnrichedErrorFixture
    {
        [Fact]
        public void Should_Set_Type_Code_Description_Errors()
        {
            var type = Create<string>();
            var code = Create<string>();
            var description = Create<string>();

            var actual = EnrichedError.Aggregate(type, code, description, EnrichedErrors);

            var expected = new
            {
                Type = type,
                Code = code,
                Description = description,
                Errors = EnrichedErrors
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }
}