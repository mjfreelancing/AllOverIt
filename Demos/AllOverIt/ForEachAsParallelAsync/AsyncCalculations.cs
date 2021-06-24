// unset

using AllOverIt.Extensions;
using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForEachAsParallelAsync
{
    public class AsyncCalculations
    {
        [Benchmark]
        public async Task MultiplySequentially()
        {
            foreach (var input in GetInputs())
            {
                await Multiply(input);
            }
        }

        [Benchmark]
        public async Task MultiplyAsParallel16()
        {
            await GetInputs().ForEachAsParallelAsync(Multiply, 16);
        }

        [Benchmark]
        public async Task MultiplyAsTask16()
        {
            await GetInputs().ForEachAsTaskAsync(Multiply, 16);
        }

        [Benchmark]
        public async Task MultiplyAsParallel100()
        {
            await GetInputs().ForEachAsParallelAsync(Multiply, 100);
        }

        [Benchmark]
        public async Task MultiplyAsTask100()
        {
            await GetInputs().ForEachAsTaskAsync(Multiply, 100);
        }

        private IEnumerable<(int input1, int input2)> GetInputs()
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