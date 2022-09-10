using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Pagination.Tests.Extensions
{
    public class ColumnItemExtensionsFixture : FixtureBase
    {
        private sealed class EntityDummy
        {
            public int Id { get; init; }
            public string Name { get; init; }
        }

        public class GetColumnValueTypesFixture : ColumnItemExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Columns_Null()
            {
                Invoking(() =>
                {
                    _ = ColumnItemExtensions.GetColumnValues(null, new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("columns");
            }

            [Fact]
            public void Should_Throw_When_Columns_Empty()
            {
                var columns = new List<IColumnDefinition>();

                Invoking(() =>
                {
                    _ = ColumnItemExtensions.GetColumnValues(columns, new { });
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("columns");
            }

            [Fact]
            public void Should_Throw_When_Reference_Null()
            {
                var columns = new List<IColumnDefinition>
                {
                    new ColumnDefinition<EntityDummy, string>(typeof(EntityDummy).GetProperty(nameof(EntityDummy.Name)), Create<bool>())
                };

                Invoking(() =>
                {
                    _ = ColumnItemExtensions.GetColumnValues(columns, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reference");
            }

            [Fact]
            public void Should_Get_Reference_Column_Value_Types()
            {
                var columns = new List<IColumnDefinition>();
                var entity = Create<EntityDummy>();

                columns.Add(new ColumnDefinition<EntityDummy, string>(entity.GetType().GetPropertyInfo("Name"), Create<bool>()));
                columns.Add(new ColumnDefinition<EntityDummy, int>(entity.GetType().GetPropertyInfo("Id"), Create<bool>()));

                var actual = ColumnItemExtensions.GetColumnValues(columns, entity);

                var expected = new object[]
                {
                    entity.Name,
                    entity.Id
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}