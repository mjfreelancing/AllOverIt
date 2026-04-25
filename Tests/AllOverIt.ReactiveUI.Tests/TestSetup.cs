using ReactiveUI;
using ReactiveUI.Builder;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;

namespace AllOverIt.ReactiveUI.Tests
{
    internal static class TestSetup
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            // ReactiveUI v23 requires builder-based initialization before any ReactiveObject/WhenAny usage.
            // WithCoreServices().BuildApp() registers core services and marks RxApp as initialized for tests.
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices()
                .BuildApp();

            // Ensure ReactiveUI uses a synchronous main thread scheduler for unit tests so
            // observables like ReactiveCommand.IsExecuting emit synchronously on subscribe.
            RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
        }
    }
}
