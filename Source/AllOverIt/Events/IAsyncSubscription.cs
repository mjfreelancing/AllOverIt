using System;
using System.Threading.Tasks;

namespace AllOverIt.Events
{
    public interface IAsyncSubscription
    {
        Func<TMessage, Task> GetHandler<TMessage>();
        Task HandleAsync<TMessage>(TMessage message);
    }
}
