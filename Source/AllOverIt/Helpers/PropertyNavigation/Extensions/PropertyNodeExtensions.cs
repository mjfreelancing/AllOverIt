namespace AllOverIt.Helpers.PropertyNavigation.Extensions
{
    /// <summary>Contains extension methods for use with <see cref="PropertyNode"/> instances.</summary>
    public static class PropertyNodeExtensions
    {
        /// <summary>Gets the name of the provided property node.</summary>
        /// <param name="node">The property node information.</param>
        /// <returns>The name of the property node.</returns>
        public static string Name(this PropertyNode node)
        {
            return node.Expression.Member.Name;
        }
    }
}