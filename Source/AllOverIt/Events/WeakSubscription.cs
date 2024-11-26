using AllOverIt.Assertion;
using System.Reflection;

namespace AllOverIt.Events
{
    internal sealed class WeakSubscription : ISubscription
    {
        private readonly WeakReference _weakReference;
        private readonly MethodInfo _handlerMethod;
        private readonly Type _handlerType;

        public WeakSubscription(Delegate handler)
        {
            _ = handler.WhenNotNull();

            _weakReference = new WeakReference(handler.Target);
            _handlerMethod = handler.GetMethodInfo();
            _handlerType = handler.GetType();
        }

        public Action<TMessage>? GetHandler<TMessage>()
        {
            if (_handlerMethod.IsStatic)
            {
                return (Action<TMessage>) _handlerMethod.CreateDelegate(_handlerType, null);
            }

            var target = _weakReference.Target;

            if (target is not null)
            {
                return (Action<TMessage>) _handlerMethod.CreateDelegate(_handlerType, target);
            }

            // The weak reference has gone
            return null;
        }

        public void Handle<TMessage>(TMessage message)
        {
            // The handler could be null if the weak reference has been garbage collected
            GetHandler<TMessage>()?.Invoke(message);
        }
    }
}
