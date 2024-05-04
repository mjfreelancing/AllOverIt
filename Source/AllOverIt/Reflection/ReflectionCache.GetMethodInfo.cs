using AllOverIt.Caching;
using AllOverIt.Extensions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Provides a default, static, cache to help improve performance where reflection is used extensively.</summary>
    public static partial class ReflectionCache
    {
        private static readonly GenericCache MethodInfoCache = [];

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <typeparamref name="TType"/> and binding options.</summary>
        /// <typeparam name="TType">The type to obtain method metadata for.</typeparam>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only method metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static IEnumerable<MethodInfo> GetMethodInfo<TType>(BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<MethodInfo>> valueResolver = default)
        {
            return GetMethodInfo(typeof(TType), bindingOptions, declaredOnly, valueResolver ?? GetMethodInfoFromTypeBindingDeclaredOnly());
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <paramref name="type"/> and binding options.</summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only method metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <paramref name="type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
        /// by  <paramref name="type"/>.</remarks>
        public static IEnumerable<MethodInfo> GetMethodInfo(Type type, BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<MethodInfo>> valueResolver = default)
        {
            var key = new GenericCacheKey<Type, BindingOptions, bool>(type, bindingOptions, declaredOnly);

            return MethodInfoCache.GetOrAdd(key, valueResolver ?? GetMethodInfoFromTypeBindingDeclaredOnly());
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <typeparamref name="TType"/> and method name.</summary>
        /// <typeparam name="TType">The type to obtain method metadata for.</typeparam>
        /// <param name="name">The name of the method.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <typeparamref name="TType"/> with a given name and no arguments.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo<TType>(string name, Func<GenericCacheKeyBase, MethodInfo> valueResolver = default)
        {
            return GetMethodInfo(typeof(TType), name, Type.EmptyTypes, valueResolver);
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <paramref name="type"/> and method name.</summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <paramref name="type"/> with a given name and no arguments.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo(Type type, string name, Func<GenericCacheKeyBase, MethodInfo> valueResolver = default)
        {
            return GetMethodInfo(type, name, Type.EmptyTypes, valueResolver);
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <typeparamref name="TType"/> method with a given name and argument types.</summary>
        /// <typeparam name="TType">The type to obtain method metadata for.</typeparam>
        /// <param name="name">The name of the method.</param>
        /// <param name="types">The argument types expected on the method</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <typeparamref name="TType"/> with a given name and argument types.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo<TType>(string name, Type[] types, Func<GenericCacheKeyBase, MethodInfo> valueResolver = default)
        {
            return GetMethodInfo(typeof(TType), name, types, valueResolver ?? GetMethodInfoFromTypeMethodNameArgTypes());
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <paramref name="type"/> method with a given name and argument types.</summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="types">The argument types expected on the method</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <paramref name="type"/> with a given name and argument types.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo(Type type, string name, Type[] types, Func<GenericCacheKeyBase, MethodInfo> valueResolver = default)
        {
            var key = new GenericCacheKey<Type, string, Type[]>(type, name, types);

            return MethodInfoCache.GetOrAdd(key, valueResolver ?? GetMethodInfoFromTypeMethodNameArgTypes());
        }

        private static Func<GenericCacheKeyBase, IEnumerable<MethodInfo>> GetMethodInfoFromTypeBindingDeclaredOnly()
        {
            return key =>
            {
                var (type, bindingOptions, declaredOnly) = (GenericCacheKey<Type, BindingOptions, bool>) key;

                return type
                    .GetMethodInfo(bindingOptions, declaredOnly)
                    .AsReadOnlyCollection();
            };
        }

        private static Func<GenericCacheKeyBase, MethodInfo> GetMethodInfoFromTypeMethodNameArgTypes()
        {
            return key =>
            {
                var (type, name, types) = (GenericCacheKey<Type, string, Type[]>) key;

                return type.GetMethodInfo(name, types);
            };
        }
    }
}