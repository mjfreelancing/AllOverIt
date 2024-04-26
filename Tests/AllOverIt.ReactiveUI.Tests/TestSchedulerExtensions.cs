using Microsoft.Reactive.Testing;

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