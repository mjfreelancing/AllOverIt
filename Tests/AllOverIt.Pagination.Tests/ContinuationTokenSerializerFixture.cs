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
            public void Should_Encode_Hash_Value_Different_To_None()
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

                var serialized1 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { IncludeHash = false });
                var serialized2 = ContinuationTokenSerializer.Serialize(continuationToken, new ContinuationTokenOptions { IncludeHash = true });

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
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void Should_Serialize_Deserialize(bool includeHash, bool useCompression)
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
                    IncludeHash = includeHash,
                    UseCompression = useCompression
                };

                var encoded = ContinuationTokenSerializer.Serialize(continuationToken, tokenOptions);

                var decoded = ContinuationTokenSerializer.Deserialize(encoded, tokenOptions);

                decoded.Should().BeEquivalentTo(continuationToken);
            }
        }
    }
}