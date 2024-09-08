﻿using AllOverIt.EntityFrameworkCore.Extensions;
using AllOverIt.EntityFrameworkCore.ValueConverters;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Assertions;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Patterns.ResourceInitialization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Runtime.CompilerServices;
using Testcontainers.PostgreSql;

namespace AllOverIt.EntityFrameworkCore.Tests.Extensions
{
    // These tests require Docker to be installed
    public class ModelBuilderExtensionsFixture : FixtureBase
    {
        public sealed class DummyEnum1 : EnrichedEnum<DummyEnum1>
        {
            public static readonly DummyEnum1 Value0 = new(0);
            public static readonly DummyEnum1 Value1 = new(1);
            public static readonly DummyEnum1 Value2 = new(2);

            private DummyEnum1()
            {
            }

            private DummyEnum1(int value, [CallerMemberName] string name = "")
                : base(value, name)
            {
            }
        }

        public sealed class DummyEnum2 : EnrichedEnum<DummyEnum2>
        {
            public static readonly DummyEnum2 Value0 = new(0);
            public static readonly DummyEnum2 Value1 = new(1);
            public static readonly DummyEnum2 Value2 = new(2);

            private DummyEnum2()
            {
            }

            private DummyEnum2(int value, [CallerMemberName] string name = "")
                : base(value, name)
            {
            }
        }

        public sealed class DummyEntity
        {
            public int Id { get; set; }
            public DummyEnum1 Prop1a { get; set; }
            public DummyEnum1 Prop1b { get; set; }
            public DummyEnum2 Prop2a { get; set; }
            public DummyEnum2 Prop2b { get; set; }
        }

        public async Task<RaiiAsync<PostgreSqlContainer>> GetPostgreSqlContainerAsync()
        {
            var postgreSqlContainer = new PostgreSqlBuilder()
                .WithDatabase(Guid.NewGuid().ToString())
                .WithUsername(Create<string>())
                .WithPassword(Create<string>())
                .Build();

            await postgreSqlContainer.StartAsync();

            return new RaiiAsync<PostgreSqlContainer>(
                () => postgreSqlContainer,
                async container =>
                {
                    await container.StopAsync();
                });
        }

        protected DbContextOptions<TDbContext> GetDbContextOptions<TDbContext>(PostgreSqlContainer container) where TDbContext : DbContext
        {
            return new DbContextOptionsBuilder<TDbContext>()
                .UseNpgsql(container.GetConnectionString())
                .Options;
        }

