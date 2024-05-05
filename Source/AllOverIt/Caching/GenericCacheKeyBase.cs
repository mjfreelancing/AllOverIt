namespace AllOverIt.Caching
{
    /// <summary>The base key type for all keys contained in a concrete <see cref="IGenericCache"/>, such as <see cref="GenericCache"/>.</summary>
    public abstract record GenericCacheKeyBase
    {
        /// <summary>The key used to index an associated value. A <see langword="null"/> value is allowed.</summary>
        public object? Key { get; init; }

        /// <summary>Constructor.</summary>
        /// <param name="key">The key to be stored in the cache. A <see langword="null"/> value is allowed.</param>
        protected GenericCacheKeyBase(object? key)
        {
            // Null values are allowed
            Key = key;
        }
    }
}