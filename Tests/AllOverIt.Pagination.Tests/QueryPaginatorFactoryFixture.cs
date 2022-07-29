using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Pagination.Tests
{
    public class QueryPaginatorFactoryFixture : QueryPaginatorFixture
    {
        private class DummyEntity
        {
        }

        private readonly IQueryPaginatorFactory _factory;

        public QueryPaginatorFactoryFixture()
        {
            _factory = new QueryPaginatorFactory();
        }

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