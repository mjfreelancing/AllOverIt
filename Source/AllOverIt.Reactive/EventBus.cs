using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AllOverIt.ReactiveUI
{
    internal sealed class EventBus : IEventBus
    {
        private Subject<object> _subject = new();

        public void Publish<TEvent>() where TEvent : new()
        {
            _subject.OnNext(new TEvent());
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            _subject.OnNext(sampleEvent);
        }

        public void Dispose()
        {
            _subject?.Dispose();
            _subject = null;
        }

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return _subject.OfType<TEvent>().AsObservable();
        }
    }
}
