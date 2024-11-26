using AllOverIt.Assertion;
using BenchmarkDotNet.Attributes;

/*
    |          Method | IterationCount |          Mean |       Error |     StdDev |  Gen 0 | Allocated |
    |---------------- |--------------- |--------------:|------------:|-----------:|-------:|----------:|
    |        If_False |              4 |      7.596 ns |   0.4954 ns |   1.461 ns |      - |         - |
    |         If_True |              4 | 17,949.133 ns | 340.1112 ns | 349.269 ns | 0.2747 |   1,216 B |
    | ThrowWhen_False |              4 |    235.707 ns |   4.6492 ns |   4.975 ns | 0.0994 |     416 B |
    |  ThrowWhen_True |              4 | 24,839.367 ns | 146.3178 ns | 122.182 ns | 0.8240 |   3,456 B |
 */

namespace ThrowWhenBenchmarking
{
    [MemoryDiagnoser]
    public class ThrowWhenBenchmark
    {
        [Params(4)]
        public int IterationCount;

        [Benchmark]
        public void If_False()
        {
            for (var i = 0; i < IterationCount; i++)
            {
                var condition = i > IterationCount;

                try
                {
                    if (condition)
                    {
                        throw new Exception($"The current value is {i}, condition = {condition}");
                    }
                }
                catch
                {
                }
            }
        }

        [Benchmark]
        public void If_True()
        {
            for (var i = 0; i < IterationCount; i++)
            {
                var condition = i < IterationCount;

                try
                {
                    if (condition)
                    {
                        throw new Exception($"The current value is {i}, condition = {condition}");
                    }
                }
                catch
                {
                }
            }
        }

        [Benchmark]
        public void ThrowWhen_False()
        {
            for (var i = 0; i < IterationCount; i++)
            {
                var condition = i > IterationCount;

                try
                {
                    Throw<Exception>.When(condition, $"The current value is {i}, condition = {condition}");
                }
                catch
                {
                }
            }
        }

        [Benchmark]
        public void ThrowWhen_True()
        {
            for (var i = 0; i < IterationCount; i++)
            {
                var condition = i < IterationCount;

                try
                {
                    Throw<Exception>.When(condition, $"The current value is {i}, condition = {condition}");
                }
                catch
                {
                }
            }
        }
    }
}