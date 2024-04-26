using AllOverIt.Async;
using AllOverIt.Events;

namespace EventAggregatorDemo
{
    class Program
    {
        static async Task Main()
        {
            var aggregator = new EventAggregator();

            aggregator.Subscribe<StringMessage>(StringMessageHandler);
            aggregator.Subscribe<LongMessage>(IntMessageHandler);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            Console.WriteLine("Messages will be published for 5 seconds...");
            Console.WriteLine();

            var task1 = RepeatingTask.Start(() =>
            {
                var message = new StringMessage
                {
                    Value = $"{DateTime.Now}"
                };

                aggregator.Publish(message);
            }, 1000, cts.Token);

            var task2 = RepeatingTask.Start(async () =>
            {
                var message = new LongMessage
                {
                    Value = DateTime.Now.Ticks
                };

                await aggregator.PublishAsync(message);
            }, 1000, cts.Token);

            try
            {
                // wait for the cancellation token to expire
                await Task.WhenAll(task1, task2);
            }
            catch (Exception)
            {
                // the cancellation token has expired
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void StringMessageHandler(StringMessage message)
        {
            Console.WriteLine($"String: {message.Value}");
        }

        private static Task IntMessageHandler(LongMessage message)
        {
            Console.WriteLine($"Int: {message.Value}");
            return Task.CompletedTask;
        }
    }
}
