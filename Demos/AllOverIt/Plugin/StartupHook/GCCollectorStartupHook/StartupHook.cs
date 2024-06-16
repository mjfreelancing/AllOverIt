// StartupHook Requirements:

//   - NO NAMESPACE

//   - Internal class named StartupHook

//   - Implementation inside 'public static void Initialize()'

//   - Must be built against the same or lower version of .NET Core than the app.

//   - The app that needs to load the hook(s) needs to set an environment variable
//     $env: DOTNET_STARTUP_HOOKS = <path to the hook DLL #1>;<path to the hook DLL #2>

// If there are any proxy DLL's (so nuget packages can be loaded, for example)
//
//   - Add a project reference to each proxy DLL

//   - Add <EnableDynamicLoading>true</EnableDynamicLoading> to the .csproj
//     so all dependencies can be loaded by the LoadContext - as recommended here: https://learn.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
//
//     Note: older examples used this : <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>

using GCCollectorStartupHook;
using System.Reflection;
using System.Runtime.Loader;

// See also: https://learn.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support

// See also: https://jeremybytes.blogspot.com/2020/01/dynamically-loading-types-in-net-core.html

internal class StartupHook
{
    // If there are no external nuget dependencies then the entire implementation could be in this DLL.
    // When there are dependencies, a proxy DLL must be used.
    private const string StartupHookProxyAssemblyName = "GCCollectorStartupHookProxy";
    private const string StartupHookProxyTypeName = $"GCCollectorStartupHookProxy.{nameof(GCCollectorStartupHookProxy.GCMetricsCollector)}";

    public static void Initialize()
    {
        Console.WriteLine("Startup Hook Initializing....");

        // If a proxy was not required, then everything could be kicked of by simply doing:
        // _ = new GCMetricsCollector();
        LoadMetricsViaProxy();

        Console.WriteLine("Startup Hook Initialized....");
    }

    private static void LoadMetricsViaProxy()
    {
        // NOTE: This sample DLL is a StartupHook that is using code copied from AllOverIt packages because dependencies are not supported by startup hooks.
        // This is not a concern for regular applications loading plugin assemblies so AllOverIt.Plugin could be referenced as usual.

        // https://github.com/dotnet/runtime/blob/main/docs/design/features/host-startup-hook.md
        // The startup hook must compatible with the dependencies specified in the main application's deps.json, since those dependencies are put
        // on the Trusted Platform Assemblies (TPA) list during the runtime startup, before StartupHook.dll is loaded. This means that StartupHook.dll
        // needs to be built against the same or lower version of .NET Core than the app. Can use the example below to show how to resolve dependencies
        // not on the TPA list from a shared location, similar to the GAC on .NET Framework. It could also be used to forcibly preload assemblies that
        // are on the TPA list from a different location. Future changes to AssemblyLoadContext could make this easier to use by making the default load
        // context or TPA list modifiable.
        //
        AssemblyLoadContext.Default.Resolving += LoadAssemblyFromSharedLocation;

        var loadContext = new PluginLoadContext(Assembly.GetExecutingAssembly().Location);
        loadContext.CreateType(StartupHookProxyAssemblyName, StartupHookProxyTypeName);
    }

    private static Assembly? LoadAssemblyFromSharedLocation(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var sharedAssemblyPath = string.Empty;      // Set to a path to a shared location - could be via an environment variable

        return !string.IsNullOrWhiteSpace(sharedAssemblyPath)
            ? AssemblyLoadContext.Default.LoadFromAssemblyPath(sharedAssemblyPath)
            : null;
    }
}
