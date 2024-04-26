using System.Reactive.Disposables;

namespace AllOverIt.Reactive.Extensions
{
    /// <summary>Provides extension methods for <see cref="CompositeDisposable"/>.</summary>
    public static class DisposableExtensions
    {
        /// <summary>Ensures the disposable is disposed when the provided <see cref="CompositeDisposable"/> is disposed.</summary>
        /// <param name="disposable">The disposable to be disposed of when <paramref name="disposables"/> is disposed.</param>
        /// <param name="disposables">The group of disposables to be collectively disposed.</param>
        public static void DisposeUsing(this IDisposable disposable, CompositeDisposable disposables)
        {
            disposables.Add(disposable);
        }
    }
}