using System.Threading.Tasks;

namespace AppSyncSubscription
{
    public interface IWorkerReady
    {
        void SetCompleted();
        Task Wait();
    }
}