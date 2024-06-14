using AllOverIt.Reactive.Messaging;
using System.Reactive.Linq;

namespace SubscribedEventsDemo
{
    internal sealed class OddEventHandler : EventBusHandler<OddEvent>
    {
        private readonly bool _negateValue;

        public OddEventHandler(IEventBus eventBus, bool negateValue)
            : base(eventBus)
        {
            _negateValue = negateValue;
        }

        public override void Handle(OddEvent @event)
        {
            Console.WriteLine($"Received an odd number event: {@event.Value}");
        }

        // This override is optional and is only required if the event needs to be modified
        // or filtered before the subscription is registered.
        protected override IObservable<OddEvent> OnEvent()
        {
            var onEvent = base.OnEvent();

            if (!_negateValue)
            {
                return onEvent;
            }

            // Demonstrating how an event can be manipulated (without modifying the original)
            // before passing it downstream.
            return onEvent.Select(@event => new OddEvent(-@event.Value));
        }
    }
}