        public class UseEnrichedEnum_Default_All_Integer : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum();
                }
            }

            [Fact]
            public async Task Should_Configure_All_Properties_As_Integer_By_Default()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(int), typeof(int));
            }
        }

        public class UseEnrichedEnum_Property_Generic : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Property<DummyEnum2>()
                            .AsName();
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Properties_Of_Type_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(string), typeof(string));
            }
        }

        public class UseEnrichedEnum_Property_Type : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Property(typeof(DummyEnum2))
                            .AsName();
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Properties_Of_Type_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(string), typeof(string));
            }
        }

        public class UseEnrichedEnum_Property_Name : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Property(nameof(DummyEntity.Prop2a))
                            .AsName();

                        options
                            .Entity<DummyEntity>()
                            .Property(nameof(DummyEntity.Prop2b))
                            .AsName();
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Properties_Of_Type_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(string), typeof(string));
            }
        }

        public class UseEnrichedEnum_Property_Name_With_ColumnType_And_MaxLength : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Property<DummyEnum2>()
                            .AsName(columnType: "character varying(50)", maxLength: 50);
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Properties_Of_Type_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                var columnDefinitions = new Dictionary<string, (string columnType, int? columnLength)>();

                columnDefinitions.Add(nameof(DummyEntity.Prop2a), ("character varying(50)", 50));
                columnDefinitions.Add(nameof(DummyEntity.Prop2b), ("character varying(50)", 50));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(string), typeof(string), columnDefinitions);
            }
        }

        public class UseEnrichedEnum_Property_Name_With_ColumnType : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Property<DummyEnum2>()
                            .AsName(columnType: "citext");
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Properties_Of_Type_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                var columnDefinitions = new Dictionary<string, (string columnType, int? columnLength)>();

                columnDefinitions.Add(nameof(DummyEntity.Prop2a), ("citext", null));
                columnDefinitions.Add(nameof(DummyEntity.Prop2b), ("citext", null));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(string), typeof(string), columnDefinitions);
            }
        }

        public class UseEnrichedEnum_Property_Name_With_MaxLength : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Property<DummyEnum2>()
                            .AsName(maxLength: 25);     // Will become "character varying(25)"
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Properties_Of_Type_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                var columnDefinitions = new Dictionary<string, (string columnType, int? columnLength)>();

                columnDefinitions.Add(nameof(DummyEntity.Prop2a), ("character varying(25)", 25));
                columnDefinitions.Add(nameof(DummyEntity.Prop2b), ("character varying(25)", 25));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(string), typeof(string), columnDefinitions);
            }
        }

        public class UseEnrichedEnum_Properties_Type : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Properties(typeof(DummyEnum2))
                            .AsName();
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Properties_Of_Type_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(int), typeof(string), typeof(string));
            }
        }

        public class UseEnrichedEnum_All_String : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options.Entity<DummyEntity>().AsName();
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_All_Properties_As_String()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                AssertPropertyExpectations(dbContext, typeof(string), typeof(string), typeof(string), typeof(string));
            }
        }

        public class UseEnrichedEnum_By_Name : ModelBuilderExtensionsFixture
        {
            public sealed class DummyDbContext : DbContext
            {
                public DbSet<DummyEntity> Entities { get; set; }

                public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) { }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    base.OnModelCreating(modelBuilder);

                    modelBuilder.UseEnrichedEnum(options =>
                    {
                        options
                            .Entity<DummyEntity>()
                            .Properties(nameof(DummyEntity.Prop1b), nameof(DummyEntity.Prop2a))
                            .AsName();
                    });
                }
            }

            [Fact]
            public async Task Should_Configure_Individual_Property_By_Name()
            {
                await using var container = await GetPostgreSqlContainerAsync();

                using var dbContext = new DummyDbContext(GetDbContextOptions<DummyDbContext>(container.Context));

                AssertPropertyExpectations(dbContext, typeof(int), typeof(string), typeof(string), typeof(int));
            }
        }

        protected IProperty[] GetConfiguredProperties(DbContext dbContext)
        {
            return dbContext.Model.GetEntityTypes().Single()
                .GetProperties()
                .Where(property => property.Name != nameof(DummyEntity.Id))
                .ToArray();
        }

        protected Dictionary<string, object> GetExpectations(Type prop1aType, Type prop1bType, Type prop2aType, Type prop2bType,
            Dictionary<string, (string columnType, int? columnLength)> columnDefinitions)
        {
            static string GetPropType(Type type) => type == typeof(int) ? "integer" : "text";

            static Type GetProp1ValueConverter(Type type)
            {
                if (type is null)
                {
                    return null;
                }

                return type == typeof(int)
                    ? typeof(EnrichedEnumValueConverter<DummyEnum1>)
                    : typeof(EnrichedEnumNameConverter<DummyEnum1>);
            }

            static Type GetProp2ValueConverter(Type type)
            {
                if (type is null)
                {
                    return null;
                }

                return type == typeof(int)
                    ? typeof(EnrichedEnumValueConverter<DummyEnum2>)
                    : typeof(EnrichedEnumNameConverter<DummyEnum2>);
            }

            (string columnType, int? columnLength) GetColumnDefinition(string propertyName)
            {
                if (columnDefinitions is null || !columnDefinitions.TryGetValue(propertyName, out var value))
                {
                    return (null, null);
                }

                return value;
            }

            var expectations = new Dictionary<string, object>();

            (string ColumnType, int? ColumnLength) columnDefinition = GetColumnDefinition(nameof(DummyEntity.Prop1a));

            expectations[nameof(DummyEntity.Prop1a)] = new
            {
                PropertyName = nameof(DummyEntity.Prop1a),
                ColumnType = columnDefinition.ColumnType ?? GetPropType(prop1aType),
                MaxLength = columnDefinition.ColumnLength,
                ValueConverter = GetProp1ValueConverter(prop1aType)
            };

            columnDefinition = GetColumnDefinition(nameof(DummyEntity.Prop1b));

            expectations[nameof(DummyEntity.Prop1b)] = new
            {
                PropertyName = nameof(DummyEntity.Prop1b),
                ColumnType = columnDefinition.ColumnType ?? GetPropType(prop1bType),
                MaxLength = columnDefinition.ColumnLength,
                ValueConverter = GetProp1ValueConverter(prop1bType)
            };

            columnDefinition = GetColumnDefinition(nameof(DummyEntity.Prop2a));

            expectations[nameof(DummyEntity.Prop2a)] = new
            {
                PropertyName = nameof(DummyEntity.Prop2a),
                ColumnType = columnDefinition.ColumnType ?? GetPropType(prop2aType),
                MaxLength = columnDefinition.ColumnLength,
                ValueConverter = GetProp2ValueConverter(prop2aType)
            };

            columnDefinition = GetColumnDefinition(nameof(DummyEntity.Prop2b));

            expectations[nameof(DummyEntity.Prop2b)] = new
            {
                PropertyName = nameof(DummyEntity.Prop2b),
                ColumnType = columnDefinition.ColumnType ?? GetPropType(prop2bType),
                MaxLength = columnDefinition.ColumnLength,
                ValueConverter = GetProp2ValueConverter(prop2bType)
            };

            return expectations;
        }

        protected void AssertPropertyExpectations(DbContext dbContext, Type prop1aType, Type prop1bType, Type prop2aType, Type prop2bType,
            Dictionary<string, (string columnType, int? columnLength)> columnDefinitions = default)
        {
            dbContext.Database.EnsureCreated();

            var properties = GetConfiguredProperties(dbContext);
            var expectations = GetExpectations(prop1aType, prop1bType, prop2aType, prop2bType, columnDefinitions);

            foreach (var property in properties)
            {
                var valueConverter = property.GetValueConverter();

                var actual = new
                {
                    PropertyName = property.Name,
                    ColumnType = property.GetColumnType(),
                    MaxLength = property.GetMaxLength(),
                    ValueConverter = valueConverter is null ? null : valueConverter.GetType()
                };

                expectations[property.Name].Should().BeEquivalentTo(actual);
            }
        }
    }
}