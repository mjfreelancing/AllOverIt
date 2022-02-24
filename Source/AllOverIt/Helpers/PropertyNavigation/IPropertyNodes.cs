using System;
using System.Collections.Generic;

namespace AllOverIt.Helpers.PropertyNavigation
{
    /// <summary>Represents one or more property nodes in an object graph.</summary>
    public interface IPropertyNodes
    {
        /// <summary>The object type that the node information describes.</summary>
        Type ObjectType { get; }

        /// <summary>One or more chained property nodes in sequence from root to leaf.</summary>
        IReadOnlyCollection<PropertyNode> Nodes { get; }
    }

    /// <summary>Represents one or more property nodes in an object graph of a specified type.</summary>
    public interface IPropertyNodes<TType> : IPropertyNodes
    {
    }
}