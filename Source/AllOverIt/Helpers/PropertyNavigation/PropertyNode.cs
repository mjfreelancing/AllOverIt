using System.Linq.Expressions;

namespace AllOverIt.Helpers.PropertyNavigation
{
    /// <summary>Provides information for a node in a navigated property chain.</summary>
    public sealed class PropertyNode
    {
        /// <summary>Expression information for a property node.</summary>
        public MemberExpression Expression { get; init; }
    }
}