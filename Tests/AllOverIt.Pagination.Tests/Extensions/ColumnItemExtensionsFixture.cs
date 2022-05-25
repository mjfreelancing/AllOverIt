using System;
using System.Collections.Generic;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using Xunit;
using AllOverIt.Pagination.Extensions;
using FluentAssertions;

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
                new ColumnDefinition<EntityDummy, string>()
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

                columns.Add(new ColumnDefinition<EntityDummy, string>
                {
                    Property = entity.GetType().GetPropertyInfo("Name")
                });

                columns.Add(new ColumnDefinition<EntityDummy, int>
                {
                    Property = entity.GetType().GetPropertyInfo("Id")
                });

                var actual = ColumnItemExtensions.GetColumnValues(columns, entity);

                var expected = new[]
                {
                new
                {
                    Type = Type.GetTypeCode(typeof(string)),
                    Value = (object)entity.Name
                },
                new
                {
                    Type = Type.GetTypeCode(typeof(int)),
                    Value = (object)entity.Id
                }
            };

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}