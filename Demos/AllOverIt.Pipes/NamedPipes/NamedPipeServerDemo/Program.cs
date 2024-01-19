using NamedPipeTypes;
using System;
using System.Threading.Tasks;

namespace NamedPipeServerDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var pipeName = "named_pipe_test_server";

            await PipeServer.RunAsync(pipeName, Constants.UseCustomReaderWriter).ConfigureAwait(false);

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}