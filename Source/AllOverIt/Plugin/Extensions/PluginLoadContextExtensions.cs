using AllOverIt.Assertion;
using AllOverIt.Plugin.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AllOverIt.Plugin.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="PluginLoadContext"/>.</summary>
    [ExcludeFromCodeCoverage]
    public static class PluginLoadContextExtensions
    {
        /// <summary>Creates an instance of the namespace qualified type specified by <paramref name="typeName"/> from the assembly
        /// called <paramref name="assemblyName"/>.</summary>
        /// <param name="loadContext">The assembly load context that has loaded the required assembly.</param>
        /// <param name="assemblyName">The name of the assembly (without its extension) containing the required type.</param>
        /// <param name="typeName">The namespace qualified name of the type to create.</param>
        /// <returns>The created instance of <paramref name="typeName"/>.</returns>
        public static object? CreateType(this PluginLoadContext loadContext, string assemblyName, string typeName)
        {
            return loadContext.CreateType<object>(assemblyName, typeName);
        }

        /// <summary>Creates an instance of the namespace qualified type specified by <paramref name="typeName"/> from the assembly
        /// called <paramref name="assemblyName"/>.</summary>
        /// <param name="loadContext">The assembly load context that has loaded the required assembly.</param>
        /// <param name="assemblyName">The name of the assembly (without its extension) containing the required type.</param>
        /// <param name="typeName">The namespace qualified name of the type to create.</param>
        /// <param name="args">Arguments to be passed to the constructor of the required type.</param>
        /// <returns>The created instance of <paramref name="typeName"/>.</returns>
        public static object? CreateType(this PluginLoadContext loadContext, string assemblyName, string typeName, params object[] args)
        {
            return loadContext.CreateType<object>(assemblyName, typeName, args);
        }

        /// <summary>Creates an instance of the namespace qualified type specified by <paramref name="typeName"/> from the assembly
        /// called <paramref name="assemblyName"/>.</summary>
        /// <typeparam name="TType">The type to cast the created instance to.</typeparam>
        /// <param name="loadContext">The assembly load context that has loaded the required assembly.</param>
        /// <param name="assemblyName">The name of the assembly (without its extension) containing the required type.</param>
        /// <param name="typeName">The namespace qualified name of the type to create.</param>
        /// <returns>The created instance of <paramref name="typeName"/> cast as a <typeparamref name="TType"/>.</returns>
        public static TType? CreateType<TType>(this PluginLoadContext loadContext, string assemblyName, string typeName) where TType : class
        {
            var requiredType = LoadTypeFromAssembly(loadContext, assemblyName, typeName);

            if (requiredType is null)
            {
                return null;
            }

            return Activator.CreateInstance(requiredType) as TType;
        }

        /// <summary>Creates an instance of the namespace qualified type specified by <paramref name="typeName"/> from the assembly
        /// called <paramref name="assemblyName"/>.</summary>
        /// <typeparam name="TType">The type to cast the created instance to.</typeparam>
        /// <param name="loadContext">The assembly load context that has loaded the required assembly.</param>
        /// <param name="assemblyName">The name of the assembly (without its extension) containing the required type.</param>
        /// <param name="typeName">The namespace qualified name of the type to create.</param>
        /// <param name="args">Arguments to be passed to the constructor of the required type.</param>
        /// <returns>The created instance of <paramref name="typeName"/> cast as a <typeparamref name="TType"/>.</returns>
        public static TType? CreateType<TType>(this PluginLoadContext loadContext, string assemblyName, string typeName, params object[] args) where TType : class
        {
            var requiredType = LoadTypeFromAssembly(loadContext, assemblyName, typeName);

            if (requiredType is null)
            {
                return null;
            }

            return Activator.CreateInstance(requiredType, args) as TType;
        }

        // assemblyName must be the DLL name without the extension
        // typeFullName must be namespace.type_name
        private static Type? LoadTypeFromAssembly(PluginLoadContext loadContext, string assemblyName, string fullTypeName)
        {
            var assemblyNameInfo = new AssemblyName(assemblyName);
            var assembly = loadContext.LoadFromAssemblyName(assemblyNameInfo);

            var requiredType = assembly.ExportedTypes.SingleOrDefault(type => type.FullName == fullTypeName);

            Throw<CannotLoadPluginTypeException>.WhenNull(requiredType, $"The {fullTypeName} type was not found in the {assemblyName} assembly.");

            return requiredType;
        }
    }
}
