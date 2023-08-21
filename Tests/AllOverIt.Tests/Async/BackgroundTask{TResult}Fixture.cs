using AllOverIt.Async;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace AllOverIt.Tests.Async
{
    public class BackgroundTask_TResult_Fixture : FixtureBase
    {
        public class Constructor_Action : BackgroundTask_TResult_Fixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask<bool>(null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public async Task Should_Cancel_Task()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask<bool>(async token =>
                {
                    await Task.Delay(-1, token);

                    return Create<bool>();
                }, cts.Token);

                cts.Cancel();

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                 .Should()
                 .ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Return_Result()
            {
                var expected = Create<bool>();

                var backgroundTask = new BackgroundTask<bool>(_ =>
                {
                    return Task.FromResult(expected);
                }, CancellationToken.None);

                var actual = await backgroundTask;

                actual.Should().Be(expected);
            }
        }

        public class Constructor_Action_ExceptionHandler : BackgroundTask_TResult_Fixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask<bool>(null, edi => true, CancellationToken.None);
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
                    _ = new BackgroundTask<bool>(token => Task.FromResult(Create<bool>()), null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionHandler");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_Throw_When_Task_Cancelled(bool handled)
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask<bool>(async token =>
                {
                    await Task.Delay(-1, token);

                    return Create<bool>();
                }, edi => handled, cts.Token);

                await Task.Yield();

                cts.Cancel();

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                .Should()
                .ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Handled()
            {
                var expected = new Exception();
                Exception actual = null;

                var backgroundTask = new BackgroundTask<bool>(token => throw expected, edi =>
                {
                    actual = edi.SourceException;
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

                var backgroundTask = new BackgroundTask<bool>(token => throw expected, edi =>
                {
                    actual = edi.SourceException;
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

            [Fact]
            public async Task Should_Return_Result()
            {
                var expected = Create<bool>();

                var backgroundTask = new BackgroundTask<bool>(_ =>
                {
                    return Task.FromResult(expected);
                }, edi => false, CancellationToken.None);

                var actual = await backgroundTask;

                actual.Should().Be(expected);
            }
        }

        public class Constructor_Options_Scheduler : BackgroundTask_TResult_Fixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask<bool>(null, TaskCreationOptions.None, TaskScheduler.Current, CancellationToken.None);
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
                    _ = new BackgroundTask<bool>(token => Task.FromResult(Create<bool>()), TaskCreationOptions.None, null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("scheduler");
            }

            [Fact]
            public async Task Should_Cancel_Task()
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask<bool>(async token =>
                {
                    await Task.Delay(-1, token);

                    return Create<bool>();
                }, TaskCreationOptions.None, TaskScheduler.Current, cts.Token);

                cts.Cancel();

                await Invoking(async () =>
                {
                    await backgroundTask;
                })
                 .Should()
                 .ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Return_Result()
            {
                var expected = Create<bool>();

                var backgroundTask = new BackgroundTask<bool>(token =>
                {
                    return Task.FromResult(expected);
                }, TaskCreationOptions.None, TaskScheduler.Current, CancellationToken.None);

                var actual = await backgroundTask;

                actual.Should().Be(expected);
            }
        }

        public class Constructor_Options_Scheduler_ExceptionHandler : BackgroundTask_TResult_Fixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new BackgroundTask<bool>(null, TaskCreationOptions.None, TaskScheduler.Current, edi => true, CancellationToken.None);
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
                    _ = new BackgroundTask<bool>(token => Task.FromResult(Create<bool>()), TaskCreationOptions.None, null, edi => true, CancellationToken.None);
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
                    _ = new BackgroundTask<bool>(token => Task.FromResult(Create<bool>()), TaskCreationOptions.None, TaskScheduler.Current, null, CancellationToken.None);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionHandler");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_Handle_When_Task_Cancelled(bool handled)
            {
                var cts = new CancellationTokenSource();

                var backgroundTask = new BackgroundTask<bool>(async token =>
                {
                    await Task.Delay(-1, token);

                    return Create<bool>();
                }, TaskCreationOptions.None, TaskScheduler.Current, edi => handled, cts.Token);

                await Task.Delay(10);

                cts.Cancel();

                if (handled)
                {
                    await Invoking(async () =>
                    {
                        await backgroundTask;
                    })
                     .Should()
                     .NotThrowAsync();
                }
                else
                {
                    await Invoking(async () =>
                    {
                        await backgroundTask;
                    })
                     .Should()
                     .ThrowAsync<TaskCanceledException>();
                }
            }

            [Fact]
            public async Task Should_Not_Throw_When_Handled()
            {
                var expected = new Exception();
                Exception actual = null;

                var backgroundTask = new BackgroundTask<bool>(token => throw expected, TaskCreationOptions.None, TaskScheduler.Current, edi =>
                {
                    actual = edi.SourceException;
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

                var backgroundTask = new BackgroundTask<bool>(token => throw expected, TaskCreationOptions.None, TaskScheduler.Current, edi =>
                {
                    actual = edi.SourceException;
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

            [Fact]
            public async Task Should_Return_Result()
            {
                var expected = Create<bool>();

                var backgroundTask = new BackgroundTask<bool>(_ =>
                {
                    return Task.FromResult(expected);
                }, TaskCreationOptions.None, TaskScheduler.Current, edi => false, CancellationToken.None);

                var actual = await backgroundTask;

                actual.Should().Be(expected);
            }
        }

        public class Implicit_Operator_Task : BackgroundTask_TResult_Fixture
        {
            [Fact]
            public void Should_Not_Throw_When_BackgroundTask_Null()
            {
                Task actual = Task.CompletedTask;

                Invoking(() =>
                {
                    BackgroundTask<bool> backgroundTask = null;

                    actual = (Task) backgroundTask;
                })
                .Should()
                .NotThrow();

                actual.Should().BeNull();
            }
        }

        public class GetAwaiter : BackgroundTask_TResult_Fixture
        {
            [Fact]
            public void Should_Await_BackgroundTask()
            {
                Invoking(() =>
                {
                    var backgroundTask = new BackgroundTask<bool>(token => Task.FromResult(Create<bool>()), CancellationToken.None);

                    backgroundTask.GetAwaiter().GetResult();
                })
                .Should()
                .NotThrow();
            }
        }

        public class DisposeAsync : BackgroundTask_TResult_Fixture
        {
            [Fact]
            public async Task Should_Cancel_When_Disposed()
            {
                var tcs = new TaskCompletionSource<bool>();
                var cancelled = false;

                var backgroundTask = new BackgroundTask<bool>(async token =>
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

                    return Create<bool>();
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
                var backgroundTask = new BackgroundTask<bool>(async token =>
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

                var backgroundTask = new BackgroundTask<bool>(async token =>
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