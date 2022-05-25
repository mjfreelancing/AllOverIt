using AllOverIt.Reflection;
using BenchmarkDotNet.Attributes;
using System;
using System.Reflection;

namespace CompiledReflectionBenchmarking
{
    internal sealed class DummyType
    {
        public int Value { get; set; }
    }

    [MemoryDiagnoser]
    public class CompiledReflection
    {
        // Reflection
        private static PropertyInfo dummyPropInfo = typeof(DummyType).GetProperty(nameof(DummyType.Value));

        // Compiled
        private static Func<DummyType, int> _compiledGet = ReflectionHelper.GetCompiledPropertyGet<DummyType, int>(nameof(DummyType.Value));
        private static Action<DummyType, int> _compiledSet = ReflectionHelper.GetCompiledPropertySet<DummyType, int>(nameof(DummyType.Value));

        [Params(100)]
        public int IterationCount;

        [Benchmark]
        public void ReflectionGet()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = dummyPropInfo.GetValue(instance);
            }
        }

        [Benchmark]
        public void ReflectionSet()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                dummyPropInfo.SetValue(instance, 20);
            }
        }

        [Benchmark]
        public void CompiledReflectionGet()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = _compiledGet.Invoke(instance);
            }
        }

        [Benchmark]
        public void CompiledReflectionSet()
        {
            var instance = GetDummyType();

            _compiledSet.Invoke(instance, 20);
        }

        private static DummyType GetDummyType()
        {
            return new DummyType
            {
                Value = 3
            };
        }
    }
}