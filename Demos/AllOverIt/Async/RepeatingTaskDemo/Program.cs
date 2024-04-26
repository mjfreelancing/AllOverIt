using AllOverIt.Async;

namespace RepeatingTaskDemo
{
    class Program
    {
        static async Task Main()
        {
            var options = new RepeatingTaskOptions
            {
                InitialDelay = TimeSpan.FromSeconds(2),
                RepeatDelay = TimeSpan.FromSeconds(5),
            };

            using var tokenSource = new CancellationTokenSource();

            Console.WriteLine($"Current time: {DateTime.Now:T}");
            Console.WriteLine($"Waiting for {options.InitialDelay} seconds, will then update the time every {options.RepeatDelay} seconds...");

            // start a repeating task
            var repeatingTask = RepeatingTask.StartAsync(() =>
            {
                Console.WriteLine($"Current time: {DateTime.Now:T}");
            }, options, tokenSource.Token);

            // wait for the user to cancel
            Console.WriteLine();
            Console.WriteLine("(Press any key to abort)");
            Console.WriteLine();

            Console.ReadKey();

            tokenSource.Cancel();

            await repeatingTask;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
