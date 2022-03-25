using AllOverIt.Caching;
using AllOverIt.Reflection;

namespace GenericCacheDemo.Keys
{
    internal sealed class PropKeyByBinding : GenericCacheKey<BindingOptions>
    {
        public PropKeyByBinding(BindingOptions bindingOptions)
            : base(bindingOptions)
        {
        }
    }
}