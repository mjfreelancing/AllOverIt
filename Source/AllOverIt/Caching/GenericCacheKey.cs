namespace AllOverIt.Caching
{
    /// <summary>A <see cref="GenericCache"/> key based on one strongly-typed key type.</summary>
    /// <typeparam name="TKey1">The key type.</typeparam>
    public class GenericCacheKey<TKey1> : GenericCacheKeyBase
    {
        /// <summary>The value of the key.</summary>
        public TKey1 Key1 { get; init; }

        /// <summary>Constructor.</summary>
        /// <param name="key1">The key value.</param>
        public GenericCacheKey(TKey1 key1)
            : base(key1)
        {
            Key1 = key1;
        }

        /// <summary>Deconstruct operator.</summary>
        /// <param name="key1">Will be set to the value of <see cref="Key1"/>.</param>
        public void Deconstruct(out TKey1 key1)
        {
            // Null values are allowed
            key1 = Key1;
        }
    }

    /// <summary>A <see cref="GenericCache"/> key based on one strongly-typed key type.</summary>
    /// <typeparam name="TKey1">The first key type.</typeparam>
    /// <typeparam name="TKey2">The second key type.</typeparam>
    public class GenericCacheKey<TKey1, TKey2> : GenericCacheKeyBase
    {
        /// <summary>The value of the first key.</summary>
        public TKey1 Key1 { get; init; }

        /// <summary>The value of the second key.</summary>
        public TKey2 Key2 { get; init; }

        /// <summary>Constructor.</summary>
        /// <param name="key1">The first key value.</param>
        /// <param name="key2">The second key value.</param>
        public GenericCacheKey(TKey1 key1, TKey2 key2)
            : base((key1, key2))
        {
            // Null values are allowed
            Key1 = key1;
            Key2 = key2;
        }

        /// <summary>Deconstruct operator.</summary>
        /// <param name="key1">Will be set to the value of <see cref="Key1"/>.</param>
        /// <param name="key2">Will be set to the value of <see cref="Key2"/>.</param>
        public void Deconstruct(out TKey1 key1, out TKey2 key2)
        {
            key1 = Key1;
            key2 = Key2;
        }
    }

    /// <summary>A <see cref="GenericCache"/> key based on one strongly-typed key type.</summary>
    /// <typeparam name="TKey1">The first key type.</typeparam>
    /// <typeparam name="TKey2">The second key type.</typeparam>
    /// <typeparam name="TKey3">The third key type.</typeparam>
    public class GenericCacheKey<TKey1, TKey2, TKey3> : GenericCacheKeyBase
    {
        /// <summary>The value of the first key.</summary>
        public TKey1 Key1 { get; init; }

        /// <summary>The value of the second key.</summary>
        public TKey2 Key2 { get; init; }

        /// <summary>The value of the third key.</summary>
        public TKey3 Key3 { get; init; }

        /// <summary>Constructor.</summary>
        /// <param name="key1">The first key value.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key3">The third key value.</param>
        public GenericCacheKey(TKey1 key1, TKey2 key2, TKey3 key3)
            : base((key1, key2, key3))
        {
            // Null values are allowed
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
        }

        /// <summary>Deconstruct operator.</summary>
        /// <param name="key1">Will be set to the value of <see cref="Key1"/>.</param>
        /// <param name="key2">Will be set to the value of <see cref="Key2"/>.</param>
        /// <param name="key3">Will be set to the value of <see cref="Key3"/>.</param>
        public void Deconstruct(out TKey1 key1, out TKey2 key2, out TKey3 key3)
        {
            key1 = Key1;
            key2 = Key2;
            key3 = Key3;
        }
    }

    /// <summary>A <see cref="GenericCache"/> key based on one strongly-typed key type.</summary>
    /// <typeparam name="TKey1">The first key type.</typeparam>
    /// <typeparam name="TKey2">The second key type.</typeparam>
    /// <typeparam name="TKey3">The third key type.</typeparam>
    /// <typeparam name="TKey4">The fourth key type.</typeparam>
    public class GenericCacheKey<TKey1, TKey2, TKey3, TKey4> : GenericCacheKeyBase
    {
        /// <summary>The value of the first key.</summary>
        public TKey1 Key1 { get; init; }

        /// <summary>The value of the second key.</summary>
        public TKey2 Key2 { get; init; }

        /// <summary>The value of the third key.</summary>
        public TKey3 Key3 { get; init; }

        /// <summary>The value of the fourth key.</summary>
        public TKey4 Key4 { get; init; }

        /// <summary>Constructor.</summary>
        /// <param name="key1">The first key value.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key3">The third key value.</param>
        /// <param name="key4">The fourth key value.</param>
        public GenericCacheKey(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4)
            : base((key1, key2, key3, key4))
        {
            // Null values are allowed
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
            Key4 = key4;
        }

        /// <summary>Deconstruct operator.</summary>
        /// <param name="key1">Will be set to the value of <see cref="Key1"/>.</param>
        /// <param name="key2">Will be set to the value of <see cref="Key2"/>.</param>
        /// <param name="key3">Will be set to the value of <see cref="Key3"/>.</param>
        /// <param name="key4">Will be set to the value of <see cref="Key4"/>.</param>
        public void Deconstruct(out TKey1 key1, out TKey2 key2, out TKey3 key3, out TKey4 key4)
        {
            key1 = Key1;
            key2 = Key2;
            key3 = Key3;
            key4 = Key4;
        }
    }
}