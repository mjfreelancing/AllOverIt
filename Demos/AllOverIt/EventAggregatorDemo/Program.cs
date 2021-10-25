using AllOverIt.Events;
using AllOverIt.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventAggregatorDemo
{
    internal sealed class StringMessage
    {
        public string  Value { get; set; }
    }

    internal sealed class LongMessage
    {
        public long Value { get; set; }
    }


    class Program
    {
        static async Task Main(string[] args)
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
            }, cts.Token, 1000);

            var task2 = RepeatingTask.Start(async () =>
            {
                var message = new LongMessage
                {
                    Value = DateTime.Now.Ticks
                };

                await aggregator.PublishAsync(message);
            }, cts.Token, 1000);

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
