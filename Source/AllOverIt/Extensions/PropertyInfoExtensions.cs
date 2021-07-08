using System.Linq;
using System.Reflection;

namespace AllOverIt.Extensions
{
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Determines if the provided <paramref name="propertyInfo"/> is for an abstract property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for an abstract property, otherwise false.</returns>
        public static bool IsAbstract(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsAbstract;
        }

        /// <summary>
        /// Determines of the <paramref name="propertyInfo"/> is for an internal property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for an internal property, otherwise false.</returns>
        public static bool IsInternal(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsAssembly;
        }

        /// <summary>
        /// Determines of the <paramref name="propertyInfo"/> is for a private property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a virtual property, otherwise false.</returns>
        public static bool IsPrivate(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsPrivate;
        }

        /// <summary>
        /// Determines of the <paramref name="propertyInfo"/> is for a protected property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a protected property, otherwise false.</returns>
        public static bool IsProtected(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsFamily;
        }

        /// <summary>
        /// Determines of the <paramref name="propertyInfo"/> is for a public property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a public property, otherwise false.</returns>
        public static bool IsPublic(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsPublic;
        }

        /// <summary>
        /// Determines of the <paramref name="propertyInfo"/> is for a static property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a static property, otherwise false.</returns>
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsStatic;
        }

        /// <summary>
        /// Determines of the <paramref name="propertyInfo"/> is for a virtual property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a virtual property, otherwise false.</returns>
        public static bool IsVirtual(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsVirtual;
        }

        /// <summary>
        /// Determines if a property is an indexer.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the property is an indexer.</returns>
        public static bool IsIndexer(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Any();
        }
    }
}