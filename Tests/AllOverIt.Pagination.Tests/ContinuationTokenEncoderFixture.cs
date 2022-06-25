using AllOverIt.Collections;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Pagination.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Pagination.Tests
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
            public void Should_Throw_When_Columns_null()
            {
                Invoking(() =>
                {
                    _ = new ContinuationTokenEncoder(null, Create<PaginationDirection>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("columns");
            }
        }

        public class EncodePreviousPage_Collection : ContinuationTokenEncoderFixture
        {
            [Fact]
            public void Should_Throw_When_References_Null()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward);

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
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward);

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

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection);

                var actual = encoder.EncodePreviousPage(entities);

                actual.Should().NotBeNullOrEmpty();

                var decoded = encoder.Decode(actual);

                expected.Should().BeEquivalentTo(decoded);
            }
        }

        public class EncodeNextPage_Collection : ContinuationTokenEncoderFixture
        {
            [Fact]
            public void Should_Throw_When_References_Null()
            {
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward);

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
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward);

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

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection);

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
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward);

                Invoking(() =>
                {
                    _ = encoder.EncodePreviousPage((object) null);
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

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection);

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
                var encoder = new ContinuationTokenEncoder(_columns, PaginationDirection.Forward);

                Invoking(() =>
                {
                    _ = encoder.EncodeNextPage((object) null);
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

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection);

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
                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection);

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
                    Values = (object[])null
                };

                var encoder = new ContinuationTokenEncoder(_columns, paginationDirection);

                var actual = encoder.EncodeLastPage();

                actual.Should().NotBeNullOrEmpty();

                var decoded = encoder.Decode(actual);

                expected.Should().BeEquivalentTo(decoded);
            }
        }
    }
}