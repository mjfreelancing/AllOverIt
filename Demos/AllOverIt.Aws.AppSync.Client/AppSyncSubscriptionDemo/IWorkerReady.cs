using System.Threading.Tasks;

namespace AppSyncSubscriptionDemo
{
    public interface IWorkerReady
    {
        void SetCompleted();
        Task Wait();
    }
}