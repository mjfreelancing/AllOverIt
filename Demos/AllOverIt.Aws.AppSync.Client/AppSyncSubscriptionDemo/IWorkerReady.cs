namespace AppSyncSubscriptionDemo
{
    public interface IWorkerReady
    {
        void SetCompleted();
        Task Wait();
    }
}