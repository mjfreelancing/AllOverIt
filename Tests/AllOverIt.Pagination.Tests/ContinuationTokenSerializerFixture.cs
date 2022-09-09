using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Pagination.Tests
{
    public class ContinuationTokenSerializerFixture : FixtureBase
    {
        public class Serialize : ContinuationTokenSerializerFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    _ = ContinuationTokenSerializer.Serialize(null, A.Fake<IContinuationTokenOptions>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("continuationToken");
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _ = ContinuationTokenSerializer.Serialize(Create<ContinuationToken>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Serialize_Different()
            {
                var continuationToken1 = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var continuationToken2 = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var serialized1 = ContinuationTokenSerializer.Serialize(continuationToken1, ContinuationTokenOptions.None);
                var serialized2 = ContinuationTokenSerializer.Serialize(continuationToken2, ContinuationTokenOptions.None);

                serialized1.Should().NotBe(serialized2);
            }

            [Fact]
            public void Should_Compress_Smaller()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var notCompressed = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { UseCompression = false });
                var compressed = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { UseCompression = true });

                notCompressed.Length
                    .Should()
                    .BeGreaterThan(compressed.Length);
            }

            [Fact]
            public void Should_Encode_Hash_Value_SHA1_Different_To_None()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serialized1 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.None });
                var serialized2 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.Sha1 });

                serialized1.Should().NotBe(serialized2);
            }

            [Fact]
            public void Should_Encode_Hash_Value_SHA256_Different_To_None()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serialized1 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.None });
                var serialized2 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.Sha256 });

                serialized1.Should().NotBe(serialized2);
            }

            [Fact]
            public void Should_Encode_Hash_Value_SHA384_Different_To_None()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serialized1 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.None });
                var serialized2 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.Sha384 });

                serialized1.Should().NotBe(serialized2);
            }

            [Fact]
            public void Should_Encode_Hash_Value_SHA512_Different_To_None()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serialized1 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.None });
                var serialized2 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { HashMode = ContinuationTokenHashMode.Sha512 });

                serialized1.Should().NotBe(serialized2);
            }
        }

        public class Deserialize : ContinuationTokenSerializerFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    var actual = ContinuationTokenSerializer.Deserialize(null, A.Fake<IContinuationTokenOptions>());

                    actual.Should().BeSameAs(ContinuationToken.None);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Token_Empty()
            {
                Invoking(() =>
                {
                    var actual = ContinuationTokenSerializer.Deserialize(string.Empty, A.Fake<IContinuationTokenOptions>());

                    actual.Should().BeSameAs(ContinuationToken.None);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Token_Whitespace()
            {
                Invoking(() =>
                {
                    var actual = ContinuationTokenSerializer.Deserialize(" ", A.Fake<IContinuationTokenOptions>());

                    actual.Should().BeSameAs(ContinuationToken.None);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _ = ContinuationTokenSerializer.Deserialize(Create<string>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("options");
            }
        }

        public class Serialize_Deserialize : ContinuationTokenSerializerFixture
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Serialize_Deserialize(bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var tokenOptions = new ContinuationTokenOptions
                { 
                    HashMode = ContinuationTokenHashMode.None,
                    UseCompression = useCompression
                };

                var encoded = ContinuationTokenSerializer.Serialize(continuationToken, tokenOptions);

                var decoded = ContinuationTokenSerializer.Deserialize(encoded, tokenOptions);

                decoded.Should().BeEquivalentTo(continuationToken);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Serialize_Deserialize_SHA1(bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var tokenOptions = new ContinuationTokenOptions
                {
                    HashMode = ContinuationTokenHashMode.Sha1,
                    UseCompression = useCompression
                };

                var encoded = ContinuationTokenSerializer.Serialize(continuationToken, tokenOptions);

                var decoded = ContinuationTokenSerializer.Deserialize(encoded, tokenOptions);

                decoded.Should().BeEquivalentTo(continuationToken);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Serialize_Deserialize_SHA256(bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var tokenOptions = new ContinuationTokenOptions
                {
                    HashMode = ContinuationTokenHashMode.Sha256,
                    UseCompression = useCompression
                };

                var encoded = ContinuationTokenSerializer.Serialize(continuationToken, tokenOptions);

                var decoded = ContinuationTokenSerializer.Deserialize(encoded, tokenOptions);

                decoded.Should().BeEquivalentTo(continuationToken);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Serialize_Deserialize_SHA384(bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var tokenOptions = new ContinuationTokenOptions
                {
                    HashMode = ContinuationTokenHashMode.Sha384,
                    UseCompression = useCompression
                };

                var encoded = ContinuationTokenSerializer.Serialize(continuationToken, tokenOptions);

                var decoded = ContinuationTokenSerializer.Deserialize(encoded, tokenOptions);

                decoded.Should().BeEquivalentTo(continuationToken);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Serialize_Deserialize_SHA512(bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var tokenOptions = new ContinuationTokenOptions
                {
                    HashMode = ContinuationTokenHashMode.Sha512,
                    UseCompression = useCompression
                };

                var encoded = ContinuationTokenSerializer.Serialize(continuationToken, tokenOptions);

                var decoded = ContinuationTokenSerializer.Deserialize(encoded, tokenOptions);

                decoded.Should().BeEquivalentTo(continuationToken);
            }
        }
    }
}