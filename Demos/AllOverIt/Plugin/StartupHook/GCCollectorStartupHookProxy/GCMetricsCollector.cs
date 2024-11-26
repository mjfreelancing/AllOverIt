#define USE_ALLOVERIT


#if USE_ALLOVERIT
#endif

using AllOverIt.Async;

namespace GCCollectorStartupHookProxy
{
    public sealed class GCMetricsCollector
    {
        public GCMetricsCollector()
        {
#if USE_ALLOVERIT
            // Wouldn't normally do it this way - just demonstrating the packages can be loaded
            RepeatingTask.StartAsync(() =>
            {
                OutputGCMetrics();

                return Task.CompletedTask;
            },
            new RepeatingTaskOptions
            {
                RepeatDelay = TimeSpan.FromSeconds(3)
            },
            CancellationToken.None);
#else
            new Thread(PollGCMetrics)
            {
                IsBackground = true,
                Name = "GCMetricsPoller"
            }.Start();
#endif
        }

#if !USE_ALLOVERIT
        private static void PollGCMetrics()
        {
            while (true)
            {
                OutputGCMetrics();

                Thread.Sleep(1000);
            }
        }
#endif

        private static void OutputGCMetrics()
        {
            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);

            Console.WriteLine($"GC.Gen0 = {gen0}");
            Console.WriteLine($"GC.Gen1 = {gen1}");
            Console.WriteLine($"GC.Gen2 = {gen2}");
        }
    }
}