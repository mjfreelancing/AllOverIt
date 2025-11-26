using System.Runtime.CompilerServices;
using ReactiveUI;
using System.Reactive.Concurrency;

namespace AllOverIt.ReactiveUI.Tests
{
    internal static class TestSetup
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            // Ensure ReactiveUI uses a synchronous main thread scheduler for unit tests so
            // observables like ReactiveCommand.IsExecuting emit synchronously on subscribe.
            RxApp.MainThreadScheduler = ImmediateScheduler.Instance;
        }
    }
}
