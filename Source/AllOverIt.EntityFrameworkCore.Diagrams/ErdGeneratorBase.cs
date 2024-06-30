using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Identifies an entity type.</summary>
    /// <param name="Type">The entity type.</param>
    /// <param name="TableName">The entity's associated table name.</param>
    public sealed record EntityIdentifier(Type Type, string TableName);     // A record as it is used as a key in a dictionary

    /// <summary>Base class for an entity relationship diagram generator.</summary>
    public abstract class ErdGeneratorBase : IErdGenerator
    {
        /// <summary>The entity relationship diagram generator options.</summary>
        protected ErdOptions Options { get; }

        /// <summary>Constructor.</summary>
        /// <param name="options">The entity relationship diagram generator options.</param>
        protected ErdGeneratorBase(ErdOptions options)
        {
            Options = options.WhenNotNull(nameof(options));
        }

        /// <summary>Generates the entity relationship diagram.</summary>
        /// <param name="dbContext">The source <see cref="DbContext"/> to generate an entity relationship diagram.</param>
        /// <returns>The generated diagram text.</returns>
        public string Generate(DbContext dbContext)
        {
            var entityColumns = GetEntityColumnDescriptors(dbContext);

            return GenerateDiagram(dbContext, entityColumns);
        }

        /// <summary>Override in a concrete class to generate the entity relationship diagram using the provided
        /// <see cref="DbContext"/> and entity-column relationship details.</summary>
        /// <param name="dbContext">The source <see cref="DbContext"/> to generate an entity relationship diagram.</param>
        /// <param name="entityColumns">A read-only dictionary containing relationship details for all entities and their
        /// associated columns discovered from the provided <paramref name="dbContext"/>.</param>
        /// <returns>An entity relationship diagram based on a provided <see cref="DbContext"/>.</returns>
        public abstract string GenerateDiagram(DbContext dbContext, EntityColumns entityColumns);

        private static EntityColumns GetEntityColumnDescriptors(DbContext dbContext)
        {
            var entityDescriptors = new Dictionary<EntityIdentifier, IReadOnlyCollection<IColumnDescriptor>>();

            var entityTypes = dbContext.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var entityName = entityType.GetTableName()!;
                var descriptors = entityType.GetProperties().SelectToArray(ColumnDescriptor.Create);

                var identifier = new EntityIdentifier(entityType.ClrType, entityName);

                entityDescriptors.Add(identifier, descriptors);
            }

            return new EntityColumns(entityDescriptors);
        }
    }
}