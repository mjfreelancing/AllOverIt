using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Formatters.Objects.Exceptions;
using AllOverIt.Helpers.PropertyNavigation;
using AllOverIt.Helpers.PropertyNavigation.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Formatters.Objects.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ObjectPropertyEnumerableOptions"/> types.</summary>
    public static class ObjectPropertyEnumerableOptionsExtensions
    {
        /// <summary>Provides the ability to set <see cref="ObjectPropertyEnumerableOptions.AutoCollatedPaths"/> using property nodes via
        /// <see cref="PropertyNavigator"/>.</summary>
        /// <param name="options">The enumerable options associated with an <see cref="ObjectPropertyFilter"/>.</param>
        /// <param name="propertyNodes">The property node information provided via <see cref="PropertyNavigator"/>.</param>
        /// <remarks>This method also validates the property to be collated is not a class type (the <see cref="ObjectPropertyFilter"/> does not support collating class types).</remarks>
        public static void SetAutoCollatedPaths(this ObjectPropertyEnumerableOptions options, params IPropertyNodes[] propertyNodes)
        {
            _ = options.WhenNotNull();
            _ = propertyNodes.WhenNotNullOrEmpty();

            var fullPaths = propertyNodes.SelectToArray(item =>
            {
                var fullNodePath = item.GetFullNodePath();

                var member = item.Nodes.Last().Expression;

                var leafNodeType = member.Member.GetMemberType();

                Type? elementType = null;

                if (leafNodeType.IsArray)
                {
                    elementType = leafNodeType.GetElementType();
                }

                if (CommonTypes.IEnumerableType.IsAssignableFrom(leafNodeType))
                {
                    if (leafNodeType.IsGenericType)
                    {
                        elementType = leafNodeType.GetGenericArguments()[0];
                    }
                }

                // elementType is null when not an array or IEnumerable
                var typeToCheck = elementType ?? leafNodeType;

                if (typeToCheck.IsClass && typeToCheck != CommonTypes.StringType)
                {
                    throw new ObjectPropertyFilterException($"The leaf property on path '{fullNodePath}' cannot be a class type ({item.ObjectType.GetFriendlyName()}).");
                }

                return fullNodePath;
            });

            options.AutoCollatedPaths = fullPaths;
        }
    }
}