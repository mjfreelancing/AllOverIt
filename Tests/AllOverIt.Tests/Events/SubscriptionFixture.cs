using AllOverIt.Events;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class SubscriptionFixture : FixtureBase
    {
        public class Constructor : SubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new Subscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("handler");
            }
        }

        public class GetHandler : SubscriptionFixture
        {
            [Fact]
            public void Should_Get_Handler()
            {
                Action<int> handler = SubscriptionHandler;

                var subscription = new Subscription(handler);

                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Should().BeSameAs(handler);
            }

            [Fact]
            public void Should_Get_Static_Handler()
            {
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
                var actual = -expected;

                Action<int> handler = value =>
                {
                    actual = value;
                };

                var subscription = new Subscription(handler);

                subscription.Handle(expected);

                actual.Should().Be(expected);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
        private void SubscriptionHandler(int _)
#pragma warning restore CA1822 // Mark members as static
        {
        }

        private static void StaticSubscriptionHandler(int _)
        {
        }
    }
}
