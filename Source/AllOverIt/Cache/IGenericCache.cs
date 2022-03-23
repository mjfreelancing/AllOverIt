using System;

namespace AllOverIt.Cache
{
    public interface IGenericCache
    {
        TValue GetOrAdd<TValue>(GenericCacheKeyBase key, Func<GenericCacheKeyBase, object> valueResolver);
    }
}