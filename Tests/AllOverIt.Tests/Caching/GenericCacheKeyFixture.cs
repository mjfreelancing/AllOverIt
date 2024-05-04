using AllOverIt.Caching;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Tests.Caching
{
    public class GenericCacheKeyFixture : FixtureBase
    {
        private record DummyCacheKey1 : GenericCacheKey<string>
        {
            public DummyCacheKey1(string key1)
                : base(key1)
            {
            }
        }

        private record DummyCacheKey2 : GenericCacheKey<string, int?>
        {
            public DummyCacheKey2(string key1, int? key2)
                : base(key1, key2)
            {
            }
        }

        private record DummyCacheKey3 : GenericCacheKey<string, int?, bool?>
        {
            public DummyCacheKey3(string key1, int? key2, bool? key3)
                : base(key1, key2, key3)
            {
            }
        }

        private record DummyCacheKey4 : GenericCacheKey<string, int?, bool?, double?>
        {
            public DummyCacheKey4(string key1, int? key2, bool? key3, double? key4)
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
                    Invoking(() => new DummyCacheKey1(null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Set_Key()
                {
                    var key1 = Create<string>();

                    var actual = new DummyCacheKey1(key1);

                    actual.Key1.Should().Be(key1);
                }

                [Fact]
                public void Should_Compare_Equality()
                {
                    var key1 = Create<string>();

                    var cacheKey1 = new DummyCacheKey1(key1);
                    var cacheKey2 = new DummyCacheKey1(key1);
                    var cacheKey3 = new DummyCacheKey1(Create<string>());

                    (cacheKey1 == cacheKey2).Should().BeTrue();
                    (cacheKey1 == cacheKey3).Should().BeFalse();

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
                    cacheKey1.Equals(cacheKey3).Should().BeFalse();
                }

                [Fact]
                public void Should_Set_Values()         // required for full code coverage
                {
                    var cacheKey1 = Create<DummyCacheKey1>();
                    var cacheKey2 = Create<DummyCacheKey1>();

                    cacheKey1.Equals(cacheKey2).Should().BeFalse();

                    cacheKey2 = cacheKey2 with
                    {
                        Key = cacheKey1.Key,
                        Key1 = cacheKey1.Key1
                    };

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
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
                    Invoking(() => new DummyCacheKey2(null, Create<int>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key2_Null()
                {
                    Invoking(() => new DummyCacheKey2(Create<string>(), null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Set_Keys()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();

                    var actual = new DummyCacheKey2(key1, key2);

                    actual.Key1.Should().Be(key1);
                    actual.Key2.Should().Be(key2);
                }

                [Fact]
                public void Should_Deconstruct()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();

                    var (actualKey1, actualKey2) = new DummyCacheKey2(key1, key2);

                    actualKey1.Should().Be(key1);
                    actualKey2.Should().Be(key2);
                }

                [Fact]
                public void Should_Compare_Equality()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();

                    var cacheKey1 = new DummyCacheKey2(key1, key2);
                    var cacheKey2 = new DummyCacheKey2(key1, key2);
                    var cacheKey3 = new DummyCacheKey2(Create<string>(), Create<int>());

                    (cacheKey1 == cacheKey2).Should().BeTrue();
                    (cacheKey1 == cacheKey3).Should().BeFalse();

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
                    cacheKey1.Equals(cacheKey3).Should().BeFalse();
                }

                [Fact]
                public void Should_Set_Values()         // required for full code coverage
                {
                    var cacheKey1 = Create<DummyCacheKey2>();
                    var cacheKey2 = Create<DummyCacheKey2>();

                    cacheKey1.Equals(cacheKey2).Should().BeFalse();

                    cacheKey2 = cacheKey2 with
                    {
                        Key = cacheKey1.Key,
                        Key1 = cacheKey1.Key1,
                        Key2 = cacheKey1.Key2
                    };

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
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
                    Invoking(() => new DummyCacheKey3(null, Create<int>(), Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key2_Null()
                {
                    Invoking(() => new DummyCacheKey3(Create<string>(), null, Create<bool>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key3_Null()
                {
                    Invoking(() => new DummyCacheKey3(Create<string>(), Create<int>(), null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Set_Keys()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();
                    var key3 = Create<bool>();

                    var actual = new DummyCacheKey3(key1, key2, key3);

                    actual.Key1.Should().Be(key1);
                    actual.Key2.Should().Be(key2);
                    actual.Key3.Should().Be(key3);
                }

                [Fact]
                public void Should_Deconstruct()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();
                    var key3 = Create<bool>();

                    var (actualKey1, actualKey2, actualKey3) = new DummyCacheKey3(key1, key2, key3);

                    actualKey1.Should().Be(key1);
                    actualKey2.Should().Be(key2);
                    actualKey3.Should().Be(key3);
                }

                [Fact]
                public void Should_Compare_Equality()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();
                    var key3 = Create<bool>();

                    var cacheKey1 = new DummyCacheKey3(key1, key2, key3);
                    var cacheKey2 = new DummyCacheKey3(key1, key2, key3);
                    var cacheKey3 = new DummyCacheKey3(Create<string>(), Create<int>(), Create<bool>());

                    (cacheKey1 == cacheKey2).Should().BeTrue();
                    (cacheKey1 == cacheKey3).Should().BeFalse();

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
                    cacheKey1.Equals(cacheKey3).Should().BeFalse();
                }

                [Fact]
                public void Should_Set_Values()         // required for full code coverage
                {
                    var cacheKey1 = Create<DummyCacheKey3>();
                    var cacheKey2 = Create<DummyCacheKey3>();

                    cacheKey1.Equals(cacheKey2).Should().BeFalse();

                    cacheKey2 = cacheKey2 with
                    {
                        Key = cacheKey1.Key,
                        Key1 = cacheKey1.Key1,
                        Key2 = cacheKey1.Key2,
                        Key3 = cacheKey1.Key3
                    };

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
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
                    Invoking(() => new DummyCacheKey4(null, Create<int>(), Create<bool>(), Create<double>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key2_Null()
                {
                    Invoking(() => new DummyCacheKey4(Create<string>(), null, Create<bool>(), Create<double>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key3_Null()
                {
                    Invoking(() => new DummyCacheKey4(Create<string>(), Create<int>(), null, Create<double>()))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Not_Throw_When_Key4_Null()
                {
                    Invoking(() => new DummyCacheKey4(Create<string>(), Create<int>(), Create<bool>(), null))
                        .Should()
                        .NotThrow();
                }

                [Fact]
                public void Should_Set_Keys()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();
                    var key3 = Create<bool>();
                    var key4 = Create<double>();

                    var actual = new DummyCacheKey4(key1, key2, key3, key4);

                    actual.Key1.Should().Be(key1);
                    actual.Key2.Should().Be(key2);
                    actual.Key3.Should().Be(key3);
                    actual.Key4.Should().Be(key4);
                }

                [Fact]
                public void Should_Deconstruct()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();
                    var key3 = Create<bool>();
                    var key4 = Create<double>();

                    var (actualKey1, actualKey2, actualKey3, actualKey4) = new DummyCacheKey4(key1, key2, key3, key4);

                    actualKey1.Should().Be(key1);
                    actualKey2.Should().Be(key2);
                    actualKey3.Should().Be(key3);
                    actualKey4.Should().Be(key4);
                }

                [Fact]
                public void Should_Compare_Equality()
                {
                    var key1 = Create<string>();
                    var key2 = Create<int>();
                    var key3 = Create<bool>();
                    var key4 = Create<double>();

                    var cacheKey1 = new DummyCacheKey4(key1, key2, key3, key4);
                    var cacheKey2 = new DummyCacheKey4(key1, key2, key3, key4);
                    var cacheKey3 = new DummyCacheKey4(Create<string>(), Create<int>(), Create<bool>(), Create<double>());

                    (cacheKey1 == cacheKey2).Should().BeTrue();
                    (cacheKey1 == cacheKey3).Should().BeFalse();

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
                    cacheKey1.Equals(cacheKey3).Should().BeFalse();
                }

                [Fact]
                public void Should_Set_Values()         // required for full code coverage
                {
                    var cacheKey1 = Create<DummyCacheKey4>();
                    var cacheKey2 = Create<DummyCacheKey4>();

                    cacheKey1.Equals(cacheKey2).Should().BeFalse();

                    cacheKey2 = cacheKey2 with
                    {
                        Key = cacheKey1.Key,
                        Key1 = cacheKey1.Key1,
                        Key2 = cacheKey1.Key2,
                        Key3 = cacheKey1.Key3,
                        Key4 = cacheKey1.Key4
                    };

                    cacheKey1.Equals(cacheKey2).Should().BeTrue();
                }
            }
        }
    }
}