using System;

namespace AllOverIt.Caching
{
    public abstract class GenericCacheKeyBase
    {
        public Type KeyType { get; init; }
        public object Key { get; init; }

        protected GenericCacheKeyBase(object key)
        {
            KeyType = key?.GetType();
            Key = key;
        }
    }
}