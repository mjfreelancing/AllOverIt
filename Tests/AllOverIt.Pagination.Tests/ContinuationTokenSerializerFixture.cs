using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
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
                    _ = ContinuationTokenSerializer.Serialize(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("continuationToken");
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

                var notCompressed = ContinuationTokenSerializer.Serialize(continuationToken, false);
                var compressed = ContinuationTokenSerializer.Serialize(continuationToken, true);

                notCompressed.Length
                    .Should()
                    .BeGreaterThan(compressed.Length);
            }
        }

        public class Deserialize : ContinuationTokenSerializerFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    var actual = ContinuationTokenSerializer.Deserialize(null);

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
                    var actual = ContinuationTokenSerializer.Deserialize(string.Empty);

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
                    var actual = ContinuationTokenSerializer.Deserialize(" ");

                    actual.Should().BeSameAs(ContinuationToken.None);
                })
                    .Should()
                    .NotThrow();
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

                var encoded = ContinuationTokenSerializer.Serialize(continuationToken, useCompression);

                var decoded = ContinuationTokenSerializer.Deserialize(encoded, useCompression);

                decoded.Should().BeEquivalentTo(continuationToken);
            }
        }
    }
}