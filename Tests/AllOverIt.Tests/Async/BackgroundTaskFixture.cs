using AllOverIt.Async;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Async
{
    public class BackgroundTaskFixture : FixtureBase
    {
        public class Constructor_Action : BackgroundTaskFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public async Task Should_Cancel_Task()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask(token => Task.Delay(-1, token), cts.Token);

                cts.Cancel();

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                 .Should()
                 .ThrowAsync<TaskCanceledException>();
            }
        }

        public class Constructor_Action_ExceptionHandler : BackgroundTaskFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(null, edi => true, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Throw_When_ExceptionHandler_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(token => Task.CompletedTask, null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionHandler");
            }

            [Fact]
            public async Task Should_Throw_When_Task_Cancelled_And_Not_Handled()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask(
                    token => Task.Delay(-1, token),
                    edi => false, cts.Token);

                await Task.Delay(10);

                cts.Cancel();

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                .Should()
                .ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Task_Cancelled_And_Handled()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask(
                    token => Task.Delay(-1, token),
                    edi => true, cts.Token);

                await Task.Delay(10);

                cts.Cancel();

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                .Should()
                .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Handled()
            {
                var expected = new Exception();
                Exception actual = null;

                var backgroundTask = new BackgroundTask(token => throw expected,
                    exception =>
                    {
                        actual = exception;
                        return true;
                    }, CancellationToken.None);

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                .Should()
                .NotThrowAsync();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public async Task Should_Throw_When_Not_Handled()
            {
                var expected = new Exception(Create<string>());
                Exception actual = null;

                var backgroundTask = new BackgroundTask(token => throw expected,
                    exception =>
                    {
                        actual = exception;
                        return false;
                    }, CancellationToken.None);

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                 .Should()
                 .ThrowAsync<Exception>()
                 .WithMessage(expected.Message);

                actual.Should().BeSameAs(expected);
            }
        }

        public class Constructor_Options_Scheduler : BackgroundTaskFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(null, TaskCreationOptions.None, TaskScheduler.Current, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Throw_When_TaskScheduler_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(token => Task.CompletedTask, TaskCreationOptions.None, null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("scheduler");
            }

            [Fact]
            public async Task Should_Cancel_Task()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask(token => Task.Delay(-1, token), TaskCreationOptions.None, TaskScheduler.Current, cts.Token);

                cts.Cancel();

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                 .Should()
                 .ThrowAsync<TaskCanceledException>();
            }
        }

        public class Constructor_Options_Scheduler_ExceptionHandler : BackgroundTaskFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(null, TaskCreationOptions.None, TaskScheduler.Current, edi => true, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Throw_When_TaskScheduler_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(token => Task.CompletedTask, TaskCreationOptions.None, null, edi => true, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("scheduler");
            }

            [Fact]
            public void Should_Throw_When_ExceptionHandler_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask(token => Task.CompletedTask, TaskCreationOptions.None, TaskScheduler.Current, null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionHandler");
            }

            [Fact]
            public async Task Should_Throw_When_Task_Cancelled_And_Not_Handled()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask(
                    token => Task.Delay(-1, token),
                    TaskCreationOptions.None,
                    TaskScheduler.Current,
                    edi => false, cts.Token);

                await Invoking(async () =>
                {
                    await Task.Run(async () =>
                    {
                        await Task.Delay(10);

                        cts.Cancel();

                        await backgroundTask;
                    });
                })
                .Should()
                .ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Task_Cancelled_And_Handled()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask(
                    token => Task.Delay(-1, token),
                    TaskCreationOptions.None,
                    TaskScheduler.Current,
                    edi => true, cts.Token);

                await Invoking(async () =>
                {
                    await Task.Run(async () =>
                    {
                        await Task.Delay(10);

                        cts.Cancel();

                        await backgroundTask;
                    });
                })
                .Should()
                .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Handled()
            {
                var expected = new Exception();
                Exception actual = null;

                var backgroundTask = new BackgroundTask(token => throw expected, TaskCreationOptions.None, TaskScheduler.Current,
                    exception =>
                    {
                        actual = exception;
                        return true;
                    }, CancellationToken.None);

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                 .Should()
                 .NotThrowAsync();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public async Task Should_Throw_When_Not_Handled()
            {
                var expected = new Exception(Create<string>());
                Exception actual = null;

                var backgroundTask = new BackgroundTask(token => throw expected, TaskCreationOptions.None, TaskScheduler.Current,
                    exception =>
                    {
                        actual = exception;
                        return false;
                    }, CancellationToken.None);

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                 .Should()
                 .ThrowAsync<Exception>()
                 .WithMessage(expected.Message);

                actual.Should().BeSameAs(expected);
            }
        }

        public class Implicit_Operator_Task : BackgroundTaskFixture
        {
            [Fact]
            public void Should_Not_Throw_When_BackgroundTask_Null()
            {
                Task actual = Task.CompletedTask;

                Invoking(() =>
                {
                    BackgroundTask backgroundTask = null;

                    actual = (Task) backgroundTask;
                })
                .Should()
                .NotThrow();

                actual.Should().BeNull();
            }
        }

        public class GetAwaiter : BackgroundTaskFixture
        {
            [Fact]
            public void Should_Await_BackgroundTask()
            {
                Invoking(() =>
                {
                    var backgroundTask = new BackgroundTask(token => Task.CompletedTask, CancellationToken.None);

                    backgroundTask.GetAwaiter().GetResult();
                })
                .Should()
                .NotThrow();
            }
        }

        public class DisposeAsync : BackgroundTaskFixture
        {
            [Fact]
            public async Task Should_Cancel_When_Disposed()
            {
                var tcs = new TaskCompletionSource<bool>();
                var cancelled = false;

                var backgroundTask = new BackgroundTask(async token =>
                {
                    try
                    {
                        tcs.SetResult(true);

                        // Will be blocked until the task is cancelled
                        await Task.Delay(-1, token);
                    }
                    catch (OperationCanceledException)
                    {
                    }

                    cancelled = true;
                }, CancellationToken.None);

                // Give the task a chance to run
                await tcs.Task;

                // Disposing will not allow the TaskCancelledException to propagate
                await backgroundTask.DisposeAsync();

                cancelled.Should().BeTrue();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Disposed()
            {
                var backgroundTask = new BackgroundTask(async token =>
                {
                    await Task.Delay(-1, token);    // The token is cancelled when the background task is disposed
                    throw new Exception();          // Not expecting this to be passed to the exception handler when disposed
                }, edi => false, CancellationToken.None);

                await Invoking(async () =>
                {
                    await backgroundTask.DisposeAsync();
                })
               .Should()
               .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Not_Invoke_Exception_Handler_When_Disposed()
            {
                var handled = false;

                var backgroundTask = new BackgroundTask(async token =>
                {
                    await Task.Delay(-1, token);    // The token is cancelled when the background task is disposed
                    throw new Exception();          // Not expecting this to be passed to the exception handler when disposed
                }, edi =>
                {
                    handled = true;
                    return false;
                }, CancellationToken.None);

                await backgroundTask.DisposeAsync();

                handled.Should().BeFalse();
            }
        }
    }
}