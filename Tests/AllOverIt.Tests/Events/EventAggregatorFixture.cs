using AllOverIt.Events;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class EventAggregatorFixture : FixtureBase
    {
        private class EventDummy
        {
            private int _counter;

            public int Input { get; set; }
            public int Output { get; set; }
            public int Counter => _counter;

            public void IncrementCounter()
            {
                Interlocked.Increment(ref _counter);
            }
        }

        private class Handler
        {
            // ReSharper disable once MemberCanBeMadeStatic.Local
            public void HandleEvent(EventDummy message)
            {
                StaticHandleMessageEvent(message);
            }
        }

        private class HandlerAsync
        {
            // ReSharper disable once MemberCanBeMadeStatic.Local
            public Task HandleEventAsync(EventDummy message)
            {
                return StaticHandleMessageEventAsync(message);
            }
        }

        public class Publish : EventAggregatorFixture
        {
            [Fact]
            public void Should_Throw_When_Async_Subscriptions_Exist()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(HandleMessageEventAsync);

                var message = new EventDummy { Input = Create<int>() };

                Invoking(() =>
                  {
                      aggregator.Publish(message);
                  })
                  .Should()
                  .Throw<InvalidOperationException>()
                  .WithMessage("Cannot publish message when async subscriptions exist");
            }

            [Fact]
            public void Should_Throw_When_Static_Async_Subscriptions_Exist()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(StaticHandleMessageEventAsync);

                var message = new EventDummy { Input = Create<int>() };

                Invoking(() =>
                  {
                      aggregator.Publish(message);
                  })
                  .Should()
                  .Throw<InvalidOperationException>()
                  .WithMessage("Cannot publish message when async subscriptions exist");
            }

            [Fact]
            public void Should_Publish_To_Event_Handler()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(HandleMessageEvent);

                var message = new EventDummy { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public void Should_Publish_To_Static_Event_Handler()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(StaticHandleMessageEvent);

                var message = new EventDummy { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public void Should_Publish_To_Multiple_Handlers()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(HandleMessageEvent);
                aggregator.Subscribe<EventDummy>(StaticHandleMessageEvent);

                var message = new EventDummy { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);
                message.Counter.Should().Be(2);
            }
        }

        public class PublishAsync : EventAggregatorFixture
        {
            [Fact]
            public async Task Should_Publish_To_Event_Handler()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(HandleMessageEvent);

                var message = new EventDummy { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public async Task Should_Publish_To_Static_Event_Handler()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(StaticHandleMessageEvent);

                var message = new EventDummy { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public async Task Should_Publish_To_Multiple_Handlers()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<EventDummy>(HandleMessageEvent);
                aggregator.Subscribe<EventDummy>(StaticHandleMessageEvent);
                aggregator.Subscribe<EventDummy>(HandleMessageEventAsync);
                aggregator.Subscribe<EventDummy>(StaticHandleMessageEventAsync);

                var message = new EventDummy { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);
                message.Counter.Should().Be(4);
            }
        }

        public class Subscribe_Action : EventAggregatorFixture
        {
            // todo: weak references are not behaving in unit tests
            //[Fact]
            //public void Should_Weak_Subscribe_To_Handler()
            //{
            //  var aggregator = new EventAggregator();

            //  var handler = new Handler();
            //  aggregator.Subscribe<EventDummy>(handler.HandleEvent);

            //  handler = null;

            //  GC.Collect();
            //  GC.Collect();

            //  var message = new EventDummy { Input = Create<int>() };

            //  aggregator.Publish(message);

            //  message.Output.Should().Be(0);
            //}

            [Fact]
            public void Should_Subscribe_To_Handler()
            {
                var aggregator = new EventAggregator();

                var handler = new Handler();
                aggregator.Subscribe<EventDummy>(handler.HandleEvent);

                var message = new EventDummy { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);
            }
        }

        public class Subscribe_Func_Task : EventAggregatorFixture
        {
            // todo: weak references are not behaving in unit tests
            //[Fact]
            //public async Task Should_Weak_Subscribe_To_Handler()
            //{
            //  var aggregator = new EventAggregator();

            //  var handler = new HandlerAsync();
            //  aggregator.Subscribe<EventDummy>(handler.HandleEventAsync);

            //  handler = null;

            //  GC.Collect();
            //  GC.Collect();

            //  var message = new EventDummy { Input = Create<int>() };

            //  await aggregator.PublishAsync(message);

            //  message.Output.Should().Be(0);
            //}

            [Fact]
            public async Task Should_Subscribe_To_Handler()
            {
                var aggregator = new EventAggregator();

                var handler = new HandlerAsync();
                aggregator.Subscribe<EventDummy>(handler.HandleEventAsync);

                var message = new EventDummy { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);
            }
        }

        public class Unsubscribe_Action : EventAggregatorFixture
        {
            [Fact]
            public void Should_Unsubscribe_From_Handler()
            {
                var aggregator = new EventAggregator();

                var handler = new Handler();
                aggregator.Subscribe<EventDummy>(handler.HandleEvent);

                var message = new EventDummy { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);

                aggregator.Unsubscribe<EventDummy>(handler.HandleEvent);
                message.Output = -message.Input;

                aggregator.Publish(message);

                message.Output.Should().Be(-message.Input);
            }
        }

        public class Unsubscribe_Func_Task : EventAggregatorFixture
        {
            [Fact]
            public async Task Should_Unsubscribe_From_Handler()
            {
                var aggregator = new EventAggregator();

                var handler = new HandlerAsync();
                aggregator.Subscribe<EventDummy>(handler.HandleEventAsync);

                var message = new EventDummy { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);

                aggregator.Unsubscribe<EventDummy>(handler.HandleEventAsync);
                message.Output = -message.Input;

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(-message.Input);
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void HandleMessageEvent(EventDummy message)
        {
            StaticHandleMessageEvent(message);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Task HandleMessageEventAsync(EventDummy message)
        {
            StaticHandleMessageEvent(message);
            return Task.CompletedTask;
        }

        private static void StaticHandleMessageEvent(EventDummy message)
        {
            message.Output = message.Input;
            message.IncrementCounter();
        }

        private static Task StaticHandleMessageEventAsync(EventDummy message)
        {
            StaticHandleMessageEvent(message);
            return Task.CompletedTask;
        }
    }
}
