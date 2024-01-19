using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Pagination.Tests
{
    public class QueryPaginatorConfigurationFixture : FixtureBase
    {
        [Fact]
        public void Should_Default_Construct()
        {
            var actual = new QueryPaginatorConfiguration();

            var expected = new
            {
                PageSize = 0,
                PaginationDirection = PaginationDirection.Forward,
                UseParameterizedQueries = true,
                ContinuationTokenOptions = new ContinuationTokenOptions()
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }
}