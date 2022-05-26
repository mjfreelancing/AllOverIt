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
        private static Func<DummyType, int> _compiledGetTyped = ReflectionHelper.GetCompiledPropertyGetter<DummyType, int>(nameof(DummyType.Value));
        //private static Func<object, object> _compiledGetObject = ReflectionHelper.GetCompiledPropertyGetter(typeof(DummyType), nameof(DummyType.Value));
        private static Action<DummyType, int> _compiledSet = ReflectionHelper.GetCompiledPropertySetter<DummyType, int>(nameof(DummyType.Value));

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
        public void CompiledReflectionGetTyped()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = _compiledGetTyped.Invoke(instance);
            }
        }

        //[Benchmark]
        //public void CompiledReflectionGetObject()
        //{
        //    var instance = GetDummyType();

        //    for (var i = 0; i < IterationCount; i++)
        //    {
        //        _ = _compiledGetObject.Invoke(instance);
        //    }
        //}

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