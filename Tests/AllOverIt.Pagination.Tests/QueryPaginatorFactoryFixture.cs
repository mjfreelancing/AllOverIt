using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Pagination.Tests
{
    public class QueryPaginatorFactoryFixture : FixtureBase
    {
        private class DummyEntity
        {
        }

        private readonly IQueryPaginatorFactory _factory;

        public QueryPaginatorFactoryFixture()
        {
            _factory = new QueryPaginatorFactory(A.Fake<IContinuationTokenEncoderFactory>());
        }

        public class Constructor : QueryPaginatorFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Encoder_Factory_Null()
            {
                Invoking(() =>
                {
                    _ = new QueryPaginatorFactory(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("continuationTokenEncoderFactory");
            }
        }

        public class CreatePaginator : QueryPaginatorFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Query_Null()
            {
                Invoking(() =>
                {
                    _ = _factory.CreatePaginator<DummyEntity>(null, Create<QueryPaginatorConfiguration>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("query");
            }

            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = _factory.CreatePaginator<DummyEntity>(Array.Empty<DummyEntity>().AsQueryable(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Create_Paginator()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();
                var config = Create<QueryPaginatorConfiguration>();

                var paginator = _factory.CreatePaginator<DummyEntity>(query, config);

                paginator.Should().BeAssignableTo<IQueryPaginator<DummyEntity>>();
            }
        }
    }
}