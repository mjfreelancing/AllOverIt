using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Migrator.Events;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>An abstract class providing the ability to report and apply pending migrations to a database.
    /// If the database does not exist then it will be created.</summary>
    /// <typeparam name="TDbContext">The <see cref="DbContext"/> representing the database to be updated.</typeparam>
    public abstract class DatabaseMigratorBase<TDbContext> : IDatabaseMigrator where TDbContext : DbContext
    {
        private readonly IDbContextFactory<TDbContext> _dbContextFactory;

        /// <inheritdoc />
        public event EventHandler<MigrationEventArgs>? OnNewMigration;

        /// <summary>Constructor.</summary>
        /// <param name="dbContextFactory">The <see cref="DbContext"/> factory used to create an instance of the <typeparamref name="TDbContext"/>.</param>
        public DatabaseMigratorBase(IDbContextFactory<TDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory.WhenNotNull(nameof(dbContextFactory));
        }

        /// <inheritdoc />
        public async Task MigrateAsync(MigrationOptions? options = default)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                ApplyOptions(dbContext.Database, options);

                var allMigrations = dbContext.Database.GetMigrations();
                var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync(CancellationToken.None);
                var pendingMigrations = allMigrations.Except(appliedMigrations).AsReadOnlyCollection();

                var onNewMigration = OnNewMigration;

                if (onNewMigration is not null && pendingMigrations.Count != 0)
                {
                    foreach (var migration in pendingMigrations)
                    {
                        var eventArgs = new MigrationEventArgs(migration);

                        onNewMigration.Invoke(this, eventArgs);
                    }
                }

                // This will create the database if it does not exist
                await dbContext.Database
                    .MigrateAsync(CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }

        private static void ApplyOptions(Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade database, MigrationOptions? options)
        {
            var commandTimeout = options?.CommandTimeout;

            if (commandTimeout.HasValue)
            {
                database.SetCommandTimeout(commandTimeout.Value);
            }
        }
    }
}
