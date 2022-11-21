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
    public class BenchmarkTests
    {
        // Reflection
        private static readonly PropertyInfo DummyPropInfo = typeof(DummyType).GetProperty(nameof(DummyType.Value));

        // Compiled
        private static readonly Func<object, object> ObjectPropertyGetterInfo = PropertyHelper.CreatePropertyGetter(DummyPropInfo);
        private static readonly Func<DummyType, object> TypedPropertyGetterInfo = PropertyHelper.CreatePropertyGetter<DummyType>(DummyPropInfo);
        private static readonly Func<DummyType, object> TypedPropertyGetterName = PropertyHelper.CreatePropertyGetter<DummyType>(nameof(DummyType.Value));

        private static readonly Action<object, object> ObjectPropertySetterInfo = PropertyHelper.CreatePropertySetter(DummyPropInfo);
        private static readonly Action<DummyType, object> TypedPropertySetterInfo = PropertyHelper.CreatePropertySetter<DummyType>(DummyPropInfo);
        private static readonly Action<DummyType, object> TypedPropertySetterName = PropertyHelper.CreatePropertySetter<DummyType>(nameof(DummyType.Value));

        [Params(4)]
        public int IterationCount;

        [Benchmark]
        public void ReflectionGet()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = DummyPropInfo.GetValue(instance);
            }
        }

        [Benchmark]
        public void Object_PropertyGetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = ObjectPropertyGetterInfo.Invoke(instance);
            }
        }

        [Benchmark]
        public void Typed_PropertyGetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = TypedPropertyGetterInfo.Invoke(instance);
            }
        }

        [Benchmark]
        public void Typed_PropertyGetter_Name()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = TypedPropertyGetterName.Invoke(instance);
            }
        }

        [Benchmark]
        public void ReflectionSet()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                DummyPropInfo.SetValue(instance, 20);
            }
        }

        [Benchmark]
        public void Object_PropertySetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                ObjectPropertySetterInfo.Invoke(instance, 20);
            }
        }

        [Benchmark]
        public void Typed_PropertySetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                TypedPropertySetterInfo.Invoke(instance, 20);
            }
        }

        [Benchmark]
        public void Typed_PropertySetter_Name()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                TypedPropertySetterName.Invoke(instance, 20);
            }
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