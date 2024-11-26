using System.Linq.Expressions;

namespace AllOverIt.Helpers.PropertyNavigation
{
    /// <inheritdoc cref="IPropertyNodes{TType}" />
    internal sealed class PropertyNodes<TType> : IPropertyNodes<TType>
    {
        private readonly List<PropertyNode> _nodes = [];

        /// <summary>Provides a <see cref="MemberExpression"/> for each navigated property node.</summary>
        public PropertyNode[] Nodes => [.. _nodes];

        /// <inheritdoc />
        public Type ObjectType => typeof(TType);

        /// <summary>Constructor.</summary>
        public PropertyNodes()
        {
        }

        internal PropertyNodes(IEnumerable<PropertyNode> nodes)
        {
            _nodes.AddRange(nodes);
        }
    }
}