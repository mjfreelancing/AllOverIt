using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.GenericHost
{
    public abstract class GenericAppBase : IGenericApp
    {
        public abstract Task StartAsync(CancellationToken cancellationToken = default);

        public virtual void OnStopping()
        {
        }

        public virtual void OnStopped()
        {
        }
    }
}