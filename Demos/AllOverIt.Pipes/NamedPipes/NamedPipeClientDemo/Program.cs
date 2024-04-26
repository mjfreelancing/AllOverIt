using NamedPipeTypes;

namespace NamedPipeClientDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var pipeName = "named_pipe_test_server";

            await PipeClient.RunAsync(pipeName, Constants.UseCustomReaderWriter).ConfigureAwait(false);

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}