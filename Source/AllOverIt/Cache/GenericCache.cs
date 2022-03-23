using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AllOverIt.Cache
{
    public class GenericCache
    {
        private class GenericCacheKeyComparer : IEqualityComparer<GenericCacheKeyBase>
        {
            public static GenericCacheKeyComparer Instance = new();

            public bool Equals(GenericCacheKeyBase x, GenericCacheKeyBase y)
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

            public int GetHashCode(GenericCacheKeyBase obj)
            {
                return obj.Key.GetHashCode();
            }
        }

        private readonly ConcurrentDictionary<GenericCacheKeyBase, object> _cache = new(GenericCacheKeyComparer.Instance);

        public static GenericCache Default = new();

        public TValue GetOrAdd<TValue>(GenericCacheKeyBase key, Func<GenericCacheKeyBase, object> valueResolver)
        {
            return (TValue) _cache.GetOrAdd(key, valueResolver);
        }
    }
}