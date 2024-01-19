using AllOverIt.Assertion;
using System;

namespace AllOverIt.EntityFrameworkCore.Migrator.Events
{
    /// <summary>Defines database migration related event arguments.</summary>
    public class MigrationEventArgs : EventArgs
    {
        /// <summary>The name of the migration.</summary>
        public string Migration { get; }

        /// <summary>Constructor.</summary>
        /// <param name="migration">The name of the migration.</param>
        public MigrationEventArgs(string migration)
        {
            Migration = migration.WhenNotNull(nameof(migration));
        }
    }
}
