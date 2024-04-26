using AllOverIt.Caching;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Tests.Caching
{
    public class GenericCacheKeyBaseFixture : FixtureBase
    {
        private record CacheKeyDummy : GenericCacheKeyBase
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