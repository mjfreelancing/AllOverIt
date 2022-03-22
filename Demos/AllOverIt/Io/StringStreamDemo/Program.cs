using System;
using System.Text;
using System.Threading.Tasks;
using AllOverIt.Io;

namespace StringStreamDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Construct the streamer with an initial string
            var stream = new TextStreamer($"{DateTime.Now:dd-MMM-yyyy}");

            // Write the remaining content using the underlying writer
            var writer = stream.GetWriter();

            writer.WriteLine();

            writer.Write("Bool=");
            writer.Write(true);
            writer.WriteLine();

            writer.WriteLine("Value=100");

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
