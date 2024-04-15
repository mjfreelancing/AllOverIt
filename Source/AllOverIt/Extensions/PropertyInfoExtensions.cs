using AllOverIt.Assertion;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="PropertyInfo"/> types.</summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>Determines if the provided <paramref name="propertyInfo"/> is for an abstract property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for an abstract property, otherwise <see langword="false" />.</returns>
        public static bool IsAbstract([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            Throw<InvalidOperationException>.WhenNull(propertyInfo.GetMethod, $"The property '{propertyInfo.Name}' is expected to have a getter.");

            return propertyInfo.GetMethod.IsAbstract;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for an internal property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for an internal property, otherwise <see langword="false" />.</returns>
        public static bool IsInternal([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            Throw<InvalidOperationException>.WhenNull(propertyInfo.GetMethod, $"The property '{propertyInfo.Name}' is expected to have a getter.");

            return propertyInfo.GetMethod.IsAssembly;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a private property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a virtual property, otherwise <see langword="false" />.</returns>
        public static bool IsPrivate([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            Throw<InvalidOperationException>.WhenNull(propertyInfo.GetMethod, $"The property '{propertyInfo.Name}' is expected to have a getter.");

            return propertyInfo.GetMethod.IsPrivate;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a protected property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a protected property, otherwise <see langword="false" />.</returns>
        public static bool IsProtected([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            Throw<InvalidOperationException>.WhenNull(propertyInfo.GetMethod, $"The property '{propertyInfo.Name}' is expected to have a getter.");

            return propertyInfo.GetMethod.IsFamily;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a public property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a public property, otherwise <see langword="false" />.</returns>
        public static bool IsPublic([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            Throw<InvalidOperationException>.WhenNull(propertyInfo.GetMethod, $"The property '{propertyInfo.Name}' is expected to have a getter.");

            return propertyInfo.GetMethod.IsPublic;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a static property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a static property, otherwise <see langword="false" />.</returns>
        public static bool IsStatic([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            Throw<InvalidOperationException>.WhenNull(propertyInfo.GetMethod, $"The property '{propertyInfo.Name}' is expected to have a getter.");

            return propertyInfo.GetMethod.IsStatic;
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a virtual property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a virtual property, otherwise <see langword="false" />.</returns>
        public static bool IsVirtual([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            Throw<InvalidOperationException>.WhenNull(propertyInfo.GetMethod, $"The property '{propertyInfo.Name}' is expected to have a getter.");

            return propertyInfo.GetMethod.IsVirtual;
        }

        /// <summary>Determines if a property is an indexer.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the property is an indexer.</returns>
        public static bool IsIndexer([NotNull] this PropertyInfo? propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetIndexParameters().Length != 0;
        }

        /// <summary>Creates a lambda expression that represents accessing a property on an object of type <typeparamref name="TType"/>.
        /// If the <paramref name="parameterName"/> is 'item' and the property info refers to a property named 'Age' then the expression will represent
        /// 'item => item.Age'.</summary>
        /// <typeparam name="TType">The object type containing the property.</typeparam>
        /// <typeparam name="TPropertyType">The property type.</typeparam>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="parameterName">The parameter name to represent the object being accessed.</param>
        /// <returns>A new lambda expression that represents accessing a property on an object.</returns>
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