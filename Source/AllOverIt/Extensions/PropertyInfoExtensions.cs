using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AllOverIt.Assertion;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="PropertyInfo"/> types.</summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>Determines if the provided <paramref name="propertyInfo"/> is for an abstract property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for an abstract property, otherwise false.</returns>
        public static bool IsAbstract(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetMethod.IsAbstract;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for an internal property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for an internal property, otherwise false.</returns>
        public static bool IsInternal(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetMethod.IsAssembly;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a private property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a virtual property, otherwise false.</returns>
        public static bool IsPrivate(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetMethod.IsPrivate;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a protected property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a protected property, otherwise false.</returns>
        public static bool IsProtected(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetMethod.IsFamily;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a public property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a public property, otherwise false.</returns>
        public static bool IsPublic(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetMethod.IsPublic;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a static property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a static property, otherwise false.</returns>
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetMethod.IsStatic;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a virtual property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the <paramref name="propertyInfo"/> is for a virtual property, otherwise false.</returns>
        public static bool IsVirtual(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetMethod.IsVirtual;
        }

        /// <summary>Determines if a property is an indexer.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns>True if the property is an indexer.</returns>
        public static bool IsIndexer(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetIndexParameters().Any();
        }

        /// <summary>Creates a lambda expression that represents accessing a field or property on an object of type <typeparamref name="TType"/>.
        /// If the <paramref name="parameterName"/> is 'item' and the property info refers to a property named 'Age' then the expression will represent
        /// 'item => item.Age'.</summary>
        /// <typeparam name="TType">The object type containing the property.</typeparam>
        /// <typeparam name="TPropertyType">The property type.</typeparam>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="parameterName">The parameter name to represent the object being accessed.</param>
        /// <returns>A new lambda expression that represents accessing a field or property on an object.</returns>
        public static Expression<Func<TType, TPropertyType>> CreateMemberAccessLambda<TType, TPropertyType>(this PropertyInfo propertyInfo, string parameterName)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));
            _ = parameterName.WhenNotNullOrEmpty(nameof(parameterName));

            // item
            var parameter = Expression.Parameter(typeof(TType), parameterName);

            // item.Age
            var propertyMemberAccess = Expression.MakeMemberAccess(parameter, propertyInfo);

            // item => item.Age
            return Expression.Lambda<Func<TType, TPropertyType>>(propertyMemberAccess, parameter);
        }
    }
}