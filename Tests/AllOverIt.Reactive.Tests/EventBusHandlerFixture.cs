using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xunit;

namespace AllOverIt.Reactive.Tests
{
    public class EventBusHandlerFixture : FixtureBase
    {
        private readonly IEventBus _eventBus = new EventBus();

        private class EventDummy
        {
            public int Value { get; set; }
        }

        private class HandlerDummy : EventBusHandler<EventDummy>
        {
            private readonly Action _onHandle;
            private readonly Func<EventDummy, bool> _predicate;

            public bool Handled { get; private set; }

            public HandlerDummy(IEventBus eventBus, Action onHandle = default)
                : base(eventBus)
            {
                _onHandle = onHandle;
            }

            public HandlerDummy(IEventBus eventBus, Func<EventDummy, bool> predicate)
                : this(eventBus, (Action)null)
            {
                _predicate = predicate;
            }

            public override void Handle(EventDummy @event)
            {
                Handled = true;
                _onHandle?.Invoke();
            }

            protected override IObservable<EventDummy> OnEvent()
            {
                var onEvent = base.OnEvent();

                if (_predicate is null)
                {
                    return onEvent;
                }

                return onEvent.Where(_predicate);
            }
        }

        public class Constructor : EventBusHandlerFixture
        {
            [Fact]
            public void Should_Throw_When_EventBus_Null()
            {
                Invoking(() =>
                {
                    _ = new HandlerDummy(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("eventBus");
            }

            [Fact]
            public void Should_Default_Not_Active()
            {
                var handler = new HandlerDummy(_eventBus);
                handler.IsActive.Should().BeFalse();
            }
        }

        // Effectively tests Handle() at the same time
        public class IsActive : EventBusHandlerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Handle_Only_When_Active(bool isActive)
            {
                var handler = new HandlerDummy(_eventBus);
                handler.IsActive = isActive;

                _eventBus.Publish<EventDummy>();

                handler.Handled.Should().Be(isActive);
            }

            [Fact]
            public void Should_Toggle_Subscription()
            {
                var count = 0;

                Action onHandle = () => count++;

                var handler = new HandlerDummy(_eventBus, onHandle);
                handler.IsActive = true;

                _eventBus.Publish<EventDummy>();

                handler.Handled.Should().BeTrue();
                count.Should().Be(1);

                // the handler should not be called again
                handler.IsActive = false;
                _eventBus.Publish<EventDummy>();

                count.Should().Be(1);

                // the handler should be called again
                handler.IsActive = true;

                _eventBus.Publish<EventDummy>();
                count.Should().Be(2);
            }
        }

        public class OnEvent : EventBusHandlerFixture
        {
            [Fact]
            public void Should_Apply_Filter()
            {
                var value = Create<int>();

                var handler = new HandlerDummy(_eventBus, @event => @event.Value == value);
                handler.IsActive = true;

                var @event = new EventDummy
                {
                    Value = value - 1,
                };

                _eventBus.Publish(@event);

                handler.Handled.Should().BeFalse();

                @event.Value = value;

                _eventBus.Publish(@event);

                handler.Handled.Should().BeTrue();
            }
        }

        public class DisposeUsing : EventBusHandlerFixture
        {
            [Fact]
            public void Should_Unsubscribe_When_Disposed()
            {
                var disposables = new CompositeDisposable();

                var count = 0;

                Action onHandle = () => count++;

                var handler = new HandlerDummy(_eventBus, onHandle);
                handler.DisposeUsing(disposables);

                handler.IsActive = true;

                _eventBus.Publish<EventDummy>();

                handler.Handled.Should().BeTrue();
                count.Should().Be(1);

                disposables.Dispose();

                _eventBus.Publish<EventDummy>();

                // the handler should not be called again
                count.Should().Be(1);
            }
        }

        public class Dispose : EventBusHandlerFixture
        {
            [Fact]
            public void Should_Unsubscribe_When_Disposed()
            {
                var count = 0;

                Action onHandle = () => count++;

                using (var handler = new HandlerDummy(_eventBus, onHandle))
                {
                    handler.IsActive = true;

                    _eventBus.Publish<EventDummy>();

                    handler.Handled.Should().BeTrue();
                    count.Should().Be(1);
                }

                _eventBus.Publish<EventDummy>();

                // the handler should not be called again
                count.Should().Be(1);
            }
        }
    }
}