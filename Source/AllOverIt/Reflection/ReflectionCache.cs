using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Reflection
{
    public class ReflectionCache : ReflectionCache2
    {
        public static ReflectionCache Instance = new ReflectionCache();

        public Func<MethodBase, bool> GetBindingPredicate(BindingOptions bindingOptions,
           Func<ReflectionCacheKeyBase, Func<MethodBase, bool>> valueResolver)
        {
            return GetOrAdd<Func<MethodBase, bool>>(new ReflectionCacheKey<BindingOptions>(bindingOptions), valueResolver);
        }

        public IEnumerable<PropertyInfo> GetPropertyInfo(Type type, BindingOptions bindingOptions, bool declaredOnly,
            Func<ReflectionCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            return GetOrAdd<IEnumerable<PropertyInfo>>(new ReflectionCacheKey<Type, BindingOptions, bool>(type, bindingOptions, declaredOnly), valueResolver);
        }

        public IEnumerable<PropertyInfo> GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly,
            Func<ReflectionCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            return GetOrAdd<IEnumerable<PropertyInfo>>(new ReflectionCacheKey<TypeInfo, bool>(typeInfo, declaredOnly), valueResolver);
        }

        public PropertyInfo GetPropertyInfo(TypeInfo typeInfo, string propertyName,
            Func<ReflectionCacheKeyBase, PropertyInfo> valueResolver)
        {
            return GetOrAdd<PropertyInfo>(new ReflectionCacheKey<TypeInfo, string>(typeInfo, propertyName), valueResolver);
        }
    }





    public class ReflectionCache0<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _cache = new();

        protected TValue GetOrAdd(TKey key, Func<TKey, TValue> valueResolver)
        {
            return _cache.GetOrAdd(key, valueResolver);
        }
    }

    //internal class BindingPredicateCache : ReflectionCache0<BindingOptions, Func<MethodBase, bool>>
    //{
    //    public static BindingPredicateCache Instance = new();

    //    public Func<MethodBase, bool> GetBindingPredicate(BindingOptions bindingOptions,
    //        Func<BindingOptions, Func<MethodBase, bool>> valueResolver)
    //    {
    //        return GetOrAdd(bindingOptions, valueResolver);
    //    }
    //}

    //internal class PropertyInfoByTypeBindingDeclaredCache : ReflectionCache0<(Type, BindingOptions, bool), IEnumerable<PropertyInfo>>
    //{
    //    public static PropertyInfoByTypeBindingDeclaredCache Instance = new();

    //    public IEnumerable<PropertyInfo> GetPropertyInfo(Type type, BindingOptions bindingOptions, bool declaredOnly,
    //        Func<(Type, BindingOptions, bool), IEnumerable<PropertyInfo>> valueResolver)
    //    {
    //        return GetOrAdd((type, bindingOptions, declaredOnly), valueResolver);
    //    }
    //}

    //internal class PropertyInfoByTypeInfoDeclaredCache : ReflectionCache0<(TypeInfo, bool), IEnumerable<PropertyInfo>>
    //{
    //    public static PropertyInfoByTypeInfoDeclaredCache Instance = new();

    //    public IEnumerable<PropertyInfo> GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly,
    //        Func<(TypeInfo, bool), IEnumerable<PropertyInfo>> valueResolver)
    //    {
    //        return GetOrAdd((typeInfo, declaredOnly), valueResolver);
    //    }
    //}

    //internal class PropertyInfoByTypeInfoPropertyNameCache : ReflectionCache0<(TypeInfo, string), PropertyInfo>
    //{
    //    public static PropertyInfoByTypeInfoPropertyNameCache Instance = new();

    //    public PropertyInfo GetPropertyInfo(TypeInfo typeInfo, string propertyName,
    //        Func<(TypeInfo, string), PropertyInfo> valueResolver)
    //    {
    //        return GetOrAdd((typeInfo, propertyName), valueResolver);
    //    }
    //}







    public abstract class ReflectionCacheKeyBase
    {
        public Type KeyType { get; init; }
        public object Key { get; init; }

        protected ReflectionCacheKeyBase(object key)
        {
            KeyType = key?.GetType();
            Key = key;
        }
    }

    public class ReflectionCacheKey<TKey1> : ReflectionCacheKeyBase
    {
        public TKey1 Key1 { get; init; }

        public ReflectionCacheKey(TKey1 key1)
            : base(key1)
        {
            Key1 = key1;
        }

        public void Deconstruct(out TKey1 key1)
        {
            key1 = Key1;
        }
    }

    public class ReflectionCacheKey<TKey1, TKey2> : ReflectionCacheKeyBase
    {
        public TKey1 Key1 { get; init; }
        public TKey2 Key2 { get; init; }

        public ReflectionCacheKey(TKey1 key1, TKey2 key2)
            : base((key1, key2))
        {
            Key1 = key1;
            Key2 = key2;
        }

        internal void Deconstruct(out TKey1 key1, out TKey2 key2)
        {
            key1 = Key1;
            key2 = Key2;
        }
    }

    public class ReflectionCacheKey<TKey1, TKey2, TKey3> : ReflectionCacheKeyBase
    {
        public TKey1 Key1 { get; init; }
        public TKey2 Key2 { get; init; }
        public TKey3 Key3 { get; init; }

        public ReflectionCacheKey(TKey1 key1, TKey2 key2, TKey3 key3)
            : base((key1, key2, key3))
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
        }

        internal void Deconstruct(out TKey1 key1, out TKey2 key2, out TKey3 key3)
        {
            key1 = Key1;
            key2 = Key2;
            key3 = Key3;
        }
    }


    public class ReflectionCacheKeyComparer : IEqualityComparer<ReflectionCacheKeyBase>
    {
        public static ReflectionCacheKeyComparer Instance = new();

        public bool Equals(ReflectionCacheKeyBase x, ReflectionCacheKeyBase y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (x.KeyType != y.KeyType)
            {
                return false;
            }

            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(ReflectionCacheKeyBase obj)
        {
            return obj.Key.GetHashCode();
        }
    }



    public class ReflectionCache2
    {
        private readonly ConcurrentDictionary<ReflectionCacheKeyBase, object> _cache
            = new ConcurrentDictionary<ReflectionCacheKeyBase, object>(ReflectionCacheKeyComparer.Instance);

        protected TValue GetOrAdd<TValue>(ReflectionCacheKeyBase key, Func<ReflectionCacheKeyBase, object> valueResolver)
        {
            return (TValue) _cache.GetOrAdd(key, valueResolver);
        }
    }


    //public class ObjectTestCache : ReflectionCache2
    //{
    //    public string GetOrAddString(string key, Func<ReflectionCacheKey, string> resolver)
    //    {
    //        var cacheKey = new ReflectionCacheKey(key);
    //        return GetOrAdd<string>(cacheKey, resolver);
    //    }

    //    public int GetOrAddInt(int key, Func<ReflectionCacheKey, int> resolver)
    //    {
    //        var cacheKey = new ReflectionCacheKey(key);

    //        return GetOrAdd<int>(cacheKey, key => 
    //        {
    //            return resolver.Invoke(key);        // can't be a method group; needs to box the value
    //        });
    //    }

    //    public ObjectTest GetOrAddObjectTest(int prop1, string prop2, Type prop3, Func<ReflectionCacheKey, ObjectTest> resolver)
    //    {
    //        var cacheKey = new ReflectionCacheKey((prop1, prop2, prop3));
    //        return GetOrAdd<ObjectTest>(cacheKey, resolver);
    //    }
    //}

    //public class ObjectTest
    //{
    //    public int Prop1 { get; set; }
    //    public string Prop2 { get; set; }
    //    public Type Prop3 { get; set; }
    //}
}