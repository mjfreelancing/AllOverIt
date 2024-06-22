namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>Specifies options to apply when performing a database migration.</summary>
    public sealed class MigrationOptions
    {
        /// <summary>An optional timeout to apply when executing migration commands.</summary>
        public TimeSpan? CommandTimeout { get; init; }
    }
}
