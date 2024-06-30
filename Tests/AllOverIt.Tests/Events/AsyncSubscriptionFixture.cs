using AllOverIt.Events;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Events
{
    public class AsyncSubscriptionFixture : FixtureBase
    {
        public class Constructor : AsyncSubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new AsyncSubscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("handler");
            }
        }

        public class GetHandler : AsyncSubscriptionFixture
        {
            [Fact]
            public void Should_Get_Handler()
            {
                Func<int, Task> handler = SubscriptionHandler;

                var subscription = new AsyncSubscription(handler);

                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Should().BeSameAs(handler);
            }

            [Fact]
            public void Should_Get_Static_Handler()
            {
                Func<int, Task> handler = StaticSubscriptionHandler;

                var subscription = new AsyncSubscription(handler);

                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Should().BeSameAs(handler);
            }
        }

        public class HandleAsync : AsyncSubscriptionFixture
        {
            [Fact]
            public async Task Should_Invoke_Handler()
            {
                var expected = Create<int>();
                var actual = -expected;

                Func<int, Task> handler = value =>
                {
                    actual = value;
                    return Task.CompletedTask;
                };

                var subscription = new AsyncSubscription(handler);

                await subscription.HandleAsync(expected);

                actual.Should().Be(expected);
            }
        }

#pragma warning disable CA1822 // Mark members as static
        private Task SubscriptionHandler(int _)
#pragma warning restore CA1822 // Mark members as static
        {
            return Task.CompletedTask;
        }

        private static Task StaticSubscriptionHandler(int _)
        {
            return Task.CompletedTask;
        }
    }
}
