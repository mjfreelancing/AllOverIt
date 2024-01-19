using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>A migration checker that determines the state of a database compared to its associated <see cref="DbContext"/>.</summary>
    public interface IMigrationChecker
    {
        /// <summary>Gets the current migration status of the database.</summary>
        /// <returns>A task that completes with the migration status of the database</returns>
        Task<MigrationStatus> GetStatusAsync();
    }
}
