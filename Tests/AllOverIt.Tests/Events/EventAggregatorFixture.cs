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
        private class DummyEvent
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
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
            public void HandleEvent(DummyEvent message)
#pragma warning restore CA1822 // Mark members as static
            {
                StaticHandleMessageEvent(message);
            }
        }

        private class HandlerAsync
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
            public Task HandleEventAsync(DummyEvent message)
#pragma warning restore CA1822 // Mark members as static
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
                aggregator.Subscribe<DummyEvent>(HandleMessageEventAsync);

                var message = new DummyEvent { Input = Create<int>() };

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
                aggregator.Subscribe<DummyEvent>(StaticHandleMessageEventAsync);

                var message = new DummyEvent { Input = Create<int>() };

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
                aggregator.Subscribe<DummyEvent>(HandleMessageEvent);

                var message = new DummyEvent { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public void Should_Publish_To_Static_Event_Handler()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<DummyEvent>(StaticHandleMessageEvent);

                var message = new DummyEvent { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public void Should_Publish_To_Multiple_Handlers()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<DummyEvent>(HandleMessageEvent);
                aggregator.Subscribe<DummyEvent>(StaticHandleMessageEvent);

                var message = new DummyEvent { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input * 2);
                message.Counter.Should().Be(2);
            }
        }

        public class PublishAsync : EventAggregatorFixture
        {
            [Fact]
            public async Task Should_Publish_To_Event_Handler()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<DummyEvent>(HandleMessageEvent);

                var message = new DummyEvent { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public async Task Should_Publish_To_Static_Event_Handler()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<DummyEvent>(StaticHandleMessageEvent);

                var message = new DummyEvent { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public async Task Should_Publish_To_Multiple_Handlers()
            {
                var aggregator = new EventAggregator();
                aggregator.Subscribe<DummyEvent>(HandleMessageEvent);
                aggregator.Subscribe<DummyEvent>(StaticHandleMessageEvent);
                aggregator.Subscribe<DummyEvent>(HandleMessageEventAsync);
                aggregator.Subscribe<DummyEvent>(StaticHandleMessageEventAsync);

                var message = new DummyEvent { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input * 4);
                message.Counter.Should().Be(4);
            }
        }

        public class Subscribe_Action : EventAggregatorFixture
        {
            [Fact]
            public async Task Should_Weak_Subscribe_To_Handler()
            {
                var aggregator = new EventAggregator();

                var handler = new Handler();
                aggregator.Subscribe<DummyEvent>(handler.HandleEvent);

                handler = null;

                await Task.Delay(100);

                GC.Collect();

                var message = new DummyEvent { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(0);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Subscribe_To_Handler(bool weakSubscription)
            {
                var aggregator = new EventAggregator();

                var handler = new Handler();
                aggregator.Subscribe<DummyEvent>(handler.HandleEvent, weakSubscription);

                var message = new DummyEvent { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);
            }
        }

        public class Subscribe_Func_Task : EventAggregatorFixture
        {
            [Fact]
            public async Task Should_Weak_Subscribe_To_Handler()
            {
                var aggregator = new EventAggregator();

                var handler = new HandlerAsync();
                aggregator.Subscribe<DummyEvent>(handler.HandleEventAsync);

                handler = null;

                await Task.Delay(100);

                GC.Collect();

                var message = new DummyEvent { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(0);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Subscribe_To_Handler(bool weakSubscription)
            {
                var aggregator = new EventAggregator();

                var handler = new HandlerAsync();
                aggregator.Subscribe<DummyEvent>(handler.HandleEventAsync, weakSubscription);

                var message = new DummyEvent { Input = Create<int>() };

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
                aggregator.Subscribe<DummyEvent>(handler.HandleEvent);

                var message = new DummyEvent { Input = Create<int>() };

                aggregator.Publish(message);

                message.Output.Should().Be(message.Input);

                aggregator.Unsubscribe<DummyEvent>(handler.HandleEvent);
                message.Output = -message.Input;

                aggregator.Publish(message);

                message.Output.Should().Be(-message.Input);
            }

            [Fact]
            public void Should_Not_Throw_When_Unsubscribe_Unknown_Handler()
            {
                Invoking(() =>
                {
                    var aggregator = new EventAggregator();

                    var handler = new Handler();

                    aggregator.Unsubscribe<DummyEvent>(handler.HandleEvent);
                })
                .Should()
                .NotThrow();
            }
        }

        public class Unsubscribe_Func_Task : EventAggregatorFixture
        {
            [Fact]
            public async Task Should_Unsubscribe_From_Handler_1()
            {
                var aggregator = new EventAggregator();

                var handler = new HandlerAsync();
                aggregator.Subscribe<DummyEvent>(handler.HandleEventAsync);

                var message = new DummyEvent { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);

                aggregator.Unsubscribe<DummyEvent>(handler.HandleEventAsync);
                message.Output = -message.Input;

                // There's no handler so the value should remain unchanged
                await aggregator.PublishAsync(message);

                message.Output.Should().Be(-message.Input);
            }

            [Fact]
            public async Task Should_Unsubscribe_From_Handler_2()
            {
                var aggregator = new EventAggregator();

                var handler1 = new HandlerAsync();
                aggregator.Subscribe<DummyEvent>(handler1.HandleEventAsync);

                var handler2 = new HandlerAsync();
                aggregator.Subscribe<DummyEvent>(handler2.HandleEventAsync);

                var message = new DummyEvent { Input = Create<int>() };

                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input * 2);

                // Unsubscribing the second handler so the internal loop iterates at least once
                aggregator.Unsubscribe<DummyEvent>(handler2.HandleEventAsync);
                message.Output = 0;

                // There's still one handler so make sure the value is updated again
                await aggregator.PublishAsync(message);

                message.Output.Should().Be(message.Input);
            }

            [Fact]
            public void Should_Not_Throw_When_Unsubscribe_Unknown_Handler()
            {
                Invoking(() =>
                {
                    var aggregator = new EventAggregator();

                    var handler = new HandlerAsync();

                    aggregator.Unsubscribe<DummyEvent>(handler.HandleEventAsync);
                })
                .Should()
                .NotThrow();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
        private void HandleMessageEvent(DummyEvent message)
#pragma warning restore CA1822 // Mark members as static
        {
            StaticHandleMessageEvent(message);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
        private Task HandleMessageEventAsync(DummyEvent message)
#pragma warning restore CA1822 // Mark members as static
        {
            StaticHandleMessageEvent(message);
            return Task.CompletedTask;
        }

        private static void StaticHandleMessageEvent(DummyEvent message)
        {
            message.Output = message.Output + message.Input;
            message.IncrementCounter();
        }

        private static Task StaticHandleMessageEventAsync(DummyEvent message)
        {
            StaticHandleMessageEvent(message);
            return Task.CompletedTask;
        }
    }
}
