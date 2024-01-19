using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using BenchmarkDotNet.Attributes;

namespace EvaluatorBenchmarking
{
    /*
    |              Method |         Mean |        Error |       StdDev |   Gen 0 | Allocated |
    |-------------------- |-------------:|-------------:|-------------:|--------:|----------:|
    |  ExplicitReferences |     21.01 ns |     0.404 ns |     0.378 ns |       - |         - |
    |       AllReferences | 10,195.89 ns |   152.591 ns |   135.268 ns |  1.4038 |   5,920 B |
    | ExplicitReferencing |  4,135.36 ns |    58.254 ns |    51.640 ns |  0.5951 |   2,520 B |
    |      AllReferencing | 79,282.98 ns | 1,496.936 ns | 1,470.191 ns | 10.6201 |  44,416 B |
    */

    [MemoryDiagnoser]
    public class LookupBenchmarks
    {
        private readonly IVariable[] _variables;
        private readonly VariableLookup _lookup;

        public LookupBenchmarks()
        {
            var compiler = new FormulaCompiler();
            var registry = new VariableRegistry();

            var a = registry.AddConstantVariable("a", 1);
            var bCompiled = compiler.Compile("a + 2", registry);
            var b = registry.AddDelegateVariable("b", bCompiled);

            var c = registry.AddLazyVariable("c", compiler.Compile("a + b", registry));
            var d = registry.AddLazyVariable("d", compiler.Compile("c", registry));
            var e = registry.AddLazyVariable("e", compiler.Compile("a+b+c", registry));
            var f = registry.AddDelegateVariable("f", compiler.Compile("b", registry));
            var g = registry.AddDelegateVariable("g", compiler.Compile("e", registry));

            _variables = [a, b, c, d, e, f, g];
            _lookup = new VariableLookup(registry);
        }

        [Benchmark]
        public void ExplicitReferences()
        {
            foreach (var variable in _variables)
            {
                _ = _lookup.GetReferencedVariables(variable, VariableLookupMode.Explicit);
            }
        }

        [Benchmark]
        public void AllReferences()
        {
            foreach (var variable in _variables)
            {
                _ = _lookup.GetReferencedVariables(variable, VariableLookupMode.All);
            }
        }

        [Benchmark]
        public void ExplicitReferencing()
        {
            foreach (var variable in _variables)
            {
                _ = _lookup.GetReferencingVariables(variable, VariableLookupMode.Explicit);
            }
        }

        [Benchmark]
        public void AllReferencing()
        {
            foreach (var variable in _variables)
            {
                _ = _lookup.GetReferencingVariables(variable, VariableLookupMode.All);
            }
        }
    }
}