using System;
using System.Reflection;
using System.Runtime.Loader;

namespace GCCollectorStartupHook
{
    // NOTE: This code has been copied (and modified to remove pre-condition checks) from AllOverIt.Plugin as dependencies are not supported in startup hooks.
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            return assemblyPath is not null
                ? LoadFromAssemblyPath(assemblyPath)
                : null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath is not null
                ? LoadUnmanagedDllFromPath(libraryPath)
                : IntPtr.Zero;
        }
    }
}