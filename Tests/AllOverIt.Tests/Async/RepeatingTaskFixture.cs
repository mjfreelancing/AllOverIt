#if !NETSTANDARD2_1

using AllOverIt.Async;
using AllOverIt.Fixture;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace AllOverIt.Tests.Async
{
    public class RepeatingTaskFixture : FixtureBase
    {
        public enum TimeUnit
        {
            Milliseconds,
            Timespan
        }

        private static readonly int _delayMilliseconds = 1000;
        private static readonly TimeSpan _delayTimeSpan = TimeSpan.FromMilliseconds(_delayMilliseconds);

        private readonly FakeTimeProvider _timeProviderFake = new();

        public class StartAsync : RepeatingTaskFixture
        {
            [Fact]
            public async Task Should_Invoke_Action()
            {
                using var cts = new CancellationTokenSource();

                var resetEvent = new ManualResetEventSlim();

                var invoked = false;

                Task DoAction()
                {
                    invoked = true;

                    resetEvent.Set();

                    cts.Cancel();

                    return Task.CompletedTask;
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                resetEvent.Wait();

                invoked.Should().BeTrue();

                await task;
            }

            [Fact]
            public async Task Should_Not_Invoke_Action_When_Cancelled()
            {
                using var cts = new CancellationTokenSource();

                cts.Cancel();

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(() => Task.CompletedTask, options, cts.Token);

                await Invoking(() => task)
                  .Should()
                  .ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Abort_When_Cancelled()
            {
                using var cts = new CancellationTokenSource();

                var invoked = false;

                async Task DoAction()
                {
                    invoked = true;
                    cts.Cancel();

                    // will throw OperationCanceledException but will be handled
                    await Task.Delay(1, cts.Token);
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                await task;

                invoked.Should().BeTrue();
            }

            [Fact]
            public async Task Should_Invoke_Action_Until_Cancelled()
            {
                var resetEvent = new ManualResetEventSlim();

                using var cts = new CancellationTokenSource();

                var invokedCount = 0;

                Task DoActionAsync()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cts.Cancel();
                    }

                    if (!resetEvent.IsSet)
                    {
                        resetEvent.Set();
                    }

                    return Task.CompletedTask;
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoActionAsync, options, cts.Token);

                // Give the background task time to actually start
                resetEvent.Wait();

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                await task;

                invokedCount.Should().Be(2);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_RepeatDelay()
            {
                var resetEvent = new ManualResetEventSlim();

                using var cts = new CancellationTokenSource();

                var invokedCount = 0;

                Task DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cts.Cancel();
                    }

                    if (!resetEvent.IsSet)
                    {
                        resetEvent.Set();
                    }

                    return Task.CompletedTask;
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                // Give the background task time to actually start
                resetEvent.Wait();

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(_delayTimeSpan - TimeSpan.FromSeconds(1));

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(TimeSpan.FromSeconds(1));

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(2);

                await task;
            }

            [Fact]
            public async Task Should_Invoke_Action_With_Initial_Delay()
            {
                using var cancellationToken = new CancellationTokenSource();

                var invokedCount = 0;

                Task DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cancellationToken.Cancel();
                    }

                    return Task.CompletedTask;
                }

                var options = new RepeatingTaskOptions
                {
                    InitialDelay = _delayTimeSpan * 2,
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cancellationToken.Token);

                // Give the background task time to actually start
                await Task.Delay(100);

                invokedCount.Should().Be(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(2);

                await task;
            }
        }

        public class Start_Action : RepeatingTaskFixture
        {
            [Fact]
            public async Task Should_Invoke_Action()
            {
                var resetEvent = new ManualResetEventSlim();

                using var cts = new CancellationTokenSource();

                var invoked = false;

                void DoAction()
                {
                    invoked = true;

                    resetEvent.Set();

                    cts.Cancel();
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                resetEvent.Wait();

                invoked.Should().BeTrue();

                await task;
            }

            [Fact]
            public async Task Should_Not_Invoke_Action_When_Cancelled()
            {
                using var cts = new CancellationTokenSource();

                cts.Cancel();

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(() => { }, options, cts.Token);

                await Invoking(() => task)
                  .Should()
                  .ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Abort_When_Cancelled()
            {
                using var cts = new CancellationTokenSource();

                var invoked = false;

                void DoAction()
                {
                    invoked = true;
                    cts.Cancel();

                    // will throw OperationCanceledException but will be handled
                    cts.Token.ThrowIfCancellationRequested();
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                await task;

                invoked.Should().BeTrue();
            }

            [Fact]
            public async Task Should_Invoke_Action_Until_Cancelled()
            {
                var resetEvent = new ManualResetEventSlim();

                using var cts = new CancellationTokenSource();

                var invokedCount = 0;

                void DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cts.Cancel();
                    }

                    if (!resetEvent.IsSet)
                    {
                        resetEvent.Set();
                    }
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                // Give the background task time to actually start
                resetEvent.Wait();

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(2);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(2);

                _timeProviderFake.Advance(_delayTimeSpan);

                await task;

                invokedCount.Should().Be(2);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_RepeatDelay()
            {
                var resetEvent = new ManualResetEventSlim();

                using var cts = new CancellationTokenSource();

                var invokedCount = 0;

                void DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cts.Cancel();
                    }

                    if (!resetEvent.IsSet)
                    {
                        resetEvent.Set();
                    }
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                // Give the background task time to actually start
                resetEvent.Wait();

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(_delayTimeSpan - TimeSpan.FromSeconds(1));

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(TimeSpan.FromSeconds(1));

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(2);

                await task;
            }

            [Fact]
            public async Task Should_Invoke_Action_With_Initial_Delay()
            {
                using var cancellationToken = new CancellationTokenSource();

                var invokedCount = 0;

                void DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cancellationToken.Cancel();
                    }
                }

                var options = new RepeatingTaskOptions
                {
                    InitialDelay = _delayTimeSpan * 2,
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cancellationToken.Token);

                // Give the background task time to actually start
                await Task.Delay(100);

                invokedCount.Should().Be(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                await Task.Delay(100);      // Allow time for the callback to invoke

                invokedCount.Should().Be(2);

                await task;
            }
        }
    }
}

#endif