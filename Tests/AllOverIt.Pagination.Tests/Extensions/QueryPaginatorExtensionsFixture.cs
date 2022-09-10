using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Pagination.Tests.Extensions
{
    public class QueryPaginatorExtensionsFixture : FixtureBase
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
        }

        private readonly IReadOnlyCollection<EntityDummy> _entities;
        private readonly Func<QueryPaginatorConfiguration, IQueryPaginator<EntityDummy>> _paginatorFactory;

        private IReadOnlyCollection<EntityDummy> CreateEntities(int count)
        {
            return CreateMany<EntityDummy>(count);
        }


        public QueryPaginatorExtensionsFixture()
        {
            // reference item used to duplicate column values
            var reference = Create<EntityDummy>();

            var values1 = CreateEntities(4);
            var values2 = CreateEntities(4);
            var values3 = CreateEntities(4);
            var values4 = CreateEntities(4);
            var values5 = CreateEntities(4);
            var values6 = CreateEntities(4);
            var values7 = CreateEntities(4);

            values1.ForEach((item, _) => item.Id = reference.Id);
            values2.ForEach((item, _) => item.FirstName = reference.FirstName);
            values3.ForEach((item, _) => item.LastName = reference.LastName);
            values4.ForEach((item, _) => item.DateOfBirth = reference.DateOfBirth);
            values5.ForEach((item, _) => item.Relationship = reference.Relationship);
            values7.ForEach((item, _) => item.Active = reference.Active);

            _entities = values1
                .Concat(values2)
                .Concat(values3)
                .Concat(values4)
                .Concat(values5)
                .Concat(values6)
                .Concat(values7)
                .ToList();

            var query =
                from entity in _entities
                select entity;

            var continuationTokenEncoderFactory = new ContinuationTokenEncoderFactory();

            _paginatorFactory = configuration => new QueryPaginator<EntityDummy>(query.AsQueryable(), configuration, continuationTokenEncoderFactory);
        }

        public class ColumnAscending_2 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_By_Two_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_By_Two_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_3 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_By_Three_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LastName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_By_Three_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_4 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_By_Three_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.LastName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LastName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_By_Three_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, entity => entity.FirstName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.FirstName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_5 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.LastName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LicenseNumber)
                    .ThenBy(item => item.LastName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, item => item.LicenseNumber, entity => entity.FirstName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.LicenseNumber)
                    .ThenBy(item => item.FirstName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_6 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.Active,
                    item => item.DateOfBirth, entity => entity.Relationship, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LicenseNumber)
                    .ThenBy(item => item.Active)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Relationship)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.Active,
                    item => item.DateOfBirth, entity => entity.Relationship, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LicenseNumber)
                    .ThenBy(item => item.Active)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Relationship)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_2 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_3 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LastName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_4 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.LastName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LastName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, entity => entity.FirstName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_5 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.LastName,
                    item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.LastName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, item => item.LicenseNumber, entity => entity.FirstName,
                    item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_6 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                // Should start on Page 1 then the next is Page 2

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.Active,
                    item => item.DateOfBirth, entity => entity.Relationship, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.Active)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Relationship)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, pageSize, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                // Should start on Page 2 then the next is Page 1

                var pageSize = _entities.Count / 2;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.Active,
                    item => item.DateOfBirth, entity => entity.Relationship, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.Active)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Relationship)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 2, pageSize, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 1, pageSize, expectedQuery, continuationToken);
            }
        }

        private IQueryPaginator<EntityDummy> CreatePaginator(int pageSize, PaginationDirection paginationDirection)
        {
            var configuration = new QueryPaginatorConfiguration
            {
                PageSize = pageSize,
                PaginationDirection = paginationDirection,
                UseParameterizedQueries = Create<bool>()        // Should not affect outcome. More efficient for memory based queries when false
            };

            return _paginatorFactory.Invoke(configuration);
        }

        private static IReadOnlyCollection<EntityDummy> AssertPagedData(IQueryPaginator<EntityDummy> paginator, int page, int pageSize,
            IOrderedEnumerable<EntityDummy> expectedQuery, string continuationToken)
        {
            var query = paginator.GetPageQuery(continuationToken);

            var actualData = query.ToList();

            var skipCount = (page - 1) * pageSize;          // page is 1-based
            var expectedData = expectedQuery.Skip(skipCount).Take(pageSize).ToList();

            expectedData.Should().ContainInOrder(actualData);

            return actualData;
        }
    }
}