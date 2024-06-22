using AllOverIt.Assertion;
using AllOverIt.Types;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="PropertyInfo"/> types.</summary>
    public static class PropertyInfoExtensions
    {
        private static readonly Type IsExternalInitType = typeof(IsExternalInit);

        /// <summary>Determines if the provided <paramref name="propertyInfo"/> is for an abstract property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="accessor">A flag to indicate if the method should consider <c>get</c>, or <c>set</c>, or both accessors..</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for an abstract property, otherwise <see langword="false" />.</returns>
        public static bool IsAbstract(this PropertyInfo propertyInfo, PropertyAccessor accessor = PropertyAccessor.Get)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return HasPropertyAccessor(propertyInfo, accessor, methodInfo => methodInfo.IsAbstract);
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for an internal property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="accessor">A flag to indicate if the method should consider <c>get</c>, or <c>set</c>, or both accessors..</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for an internal property, otherwise <see langword="false" />.</returns>
        public static bool IsInternal(this PropertyInfo propertyInfo, PropertyAccessor accessor = PropertyAccessor.Get)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return HasPropertyAccessor(propertyInfo, accessor, methodInfo => methodInfo.IsAssembly);
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a private property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="accessor">A flag to indicate if the method should consider <c>get</c>, or <c>set</c>, or both accessors..</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a virtual property, otherwise <see langword="false" />.</returns>
        public static bool IsPrivate(this PropertyInfo propertyInfo, PropertyAccessor accessor = PropertyAccessor.Get)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return HasPropertyAccessor(propertyInfo, accessor, methodInfo => methodInfo.IsPrivate);
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a protected property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="accessor">A flag to indicate if the method should consider <c>get</c>, or <c>set</c>, or both accessors..</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a protected property, otherwise <see langword="false" />.</returns>
        public static bool IsProtected(this PropertyInfo propertyInfo, PropertyAccessor accessor = PropertyAccessor.Get)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return HasPropertyAccessor(propertyInfo, accessor, methodInfo => methodInfo.IsFamily);
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a public property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="accessor">A flag to indicate if the method should consider <c>get</c>, or <c>set</c>, or both accessors..</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a public property, otherwise <see langword="false" />.</returns>
        public static bool IsPublic(this PropertyInfo propertyInfo, PropertyAccessor accessor = PropertyAccessor.Get)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return HasPropertyAccessor(propertyInfo, accessor, methodInfo => methodInfo.IsPublic);
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a static property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="accessor">A flag to indicate if the method should consider <c>get</c>, or <c>set</c>, or both accessors..</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a static property, otherwise <see langword="false" />.</returns>
        public static bool IsStatic(this PropertyInfo propertyInfo, PropertyAccessor accessor = PropertyAccessor.Get)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return HasPropertyAccessor(propertyInfo, accessor, methodInfo => methodInfo.IsStatic);
        }

        /// <summary>Determines of the <paramref name="propertyInfo"/> is for a virtual property.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <param name="accessor">A flag to indicate if the method should consider <c>get</c>, or <c>set</c>, or both accessors..</param>
        /// <returns><see langword="true" /> if the <paramref name="propertyInfo"/> is for a virtual property, otherwise <see langword="false" />.</returns>
        public static bool IsVirtual(this PropertyInfo propertyInfo, PropertyAccessor accessor = PropertyAccessor.Get)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return HasPropertyAccessor(propertyInfo, accessor, methodInfo => methodInfo.IsVirtual);
        }

        /// <summary>Determines if a property is an indexer.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="true" /> if the property is an indexer.</returns>
        public static bool IsIndexer(this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetIndexParameters().Length != 0;
        }

        /// <summary>Determines if a property's has an <c>init</c> accessor.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="True" /> if the property has an <c>init</c> accessor.</returns>
        public static bool IsInitOnly(this PropertyInfo propertyInfo)
        {
            var setMethod = propertyInfo
                .WhenNotNull(nameof(propertyInfo))
                .SetMethod;

            if (setMethod is null)
            {
                return false;
            }

            return setMethod.ReturnParameter
                .GetRequiredCustomModifiers()
                .Contains(IsExternalInitType);
        }

        /// <summary>Determines if a property is compiler generated, such as a record's <c>EqualityContract</c>.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for a property.</param>
        /// <returns><see langword="True" /> if the property is compiler generated.</returns>
        public static bool IsCompilerGenerated([NotNull] this PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return propertyInfo.GetCustomAttribute<CompilerGeneratedAttribute>() is not null;
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

        private static bool HasPropertyAccessor(PropertyInfo propertyInfo, PropertyAccessor accessor, Func<MethodInfo, bool> predicate)
        {
            var result = true;

            if (accessor.HasFlag(PropertyAccessor.Get))
            {
                result = propertyInfo.GetMethod is not null && predicate.Invoke(propertyInfo.GetMethod);
            }

            if (result && accessor.HasFlag(PropertyAccessor.Set))
            {
                result = propertyInfo.SetMethod is not null && predicate.Invoke(propertyInfo.SetMethod);
            }

            if (result && accessor.HasFlag(PropertyAccessor.Init))
            {
                result = propertyInfo.IsInitOnly();
            }

            return result;
        }
    }
}