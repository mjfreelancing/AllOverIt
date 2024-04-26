using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System.Linq.Expressions;

namespace AllOverIt.Helpers.PropertyNavigation.Extensions
{
    /// <summary>Contains extension methods for use with <see cref="IPropertyNodes{TType}"/> instances.</summary>
    public static class PropertyNodesExtensions
    {
        /// <summary>Navigates from the root of an object type or one of its properties to an adjacent property within the current object graph.</summary>
        /// <typeparam name="TType">The object type being navigated.</typeparam>
        /// <typeparam name="TProperty">The next property type being navigated to.</typeparam>
        /// <param name="propertyNodes">Contains the current list of nodes already navigated.</param>
        /// <param name="expression">An expression to the next property in the navigation chain. This can include multiple child properties up until
        /// the desired leaf property is reached.</param>
        /// <returns>The same <see cref="IPropertyNodes{TType}"/> instance so subsequent navigation calls can be chained in a fluent syntax.</returns>
        /// <remarks></remarks>
        public static IPropertyNodes<TProperty> Navigate<TType, TProperty>(this IPropertyNodes<TType> propertyNodes, Expression<Func<TType, TProperty>> expression)
        {
            _ = propertyNodes.WhenNotNull(nameof(propertyNodes));
            _ = expression.WhenNotNull(nameof(expression));

            return CreateFrom(propertyNodes, expression);
        }

        /// <summary>Navigates from the root of an object type or one of its properties to an adjacent property within the current object graph.</summary>
        /// <typeparam name="TType">The object type being navigated.</typeparam>
        /// <typeparam name="TProperty">The next property type being navigated to.</typeparam>
        /// <param name="propertyNodes">Contains the current list of nodes already navigated.</param>
        /// <param name="expression">An expression to the next enumerable property in the navigation chain. To navigate beyond enumerable properties
        /// additional calls to Navigate must be made.</param>
        /// <returns>The same <see cref="IPropertyNodes{TType}"/> instance so subsequent navigation calls can be chained in a fluent syntax.</returns>
        public static IPropertyNodes<TProperty> Navigate<TType, TProperty>(this IPropertyNodes<TType> propertyNodes, Expression<Func<TType, IEnumerable<TProperty>>> expression)
        {
            _ = propertyNodes.WhenNotNull(nameof(propertyNodes));
            _ = expression.WhenNotNull(nameof(expression));

            return CreateFrom(propertyNodes, expression);
        }

        /// <summary>Gets the full navigated property path with each property name separated by the '.' (dot) character.</summary>
        /// <param name="propertyNodes">Contains the list of navigated property nodes.</param>
        /// <returns>The full, dot separated, property path.</returns>
        public static string GetFullNodePath(this IPropertyNodes propertyNodes)
        {
            _ = propertyNodes.WhenNotNull(nameof(propertyNodes));

            return string.Join(".", propertyNodes.Nodes.Select(item => item.Name()));
        }

        private static PropertyNodes<TProperty> CreateFrom<TType, TProperty>(this IPropertyNodes<TType> other, Expression<Func<TType, TProperty>> expression)
        {
            var nodes = GetNodes(expression);

            return new PropertyNodes<TProperty>(other.Nodes.Concat(nodes));
        }

        private static PropertyNodes<TProperty> CreateFrom<TType, TProperty>(IPropertyNodes<TType> other, Expression<Func<TType, IEnumerable<TProperty>>> expression)
        {
            var nodes = GetNodes(expression);

            return new PropertyNodes<TProperty>(other.Nodes.Concat(nodes));
        }

        private static IEnumerable<PropertyNode> GetNodes(Expression expression)
        {
            var memberExpressions = expression.GetMemberExpressions();

            foreach (var memberExpression in memberExpressions)
            {
                yield return new PropertyNode
                {
                    Expression = memberExpression
                };
            }
        }
    }
}