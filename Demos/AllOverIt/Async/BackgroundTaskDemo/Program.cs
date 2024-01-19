using AllOverIt.Async;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTaskDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var cts = new CancellationTokenSource();

            Console.WriteLine("Starting...");

            var task1 = new BackgroundTask(async cancellationToken =>
            {
                await Task.Delay(2500, cancellationToken);
                Console.WriteLine("Task 1 Completed");
            }, cts.Token);


            var task2 = new BackgroundTask<long>(async cancellationToken =>
            {
                await Task.Delay(2000, cancellationToken);
                Console.WriteLine("Task 2 Completed");

                return DateTime.Now.Ticks;
            }, TaskCreationOptions.LongRunning, TaskScheduler.Default, cts.Token);


            var task3 = new BackgroundTask(_ => throw new Exception("Task 3 Error !!!"), TaskCreationOptions.LongRunning, TaskScheduler.Default,
                edi =>
                {
                    Console.WriteLine($"Caught an exception: {edi.SourceException.Message}");
                    return true;        // handled
                },
                cts.Token);


            var task4 = new BackgroundTask(_ => throw new Exception("Task 4 Error !!!"), edi =>
            {
                Console.WriteLine($"Caught an exception: {edi.SourceException.Message}");
                return true;        // handled
            }, cts.Token);

            // To test cancelling the tasks
            //await Task.Delay(100);
            //cts.Cancel();

            // Can await individually - it uses GetAwaiter()
            // await task1;
            // ...and others

            // Or like this - it uses an implicit Task operator
            await Task.WhenAll(task1, task2, task3, task4);

            // .Result is OK after awaiting the task
            var t2Result = ((Task<long>) task2).Result;

            Console.WriteLine();
            Console.WriteLine($"Task2 returned a value of {t2Result}");
            Console.WriteLine();

            Console.WriteLine("All tasks have completed.");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}