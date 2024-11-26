using AllOverIt.Assertion;
using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>A migration checker that determines the state of a database compared to its associated <see cref="DbContext"/>.</summary>
    public abstract class MigrationCheckerBase<TDbContext> : IMigrationChecker where TDbContext : DbContext
    {
        private readonly IDbContextFactory<TDbContext> _dbContextFactory;

        /// <summary>Constructor.</summary>
        /// <param name="dbContextFactory">The <see cref="DbContext"/> factory used to create an instance of the <typeparamref name="TDbContext"/>.</param>
        public MigrationCheckerBase(IDbContextFactory<TDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory.WhenNotNull();
        }

        /// <inheritdoc />
        public async Task<MigrationStatus> GetStatusAsync()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var applicationMigrationCount = dbContext.Database.GetMigrations().Count();

                var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync(CancellationToken.None);
                var appliedMigrationCount = appliedMigrations.Count();

                if (applicationMigrationCount == appliedMigrationCount)
                {
                    return MigrationStatus.InSync;
                }

                return applicationMigrationCount > appliedMigrationCount
                    ? MigrationStatus.ApplicationIsAhead
                    : MigrationStatus.DatabaseIsAhead;
            }
        }
    }
}
