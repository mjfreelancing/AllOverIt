using AllOverIt.Fixture;
using AllOverIt.Reactive.Messaging;
using Shouldly;
using System.Reactive.Linq;

namespace AllOverIt.Reactive.Tests.Messaging
{
    public class EventBusFixture : FixtureBase
    {
        private class EventDummy
        {
        }

        public class Publish_GetEvent : EventBusFixture
        {
            [Fact]
            public void Should_Publish_Receive_Event()
            {
                var received = false;

                using (var eventBus = new EventBus())
                {
                    eventBus.GetEvent<EventDummy>().Subscribe(_ => { received = true; });

                    eventBus.Publish<EventDummy>();
                }

                received.ShouldBeTrue();
            }
        }

        public class Publish_Arg_GetEvent : EventBusFixture
        {
            [Fact]
            public void Should_Publish_Receive_Event()
            {
                EventDummy expected = new EventDummy();
                EventDummy actual = null;

                using (var eventBus = new EventBus())
                {
                    eventBus.GetEvent<EventDummy>().Subscribe(@event => { actual = @event; });

                    eventBus.Publish(expected);
                }

                actual.ShouldBeSameAs(expected);
            }
        }
    }
}


