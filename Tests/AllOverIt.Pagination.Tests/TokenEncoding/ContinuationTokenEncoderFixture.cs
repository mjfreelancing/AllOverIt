using AllOverIt.Collections;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenEncoderFixture : FixtureBase
    {
        private sealed class EntityDummy
        {
            public int Id { get; init; }
            public string Name { get; init; }
        }

        private readonly IReadOnlyCollection<IColumnDefinition> _columns;

        public ContinuationTokenEncoderFixture()
        {
            _columns = new IColumnDefinition[]
            {
                new ColumnDefinition<EntityDummy, string>(typeof(EntityDummy).GetProperty(nameof(EntityDummy.Name)), Create<bool>()),
                new ColumnDefinition<EntityDummy, int>(typeof(EntityDummy).GetProperty(nameof(EntityDummy.Id)), Create<bool>())
            };
        }

        public class Constructor : ContinuationTokenEncoderFixture
        {
            [Fact]
            public void Should_Throw_When_Columns_Null()
            {
                Invoking(() =>
                {
                    _ = new ContinuationTokenEncoder(null, Create<PaginationDirection>(), A.Fake<IContinuationTokenSerializer>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("columns");
            }

            [Fact]
            public void Should_Throw_When_Columns_Empty()
            {
                Invoking(() =>
                {
                    _ = new ContinuationTokenEncoder(new List<IColumnDefinition>(), Create<PaginationDirection>(), A.Fake<IContinuationTokenSerializer>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("columns");
            }
        }

        public class EncodePreviousPage_Collection : ContinuationTokenEncoderFixture
        {
            [Fact]
            public void Should_Throw_When_References_Null()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward, A.Fake<IContinuationTokenSerializer>());

                Invoking(() =>
                {
                    _ = encoder.EncodePreviousPage((IReadOnlyCollection<EntityDummy>) null);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("At least one reference entity is required to create a continuation token.");
            }

            [Fact]
            public void Should_Throw_When_References_Empty()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward, A.Fake<IContinuationTokenSerializer>());

                Invoking(() =>
                {
                    _ = encoder.EncodePreviousPage(Collection.EmptyReadOnly<EntityDummy>());
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("At least one reference entity is required to create a continuation token.");
            }


            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Token(PaginationDirection paginationDirection)
            {
                var entities = CreateMany<EntityDummy>();

                var expectedReference = paginationDirection switch
                {
                    PaginationDirection.Forward => entities.FirstElement(),
                    PaginationDirection.Backward => entities.LastElement(),
                    _ => throw new InvalidOperationException()
                };

                var expected = new
                {
                    Direction = paginationDirection.Reverse(),
                    Values = new object[] { expectedReference.Name, expectedReference.Id }
                };

                var serializerFake = A.Fake<IContinuationTokenSerializer>();

                IContinuationToken actualToken = default;

                A.CallTo(() => serializerFake.Serialize(A<IContinuationToken>.Ignored))
                    .Invokes(call => actualToken = call.Arguments.Get<IContinuationToken>(0));

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializerFake);

                _ = encoder.EncodePreviousPage(entities);

                expected.Should().BeEquivalentTo(actualToken);
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Decode_Token(PaginationDirection paginationDirection)
            {
                var entities = CreateMany<EntityDummy>();

                var expectedReference = paginationDirection switch
                {
                    PaginationDirection.Forward => entities.FirstElement(),
                    PaginationDirection.Backward => entities.LastElement(),
                    _ => throw new InvalidOperationException()
                };

                var expected = new
                {
                    Direction = paginationDirection.Reverse(),
                    Values = new object[] { expectedReference.Name, expectedReference.Id }
                };

                var serializer = new ContinuationTokenSerializer(Create<ContinuationTokenOptions>());
                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);

                var actual = encoder.EncodePreviousPage(entities);

                actual.Should().NotBeNullOrEmpty();

                var decoded = encoder.Decode(actual);

                expected.Should().BeEquivalentTo(decoded);
            }

            [Fact]
            public void Encoding_With_Compression_Should_Not_Match_Without_Compression()
            {
                var entities = CreateMany<EntityDummy>();
                var paginationDirection = Create<PaginationDirection>();

                var serializer = new ContinuationTokenSerializer(new ContinuationTokenOptions { UseCompression = true });
                var compressionEncoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);
                var compressed = compressionEncoder.EncodeNextPage(entities[0]);

                serializer = new ContinuationTokenSerializer(new ContinuationTokenOptions { UseCompression = true });
                var nonCompressionEncoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);
                var nonCompressed = nonCompressionEncoder.EncodeNextPage(entities[0]);

                compressed.Should().NotBeSameAs(nonCompressed);
            }
        }

        public class EncodeNextPage_Collection : ContinuationTokenEncoderFixture
        {
            [Fact]
            public void Should_Throw_When_References_Null()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward, A.Fake<IContinuationTokenSerializer>());

                Invoking(() =>
                {
                    _ = encoder.EncodeNextPage((IReadOnlyCollection<EntityDummy>) null);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("At least one reference entity is required to create a continuation token.");
            }

            [Fact]
            public void Should_Throw_When_References_Empty()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward, A.Fake<IContinuationTokenSerializer>());

                Invoking(() =>
                {
                    _ = encoder.EncodeNextPage(Collection.EmptyReadOnly<EntityDummy>());
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("At least one reference entity is required to create a continuation token.");
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Token(PaginationDirection paginationDirection)
            {
                var entities = CreateMany<EntityDummy>();

                var expectedReference = paginationDirection switch
                {
                    PaginationDirection.Forward => entities.LastElement(),
                    PaginationDirection.Backward => entities.FirstElement(),
                    _ => throw new InvalidOperationException()
                };

                var expected = new
                {
                    Direction = paginationDirection,
                    Values = new object[] { expectedReference.Name, expectedReference.Id }
                };

                var serializerFake = A.Fake<IContinuationTokenSerializer>();

                IContinuationToken actualToken = default;

                A.CallTo(() => serializerFake.Serialize(A<IContinuationToken>.Ignored))
                    .Invokes(call => actualToken = call.Arguments.Get<IContinuationToken>(0));

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializerFake);

                _ = encoder.EncodeNextPage(entities);

                expected.Should().BeEquivalentTo(actualToken);
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Decode_Token(PaginationDirection paginationDirection)
            {
                var entities = CreateMany<EntityDummy>();

                var expectedReference = paginationDirection switch
                {
                    PaginationDirection.Forward => entities.LastElement(),
                    PaginationDirection.Backward => entities.FirstElement(),
                    _ => throw new InvalidOperationException()
                };

                var expected = new
                {
                    Direction = paginationDirection,
                    Values = new object[] { expectedReference.Name, expectedReference.Id }
                };

                var serializer = new ContinuationTokenSerializer(Create<ContinuationTokenOptions>());
                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);

                var actual = encoder.EncodeNextPage(entities);

                actual.Should().NotBeNullOrEmpty();

                var decoded = encoder.Decode(actual);

                expected.Should().BeEquivalentTo(decoded);
            }
        }

        public class EncodePreviousPage_Object : ContinuationTokenEncoderFixture
        {
            [Fact]
            public void Should_Throw_When_References_Null()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward, A.Fake<IContinuationTokenSerializer>());

                Invoking(() =>
                {
                    _ = encoder.EncodePreviousPage( null);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("A reference entity is required to create a continuation token.");
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Token(PaginationDirection paginationDirection)
            {
                var entity = Create<EntityDummy>();

                var expected = new
                {
                    Direction = paginationDirection.Reverse(),
                    Values = new object[] { entity.Name, entity.Id }
                };

                var serializerFake = A.Fake<IContinuationTokenSerializer>();

                IContinuationToken actualToken = default;

                A.CallTo(() => serializerFake.Serialize(A<IContinuationToken>.Ignored))
                    .Invokes(call => actualToken = call.Arguments.Get<IContinuationToken>(0));

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializerFake);

                _ = encoder.EncodePreviousPage(entity);

                expected.Should().BeEquivalentTo(actualToken);
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Decode_Token(PaginationDirection paginationDirection)
            {
                var entity = Create<EntityDummy>();

                var expected = new
                {
                    Direction = paginationDirection.Reverse(),
                    Values = new object[] { entity.Name, entity.Id }
                };

                var serializer = new ContinuationTokenSerializer(Create<ContinuationTokenOptions>());
                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);

                var actual = encoder.EncodePreviousPage(entity);

                actual.Should().NotBeNullOrEmpty();

                var decoded = encoder.Decode(actual);

                expected.Should().BeEquivalentTo(decoded);
            }
        }

        public class EncodeNextPage_Object : ContinuationTokenEncoderFixture
        {
            [Fact]
            public void Should_Throw_When_References_Null()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward, A.Fake<IContinuationTokenSerializer>());

                Invoking(() =>
                {
                    _ = encoder.EncodeNextPage( null);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("A reference entity is required to create a continuation token.");
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Token(PaginationDirection paginationDirection)
            {
                var entity = Create<EntityDummy>();

                var expected = new
                {
                    Direction = paginationDirection,
                    Values = new object[] { entity.Name, entity.Id }
                };

                var serializerFake = A.Fake<IContinuationTokenSerializer>();

                IContinuationToken actualToken = default;

                A.CallTo(() => serializerFake.Serialize(A<IContinuationToken>.Ignored))
                    .Invokes(call => actualToken = call.Arguments.Get<IContinuationToken>(0));

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializerFake);

                _ = encoder.EncodeNextPage(entity);

                expected.Should().BeEquivalentTo(actualToken);
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Decode_Token(PaginationDirection paginationDirection)
            {
                var entity = Create<EntityDummy>();

                var expected = new
                {
                    Direction = paginationDirection,
                    Values = new object[] { entity.Name, entity.Id }
                };

                var serializer = new ContinuationTokenSerializer(Create<ContinuationTokenOptions>());
                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);

                var actual = encoder.EncodeNextPage(entity);

                actual.Should().NotBeNullOrEmpty();

                var decoded = encoder.Decode(actual);

                expected.Should().BeEquivalentTo(decoded);
            }
        }

        public class EncodeFirstPage : ContinuationTokenEncoderFixture
        {
            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_First_Page(PaginationDirection paginationDirection)
            {
                var serializer = new ContinuationTokenSerializer(Create<ContinuationTokenOptions>());
                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);

                var actual = encoder.EncodeFirstPage();

                actual.Should().BeEmpty();
            }
        }

        public class EncodeLastPage : ContinuationTokenEncoderFixture
        {
            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Last_Page(PaginationDirection paginationDirection)
            {
                var expected = new
                {
                    Direction = paginationDirection.Reverse(),
                    Values = (object[]) null
                };

                var serializerFake = A.Fake<IContinuationTokenSerializer>();

                IContinuationToken actualToken = default;

                A.CallTo(() => serializerFake.Serialize(A<IContinuationToken>.Ignored))
                    .Invokes(call => actualToken = call.Arguments.Get<IContinuationToken>(0));

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializerFake);

                _ = encoder.EncodeLastPage();

                expected.Should().BeEquivalentTo(actualToken);
            }

            [Theory]
            [InlineData(PaginationDirection.Forward)]
            [InlineData(PaginationDirection.Backward)]
            public void Should_Encode_Decode_Last_Page(PaginationDirection paginationDirection)
            {
                var expected = new
                {
                    Direction = paginationDirection.Reverse(),
                    Values = (object[]) null
                };

                var serializer = new ContinuationTokenSerializer(Create<ContinuationTokenOptions>());
                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection, serializer);

                var actual = encoder.EncodeLastPage();

                actual.Should().NotBeNullOrEmpty();

                var decoded = encoder.Decode(actual);

                expected.Should().BeEquivalentTo(decoded);
            }
        }
    }
}