using System;
using System.Text;
using System.Threading.Tasks;
using AllOverIt.Io;

namespace StringStreamDemo
{
    internal class Program
    {
        static async Task Main()
        {
            // Construct the streamer with an initial string
            var stream = new TextStreamer($"{DateTime.Now:dd-MMM-yyyy}");

            // Write the remaining content using the underlying writer
            var writer = stream.GetWriter();

            await writer.WriteLineAsync();

            await writer.WriteAsync("Bool=");
            writer.Write(true);
            await writer.WriteLineAsync();

            await writer.WriteLineAsync("Value=100");

            var sb = new StringBuilder();
            sb.Append(1);
            sb.Append(2);
            sb.Append(3);

            await writer.WriteLineAsync(sb);

            var output = stream.ToString();

            Console.WriteLine("Streamed:");
            Console.WriteLine($"{output}");
            Console.WriteLine();

            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
