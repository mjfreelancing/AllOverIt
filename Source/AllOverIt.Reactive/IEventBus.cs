using System;

namespace AllOverIt.ReactiveUI
{
    public interface IEventBus : IDisposable
    {
        void Publish<TEvent>() where TEvent : new();
        void Publish<TEvent>(TEvent sampleEvent);

        IObservable<TEvent> GetEvent<TEvent>();
    }
}
