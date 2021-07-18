using AllOverIt.Process;
using System;
using System.Threading.Tasks;

namespace ExecuteProcessAndWait
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Close notepad within 10 seconds so this console application can complete");

            try
            {
                var output = await Process.ExecuteAndWaitAsync(@"C:\Windows", "notepad.exe", string.Empty, 10000);

                Console.WriteLine($"Exit Code = {output.ExitCode}");
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
