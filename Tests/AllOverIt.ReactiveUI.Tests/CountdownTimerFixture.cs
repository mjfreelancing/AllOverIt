using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests
{
    public class CountdownTimerFixture : FixtureBase
    {
        public class Configure : CountdownTimerFixture
        {
            [Fact]
            public void Should_Initialize()
            {
                var totalMilliseconds = GetWithinRange(10000, 12000);
                var updateIntervalMilliseconds = GetWithinRange(1000, 1500);

                using (var timer = new CountdownTimer())
                {
                    timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                    timer.TotalMilliseconds.Should().Be(totalMilliseconds);
                    timer.TotalTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(1));
                    timer.IsRunning.Should().BeFalse();

                    // The timer hasn't started yet
                    timer.RemainingMilliseconds.Should().Be(0);
                    timer.RemainingTimeSpan.Should().Be(TimeSpan.FromMilliseconds(0));
                }
            }

            [Fact]
            public void Should_Use_ObserveOn_Scheduler()
            {
                var totalMilliseconds = GetWithinRange(10000, 12000);
                var updateIntervalMilliseconds = GetWithinRange(1000, 1500);

                var observeOnSchedulerFake = this.CreateStub<IScheduler>();

                var scheduled = false;

                // Need to use this approach as we can't assert Schedule<T> was called
                A.CallTo(observeOnSchedulerFake)
                    .Where(call => call.Method.Name == "Schedule")
                    .Invokes(call =>
                    {
                        scheduled = true;
                    });

                var scheduler = new TestScheduler();
                scheduler.Start();

                using (var timer = new CountdownTimer(scheduler))
                {
                    timer.Configure(totalMilliseconds, updateIntervalMilliseconds, observeOnSchedulerFake);

                    timer.Start();

                    scheduler.AdvanceByMilliseconds(totalMilliseconds * 2);

                    scheduled.Should().BeTrue();
                }
            }

            [Fact]
            public void Should_Throw_When_Configure_When_Already_Running()
            {
                var totalMilliseconds = GetWithinRange(10000, 12000);
                var updateIntervalMilliseconds = GetWithinRange(1000, 1500);

                using (var timer = new CountdownTimer())
                {
                    timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                    timer.Start();

                    Invoking(() =>
                    {
                        timer.Configure(totalMilliseconds, updateIntervalMilliseconds);
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The countdown timer period cannot be modified while executing.");
                }
            }        
        }

        public class Start : CountdownTimerFixture
        {
            [Fact]
            public void Should_Throw_When_Not_Configured()
            {
                Invoking(() =>
                {
                    using (var timer = new CountdownTimer())
                    {
                        timer.Start();
                    }
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage($"The {nameof(ICountdownTimer.Configure)}() method must be called first.");
            }

            [Fact]
            public void Should_Throw_When_Already_Running()
            {
                Invoking(() =>
                {
                    using (var timer = new CountdownTimer())
                    {
                        timer.Configure(Create<double>(), Create<double>());
                        timer.Start();

                        timer.Start();
                    }
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The countdown timer is already executing.");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            public void Should_Update_Remaining_Time_When_Started(int skipFactor)
            {
                var totalMilliseconds = GetWithinRange(10000, 12000);
                var updateIntervalMilliseconds = GetWithinRange(1000, 1500);

                using (var timer = new CountdownTimer())
                {
                    timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                    timer.TotalMilliseconds.Should().Be(totalMilliseconds);
                    timer.TotalTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(1));
                    timer.IsRunning.Should().BeFalse();

                    var skipMilliseconds = skipFactor * GetWithinRange(100, 200);
                    timer.Start(skipMilliseconds);

                    var remaining = totalMilliseconds - skipMilliseconds;

                    timer.RemainingMilliseconds.Should().Be(remaining);
                    timer.RemainingTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(remaining), TimeSpan.FromMilliseconds(1));
                }
            }

            [Theory]
            [InlineData(0, 0)]
            [InlineData(1, 0)]
            [InlineData(0, 1)]
            [InlineData(1, 1)]
            public void Should_Update_Notifications_While_Running(int skipFactor, int skipTimeMode)
            {
                var actualNotifications = new List<double>();

                double totalMilliseconds = (int) GetWithinRange(10000, 12000);
                double updateIntervalMilliseconds = (int) GetWithinRange(1000, 1500);
                var skipMilliseconds = skipFactor * GetWithinRange(100, 200);

                var expectedRemaining = totalMilliseconds - skipMilliseconds;
                var expectedNotifications = new List<double>(new[] { 0, totalMilliseconds - skipMilliseconds });

                while (expectedRemaining > 0)
                {
                    expectedRemaining -= updateIntervalMilliseconds;

                    if (expectedRemaining < 0)
                    {
                        expectedRemaining = 0;
                    }

                    expectedNotifications.Add(expectedRemaining);
                }

                var scheduler = new TestScheduler();
                scheduler.Start();

                using (var timer = new CountdownTimer(scheduler))
                {
                    timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                    timer.WhenAnyValue(vm => vm.RemainingMilliseconds)
                        .Subscribe(value => actualNotifications.Add(value));

                    timer.TotalMilliseconds.Should().Be(totalMilliseconds);
                    timer.TotalTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(1));
                    timer.IsRunning.Should().BeFalse();

                    if (skipTimeMode == 0)
                    {
                        timer.Start(skipMilliseconds);
                    }
                    else
                    {
                        timer.Start(TimeSpan.FromMilliseconds(skipMilliseconds));
                    }

                    timer.IsRunning.Should().BeTrue();

                    scheduler.AdvanceByMilliseconds(totalMilliseconds * 2);

                    timer.RemainingMilliseconds.Should().Be(0);
                    timer.RemainingTimeSpan.Should().Be(TimeSpan.FromMilliseconds(0));
                    timer.IsRunning.Should().BeFalse();

                    expectedNotifications.Should().ContainInOrder(actualNotifications);
                }
            }

            [Theory]
            [InlineData(0, 0)]
            [InlineData(1, 0)]
            [InlineData(0, 1)]
            [InlineData(1, 1)]
            public void Should_Cancel_While_Running(int skipFactor, int skipTimeMode)
            {
                double totalMilliseconds = (int) GetWithinRange(10000, 12000);
                double updateIntervalMilliseconds = (int) GetWithinRange(1000, 1500);
                var skipMilliseconds = skipFactor * GetWithinRange(100, 200);

                var scheduler = new TestScheduler();
                scheduler.Start();

                using (var cts = new CancellationTokenSource())
                {
                    using (var timer = new CountdownTimer(scheduler))
                    {
                        timer.Configure(totalMilliseconds, updateIntervalMilliseconds, null, cts.Token);

                        timer.TotalMilliseconds.Should().Be(totalMilliseconds);
                        timer.TotalTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(1));
                        timer.IsRunning.Should().BeFalse();

                        if (skipTimeMode == 0)
                        {
                            timer.Start(skipMilliseconds);
                        }
                        else
                        {
                            timer.Start(TimeSpan.FromMilliseconds(skipMilliseconds));
                        }

                        timer.IsRunning.Should().BeTrue();

                        scheduler.AdvanceByMilliseconds(updateIntervalMilliseconds * 2);

                        timer.RemainingMilliseconds.Should().BeGreaterThan(0);
                        timer.IsRunning.Should().BeTrue();

                        cts.Cancel();

                        scheduler.AdvanceByMilliseconds(updateIntervalMilliseconds * 2);

                        timer.RemainingMilliseconds.Should().Be(0);
                        timer.IsRunning.Should().BeFalse();
                    }
                }
            }

            [Fact]
            public void Should_Start_After_Stop()
            {
                using (var timer = new CountdownTimer())
                {
                    timer.Configure(Create<double>(), Create<double>());

                    timer.IsRunning.Should().BeFalse();

                    timer.Start();

                    timer.IsRunning.Should().BeTrue();

                    timer.Stop();

                    timer.IsRunning.Should().BeFalse();

                    timer.Start();

                    timer.IsRunning.Should().BeTrue();
                }
            }
        }

        public class Stop : CountdownTimerFixture
        {
            [Fact]
            public void Should_Stop()
            {
                using (var timer = new CountdownTimer())
                {
                    timer.Configure(Create<double>(), Create<double>());

                    Invoking(() =>
                    {
                        timer.Stop();
                    })
                    .Should()
                    .NotThrow();
                }
            }

            [Fact]
            public void Should_Not_Throw_When_Already_Stopped()
            {
                using (var timer = new CountdownTimer())
                {
                    timer.Configure(Create<double>(), Create<double>());

                    timer.Start();

                    timer.IsRunning.Should().BeTrue();

                    timer.Stop();

                    timer.IsRunning.Should().BeFalse();
                }
            }
        }

        public class Observables : CountdownTimerFixture
        {
            [Fact]
            public void Should_Notify_IsRunning()
            {
                var scheduler = new TestScheduler();

                using (var timer = new CountdownTimer())
                {
                    timer.Configure(10000, 1000);

                    var isRunning = false;

                    timer.WhenAnyValue(vm => vm.IsRunning)
                        .Subscribe(value =>
                        {
                            isRunning = value;
                        });

                    isRunning.Should().BeFalse();

                    timer.Start();

                    isRunning.Should().BeTrue();
                }
            }

            [Fact]
            public void Should_Notify_When_Completed()
            {
                double totalMilliseconds = (int) GetWithinRange(10000, 12000);
                double updateIntervalMilliseconds = (int) GetWithinRange(1000, 1500);

                var scheduler = new TestScheduler();
                scheduler.Start();

                var completed = false;

                using (var timer = new CountdownTimer(scheduler))
                {
                    timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                    timer.WhenCompleted()
                        .Subscribe(value => completed = value);

                    timer.Start();

                    scheduler.AdvanceBy(TimeSpan.FromMilliseconds(totalMilliseconds * 2).Ticks);

                    completed.Should().BeTrue();
                }
            }
        }
    }
}