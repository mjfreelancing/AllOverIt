#if !NETSTANDARD2_1

using AllOverIt.Assertion;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace AllOverIt.Plugin
{
    /// <summary>Implements a runtime scope for assembly loading.</summary>
    [ExcludeFromCodeCoverage]
    public class PluginLoadContext : AssemblyLoadContext
    {
        // The AssemblyDependencyResolver object is constructed with the path to a .NET class library (plugin).
        // It resolves assemblies and native libraries to their relative paths based on the .deps.json file for
        // the class library whose path was passed to the AssemblyDependencyResolver constructor.
        private readonly AssemblyDependencyResolver _resolver;

        /// <summary>Constructor.</summary>
        /// <param name="pluginPath">This context resolves assemblies and native libraries using relative paths
        /// based on the .deps.json file for the class library using this path.</param>
        public PluginLoadContext(string pluginPath)
        {
            _ = pluginPath.WhenNotNullOrEmpty();

            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        /// <inheritdoc/>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            return assemblyPath is not null
                ? LoadFromAssemblyPath(assemblyPath)
                : null;
        }

        /// <inheritdoc/>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath is not null
                ? LoadUnmanagedDllFromPath(libraryPath)
                : IntPtr.Zero;
        }
    }
}

#endif
