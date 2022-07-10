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
        private static readonly PropertyInfo dummyPropInfo = typeof(DummyType).GetProperty(nameof(DummyType.Value));

        // Compiled
        private static readonly Func<object, object> _objectPropertyGetterInfo = PropertyHelper.CreatePropertyGetter(dummyPropInfo);
        private static readonly Func<DummyType, object> _typedPropertyGetterInfo = PropertyHelper.CreatePropertyGetter<DummyType>(dummyPropInfo);
        private static readonly Func<DummyType, object> _typedPropertyGetterName = PropertyHelper.CreatePropertyGetter<DummyType>(nameof(DummyType.Value));

        private static readonly Action<object, object> _objectPropertySetterInfo = PropertyHelper.CreatePropertySetter(dummyPropInfo);
        private static readonly Action<DummyType, object> _typedPropertySetterInfo = PropertyHelper.CreatePropertySetter<DummyType>(dummyPropInfo);
        private static readonly Action<DummyType, object> _typedPropertySetterName = PropertyHelper.CreatePropertySetter<DummyType>(nameof(DummyType.Value));

        [Params(4)]
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
        public void Object_PropertyGetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = _objectPropertyGetterInfo.Invoke(instance);
            }
        }

        [Benchmark]
        public void Typed_PropertyGetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = _typedPropertyGetterInfo.Invoke(instance);
            }
        }

        [Benchmark]
        public void Typed_PropertyGetter_Name()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _ = _typedPropertyGetterName.Invoke(instance);
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
        public void Object_PropertySetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _objectPropertySetterInfo.Invoke(instance, 20);
            }
        }

        [Benchmark]
        public void Typed_PropertySetter_Info()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _typedPropertySetterInfo.Invoke(instance, 20);
            }
        }

        [Benchmark]
        public void Typed_PropertySetter_Name()
        {
            var instance = GetDummyType();

            for (var i = 0; i < IterationCount; i++)
            {
                _typedPropertySetterName.Invoke(instance, 20);
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