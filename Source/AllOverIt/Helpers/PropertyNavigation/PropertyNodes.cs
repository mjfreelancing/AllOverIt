using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Helpers.PropertyNavigation
{
    /// <summary>Provides information for all property nodes navigated on a specific type.</summary>
    /// <typeparam name="TType">The object type associated with the navigated property nodes.</typeparam>
    internal sealed class PropertyNodes<TType> : IPropertyNodes<TType>
    {
        private readonly List<PropertyNode> _nodes = new();

        /// <summary>Provides a <see cref="MemberExpression"/> for each navigated property node.</summary>
        public IReadOnlyCollection<PropertyNode> Nodes => _nodes;

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