namespace AllOverIt.Caching
{
    public abstract class GenericCacheKeyBase
    {
        public object Key { get; init; }

        protected GenericCacheKeyBase(object key)
        {
            Key = key;
        }
    }
}