using AllOverIt.Fixture;
using AllOverIt.Pagination.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Pagination.Tests.Extensions
{
    public class PaginationDirectionExtensionsFixture : FixtureBase
    {
        public class Reverse : PaginationDirectionExtensionsFixture
        {
            [Theory]
            [InlineData(PaginationDirection.Forward, PaginationDirection.Backward)]
            [InlineData(PaginationDirection.Backward, PaginationDirection.Forward)]
            public void Should_Return_Reverse_Direction(PaginationDirection direction, PaginationDirection expected)
            {
                var actual = PaginationDirectionExtensions.Reverse(direction);

                actual.Should().Be(expected);
            }
        }
    }
}