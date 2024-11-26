using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Pagination.TokenEncoding;
using FluentAssertions;

namespace AllOverIt.Pagination.Tests
{
    public class QueryPaginatorFixture : FixtureBase
    {
        /*
           Data       Page      Ascending-Forward        Ascending-Backward        Descending-Forward        Descending-Backward
           =====================================================================================================================
            1                          1                        10                        12                         3                         
            2          1               2                        11                        11                         2        
            3                          3                        12                        10                         1        
           =====================================================================================================================
            4                          4                        7                         9                          6        
            5          2               5                        8                         8                          5        
            6                          6                        9                         7                          4        
           =====================================================================================================================
            7                          7                        4                         6                          9        
            8          3               8                        5                         5                          8        
            9                          9                        6                         4                          7        
           =====================================================================================================================
            10                         10                       1                         3                          12       
            11         4               11                       2                         2                          11       
            12                         12                       3                         1                          10 
           =====================================================================================================================
         */

        private enum Status
        {
            Single,
            Defacto,
            Married,
            Widowed
        }

        private sealed class DummyEntity
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public Guid Reference { get; set; }
            public Status Status { get; set; }
            public int Age;     // Purposely a field
        }

        private readonly IContinuationTokenEncoderFactory _continuationTokenEncoderFactory;

        public QueryPaginatorFixture()
        {
            var serializerFactory = new ContinuationTokenSerializerFactory();
            _continuationTokenEncoderFactory = new ContinuationTokenEncoderFactory(serializerFactory);
        }

        public class Constructor : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Query_Null()
            {
                Invoking(() =>
                {
                    _ = new QueryPaginator<DummyEntity>(null, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("query");
            }

            [Fact]
            public void Should_Set_BaseQuery()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory);

                paginator.BaseQuery.Should().BeSameAs(query);
            }

            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = new QueryPaginator<DummyEntity>(Array.Empty<DummyEntity>().AsQueryable(), null, _continuationTokenEncoderFactory);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Throw_When_Token_Encoder_Factory_Null()
            {
                Invoking(() =>
                {
                    _ = new QueryPaginator<DummyEntity>(Array.Empty<DummyEntity>().AsQueryable(), Create<QueryPaginatorConfiguration>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("continuationTokenEncoderFactory");
            }
        }

        public class Create : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Query_Null()
            {
                Invoking(() =>
                {
                    _ = QueryPaginator<DummyEntity>.Create(null, Create<QueryPaginatorConfiguration>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("query");
            }

            [Fact]
            public void Should_Set_BaseQuery()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = QueryPaginator<DummyEntity>.Create(query, Create<QueryPaginatorConfiguration>());

                paginator.BaseQuery.Should().BeSameAs(query);
            }

            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = QueryPaginator<DummyEntity>.Create(Array.Empty<DummyEntity>().AsQueryable(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Return_QueryPaginator()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = QueryPaginator<DummyEntity>.Create(query, Create<QueryPaginatorConfiguration>());

                paginator.Should().BeOfType<QueryPaginator<DummyEntity>>();
            }
        }

        public class GetQueryDirection : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Return_Forward_When_Configured_Forward_And_Token_Null()
            {
                var configuration = new QueryPaginatorConfiguration();

                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, configuration, _continuationTokenEncoderFactory)
                    .ColumnAscending<int>(entity => entity.Id);

                var actual = paginator.GetQueryDirection(null);

                actual.Should().Be(PaginationDirection.Forward);
            }

            [Fact]
            public void Should_Return_Forward_When_Configured_Forward_And_Token_Empty()
            {
                var configuration = new QueryPaginatorConfiguration();

                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, configuration, _continuationTokenEncoderFactory)
                    .ColumnAscending<int>(entity => entity.Id);

                var actual = paginator.GetQueryDirection(string.Empty);

                actual.Should().Be(PaginationDirection.Forward);
            }

            [Fact]
            public void Should_Return_Forward_When_Configured_Forward_And_Token_Whitespace()
            {
                var configuration = new QueryPaginatorConfiguration();

                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, configuration, _continuationTokenEncoderFactory)
                    .ColumnAscending<int>(entity => entity.Id);

                var actual = paginator.GetQueryDirection("  ");

                actual.Should().Be(PaginationDirection.Forward);
            }

            [Fact]
            public void Should_Return_Backward_When_Configured_Backward_And_Token_Null()
            {
                var configuration = new QueryPaginatorConfiguration
                {
                    PaginationDirection = PaginationDirection.Backward
                };

                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, configuration, _continuationTokenEncoderFactory)
                    .ColumnAscending<int>(entity => entity.Id);

                var actual = paginator.GetQueryDirection(null);

                actual.Should().Be(PaginationDirection.Backward);
            }

