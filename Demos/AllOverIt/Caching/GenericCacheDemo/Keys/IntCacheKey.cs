using AllOverIt.Caching;

namespace GenericCacheDemo.Keys
{
    internal sealed class IntCacheKey : GenericCacheKey<int>
    {
        public IntCacheKey(int value)
            : base(value)
        {
        }
    }
}