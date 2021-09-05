using System.Threading;
using System.Threading.Tasks;

namespace AppSyncSubscription
{
    public sealed class WorkerReady : IWorkerReady
    {
        // Must use this over a TaskCompletionSource as the console / worker are running in different threads.
        // Attempting to use the TCS results in the Wait() task blocking when awaited.
        private readonly SemaphoreSlim _semaphore = new(0, 1);

        public void SetCompleted()
        {
            _semaphore.Release();
        }

        public Task Wait()
        {
            return _semaphore.WaitAsync();
        }
    }
}