            [Fact]
            public void Should_Return_Backward_When_Configured_Backward_And_Token_Empty()
            {
                var configuration = new QueryPaginatorConfiguration
                {
                    PaginationDirection = PaginationDirection.Backward
                };

                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, configuration, _continuationTokenEncoderFactory)
                    .ColumnAscending<int>(entity => entity.Id);

                var actual = paginator.GetQueryDirection(string.Empty);

                actual.Should().Be(PaginationDirection.Backward);
            }

            [Fact]
            public void Should_Return_Backward_When_Configured_Backward_And_Token_Whitespace()
            {
                var configuration = new QueryPaginatorConfiguration
                {
                    PaginationDirection = PaginationDirection.Backward
                };

                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, configuration, _continuationTokenEncoderFactory)
                    .ColumnAscending<int>(entity => entity.Id);

                var actual = paginator.GetQueryDirection("  ");

                actual.Should().Be(PaginationDirection.Backward);
            }

            [Fact]
            public void Should_Return_Forward_When_Configured_Forward_And_Token_Encoded_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var actual = paginator.GetQueryDirection(token);

                actual.Should().Be(PaginationDirection.Forward);
            }

            [Fact]
            public void Should_Return_Backward_When_Configured_Forward_And_Token_Encoded_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodePreviousPage(page1);

                var actual = paginator.GetQueryDirection(token);

                actual.Should().Be(PaginationDirection.Backward);
            }

            [Fact]
            public void Should_Return_Backward_When_Configured_Backward_And_Token_Encoded_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var actual = paginator.GetQueryDirection(token);

                actual.Should().Be(PaginationDirection.Backward);
            }

            [Fact]
            public void Should_Return_Forward_When_Configured_Backward_And_Token_Encoded_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodePreviousPage(page1);

                var actual = paginator.GetQueryDirection(token);

                actual.Should().Be(PaginationDirection.Forward);
            }
        }

        public class ColumnAscending : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    _ = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory)
                        .ColumnAscending<DummyEntity>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Return_Self()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var expected = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory);

                var actual = expected.ColumnAscending(entity => entity.FirstName);

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Add_Column_Ascending()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, new QueryPaginatorConfiguration(), _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain(".OrderBy(entity => entity.FirstName)");
            }

            [Fact]
            public void Should_Add_Column_Ascending_Backward()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain($".OrderByDescending(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }

            [Fact]
            public void Should_Throw_When_Querying_Has_Begun()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.FirstName);

                _ = paginator.GetPageQuery();

                Invoking(() =>
                {
                    paginator.ColumnAscending(entity => entity.Id);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("Additional columns cannot be added once pagination has begun.");
            }

            [Fact]
            public void Should_Throw_When_Querying_A_Field()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    _ = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory)
                        .ColumnAscending(entity => entity.Age);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("Paginated queries do not support fields.");
            }
        }

        public class ColumnDescending : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    _ = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory)
                        .ColumnDescending<DummyEntity>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Return_Self()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var expected = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory);

                var actual = expected.ColumnDescending(entity => entity.FirstName);

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Add_Column_Descending()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, new QueryPaginatorConfiguration(), _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain(".OrderByDescending(entity => entity.FirstName)");
            }

            [Fact]
            public void Should_Add_Column_Descending_Backward()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .Contain($".OrderBy(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }

            [Fact]
            public void Should_Throw_When_Querying_Has_Begun()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var paginator = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.FirstName);

                _ = paginator.GetPageQuery();

                Invoking(() =>
                {
                    paginator.ColumnDescending(entity => entity.Id);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("Additional columns cannot be added once pagination has begun.");
            }

            [Fact]
            public void Should_Throw_When_Querying_A_Field()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    _ = new QueryPaginator<DummyEntity>(query, Create<QueryPaginatorConfiguration>(), _continuationTokenEncoderFactory)
                        .ColumnDescending(entity => entity.Age);
                })
                .Should()
                .Throw<PaginationException>()
                .WithMessage("Paginated queries do not support fields.");
            }
        }

        public class GetPageQuery : QueryPaginatorFixture
        {
            private class EntityWithNullable
            {
                public int? Id { get; set; }
            }

            [Fact]
            public void Should_Throw_When_No_Columns_Defined()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = paginator.GetPageQuery();
                })
                   .Should()
                   .Throw<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public void Should_Get_First_Page_When_Ascending_Forward_And_No_ContinuationToken()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderBy(entity => entity.FirstName).Take({config.PageSize})");
            }

            [Fact]
            public void Should_Get_First_Page_When_Descending_Backward_And_No_ContinuationToken()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderBy(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }

            [Fact]
            public void Should_Get_Last_Page_When_Ascending_Backward_And_No_ContinuationToken()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderByDescending(entity => entity.FirstName).Take({config.PageSize}).Reverse()");
            }

            [Fact]
            public void Should_Get_Last_Page_When_Descending_Forward_And_No_ContinuationToken()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = Create<int>(),
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.FirstName);

                query = paginator.GetPageQuery();

                query.ToString()
                    .Should()
                    .EndWith($".OrderByDescending(entity => entity.FirstName).Take({config.PageSize})");
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(p2).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(p3).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(p3.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(p2.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Ascending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(p2).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Ascending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(p3).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Descending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(p3.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Descending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                token = paginator.TokenEncoder.EncodePreviousPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page.SequenceEqual(p2.Reverse()).Should().BeTrue();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Next_Page_Using_Parameterized_Queries(bool useParameterizedQueries)
            {
                var getData = Enumerable
                    .Range(1, 12)
                    .Select(index =>
                    {
                        var entity = Create<DummyEntity>();
                        entity.Id = index;

                        return entity;
                    });

                // Get data where the Id's are duplicated
                var all = getData.Concat(getData).AsReadOnlyCollection();

                var sorted = all.OrderBy(item => item.Id).ThenBy(item => item.FirstName).AsReadOnlyCollection();

                var p1 = sorted.Take(5).AsReadOnlyCollection();
                var p2 = sorted.Skip(5).Take(5).AsReadOnlyCollection();

                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 5,
                    PaginationDirection = PaginationDirection.Forward,
                    UseParameterizedQueries = useParameterizedQueries
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id)
                    .ColumnAscending(entity => entity.FirstName);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(p2).Should().BeTrue();
            }

            [Fact]
            public void Should_Support_Nullable_Members()
            {
                // Token values don't have type info - if there's an int? with a value then the decoded value is simply an int.
                // This test ensures the values are converted back to int? when assigning to the model.
                var nullableAll = CreateMany<EntityWithNullable>(5);

                var nullableSorted = nullableAll.OrderBy(item => item.Id).AsReadOnlyCollection();

                var p1 = nullableSorted.Take(2).AsReadOnlyCollection();
                var p2 = nullableSorted.Skip(2).Take(2).AsReadOnlyCollection();

                var nullableQuery = nullableAll.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 2,
                    PaginationDirection = PaginationDirection.Forward,
                    UseParameterizedQueries = false
                };

                var nullablePaginator = new QueryPaginator<EntityWithNullable>(nullableQuery, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = nullablePaginator.GetPageQuery().ToList();

                var token = nullablePaginator.TokenEncoder.EncodeNextPage(page1);

                // Make sure the values in the token are decoded correctly.
                var page2 = nullablePaginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(p2).Should().BeTrue();
            }
        }

        public class GetPreviousPageQuery : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_No_Columns_Defined()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = paginator.GetPreviousPageQuery(null);
                })
                   .Should()
                   .Throw<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public void Should_Get_Last_Page_When_Ascending_Forward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.SequenceEqual(p4).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_First_Page_When_Ascending_Backward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.SequenceEqual(p1).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_First_Page_When_Descending_Forward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.SequenceEqual(p1.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Last_Page_When_Descending_Backward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetPreviousPageQuery(null).ToList();

                page.SequenceEqual(p4.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Ascending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.First()).ToList();

                page.SequenceEqual(p2).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Ascending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.Last()).ToList();

                page.SequenceEqual(p3).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Descending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.First()).ToList();

                page.SequenceEqual(p3.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Previous_Page_When_Descending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetPageQuery().ToList();
                var token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();
                token = paginator.TokenEncoder.EncodeNextPage(page);
                page = paginator.GetPageQuery(token).ToList();

                page = paginator.GetPreviousPageQuery(page.Last()).ToList();

                page.SequenceEqual(p2.Reverse()).Should().BeTrue();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Get_Previous_Page_Using_Parameterized_Queries(bool useParameterizedQueries)
            {
                var getData = Enumerable
                    .Range(1, 12)
                    .Select(index =>
                    {
                        var entity = Create<DummyEntity>();
                        entity.Id = index;

                        return entity;
                    });

                // Get data where the Id's are duplicated
                var all = getData.Concat(getData).AsReadOnlyCollection();

                var sorted = all.OrderByDescending(item => item.Id).ThenByDescending(item => item.FirstName).AsReadOnlyCollection();

                var p1 = sorted.Take(5).AsReadOnlyCollection();
                var p2 = sorted.Skip(5).Take(5).AsReadOnlyCollection();

                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 5,
                    PaginationDirection = PaginationDirection.Forward,
                    UseParameterizedQueries = useParameterizedQueries
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id)
                    .ColumnDescending(entity => entity.FirstName);

                var page1 = paginator.GetPageQuery().ToList();

                var token = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = paginator.GetPageQuery(token).ToList();

                page2.SequenceEqual(p2).Should().BeTrue();
            }
        }

        public class GetNextPageQuery : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_No_Columns_Defined()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = paginator.GetNextPageQuery(null);
                })
                   .Should()
                   .Throw<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public void Should_Get_First_Page_When_Ascending_Forward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(p1).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Last_Page_When_Ascending_Backward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(p4).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Last_Page_When_Descending_Forward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(p4.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_First_Page_When_Descending_Backward_And_Null_Reference()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page = paginator.GetNextPageQuery(null).ToList();

                page.SequenceEqual(p1.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.Last()).ToList();

                page2.SequenceEqual(p2).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Ascending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.First()).ToList();

                page2.SequenceEqual(p3).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Forward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.Last()).ToList();

                page2.SequenceEqual(p3.Reverse()).Should().BeTrue();
            }

            [Fact]
            public void Should_Get_Next_Page_When_Descending_Backward()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Backward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnDescending(entity => entity.Id);

                var page1 = paginator.GetPageQuery().ToList();

                var page2 = paginator.GetNextPageQuery(page1.First()).ToList();

                page2.SequenceEqual(p2.Reverse()).Should().BeTrue();
            }
        }

        public class HasPreviousPage : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Reference_Entity_Null()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = new QueryPaginatorConfiguration
                    {
                        PageSize = 3,
                        PaginationDirection = PaginationDirection.Forward
                    };

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = paginator.HasPreviousPage(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reference");
            }

            [Fact]
            public void Should_Throw_When_No_Columns_Defined()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = paginator.HasPreviousPage(Create<DummyEntity>());
                })
                   .Should()
                   .Throw<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public void Should_Return_False_When_No_Previous_Data()
            {
                var (all, p1, p2, p3, p4) = GetEntities();

                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = paginator.HasPreviousPage(p1.First());

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Previous_Data()
            {
                var (all, p1, p2, p3, p4) = GetEntities();

                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = paginator.HasPreviousPage(p1.Skip(1).First());

                actual.Should().BeTrue();
            }
        }

        public class HasPreviousPageAsync : QueryPaginatorFixture
        {
            [Fact]
            public async Task Should_Throw_When_Reference_Entity_Null()
            {
                await Invoking(async () =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = new QueryPaginatorConfiguration
                    {
                        PageSize = 3,
                        PaginationDirection = PaginationDirection.Forward
                    };

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = await paginator.HasPreviousPageAsync(
                        null,
                        (queryable, expression, cancellationToken) => Task.FromResult(false),
                        CancellationToken.None);
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reference");
            }

            [Fact]
            public async Task Should_Throw_When_No_Columns_Defined()
            {
                await Invoking(async () =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = await paginator.HasPreviousPageAsync(
                        Create<DummyEntity>(),
                        (queryable, expression, cancellationToken) => Task.FromResult(false),
                        CancellationToken.None);
                })
                   .Should()
                   .ThrowAsync<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public async Task Should_Return_False_When_No_Previous_Data()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = await paginator.HasPreviousPageAsync(
                    Create<DummyEntity>(),
                    (queryable, expression, cancellationToken) => Task.FromResult(false),
                    CancellationToken.None);

                actual.Should().BeFalse();
            }

            [Fact]
            public async Task Should_Return_True_When_Previous_Data()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = await paginator.HasPreviousPageAsync(
                    Create<DummyEntity>(),
                    (queryable, expression, cancellationToken) => Task.FromResult(true),
                    CancellationToken.None);

                actual.Should().BeTrue();
            }
        }

        public class HasNextPage : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Throw_When_Reference_Entity_Null()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = new QueryPaginatorConfiguration
                    {
                        PageSize = 3,
                        PaginationDirection = PaginationDirection.Forward
                    };

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = paginator.HasNextPage(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reference");
            }

            [Fact]
            public void Should_Throw_When_No_Columns_Defined()
            {
                Invoking(() =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = paginator.HasNextPage(Create<DummyEntity>());
                })
                   .Should()
                   .Throw<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public void Should_Return_False_When_No_Next_Data()
            {
                var (all, p1, p2, p3, p4) = GetEntities();

                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = paginator.HasNextPage(p4.Last());

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Next_Data()
            {
                var (all, p1, p2, p3, p4) = GetEntities();

                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = paginator.HasNextPage(p4.Skip(1).First());

                actual.Should().BeTrue();
            }
        }

        public class HasNextPageAsync : QueryPaginatorFixture
        {
            [Fact]
            public async Task Should_Throw_When_Reference_Entity_Null()
            {
                await Invoking(async () =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = new QueryPaginatorConfiguration
                    {
                        PageSize = 3,
                        PaginationDirection = PaginationDirection.Forward
                    };

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = await paginator.HasNextPageAsync(
                        null,
                        (queryable, expression, cancellationToken) => Task.FromResult(false),
                        CancellationToken.None);
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reference");
            }

            [Fact]
            public async Task Should_Throw_When_No_Columns_Defined()
            {
                await Invoking(async () =>
                {
                    var query = Array.Empty<DummyEntity>().AsQueryable();

                    var config = Create<QueryPaginatorConfiguration>();

                    var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory);

                    _ = await paginator.HasNextPageAsync(
                        Create<DummyEntity>(),
                        (queryable, expression, cancellationToken) => Task.FromResult(false),
                        CancellationToken.None);
                })
                   .Should()
                   .ThrowAsync<PaginationException>()
                   .WithMessage("At least one column must be defined for pagination.");
            }

            [Fact]
            public async Task Should_Return_False_When_No_Next_Data()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = await paginator.HasNextPageAsync(
                    Create<DummyEntity>(),
                    (queryable, expression, cancellationToken) => Task.FromResult(false),
                    CancellationToken.None);

                actual.Should().BeFalse();
            }

            [Fact]
            public async Task Should_Return_True_When_Next_Data()
            {
                var query = Array.Empty<DummyEntity>().AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 3,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Id);

                var actual = await paginator.HasNextPageAsync(
                    Create<DummyEntity>(),
                    (queryable, expression, cancellationToken) => Task.FromResult(true),
                    CancellationToken.None);

                actual.Should().BeTrue();
            }
        }

        public class Comparisons : QueryPaginatorFixture
        {
            [Fact]
            public void Should_Compare_Enum_Guid()
            {
                var (all, p1, p2, p3, p4) = GetEntities();
                var query = all.AsQueryable();

                var config = new QueryPaginatorConfiguration
                {
                    PageSize = 9,
                    PaginationDirection = PaginationDirection.Forward
                };

                var paginator = new QueryPaginator<DummyEntity>(query, config, _continuationTokenEncoderFactory)
                    .ColumnAscending(entity => entity.Status)
                    .ColumnAscending(entity => entity.Reference);

                var expected = all
                    .OrderBy(item => item.Status)
                    .ThenBy(item => item.Reference)
                    .Skip(9)
                    .ToList();

                var results = paginator.GetPageQuery().ToList();

                query = paginator.GetNextPageQuery(results.Last());

                results = query.ToList();

                // Should only be the last page left
                results.SequenceEqual(expected).Should().BeTrue();
            }
        }

        private (
            IReadOnlyCollection<DummyEntity> All,
            IReadOnlyCollection<DummyEntity> Page1,
            IReadOnlyCollection<DummyEntity> Page2,
            IReadOnlyCollection<DummyEntity> Page3,
            IReadOnlyCollection<DummyEntity> Page4) GetEntities()
        {
            var all = Enumerable
                .Range(1, 12)
                .SelectToReadOnlyCollection(index =>
                {
                    var entity = Create<DummyEntity>();
                    entity.Id = index;

                    return entity;
                });

            var p1 = all.Take(3).AsReadOnlyCollection();
            var p2 = all.Skip(3).Take(3).AsReadOnlyCollection();
            var p3 = all.Skip(6).Take(3).AsReadOnlyCollection();
            var p4 = all.Skip(9).Take(3).AsReadOnlyCollection();

            return (all, p1, p2, p3, p4);
        }
    }
}