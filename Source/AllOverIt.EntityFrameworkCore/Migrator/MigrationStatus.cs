using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>The migration status of a database compared to its associated <see cref="DbContext"/>.</summary>
    public enum MigrationStatus
    {
        /// <summary>The database migration status matches the application's <see cref="DbContext"/>.</summary>
        InSync,

        /// <summary>The application's <see cref="DbContext"/> contains more migrations than the database.</summary>
        ApplicationIsAhead,

        /// <summary>The database contains more migrations than the <see cref="DbContext"/> used by the application.</summary>
        DatabaseIsAhead
    }
}
