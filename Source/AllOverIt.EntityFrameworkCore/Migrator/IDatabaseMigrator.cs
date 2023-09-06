using AllOverIt.EntityFrameworkCore.Migrator.Events;
using System;
using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>Represents a database migrator.</summary>
    public interface IDatabaseMigrator
    {
        /// <summary>Raised with the name of a migration to be applied to the database.</summary>
        event EventHandler<MigrationEventArgs> OnNewMigration;

        /// <summary>Applies pending migrations to a database. If the database does not exist then it will be created.</summary>
        /// <param name="options">Provides migration options.</param>
        /// <returns>A task that completes when the migration is completely applied.</returns>
        Task MigrateAsync(MigrationOptions options = default);
    }
}
