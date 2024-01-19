using AllOverIt.Reactive;
using AllOverIt.Reactive.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SubscribedEventsDemo
{
    internal class Program
    {
        static void Main()
        {
            using (var disposables = new CompositeDisposable())
            {
                var subject = new Subject<int>();
                var eventBus = new EventBus();

                subject
                    .Where(value => value % 2 == 0)
                    .Subscribe(value =>
                    {
                        eventBus.Publish<EvenEvent>();
                    })
                    .DisposeUsing(disposables);

                subject
                    .Where(value => value % 2 == 1)
                    .Subscribe(value =>
                    {
                        var oddEvent = new OddEvent(value);
                        eventBus.Publish(oddEvent);
                    })
                    .DisposeUsing(disposables);

                subject
                    .WaitUntil(value => value > 25)
                    .Subscribe(value =>
                    {
                        // Will only fire once
                        Console.WriteLine();
                        Console.WriteLine($"*** Waited until value > 25 (value = {value}) ***");
                        Console.WriteLine();
                    })
                    .DisposeUsing(disposables);

                // Subscribe directly to the event bus
                eventBus.GetEvent<EvenEvent>()
                    .Subscribe(evt =>
                    {
                        Console.WriteLine("Received an even number event");
                    })
                    .DisposeUsing(disposables);

                // Subscribe via a handler - this handler will negate the value without modifying the original event
                new OddEventHandler(eventBus, true)
                {
                    IsActive = true
                }
                .DisposeUsing(disposables);

                // Another OddEvent handler, but this one does not negate the value
                new OddEventHandler(eventBus, false)
                {
                    IsActive = true
                }
                .DisposeUsing(disposables);

                for (var i = 20; i <= 30; i++)
                {
                    subject.OnNext(i);
                }
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
