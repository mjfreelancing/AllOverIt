using Microsoft.Reactive.Testing;
using System;

namespace AllOverIt.ReactiveUI.Tests
{
    internal static class TestSchedulerExtensions
    {
        public static void AdvanceByMilliseconds(this TestScheduler scheduler, double milliseconds)
        {
            var advanceBy = TimeSpan.FromMilliseconds(milliseconds).Ticks;
            scheduler.AdvanceBy(advanceBy);
        }
    }
}