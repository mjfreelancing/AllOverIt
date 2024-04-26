﻿using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Pagination.Tests
{
    public class ColumnDefinitionFixture : FixtureBase
    {
        private sealed class DummyEntity
        {
            public int Id { get; init; }
            public string Name { get; init; }
        }

        private IReadOnlyCollection<DummyEntity> _entities;

        public ColumnDefinitionFixture()
        {
            _entities = CreateMany<DummyEntity>();
        }

        public class Constructor : ColumnDefinitionFixture
        {
            [Fact]
            public void Should_Throw_When_ProopertyInfo_Null()
            {
                Invoking(() =>
                {
                    _ = new ColumnDefinition<DummyEntity, int>(null, Create<bool>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("property");
            }
        }

        public class ApplyColumnOrderTo : ColumnDefinitionFixture
        {
            [Fact]
            public void Should_Throw_When_Queryable_Null()
            {
                var property = typeof(DummyEntity).GetProperty(nameof(DummyEntity.Id));

                var idPropertyDefinition = new ColumnDefinition<DummyEntity, int>(property, Create<bool>());

                Invoking(() =>
                {
                    idPropertyDefinition.ApplyColumnOrderTo(null, Create<PaginationDirection>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("queryable");
            }

            [Theory]
            [InlineData(true, PaginationDirection.Forward)]
            [InlineData(false, PaginationDirection.Forward)]
            [InlineData(true, PaginationDirection.Backward)]
            [InlineData(false, PaginationDirection.Backward)]
            public void Should_OrderBy_Id(bool ascending, PaginationDirection direction)
            {
                var property = typeof(DummyEntity).GetProperty(nameof(DummyEntity.Id));

                var idPropertyDefinition = new ColumnDefinition<DummyEntity, int>(property, ascending);

                var query = idPropertyDefinition.ApplyColumnOrderTo(_entities.AsQueryable(), direction);

                var expected = (ascending, direction) switch
                {
                    (true, PaginationDirection.Forward) => _entities.OrderBy(item => item.Id),
                    (false, PaginationDirection.Forward) => _entities.OrderByDescending(item => item.Id),
                    (true, PaginationDirection.Backward) => _entities.OrderByDescending(item => item.Id),
                    (false, PaginationDirection.Backward) => _entities.OrderBy(item => item.Id),
                    _ => throw new InvalidOperationException()
                };

                var actual = query.ToList();

                expected.Should().ContainInOrder(actual);
            }
        }

        public class ThenApplyColumnOrderTo : ColumnDefinitionFixture
        {
            [Fact]
            public void Should_Throw_When_Queryable_Null()
            {
                var property = typeof(DummyEntity).GetProperty(nameof(DummyEntity.Id));

                var idPropertyDefinition = new ColumnDefinition<DummyEntity, int>(property, Create<bool>());

                Invoking(() =>
                {
                    idPropertyDefinition.ThenApplyColumnOrderTo(null, Create<PaginationDirection>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("queryable");
            }

            [Theory]
            [InlineData(true, PaginationDirection.Forward)]
            [InlineData(false, PaginationDirection.Forward)]
            [InlineData(true, PaginationDirection.Backward)]
            [InlineData(false, PaginationDirection.Backward)]
            public void Should_OrderBy_Id_Then_Name(bool ascending, PaginationDirection direction)
            {
                var newEntities = _entities
                    .Select(item => new DummyEntity
                    {
                        Id = item.Id,
                        Name = Create<string>()
                    });

                _entities = _entities
                    .Concat(newEntities)
                    .AsReadOnlyCollection();

                var idProperty = typeof(DummyEntity).GetProperty(nameof(DummyEntity.Id));

                var idPropertyDefinition = new ColumnDefinition<DummyEntity, int>(idProperty, ascending);

                var query = idPropertyDefinition.ApplyColumnOrderTo(_entities.AsQueryable(), direction);

                var nameProperty = typeof(DummyEntity).GetProperty(nameof(DummyEntity.Name));

                var namePropertyDefinition = new ColumnDefinition<DummyEntity, string>(nameProperty, ascending);

                query = namePropertyDefinition.ThenApplyColumnOrderTo(query, direction);

                var expected = (ascending, direction) switch
                {
                    (true, PaginationDirection.Forward) => _entities.OrderBy(item => item.Id).ThenBy(item => item.Name),
                    (false, PaginationDirection.Forward) => _entities.OrderByDescending(item => item.Id).ThenByDescending(item => item.Name),
                    (true, PaginationDirection.Backward) => _entities.OrderByDescending(item => item.Id).ThenByDescending(item => item.Name),
                    (false, PaginationDirection.Backward) => _entities.OrderBy(item => item.Id).ThenBy(item => item.Name),
                    _ => throw new InvalidOperationException()
                };

                var actual = query.ToList();

                expected.Should().ContainInOrder(actual);
            }
        }
    }
}