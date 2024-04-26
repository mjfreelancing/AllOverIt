using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private sealed class DummyEntity
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public Relationship Relationship { get; set; }
            public int LicenseNumber { get; set; }
            public bool Active { get; set; }
        }

        private readonly IReadOnlyCollection<DummyEntity> _entities;
        private readonly Func<QueryPaginatorConfiguration, IQueryPaginator<DummyEntity>> _paginatorFactory;

        private IReadOnlyCollection<DummyEntity> CreateEntities(int count)
        {
            return CreateMany<DummyEntity>(count);
        }


        public QueryPaginatorExtensionsFixture()
        {
            // reference item used to duplicate column values
            var reference = Create<DummyEntity>();

            var values1 = CreateEntities(4);
            var values2 = CreateEntities(4);
            var values3 = CreateEntities(4);
            var values4 = CreateEntities(4);
            var values5 = CreateEntities(4);
            var values6 = CreateEntities(4);
            var values7 = CreateEntities(5);

            values1.ForEach((item, _) => item.Id = reference.Id);
            values2.ForEach((item, _) => item.FirstName = reference.FirstName);
            values3.ForEach((item, _) => item.LastName = reference.LastName);
            values4.ForEach((item, _) => item.DateOfBirth = reference.DateOfBirth);
            values5.ForEach((item, _) => item.Relationship = reference.Relationship);
            values6.ForEach((item, _) => item.LicenseNumber = reference.LicenseNumber);
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

            var serializerFactory = new ContinuationTokenSerializerFactory();
            var continuationTokenEncoderFactory = new ContinuationTokenEncoderFactory(serializerFactory);

            _paginatorFactory = configuration => new QueryPaginator<DummyEntity>(query.AsQueryable(), configuration, continuationTokenEncoderFactory);
        }

        public class ColumnAscending_2 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_By_Two_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_By_Two_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_3 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_By_Three_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LastName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_By_Three_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_4 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_By_Three_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.LastName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LastName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_By_Three_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, entity => entity.FirstName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.FirstName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_5 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.LastName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.LicenseNumber)
                    .ThenBy(item => item.LastName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.LastName, item => item.LicenseNumber, entity => entity.FirstName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.LastName)
                    .ThenBy(item => item.LicenseNumber)
                    .ThenBy(item => item.FirstName)
                    .ThenBy(item => item.DateOfBirth)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnAscending_6 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

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

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

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

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_2 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_3 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.LastName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LastName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_4 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.LastName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LastName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, entity => entity.FirstName, item => item.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_5 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.LastName,
                                      entity => entity.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.LastName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.LastName, item => item.LicenseNumber, entity => entity.FirstName,
                                      entity => entity.DateOfBirth, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.LastName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class ColumnDescending_6 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_Columns_Forward_From_Start()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnDescending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.Active,
                                      entity => entity.DateOfBirth, entity => entity.Relationship, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.Active)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Relationship)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }

            [Fact]
            public void Should_Order_Columns_Backward_From_End()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 2;

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.FirstName, item => item.LicenseNumber, entity => entity.Active,
                                      entity => entity.DateOfBirth, entity => entity.Relationship, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.LicenseNumber)
                    .ThenByDescending(item => item.Active)
                    .ThenByDescending(item => item.DateOfBirth)
                    .ThenByDescending(item => item.Relationship)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                _ = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
            }
        }

        public class GetPageResults : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Get_First_Page()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 3;     // Should get page counts of 9, 9, 9, 2

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var actual = QueryPaginatorExtensions.GetPageResults(paginator, null);

                actual.Should().BeEquivalentTo(new
                {
                    Results = page1,
                    TotalCount = _entities.Count,
                    CurrentToken = (string) null,
                    PreviousToken = (string) null,
                    NextToken = paginator.TokenEncoder.EncodeNextPage(page1)
                });
            }

            [Fact]
            public void Should_Not_Have_Next_Or_Previous_Page()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count;

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var actual = QueryPaginatorExtensions.GetPageResults(paginator, null);

                actual.Should().BeEquivalentTo(new
                {
                    Results = expectedQuery.ToList(),
                    TotalCount = _entities.Count,
                    CurrentToken = (string) null,
                    PreviousToken = (string) null,
                    NextToken = (string) null
                });
            }

            [Fact]
            public void Should_Get_Next_Page()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 3;     // Should get page counts of 9, 9, 9, 2

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);

                var actual = QueryPaginatorExtensions.GetPageResults(paginator, continuationToken);

                actual.Should().BeEquivalentTo(new
                {
                    Results = page2,
                    TotalCount = _entities.Count,
                    CurrentToken = continuationToken,
                    PreviousToken = paginator.TokenEncoder.EncodePreviousPage(page2),
                    NextToken = paginator.TokenEncoder.EncodeNextPage(page2)
                });
            }

            [Fact]
            public void Should_Get_Last_Page()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 3;     // Should get page counts of 9, 9, 9, 2

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var actual = QueryPaginatorExtensions.GetPageResults(paginator, null);

                actual.Should().BeEquivalentTo(new
                {
                    Results = page1,
                    TotalCount = _entities.Count,
                    CurrentToken = (string) null,
                    PreviousToken = (string) null,
                    NextToken = paginator.TokenEncoder.EncodeNextPage(page1)
                });
            }

            [Fact]
            public void Should_Get_Previous_Page()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 3;     // Should get page counts of 9, 9, 9, 2

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);

                var actual = QueryPaginatorExtensions.GetPageResults(paginator, continuationToken);

                actual.Should().BeEquivalentTo(new
                {
                    Results = page2,
                    TotalCount = _entities.Count,
                    CurrentToken = continuationToken,
                    PreviousToken = paginator.TokenEncoder.EncodePreviousPage(page2),
                    NextToken = paginator.TokenEncoder.EncodeNextPage(page2)
                });
            }
        }

        public class Mixed_Functional : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Navigate_Pages_Forward()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 3;     // Should get page counts of 9, 9, 9, 2

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, true);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderBy(item => item.FirstName)
                    .ThenBy(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);
                paginator.HasPreviousPage(page1.First()).Should().BeFalse();
                paginator.HasNextPage(page1.Last()).Should().BeTrue();

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
                paginator.HasPreviousPage(page2.First()).Should().BeTrue();
                paginator.HasNextPage(page2.Last()).Should().BeTrue();

                continuationToken = paginator.TokenEncoder.EncodeNextPage(page2);

                var page3 = AssertPagedData(paginator, 3, skipSteps, expectedQuery, continuationToken);
                paginator.HasPreviousPage(page3.First()).Should().BeTrue();
                paginator.HasNextPage(page3.Last()).Should().BeTrue();

                continuationToken = paginator.TokenEncoder.EncodeNextPage(page3);

                var page4 = AssertPagedData(paginator, 4, skipSteps, expectedQuery, continuationToken);
                paginator.HasPreviousPage(page4.First()).Should().BeTrue();
                paginator.HasNextPage(page4.Last()).Should().BeFalse();
            }

            [Fact]
            public void Should_Navigate_Pages_Reverse()
            {
                _entities.Count.Should().Be(29);
                var pageSize = _entities.Count / 3;     // Should get page counts of 9, 9, 9, 2

                var skipSteps = GetSkipSteps(_entities.Count, pageSize, false);

                var paginator = CreatePaginator(pageSize, PaginationDirection.Backward)
                    .ColumnDescending(entity => entity.FirstName, entity => entity.Id);

                var expectedQuery = _entities
                    .OrderByDescending(item => item.FirstName)
                    .ThenByDescending(item => item.Id);

                var page1 = AssertPagedData(paginator, 1, skipSteps, expectedQuery, null);
                paginator.HasPreviousPage(page1.Last()).Should().BeFalse();
                paginator.HasNextPage(page1.First()).Should().BeTrue();

                var continuationToken = paginator.TokenEncoder.EncodeNextPage(page1);

                var page2 = AssertPagedData(paginator, 2, skipSteps, expectedQuery, continuationToken);
                paginator.HasPreviousPage(page2.Last()).Should().BeTrue();
                paginator.HasNextPage(page2.First()).Should().BeTrue();

                continuationToken = paginator.TokenEncoder.EncodeNextPage(page2);

                var page3 = AssertPagedData(paginator, 3, skipSteps, expectedQuery, continuationToken);
                paginator.HasPreviousPage(page3.Last()).Should().BeTrue();
                paginator.HasNextPage(page3.First()).Should().BeTrue();

                continuationToken = paginator.TokenEncoder.EncodeNextPage(page3);

                var page4 = AssertPagedData(paginator, 4, skipSteps, expectedQuery, continuationToken);
                paginator.HasPreviousPage(page4.Last()).Should().BeTrue();
                paginator.HasNextPage(page4.First()).Should().BeFalse();
            }
        }

        private IQueryPaginator<DummyEntity> CreatePaginator(int pageSize, PaginationDirection paginationDirection)
        {
            var configuration = new QueryPaginatorConfiguration
            {
                PageSize = pageSize,
                PaginationDirection = paginationDirection,
                UseParameterizedQueries = Create<bool>()        // Should not affect outcome. More efficient for memory based queries when false
            };

            return _paginatorFactory.Invoke(configuration);
        }

        private static IReadOnlyCollection<DummyEntity> AssertPagedData(IQueryPaginator<DummyEntity> paginator, int page,
            (int Skip, int Take)[] skipSteps, IOrderedEnumerable<DummyEntity> expectedQuery, string continuationToken)
        {
            var query = paginator.GetPageQuery(continuationToken);

            var actualData = query.ToList();

            var expectedData = expectedQuery.Skip(skipSteps[page - 1].Skip).Take(skipSteps[page - 1].Take).ToList();

            expectedData.Should().ContainInOrder(actualData);

            return actualData;
        }

        private static (int Skip, int Take)[] GetSkipSteps(int total, int pageSize, bool forward)
        {
            var skips = new List<(int, int)>();

            var cumulative = 0;

            while (cumulative < total)
            {
                if (cumulative + pageSize <= total)
                {
                    if (forward)
                    {
                        skips.Add((cumulative, pageSize));
                    }
                    else
                    {
                        skips.Add((total - cumulative - pageSize, pageSize));
                    }
                }
                else
                {
                    if (forward)
                    {
                        skips.Add((cumulative, total - cumulative));
                    }
                    else
                    {
                        skips.Add((0, total - cumulative));
                    }
                }

                cumulative += pageSize;
            }

            return skips.ToArray();
        }
    }
}