namespace AllOverIt.Helpers.PropertyNavigation
{
    /// <summary>One or more property nodes in an object graph.</summary>
    public interface IPropertyNodes
    {
        /// <summary>The object type that the node information describes.</summary>
        Type ObjectType { get; }

        /// <summary>One or more chained property nodes in sequence from root to leaf.</summary>
        IReadOnlyCollection<PropertyNode> Nodes { get; }
    }

    /// <summary>Provides information for all property nodes in an object graph of a specified type.</summary>
    /// <typeparam name="TType">The object type associated with the navigated property nodes.</typeparam>
    public interface IPropertyNodes<TType> : IPropertyNodes
    {
    }
}