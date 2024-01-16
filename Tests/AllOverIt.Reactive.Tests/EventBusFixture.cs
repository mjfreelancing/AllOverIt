using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Reactive.Linq;
using Xunit;

namespace AllOverIt.Reactive.Tests
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

                received.Should().BeTrue();
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

                actual.Should().BeSameAs(expected);
            }
        }
    }
}