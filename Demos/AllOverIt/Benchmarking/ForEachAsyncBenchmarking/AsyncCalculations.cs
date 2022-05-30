using AllOverIt.Extensions;
using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForEachAsyncBenchmarking
{
    [MemoryDiagnoser]
    public class AsyncCalculations
    {
        private static readonly IEnumerable<(int input1, int input2)> Inputs = GetInputs();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Benchmark does not support static methods")]
        [Benchmark]
        public async Task MultiplySequentially()
        {
            foreach (var input in Inputs)
            {
                await Multiply(input);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Benchmark does not support static methods")]
        [Benchmark]
        public async Task MultiplyAsParallel16()
        {
            await Inputs.ForEachAsParallelAsync(Multiply, 16);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Benchmark does not support static methods")]
        [Benchmark]
        public async Task MultiplyAsTask16()
        {
            await Inputs.ForEachAsTaskAsync(Multiply, 16);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Benchmark does not support static methods")]
        [Benchmark]
        public async Task MultiplyAsParallel100()
        {
            await Inputs.ForEachAsParallelAsync(Multiply, 100);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Benchmark does not support static methods")]
        [Benchmark]
        public async Task MultiplyAsTask100()
        {
            await Inputs.ForEachAsTaskAsync(Multiply, 100);
        }

        private static IEnumerable<(int input1, int input2)> GetInputs()
        {
            return
                from input1 in Enumerable.Range(1, 10)
                from input2 in Enumerable.Range(1, 10)
                select(input1, input2);
        }
        
        private static Task Multiply((int input1, int input2) input)
        {
            _ = input.input1 * input.input2;
            return Task.Delay(1);
        }
    }
}