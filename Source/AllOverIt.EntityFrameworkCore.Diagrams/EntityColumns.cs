using AllOverIt.Collections;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>A read-only dictionary containing relationship details for all entities and their associated columns discovered
    /// from a <see cref="DbContext"/>. The keys identify each entity and their value provides the associated column descriptors.</summary>
    public sealed class EntityColumns : ReadOnlyDictionary<EntityIdentifier, IReadOnlyCollection<ColumnDescriptor>>
    {
        internal EntityColumns(IDictionary<EntityIdentifier, IReadOnlyCollection<ColumnDescriptor>> entityColumns)
            : base(entityColumns)
        {            
        }
    }
}