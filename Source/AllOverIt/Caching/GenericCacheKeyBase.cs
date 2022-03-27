namespace AllOverIt.Caching
{
    /// <summary>The base key type for all keys contained in a concrete <see cref="IGenericCache"/>, such as <see cref="GenericCache"/>.</summary>
    public abstract class GenericCacheKeyBase
    {
        /// <summary>The key used to index an associated value.</summary>
        public object Key { get; init; }

        /// <summary>Constructor.</summary>
        /// <param name="key">The key to be stored in the cache.</param>
        protected GenericCacheKeyBase(object key)
        {
            // Null values are allowed
            Key = key;
        }
    }
}