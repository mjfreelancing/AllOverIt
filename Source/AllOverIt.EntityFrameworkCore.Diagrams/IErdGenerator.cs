using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Represents an entity relationship diagram generator.</summary>
    public interface IErdGenerator
    {
        /// <summary>Generates an entity relationship diagram as a string.</summary>
        /// <param name="dbContext">The EntityFramework <see cref="DbContext"/> used for describing the diagram to be generated.</param>
        /// <returns>An entity relationship diagram as a string.</returns>
        string Generate(DbContext dbContext);
    }
}