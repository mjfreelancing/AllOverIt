using System.Linq.Expressions;

namespace AllOverIt.Helpers.PropertyNavigation
{
    /// <summary>Provides the ability to navigate a property chain of a specified type to obtain <see cref="MemberExpression"/> information for each property.</summary>
    public static class PropertyNavigator
    {
        /// <summary>Initiates the property navigation for a specified type.</summary>
        /// <typeparam name="TType">The object type to navigate.</typeparam>
        /// <returns>An <see cref="IPropertyNodes{TType}"/> that can be used to navigate into a property chain.</returns>
        public static IPropertyNodes<TType> For<TType>()
        {
            return new PropertyNodes<TType>();
        }
    }
}