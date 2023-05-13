﻿using AllOverIt.Async;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Async
{
    public class RepeatingTaskFixture : FixtureBase
    {
        public enum TimeUnit
        {
            Milliseconds,
            Timespan
        }

        private static readonly int _delayMilliseconds = 10;
        private static readonly TimeSpan _delayTimeSpan = TimeSpan.FromMilliseconds(_delayMilliseconds);

        public class Start_ActionAsync : RepeatingTaskFixture
        {
            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
                var invoked = false;

                Task DoAction()
                {
                    invoked = true;
                    cancellationToken.Cancel();

                    return Task.CompletedTask;
                }

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, _delayTimeSpan, cancellationToken.Token);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Not_Invoke_Action_When_Cancelled(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
                cancellationToken.Cancel();

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(() => Task.CompletedTask, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(() => Task.CompletedTask, _delayTimeSpan, cancellationToken.Token);

                await Invoking(() => task)
                  .Should()
                  .ThrowAsync<TaskCanceledException>();
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Abort_When_Cancelled(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
                var invoked = false;

                async Task DoAction()
                {
                    invoked = true;
                    cancellationToken.Cancel();

                    // will throw OperationCanceledException but will be handled
                    await Task.Delay(1, cancellationToken.Token);
                }

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, _delayTimeSpan, cancellationToken.Token);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action_Until_Cancelled(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
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

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, _delayTimeSpan, cancellationToken.Token);

                await task.ConfigureAwait(false);

                invokedCount.Should().Be(2);
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action_With_RepeatDelay(TimeUnit timeUnit)
            {
                const int repeatDelay = 100;
                var cancellationToken = new CancellationTokenSource();
                var invokedCount = 0;
                var delays = new List<long>();
                var lastElapsed = 0L;

                Task DoAction(Stopwatch stopwatch)
                {
                    var elapsed = stopwatch.ElapsedMilliseconds - lastElapsed;
                    lastElapsed = elapsed;

                    delays.Add(elapsed);

                    invokedCount++;

                    if (invokedCount == 3)
                    {
                        cancellationToken.Cancel();
                    }

                    return Task.CompletedTask;
                }

                var stopwatch = Stopwatch.StartNew();

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(() => DoAction(stopwatch), repeatDelay, cancellationToken.Token)
                    : RepeatingTask.Start(() => DoAction(stopwatch), TimeSpan.FromMilliseconds(repeatDelay), cancellationToken.Token);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                delays[0].Should().BeLessThan(repeatDelay);                 // Should be invoked without delay
                delays[1].Should().BeGreaterOrEqualTo(repeatDelay - 1);     // Providing some latitude
                delays[2].Should().BeGreaterOrEqualTo(repeatDelay - 1);
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action_With_Initial_Delay(TimeUnit timeUnit)
            {
                const int initialDelay = 200;
                const int repeatDelay = 100;
                var cancellationToken = new CancellationTokenSource();
                var invokedCount = 0;
                var delays = new List<long>();
                var stopwatch = Stopwatch.StartNew();
                var lastElapsed = 0L;

                Task DoAction()
                {
                    delays.Add(stopwatch.ElapsedMilliseconds - lastElapsed);
                    invokedCount++;

                    // re-evaluate to eliminate delays with the Add() method
                    lastElapsed = stopwatch.ElapsedMilliseconds;

                    if (invokedCount == 3)
                    {
                        cancellationToken.Cancel();
                    }

                    return Task.CompletedTask;
                }

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, initialDelay, repeatDelay, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, TimeSpan.FromMilliseconds(initialDelay), TimeSpan.FromMilliseconds(repeatDelay), cancellationToken.Token);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                delays[0].Should().BeGreaterOrEqualTo(initialDelay - 1);    // Providing some latitude
                delays[1].Should().BeGreaterOrEqualTo(repeatDelay - 1);
                delays[2].Should().BeGreaterOrEqualTo(repeatDelay - 1);
            }
        }

        public class Start_Action : RepeatingTaskFixture
        {
            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
                var invoked = false;

                void DoAction()
                {
                    invoked = true;
                    cancellationToken.Cancel();
                }

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, _delayTimeSpan, cancellationToken.Token);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Not_Invoke_Action_When_Cancelled(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
                cancellationToken.Cancel();

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(() => { }, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(() => { }, _delayTimeSpan, cancellationToken.Token);

                await Invoking(() => task)
                  .Should()
                  .ThrowAsync<TaskCanceledException>();
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Abort_When_Cancelled(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
                var invoked = false;

                void DoAction()
                {
                    invoked = true;
                    cancellationToken.Cancel();

                    // will throw OperationCanceledException but will be handled
                    Task.Delay(1, cancellationToken.Token).GetAwaiter().GetResult();
                }

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, _delayTimeSpan, cancellationToken.Token);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action_Until_Cancelled(TimeUnit timeUnit)
            {
                var cancellationToken = new CancellationTokenSource();
                var invokedCount = 0;

                void DoAction()
                {
                    invokedCount++;

                    if (invokedCount == 2)
                    {
                        cancellationToken.Cancel();
                    }
                }

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, _delayMilliseconds, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, _delayTimeSpan, cancellationToken.Token);

                await task.ConfigureAwait(false);

                invokedCount.Should().Be(2);
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action_With_RepeatDelay(TimeUnit timeUnit)
            {
                const int repeatDelay = 100;
                var cancellationToken = new CancellationTokenSource();
                var invokedCount = 0;
                var delays = new List<int>();
                var stopwatch = new Stopwatch();
                var lastElapsed = 0L;

                void DoAction()
                {
                    var elapsed = (int)(stopwatch.ElapsedMilliseconds - lastElapsed);

                    delays.Add(elapsed);

                    invokedCount++;

                    if (invokedCount == 3)
                    {
                        cancellationToken.Cancel();
                    }

                    lastElapsed = stopwatch.ElapsedMilliseconds;
                }

                stopwatch.Start();

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, repeatDelay, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, TimeSpan.FromMilliseconds(repeatDelay), cancellationToken.Token);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                // delays[0] can be ignored since it is the first run with no initial delay.
                // The other delays cannot be guaranteed to be close to 'repeatDelay', so check >=
                delays[1].Should().BeGreaterThanOrEqualTo(repeatDelay);
                delays[2].Should().BeGreaterThanOrEqualTo(repeatDelay);
            }

            [Theory]
            [InlineData(TimeUnit.Milliseconds)]
            [InlineData(TimeUnit.Timespan)]
            public async Task Should_Invoke_Action_With_Initial_Delay(TimeUnit timeUnit)
            {
                const int initialDelay = 200;
                const int repeatDelay = 100;
                var cancellationToken = new CancellationTokenSource();
                var invokedCount = 0;
                var delays = new List<long>();
                var stopwatch = Stopwatch.StartNew();
                var lastElapsed = 0L;

                void DoAction()
                {
                    delays.Add(stopwatch.ElapsedMilliseconds - lastElapsed);
                    invokedCount++;

                    // re-evaluate to eliminate delays with the Add() method
                    lastElapsed = stopwatch.ElapsedMilliseconds;

                    if (invokedCount == 3)
                    {
                        cancellationToken.Cancel();
                    }
                }

                var task = timeUnit == TimeUnit.Milliseconds
                    ? RepeatingTask.Start(DoAction, initialDelay, repeatDelay, cancellationToken.Token)
                    : RepeatingTask.Start(DoAction, TimeSpan.FromMilliseconds(initialDelay), TimeSpan.FromMilliseconds(repeatDelay), cancellationToken.Token);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                delays[0].Should().BeGreaterOrEqualTo(initialDelay);
                delays[1].Should().BeGreaterOrEqualTo(repeatDelay);
                delays[2].Should().BeGreaterOrEqualTo(repeatDelay);
            }
        }
    }
}
