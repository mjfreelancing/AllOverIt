using AllOverIt.Async;

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
                exception =>
                {
                    Console.WriteLine($"Caught an exception: {exception.Message}");
                    return true;        // handled
                },
                cts.Token);


            var task4 = new BackgroundTask(_ => throw new Exception("Task 4 Error !!!"), exception =>
            {
                Console.WriteLine($"Caught an exception: {exception.Message}");
                return true;        // handled
            }, cts.Token);


            var task5 = new BackgroundTask(async cancellationToken =>
            {
                Console.WriteLine("Task 5 Waiting...");

                await Task.Delay(5000, cancellationToken);

                Console.WriteLine("Task 5 Completed");      // Will not be logged
            }, cts.Token);

            var task6 = new BackgroundTask(Task6Impl, exception =>
            {
                Console.WriteLine("Task 6, last chance to handle an exception. Returning false.");

                return false;
            }, cts.Token);

            await task5.DisposeAsync();

            Console.WriteLine("Task 5 Disposed.");

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
            Console.WriteLine($"Task 2 returned a value of {t2Result}");
            Console.WriteLine();

            try
            {
                // task6 re-throws an exception that will be propagated when awaited
                await task6;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task 6 re-threw an exception, raised when awaited: {ex.Message}");
                Console.WriteLine();
            }

            Console.WriteLine("All tasks have completed.");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static async Task Task6Impl(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(500, cancellationToken);

                throw new Exception("Task 6 threw an error");
            }
            catch (Exception exception)
            {
                throw new Exception("Re-throwing an exception in Task 6", exception);
            }
        }
    }
}