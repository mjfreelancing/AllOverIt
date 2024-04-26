using AllOverIt.Assertion;
using System.Reflection;

namespace AllOverIt.Events
{
    internal sealed class AsyncWeakSubscription : IAsyncSubscription
    {
        private readonly WeakReference _weakReference;
        private readonly MethodInfo _handlerMethod;
        private readonly Type _handlerType;

        public AsyncWeakSubscription(Delegate handler)
        {
            _ = handler.WhenNotNull(nameof(handler));

            _weakReference = new WeakReference(handler.Target);
            _handlerMethod = handler.GetMethodInfo();
            _handlerType = handler.GetType();
        }

        public Func<TMessage, Task> GetHandler<TMessage>()
        {
            if (_handlerMethod.IsStatic)
            {
                return (Func<TMessage, Task>) _handlerMethod.CreateDelegate(_handlerType, null);
            }

            var target = _weakReference.Target;

            if (target != null)
            {
                return (Func<TMessage, Task>) _handlerMethod.CreateDelegate(_handlerType, target);
            }

            // the weak reference has gone
            return _ => Task.CompletedTask;
        }

        public Task HandleAsync<TMessage>(TMessage message)
        {
            // the handler could be null if the weak reference has been garbage collected
            // (GetHandler returns a completed task if this is the case)
            return GetHandler<TMessage>().Invoke(message);
        }
    }
}
