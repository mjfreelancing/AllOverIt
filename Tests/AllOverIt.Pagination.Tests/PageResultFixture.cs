using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Pagination.Tests
{
    public class PageResultFixture : FixtureBase
    {
        public class CreateFrom : PageResultFixture
        {
            [Fact]
            public void Should_Create_From_Other_Results()
            {
                var firstResults = Create<PageResult<string>>();
                var results = CreateMany<int>(firstResults.Results.Count);

                var actual = PageResult<int>.CreateFrom(firstResults, results);

                var expected = new
                {
                    Results = results.AsReadOnlyCollection(),
                    TotalCount = firstResults.TotalCount,
                    CurrentToken = firstResults.CurrentToken,
                    PreviousToken = firstResults.PreviousToken,
                    NextToken = firstResults.NextToken
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}