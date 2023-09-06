using System;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>Specifies options to apply when performing a database migration.</summary>
    public sealed class MigrationOptions
    {
        /// <summary>The timeout to apply when executing migration commands. Optional.</summary>
        public TimeSpan? CommandTimeout { get; init; }
    }
}
