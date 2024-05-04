using AllOverIt.Extensions;

namespace SelectAsParallelDemo
{
    internal class Program
    {
        static async Task Main(/*string[] args*/)
        {
            var itemCount = 25;
            var degreeOfParallelism = 4;
            var maxDelay = 1000;

            var count = 0;
            var rnd = new Random();

            Console.WriteLine($"Processing {itemCount} items with a degree of parallelism = {degreeOfParallelism}");
            Console.WriteLine();

            var items = Enumerable.Range(1, itemCount);

            var forItems = items.SelectAsParallelAsync(async (item, token) =>
            {
                Interlocked.Increment(ref count);

                await Task.Delay((int) Math.Floor(rnd.NextDouble() * maxDelay), token);

                return item;
            }, degreeOfParallelism);

            await foreach (var item in forItems)
            {
                var currentCount = Interlocked.Decrement(ref count) + 1;

                Console.WriteLine($"Finished item {item}, currently {currentCount} in parallel");
            }
        }
    }
}