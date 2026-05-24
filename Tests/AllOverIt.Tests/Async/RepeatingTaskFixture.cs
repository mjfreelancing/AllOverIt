using AllOverIt.Async;
using AllOverIt.Fixture;
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

                invoked.ShouldBeTrue();

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
                  .ShouldThrowAsync<TaskCanceledException>();
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

                invoked.ShouldBeTrue();
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

                invokedCount.ShouldBe(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                await task;

                invokedCount.ShouldBe(2);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_RepeatDelay()
            {
                using var semaphore = new SemaphoreSlim(0);
                using var cts = new CancellationTokenSource();

                var invokedCount = 0;

                Task DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cts.Cancel();
                    }

                    semaphore.Release();

                    return Task.CompletedTask;
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                // Wait for the first invocation
                await semaphore.WaitAsync();

                invokedCount.ShouldBe(1);

                // Give the worker a moment to schedule the repeat delay before advancing fake time
                await Task.Delay(50);

                _timeProviderFake.Advance(_delayTimeSpan - TimeSpan.FromSeconds(1));

                // Should not advance yet - still waiting
                invokedCount.ShouldBe(1);

                _timeProviderFake.Advance(TimeSpan.FromSeconds(1));

                // Wait for the second invocation with timeout
                var waited = await semaphore.WaitAsync(TimeSpan.FromSeconds(5));

                waited.ShouldBeTrue("the second invocation should complete within timeout");

                invokedCount.ShouldBe(2);

                await task;
            }

            [Fact]
            public async Task Should_Invoke_Action_With_Initial_Delay()
            {
                using var semaphore = new SemaphoreSlim(0);
                using var cancellationToken = new CancellationTokenSource();

                var invokedCount = 0;

                Task DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cancellationToken.Cancel();
                    }

                    semaphore.Release();

                    return Task.CompletedTask;
                }

                var options = new RepeatingTaskOptions
                {
                    InitialDelay = _delayTimeSpan * 2,
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cancellationToken.Token);

                // Give the background task time to actually start (but not invoke yet due to initial delay)
                await Task.Delay(100);

                invokedCount.ShouldBe(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                // Still should not have invoked - needs 2x delay
                invokedCount.ShouldBe(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                // Give the worker a moment to process timer completion
                await Task.Delay(50);

                // Wait for the first invocation with timeout
                var waited1 = await semaphore.WaitAsync(TimeSpan.FromSeconds(5));

                waited1.ShouldBeTrue("the first invocation should complete within timeout");

                invokedCount.ShouldBe(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                // Give the worker a moment to process timer completion
                await Task.Delay(50);

                // Wait for the second invocation with timeout
                var waited2 = await semaphore.WaitAsync(TimeSpan.FromSeconds(5));

                waited2.ShouldBeTrue("the second invocation should complete within timeout");

                invokedCount.ShouldBe(2);

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

                invoked.ShouldBeTrue();

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
                  .ShouldThrowAsync<TaskCanceledException>();
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

                invoked.ShouldBeTrue();
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

                // Give the background task time to actually start.
                resetEvent.Wait();

                invokedCount.ShouldBe(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                await task;

                invokedCount.ShouldBe(2);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_RepeatDelay()
            {
                using var semaphore = new SemaphoreSlim(0);
                using var cts = new CancellationTokenSource();

                var invokedCount = 0;

                void DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cts.Cancel();
                    }

                    semaphore.Release();
                }

                var options = new RepeatingTaskOptions
                {
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cts.Token);

                // Wait for the first invocation
                await semaphore.WaitAsync();

                invokedCount.ShouldBe(1);

                // Give the worker a moment to schedule the repeat delay before advancing fake time
                await Task.Delay(50);

                _timeProviderFake.Advance(_delayTimeSpan - TimeSpan.FromSeconds(1));

                // Should not advance yet - still waiting
                invokedCount.ShouldBe(1);

                _timeProviderFake.Advance(TimeSpan.FromSeconds(1));

                // Wait for the second invocation with timeout
                var waited = await semaphore.WaitAsync(TimeSpan.FromSeconds(5));

                waited.ShouldBeTrue("the second invocation should complete within timeout");

                invokedCount.ShouldBe(2);

                await task;
            }

            [Fact]
            public async Task Should_Invoke_Action_With_Initial_Delay()
            {
                using var semaphore = new SemaphoreSlim(0);
                using var cancellationToken = new CancellationTokenSource();

                var invokedCount = 0;

                void DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cancellationToken.Cancel();
                    }

                    semaphore.Release();
                }

                var options = new RepeatingTaskOptions
                {
                    InitialDelay = _delayTimeSpan * 2,
                    RepeatDelay = _delayTimeSpan,
                    TimeProvider = _timeProviderFake
                };

                var task = RepeatingTask.StartAsync(DoAction, options, cancellationToken.Token);

                // Give the background task time to actually start (but not invoke yet due to initial delay)
                await Task.Delay(100);

                invokedCount.ShouldBe(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                // Still should not have invoked - needs 2x delay
                invokedCount.ShouldBe(0);

                _timeProviderFake.Advance(_delayTimeSpan);

                // Give the worker a moment to process timer completion
                await Task.Delay(50);

                // Wait for the first invocation with timeout
                var waited1 = await semaphore.WaitAsync(TimeSpan.FromSeconds(5));

                waited1.ShouldBeTrue("the first invocation should complete within timeout");

                invokedCount.ShouldBe(1);

                _timeProviderFake.Advance(_delayTimeSpan);

                // Give the worker a moment to process timer completion
                await Task.Delay(50);

                // Wait for the second invocation with timeout
                var waited2 = await semaphore.WaitAsync(TimeSpan.FromSeconds(5));

                waited2.ShouldBeTrue("the second invocation should complete within timeout");

                invokedCount.ShouldBe(2);

                await task;
            }
        }
    }
}




