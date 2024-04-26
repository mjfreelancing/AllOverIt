using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Extensions
{
    public class TaskExtensionsFixture : FixtureBase
    {
        public class FireAndForget : TaskExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Handler_Null()
            {
                Invoking(() =>
                {
                    AllOverIt.Extensions.TaskExtensions.FireAndForget(Task.CompletedTask, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionHandler");
            }


            [Fact]
            public void Should_Not_Call_Handler_When_Task_Completed()
            {
                var handlerCalled = false;

                AllOverIt.Extensions.TaskExtensions.FireAndForget(Task.CompletedTask, _ => handlerCalled = true);

                handlerCalled.Should().BeFalse();
            }

            [Fact]
            public void Should_Handle_Faulted_Task()
            {
                var exception = new InvalidOperationException();

                var completionSource = new TaskCompletionSource<bool>();
                completionSource.SetException(exception);

                Exception actual = null;

                AllOverIt.Extensions.TaskExtensions.FireAndForget(completionSource.Task, exception => actual = exception);

                actual.Should().BeSameAs(exception);
            }
        }

        public class DoFireAndForget : TaskExtensionsFixture
        {
            [Fact]
            public async Task Should_Not_Call_Handler_When_Task_Completed()
            {
                var handlerCalled = false;

                await AllOverIt.Extensions.TaskExtensions.DoFireAndForget(Task.CompletedTask, _ => handlerCalled = true);

                handlerCalled.Should().BeFalse();
            }

            [Fact]
            public async Task Should_Await_Non_Faulted_Task()
            {
                Exception actual = null;

                await AllOverIt.Extensions.TaskExtensions.DoFireAndForget(Task.Delay(1), exception => actual = exception);

                actual.Should().BeNull();
            }

            [Fact]
            public async Task Should_Handle_Faulted_Task()
            {
                var exception = new InvalidOperationException();

                var completionSource = new TaskCompletionSource<bool>();
                completionSource.SetException(exception);

                Exception actual = null;

                await AllOverIt.Extensions.TaskExtensions.DoFireAndForget(completionSource.Task, exception => actual = exception);

                actual.Should().BeSameAs(exception);
            }
        }
    }
}
