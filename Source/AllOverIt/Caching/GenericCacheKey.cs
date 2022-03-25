using AllOverIt.Helpers;

namespace AllOverIt.Caching
{
    public class GenericCacheKey<TKey1> : GenericCacheKeyBase
    {
        public TKey1 Key1 { get; init; }

        public GenericCacheKey(TKey1 key1)
            : base(key1)
        {
            Key1 = key1;
        }

        public void Deconstruct(out TKey1 key1)
        {
            key1 = Key1;
        }
    }

    public class GenericCacheKey<TKey1, TKey2> : GenericCacheKeyBase
    {
        public TKey1 Key1 { get; init; }
        public TKey2 Key2 { get; init; }

        public GenericCacheKey(TKey1 key1, TKey2 key2)
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

    public class GenericCacheKey<TKey1, TKey2, TKey3> : GenericCacheKeyBase
    {
        public TKey1 Key1 { get; init; }
        public TKey2 Key2 { get; init; }
        public TKey3 Key3 { get; init; }

        public GenericCacheKey(TKey1 key1, TKey2 key2, TKey3 key3)
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

    public class GenericCacheKey<TKey1, TKey2, TKey3, TKey4> : GenericCacheKeyBase
    {
        public TKey1 Key1 { get; init; }
        public TKey2 Key2 { get; init; }
        public TKey3 Key3 { get; init; }
        public TKey4 Key4 { get; init; }

        public GenericCacheKey(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4)
            : base((key1, key2, key3, key4))
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
            Key4 = key4;
        }

        internal void Deconstruct(out TKey1 key1, out TKey2 key2, out TKey3 key3, out TKey4 key4)
        {
            key1 = Key1;
            key2 = Key2;
            key3 = Key3;
            key4 = Key4;
        }
    }
}