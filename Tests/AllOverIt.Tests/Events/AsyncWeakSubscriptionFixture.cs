using AllOverIt.Events;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class AsyncWeakSubscriptionFixture : AoiFixtureBase
    {
        public class Constructor : AsyncWeakSubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new AsyncWeakSubscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("handler");
            }
        }

        public class GetHandler : AsyncWeakSubscriptionFixture
        {
            private class DummyHandler
            {
                public static int ActualValue { get; set; }

                public static Task StaticHandler(int value)
                {
                    ActualValue = value;
                    return Task.CompletedTask;
                }
            }

            [Fact]
            public async Task Should_Get_Handler()
            {
                var expected = Create<int>();
                var actual = -expected;

                // ReSharper disable once ConvertToLocalFunction
                Func<int, Task> handler = value =>
                {
                    actual = value;
                    return Task.CompletedTask;
                };

                var subscription = new AsyncWeakSubscription(handler);

                // unlike AsyncSubscription, AsyncWeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                await registeredHandler.Invoke(expected);

                actual.Should().Be(expected);
            }

            // Weak reference related tests are not working
            //[Fact]
            //public async Task Should_Not_Handle_Disposed_Handler()
            //{
            //  var expected = Create<int>();
            //  var actual = -expected;

            //  // ReSharper disable once ConvertToLocalFunction
            //  Func<int, Task> handler = value =>
            //  {
            //    actual = value;
            //    return Task.CompletedTask;
            //  };

            //  var subscription = new AsyncWeakSubscription(handler);

            //  handler = null;
            //  GC.Collect();
            //  GC.Collect();

            //  var registeredHandler = subscription.GetHandler<int>();

            //  await registeredHandler.Invoke(expected);

            //  actual.Should().Be(-expected);
            //}

            [Fact]
            public async Task Should_Get_Static_Handler()
            {
                var expected = Create<int>();

                // ReSharper disable once ConvertToLocalFunction
                Func<int, Task> handler = DummyHandler.StaticHandler;

                var subscription = new AsyncWeakSubscription(handler);

                // unlike AsyncSubscription, AsyncWeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                DummyHandler.ActualValue = -expected;
                await registeredHandler.Invoke(expected);

                DummyHandler.ActualValue.Should().Be(expected);
            }
        }

        public class HandleAsync : AsyncWeakSubscriptionFixture
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

                var subscription = new AsyncWeakSubscription(handler);

                await subscription.HandleAsync(expected);

                actual.Should().Be(expected);
            }
        }
    }
}
