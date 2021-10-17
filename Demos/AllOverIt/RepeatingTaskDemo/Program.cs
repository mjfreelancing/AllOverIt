using System;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Tasks;

namespace RepeatingTaskDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const int initialDelay = 5000;
            const int repeatDelay = 1000;
            var tokenSource = new CancellationTokenSource();

            Console.WriteLine($"Waiting for {initialDelay}ms, will then update the time every {repeatDelay}ms...");

            // start a repeating task
            var repeatingTask = RepeatingTask.Start(() =>
            {
                Console.WriteLine($"Current time: {DateTime.Now:T}");
            }, tokenSource.Token, repeatDelay, initialDelay);

            // wait for the user to cancel
            Console.WriteLine("(Press any key to abort)");
            Console.ReadKey();

            tokenSource.Cancel();

            await repeatingTask;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
