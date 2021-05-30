using AllOverIt.Events;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class AsyncSubscriptionFixture : AoiFixtureBase
    {
        public class Constructor : AsyncSubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new AsyncSubscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithMessage(GetExpectedArgumentNullExceptionMessage("handler"));
            }
        }

        public class GetHandler : AsyncSubscriptionFixture
        {
            [Fact]
            public void Should_Get_Handler()
            {
                // ReSharper disable once ConvertToLocalFunction
                Func<int, Task> handler = SubscriptionHandler;

                var subscription = new AsyncSubscription(handler);

                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Should().BeSameAs(handler);
            }

            [Fact]
            public void Should_Get_Static_Handler()
            {
                // ReSharper disable once ConvertToLocalFunction
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
                int actual = -expected;

                // ReSharper disable once ConvertToLocalFunction
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

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Task SubscriptionHandler(int value)
        {
            return Task.CompletedTask;
        }

        private static Task StaticSubscriptionHandler(int value)
        {
            return Task.CompletedTask;
        }
    }
}
