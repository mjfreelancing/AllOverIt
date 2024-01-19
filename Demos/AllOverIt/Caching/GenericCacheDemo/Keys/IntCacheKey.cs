using AllOverIt.Caching;

namespace GenericCacheDemo.Keys
{
    internal sealed record IntCacheKey : GenericCacheKey<int>
    {
        public IntCacheKey(int value)
            : base(value)
        {
        }
    }
}