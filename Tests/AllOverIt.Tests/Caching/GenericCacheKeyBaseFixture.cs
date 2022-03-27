using AllOverIt.Caching;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Caching
{
    public class GenericCacheKeyBaseFixture : FixtureBase
    {
        private class CacheKeyDummy : GenericCacheKeyBase
        {
            public CacheKeyDummy(object key)
                : base(key)
            {
            }
        }

        public class Constructor : GenericCacheKeyBaseFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Key_Null()
            {
                Invoking(() => new CacheKeyDummy(null))
                    .Should()
                    .NotThrow();
            }
        }
    }
}