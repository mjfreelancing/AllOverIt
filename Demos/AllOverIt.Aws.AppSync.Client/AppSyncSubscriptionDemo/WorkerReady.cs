using System.Threading;
using System.Threading.Tasks;

namespace AppSyncSubscriptionDemo
{
    public sealed class WorkerReady : IWorkerReady
    {
        // Must use this over a TaskCompletionSource as the console / worker are running in different threads.
        // Attempting to use the TCS results in the Wait() task blocking when awaited.
        // Note: NOT using AwaitableLock since this initializes as (1, 1)
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