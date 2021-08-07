using AllOverIt.Fixture;
using AllOverIt.Tasks;
using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Tasks
{
    public class RepeatingTaskFixture : FixtureBase
    {
        public class Start_ActionAsync : RepeatingTaskFixture
        {
            [Fact]
            public async Task Should_Invoke_Action()
            {
                var cancellationToken = new CancellationTokenSource();
                var invoked = false;

                Task DoAction()
                {
                    invoked = true;
                    cancellationToken.Cancel();

                    return Task.CompletedTask;
                }

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, 10);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Invoke_Action_When_Cancelled()
            {
                var cancellationToken = new CancellationTokenSource();
                cancellationToken.Cancel();

                var task = RepeatingTask.Start(() => Task.CompletedTask, cancellationToken.Token, 10);

                Invoking(() => task)
                  .Should()
                  .Throw<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Abort_When_Cancelled()
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

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, 10);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Fact]
            public async Task Should_Invoke_Action_Until_Cancelled()
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

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, 10);

                await task.ConfigureAwait(false);

                invokedCount.Should().Be(2);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_RepeatDelay()
            {
                const int repeatDelay = 100;
                var cancellationToken = new CancellationTokenSource();
                var invokedCount = 0;
                var delays = new List<long>();
                var stopwatch = new Stopwatch();
                var lastElapsed = 0L;

                Task DoAction()
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

                stopwatch.Start();

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, repeatDelay);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                delays[0].Should().BeLessThan(repeatDelay);   // should be invoked without delay
                delays[1].Should().BeGreaterOrEqualTo(repeatDelay);
                delays[2].Should().BeGreaterOrEqualTo(repeatDelay);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_Initial_Delay()
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

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, repeatDelay, initialDelay);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                delays[0].Should().BeGreaterOrEqualTo(initialDelay);
                delays[1].Should().BeGreaterOrEqualTo(repeatDelay);
                delays[2].Should().BeGreaterOrEqualTo(repeatDelay);
            }
        }

        public class Start_Action : RepeatingTaskFixture
        {
            [Fact]
            public async Task Should_Invoke_Action()
            {
                var cancellationToken = new CancellationTokenSource();
                var invoked = false;

                void DoAction()
                {
                    invoked = true;
                    cancellationToken.Cancel();
                }

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, 10);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Invoke_Action_When_Cancelled()
            {
                var cancellationToken = new CancellationTokenSource();
                cancellationToken.Cancel();

                var task = RepeatingTask.Start(() => { }, cancellationToken.Token, 10);

                Invoking(() => task)
                  .Should()
                  .Throw<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Abort_When_Cancelled()
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

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, 10);

                await task.ConfigureAwait(false);

                invoked.Should().BeTrue();
            }

            [Fact]
            public async Task Should_Invoke_Action_Until_Cancelled()
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

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, 10);

                await task.ConfigureAwait(false);

                invokedCount.Should().Be(2);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_RepeatDelay()
            {
                const int repeatDelay = 100;
                var cancellationToken = new CancellationTokenSource();
                var invokedCount = 0;
                var delays = new List<long>();
                var stopwatch = new Stopwatch();
                var lastElapsed = 0L;

                void DoAction()
                {
                    var elapsed = stopwatch.ElapsedMilliseconds - lastElapsed;
                    lastElapsed = elapsed;

                    delays.Add(elapsed);

                    invokedCount++;

                    if (invokedCount == 3)
                    {
                        cancellationToken.Cancel();
                    }
                }

                stopwatch.Start();

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, repeatDelay);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                delays[0].Should().BeLessThan(repeatDelay);                 // should be invoked without delay
                delays[1].Should().BeGreaterOrEqualTo(repeatDelay - 1);     // occasionally comes in at 99ms
                delays[2].Should().BeGreaterOrEqualTo(repeatDelay - 1);
            }

            [Fact]
            public async Task Should_Invoke_Action_With_Initial_Delay()
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

                var task = RepeatingTask.Start(DoAction, cancellationToken.Token, repeatDelay, initialDelay);

                await task.ConfigureAwait(false);

                stopwatch.Stop();

                delays[0].Should().BeGreaterOrEqualTo(initialDelay);
                delays[1].Should().BeGreaterOrEqualTo(repeatDelay);
                delays[2].Should().BeGreaterOrEqualTo(repeatDelay);
            }
        }
    }
}
