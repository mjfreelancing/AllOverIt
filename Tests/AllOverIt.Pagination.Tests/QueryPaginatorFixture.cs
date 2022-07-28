using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.Exceptions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Pagination.Tests
{
    public class QueryPaginatorFixture : FixtureBase
    {
        private enum Relationship
        {
            Single,
            Defacto,
            Married,
            Widowed
        }

        private sealed class EntityDummy
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public Relationship Relationship { get; set; }
            public int LicenseNumber { get; set; }
            public bool Active { get; set; }

            public EntityDummy()
            {

            }
        }

        public class Constructor : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Query_Null()
            {
                Invoking(() =>
                {
                    _ = new QueryPaginator<EntityDummy>(null, Create<QueryPaginatorConfiguration>());
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
                    _ = new QueryPaginator<EntityDummy>(Array.Empty<EntityDummy>().AsQueryable(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }
        }

        public class ColumnAscending : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<EntityDummy>().AsQueryable();

                    _ = new QueryPaginator<EntityDummy>(query, Create<QueryPaginatorConfiguration>())
                        .ColumnAscending<EntityDummy>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Return_Self()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var expected = new QueryPaginator<EntityDummy>(query, Create<QueryPaginatorConfiguration>());

                var actual = expected.ColumnAscending(entity => entity.FirstName);

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Add_Column_Ascending()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var paginator = new QueryPaginator<EntityDummy>(query, new QueryPaginatorConfiguration())
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain(".OrderBy(entity => entity.FirstName)");
            }

            [Fact]
            public void Should_Add_Column_Ascending_Backward()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();
                
                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain($".OrderByDescending(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }
        }

        public class ColumnDescending : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<EntityDummy>().AsQueryable();

                    _ = new QueryPaginator<EntityDummy>(query, Create<QueryPaginatorConfiguration>())
                        .ColumnDescending<EntityDummy>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Return_Self()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var expected = new QueryPaginator<EntityDummy>(query, Create<QueryPaginatorConfiguration>());

                var actual = expected.ColumnDescending(entity => entity.FirstName);

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Add_Column_Descending()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var paginator = new QueryPaginator<EntityDummy>(query, new QueryPaginatorConfiguration())
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain(".OrderByDescending(entity => entity.FirstName)");
            }

            [Fact]
            public void Should_Add_Column_Descending_Backward()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain($".OrderBy(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }
        }

        public class GetPageQuery : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_No_Columns_Defined()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<EntityDummy>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<EntityDummy>(query, config);

                    _ = paginator.GetPageQuery();
                })
                   .Should()
                   .Throw<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public void Should_Get_First_Page_When_Ascending_Forward_And_No_ContinuationToken()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderBy(entity => entity.FirstName).Take({config.PageSize})");
            }

            [Fact]
            public void Should_Get_First_Page_When_Descending_Backward_And_No_ContinuationToken()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderBy(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }

            [Fact]
            public void Should_Get_Last_Page_When_Ascending_Backward_And_No_ContinuationToken()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderByDescending(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }

            [Fact]
            public void Should_Get_Last_Page_When_Descending_Forward_And_No_ContinuationToken()
            {
                var query = Array.Empty<EntityDummy>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderByDescending(entity => entity.FirstName).Take({config.PageSize})");
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Ascending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Ascending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Descending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Descending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }
        }

        public class GetPreviousPageQuery : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Get_Last_Page_When_Ascending_Forward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_First_Page_When_Ascending_Backward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_First_Page_When_Descending_Forward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Last_Page_When_Descending_Backward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Ascending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.First()).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Ascending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.Last()).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Descending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.First()).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Prev_Page_When_Descending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.Last()).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }
        }

        public class GetNextPageQuery : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Get_First_Page_When_Ascending_Forward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Last_Page_When_Ascending_Backward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_First_Page_When_Descending_Forward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Last_Page_When_Descending_Backward_And_Null_Reference()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Take(config.PageSize)
                    .ToList();

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.Last()).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnAscending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.First()).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Forward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderByDescending(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.Last()).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Backward()
            {
                var entities = GetEntities();
                var query = entities.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<EntityDummy>(query, config)
                    .ColumnDescending(entity => entity.Id);

                var expected = query.OrderBy(item => item.Id)
                    .Skip(config.PageSize)
                    .Take(config.PageSize)
                    .Reverse()
                    .ToList();

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.First()).ToList();

                page2.SequenceEqual(expected).Should().BeTrue();
            }
        }

        private IReadOnlyCollection<EntityDummy> GetEntities()
        {
            return Enumerable
                .Range(1, 12)
                .SelectAsReadOnlyCollection(index =>
                {
                    var entity = Create<EntityDummy>();
                    entity.Id = index;

                    return entity;
                });
        }
    }


    /*
     
     
             Data       Ascending-Forward         Ascending-Backward         Descending-Forward          Descending-Backward
              1                1                          10                          12                           3                         
              2                2                          11                          11                           2        
              3                3                          12                          10                           1        

              4                4                          7                           9                            6        
              5                5                          8                           8                            5        
              6                6                          9                           7                            4        

              7                7                          4                           6                            9        
              8                8                          5                           5                            8        
              9                9                          6                           4                            7        

              10               10                         1                           3                            12       
              11               11                         2                           2                            11       
              12               12                         3                           1                            10       
     
     
     
     
     
     
     
     
     
     
     */
}