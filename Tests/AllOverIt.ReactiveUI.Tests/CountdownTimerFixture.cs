using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using System.Reactive.Concurrency;

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

                    timer.TotalMilliseconds.ShouldBe(totalMilliseconds);
                    timer.TotalTimeSpan.TotalMilliseconds.ShouldBe(totalMilliseconds, 1);
                    timer.IsRunning.ShouldBeFalse();

                    // The timer hasn't started yet
                    timer.RemainingMilliseconds.ShouldBe(0);
                    timer.RemainingTimeSpan.ShouldBe(TimeSpan.FromMilliseconds(0));
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

                    scheduled.ShouldBeTrue();
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

                    Should.Throw<InvalidOperationException>(() =>
                    {
                        timer.Configure(totalMilliseconds, updateIntervalMilliseconds);
                    })
                    .Message.ShouldBe("The countdown timer period cannot be modified while executing.");
                }
            }
        }

        public class Start : CountdownTimerFixture
        {
            [Fact]
            public void Should_Throw_When_Not_Configured()
            {
                Should.Throw<InvalidOperationException>(() =>
                {
                    using (var timer = new CountdownTimer())
                    {
                        timer.Start();
                    }
                })
                .Message.ShouldBe($"The {nameof(ICountdownTimer.Configure)}() method must be called first.");
            }

            [Fact]
            public void Should_Throw_When_Already_Running()
            {
                Should.Throw<InvalidOperationException>(() =>
                {
                    using (var timer = new CountdownTimer())
                    {
                        timer.Configure(Create<double>(), Create<double>());
                        timer.Start();

                        timer.Start();
                    }
                })
                .Message.ShouldBe("The countdown timer is already executing.");
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

                    timer.TotalMilliseconds.ShouldBe(totalMilliseconds);
                    timer.TotalTimeSpan.TotalMilliseconds.ShouldBe(totalMilliseconds, 1);
                    timer.IsRunning.ShouldBeFalse();

                    var skipMilliseconds = skipFactor * GetWithinRange(100, 200);
                    timer.Start(skipMilliseconds);

                    var remaining = totalMilliseconds - skipMilliseconds;

                    timer.RemainingMilliseconds.ShouldBe(remaining);
                    timer.RemainingTimeSpan.TotalMilliseconds.ShouldBe(remaining, 1);
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

                    timer.TotalMilliseconds.ShouldBe(totalMilliseconds);
                    timer.TotalTimeSpan.TotalMilliseconds.ShouldBe(totalMilliseconds, 1);
                    timer.IsRunning.ShouldBeFalse();

                    if (skipTimeMode == 0)
                    {
                        timer.Start(skipMilliseconds);
                    }
                    else
                    {
                        timer.Start(TimeSpan.FromMilliseconds(skipMilliseconds));
                    }

                    timer.IsRunning.ShouldBeTrue();

                    scheduler.AdvanceByMilliseconds(totalMilliseconds * 2);

                    timer.RemainingMilliseconds.ShouldBe(0);
                    timer.RemainingTimeSpan.ShouldBe(TimeSpan.FromMilliseconds(0));
                    timer.IsRunning.ShouldBeFalse();

                    expectedNotifications.ShouldBe(actualNotifications);
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

                        timer.TotalMilliseconds.ShouldBe(totalMilliseconds);
                        timer.TotalTimeSpan.TotalMilliseconds.ShouldBe(totalMilliseconds, 1);
                        timer.IsRunning.ShouldBeFalse();

                        if (skipTimeMode == 0)
                        {
                            timer.Start(skipMilliseconds);
                        }
                        else
                        {
                            timer.Start(TimeSpan.FromMilliseconds(skipMilliseconds));
                        }

                        timer.IsRunning.ShouldBeTrue();

                        scheduler.AdvanceByMilliseconds(updateIntervalMilliseconds * 2);

                        timer.RemainingMilliseconds.ShouldBeGreaterThan(0);
                        timer.IsRunning.ShouldBeTrue();

                        cts.Cancel();

                        scheduler.AdvanceByMilliseconds(updateIntervalMilliseconds * 2);

                        timer.RemainingMilliseconds.ShouldBe(0);
                        timer.IsRunning.ShouldBeFalse();
                    }
                }
            }

            [Fact]
            public void Should_Start_After_Stop()
            {
                using (var timer = new CountdownTimer())
                {
                    timer.Configure(Create<double>(), Create<double>());

                    timer.IsRunning.ShouldBeFalse();

                    timer.Start();

                    timer.IsRunning.ShouldBeTrue();

                    timer.Stop();

                    timer.IsRunning.ShouldBeFalse();

                    timer.Start();

                    timer.IsRunning.ShouldBeTrue();
                }
            }

            [Fact]
            public void Should_Throw_If_Start_When_Already_Disposed()
            {
                var timer = new CountdownTimer();

                timer.Configure(Create<double>(), Create<double>());
                timer.Dispose();

                Should.Throw<ObjectDisposedException>(() =>
                {
                    timer.Start();
                })
                    .Message.ShouldBe("The countdown timer is already disposed.");
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

                    Should.NotThrow(() =>
                    {
                        timer.Stop();
                    });
                }
            }

            [Fact]
            public void Should_Not_Throw_When_Already_Stopped()
            {
                using (var timer = new CountdownTimer())
                {
                    timer.Configure(Create<double>(), Create<double>());

                    timer.Start();

                    timer.IsRunning.ShouldBeTrue();

                    timer.Stop();

                    timer.IsRunning.ShouldBeFalse();
                }
            }
        }

        public class Observables : CountdownTimerFixture
        {
            [Fact]
            public void Should_Notify_IsRunning()
            {
                using (var timer = new CountdownTimer())
                {
                    timer.Configure(10000, 1000);

                    var isRunning = false;

                    timer.WhenAnyValue(vm => vm.IsRunning)
                        .Subscribe(value =>
                        {
                            isRunning = value;
                        });

                    isRunning.ShouldBeFalse();

                    timer.Start();

                    isRunning.ShouldBeTrue();
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

                    scheduler.AdvanceByMilliseconds(totalMilliseconds * 2);

                    completed.ShouldBeTrue();
                }
            }
        }
    }
}






