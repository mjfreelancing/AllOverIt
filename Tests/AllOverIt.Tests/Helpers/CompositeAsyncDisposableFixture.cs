using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class CompositeAsyncDisposableFixture : FixtureBase
    {
        public class Constructor : CompositeAsyncDisposableFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        _ = new CompositeAsyncDisposable();
                    })
                    .Should()
                    .NotThrow();
            }
        }
    }
}