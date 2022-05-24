using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Serialization.NewtonsoftJson;
using AllOverIt.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AllOverIt.Serialization.SystemTextJson.Converters;
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
        }

        private readonly IReadOnlyCollection<EntityDummy> _entities;
        private readonly Func<int, PaginationDirection, IQueryPaginator<EntityDummy>> _paginatorFactory;


        private IReadOnlyCollection<EntityDummy> CreateEntities(int count)
        {
            //return CreateMany<EntityDummy>(count);

            var entities = CreateMany<EntityDummy>(count);

            return entities.SelectAsReadOnlyCollection(item => new EntityDummy
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                DateOfBirth = item.DateOfBirth,
                Relationship = item.Relationship,
                LicenseNumber = item.LicenseNumber
            });
        }


        public QueryPaginatorExtensionsFixture()
        {
            // reference item used to duplicate column values
            var reference = CreateEntities(1).Single();// Create<EntityDummy>();

            var values1 = CreateEntities(3);
            var values2 = CreateEntities(3);
            var values3 = CreateEntities(3);
            var values4 = CreateEntities(3);
            var values5 = CreateEntities(3);
            var values6 = CreateEntities(3);

            values1.ForEach((item, _) => item.Id = reference.Id);
            values2.ForEach((item, _) => item.FirstName = reference.FirstName);
            values3.ForEach((item, _) => item.LastName = reference.LastName);
            values4.ForEach((item, _) => item.DateOfBirth = reference.DateOfBirth);
            values5.ForEach((item, _) => item.Relationship = reference.Relationship);
            values6.ForEach((item, _) => item.LicenseNumber = reference.LicenseNumber);

            _entities = values1
                .Concat(values2)
                .Concat(values3)
                .Concat(values4)
                .Concat(values5)
                .Concat(values6)
                .ToList();

            var query =
                from entity in _entities
                select entity;

            var options = new JsonSerializerOptions();
            options.Converters.Add(new SystemObjectNewtonsoftCompatibleConverter());

            var config = new QueryPaginatorConfig
            {
                //Serializer = new NewtonsoftJsonSerializer()
                Serializer = new SystemTextJsonSerializer(options)
            };

            _paginatorFactory = (pageSize, paginationDirection) => new QueryPaginator<EntityDummy>(query.AsQueryable(), config, pageSize, paginationDirection);
        }

        public class ColumnAscending_2 : QueryPaginatorExtensionsFixture
        {
            [Fact]
            public void Should_Order_By_Two_Columns_Forward()
            {
                var paginator = CreatePaginator(PaginationDirection.Forward)
                    .ColumnAscending(entity => entity.FirstName, entity => entity.Id);

                var query1 = paginator.GetPageQuery();
                var page1 = query1.ToList();

                var continuationToken = paginator.ContinuationTokenEncoder.EncodeNextPage(page1);

                var query2 = paginator.GetPageQuery(continuationToken);
                var page2 = query2.ToList();

            }

            [Fact]
            public void Should_Order_By_Two_Columns_Backward()
            {
                var query = CreatePaginator(PaginationDirection.Backward)
                    .ColumnAscending(entity => entity.Id, entity => entity.FirstName)
                    .GetPageQuery();

                var actual = query.ToList();

            }



        }

        public class ColumnAscending_3 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnAscending_4 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnAscending_5 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnAscending_6 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnDescending_2 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnDescending_3 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnDescending_4 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnDescending_5 : QueryPaginatorExtensionsFixture
        {

        }

        public class ColumnDescending_6 : QueryPaginatorExtensionsFixture
        {

        }

        private IQueryPaginator<EntityDummy> CreatePaginator(PaginationDirection paginationDirection)
        {
            return _paginatorFactory.Invoke(_entities.Count / 2, paginationDirection);
        }
    }
}