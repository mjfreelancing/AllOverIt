using AllOverIt.Events;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class SubscriptionFixture : AoiFixtureBase
    {
        public class Constructor : SubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new Subscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithMessage(GetExpectedArgumentNullExceptionMessage("handler"));
            }
        }

        public class GetHandler : SubscriptionFixture
        {
            [Fact]
            public void Should_Get_Handler()
            {
                // ReSharper disable once ConvertToLocalFunction
                Action<int> handler = SubscriptionHandler;

                var subscription = new Subscription(handler);

                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Should().BeSameAs(handler);
            }

            [Fact]
            public void Should_Get_Static_Handler()
            {
                // ReSharper disable once ConvertToLocalFunction
                Action<int> handler = StaticSubscriptionHandler;

                var subscription = new Subscription(handler);

                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Should().BeSameAs(handler);
            }
        }

        public class Handle : SubscriptionFixture
        {
            [Fact]
            public void Should_Invoke_Handler()
            {
                var expected = Create<int>();
                int actual = -expected;

                // ReSharper disable once ConvertToLocalFunction
                Action<int> handler = value =>
                {
                    actual = value;
                };

                var subscription = new Subscription(handler);

                subscription.Handle(expected);

                actual.Should().Be(expected);
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void SubscriptionHandler(int value)
        {
        }

        private static void StaticSubscriptionHandler(int value)
        {
        }
    }
}
