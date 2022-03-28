using AllOverIt.Caching;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Caching
{
    public class GenericCacheKeyFixture : FixtureBase
    {
        private class CacheKeyDummy1 : GenericCacheKey<string>
        {
            public CacheKeyDummy1(string key1)
                : base(key1)
            {
            }
        }

        private class CacheKeyDummy2 : GenericCacheKey<string, int?>
        {
            public CacheKeyDummy2(string key1, int? key2)
                : base(key1, key2)
            {
            }
        }

        private class CacheKeyDummy3 : GenericCacheKey<string, int?, bool?>
        {
            public CacheKeyDummy3(string key1, int? key2, bool? key3)
                : base(key1, key2, key3)
            {
            }
        }

        private class CacheKeyDummy4 : GenericCacheKey<string, int?, bool?, double?>
        {
            public CacheKeyDummy4(string key1, int? key2, bool? key3, double? key4)
                : base(key1, key2, key3, key4)
            {
            }
        }

        public class GenericCacheKey_One : GenericCacheKeyFixture
        {
            public class Constructor : GenericCacheKey_One
            {
                [Fact]
                public void Should_Not_Throw_When_Key_Null()
                {
                    Invoking(() => new CacheKeyDummy1(null))
                        .Should()
                        .NotThrow();
                }
            }
        }

        public class GenericCacheKey_Two : GenericCacheKeyFixture
        {
            public class Constructor : GenericCacheKey_Two
            {
                [Fact]
                public void Should_Not_Throw_When_Key1_Null()
                {
                    Invoking(() => new CacheKeyDummy2(null, Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key2_Null()
                {
                    Invoking(() => new CacheKeyDummy2(Create<string>(), null))
                        .Should()
                        .NotThrow();
                }
            }
        }

        public class GenericCacheKey_Three : GenericCacheKeyFixture
        {
            public class Constructor : GenericCacheKey_Three
            {
                [Fact]
                public void Should_Not_Throw_When_Key1_Null()
                {
                    Invoking(() => new CacheKeyDummy3(null, Create<int>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key2_Null()
                {
                    Invoking(() => new CacheKeyDummy3(Create<string>(), null, Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key3_Null()
                {
                    Invoking(() => new CacheKeyDummy3(Create<string>(), Create<int>(), null))
                        .Should()
                        .NotThrow();
                }
            }
        }

        public class GenericCacheKey_Four : GenericCacheKeyFixture
        {
            public class Constructor : GenericCacheKey_Four
            {
                [Fact]
                public void Should_Not_Throw_When_Key1_Null()
                {
                    Invoking(() => new CacheKeyDummy4(null, Create<int>(), Create<bool>(), Create<double>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key2_Null()
                {
                    Invoking(() => new CacheKeyDummy4(Create<string>(), null, Create<bool>(), Create<double>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key3_Null()
                {
                    Invoking(() => new CacheKeyDummy4(Create<string>(), Create<int>(), null, Create<double>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key4_Null()
                {
                    Invoking(() => new CacheKeyDummy4(Create<string>(), Create<int>(), Create<bool>(), null))
                        .Should()
                        .NotThrow();
                }
            }
        }
    }
}