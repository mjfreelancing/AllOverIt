using AllOverIt.Reactive;
using AllOverIt.Reactive.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SubscribedEvents
{
    internal class Program
    {
        static void Main()
        {
            using (var disposables = new CompositeDisposable())
            {
                var subject = new Subject<int>();
                var bus = new EventBus();

                subject
                    .Where(value => value % 2 == 0)
                    .Subscribe(value =>
                    {
                        bus.Publish<EvenEvent>();
                    })
                    .DisposeUsing(disposables);

                subject
                    .Where(value => value % 2 == 1)
                    .Subscribe(value =>
                    {
                        var oddEvent = new OddEvent(value);
                        bus.Publish(oddEvent);
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
                    });

                bus.GetEvent<EvenEvent>()
                    .Subscribe(evt =>
                    {
                        Console.WriteLine("Received an even number event");
                    })
                    .DisposeUsing(disposables);

                bus.GetEvent<OddEvent>()
                    .Subscribe(evt =>
                    {
                        Console.WriteLine($"Received an odd number event: {evt.Value}");
                    })
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
