using AllOverIt.Events;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Events
{
    public class WeakSubscriptionFixture : FixtureBase
    {
        private class HandlerDummy
        {
            public static int ActualValue { get; set; }

            public void Handler(int value)
            {
            }

            public static void StaticHandler(int value)
            {
                ActualValue = value;
            }

            public static HandlerDummy Create()
            {
                return new HandlerDummy();
            }
        }

        public class Constructor : WeakSubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new WeakSubscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("handler");
            }
        }

        public class GetHandler : WeakSubscriptionFixture
        {
            [Fact]
            public void Should_Get_Handler()
            {
                var expected = Create<int>();
                var actual = -expected;

                Action<int> handler = value =>
                {
                    actual = value;
                };

                var subscription = new WeakSubscription(handler);

                // unlike AsyncSubscription, AsyncWeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Invoke(expected);

                actual.Should().Be(expected);
            }

            [Fact]
            public async Task Should_Not_Handle_Disposed_Handler()
            {
                var handler = HandlerDummy.Create().Handler;

                var subscription = new WeakSubscription(handler);

                handler = null;

                await Task.Delay(100);

                GC.Collect();

                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Should().BeNull();
            }

            [Fact]
            public void Should_Get_Static_Handler()
            {
                var expected = Create<int>();

                Action<int> handler = HandlerDummy.StaticHandler;

                var subscription = new WeakSubscription(handler);

                // unlike AsyncSubscription, WeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                HandlerDummy.ActualValue = -expected;
                registeredHandler.Invoke(expected);

                HandlerDummy.ActualValue.Should().Be(expected);
            }
        }

        public class Handle : WeakSubscriptionFixture
        {
            [Fact]
            public void Should_Invoke_Handler()
            {
                var expected = Create<int>();
                int actual = -expected;

                Action<int> handler = value =>
                {
                    actual = value;
                };

                var subscription = new WeakSubscription(handler);

                subscription.Handle(expected);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Handler_Is_Different_Type()
            {
                Invoking(() =>
                {
                    Action<int> handler = value =>
                    {
                    };

                    var subscription = new WeakSubscription(handler);

                    subscription.Handle(string.Empty);
                })
                .Should()
                .Throw<InvalidCastException>();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Handler_Disposed()
            {
                var handler = HandlerDummy.Create().Handler;

                var subscription = new WeakSubscription(handler);

                handler = null;

                await Task.Delay(100);

                GC.Collect();

                var value = Create<int>();

                Invoking(() =>
                {
                    // We cannot assert the handler is not invoked because it is null.
                    // Other tests for GetHandler() prove the handler is returned as null.
                    subscription.Handle<int>(value);
                })
                .Should()
                .NotThrow();
            }
        }
    }
}
