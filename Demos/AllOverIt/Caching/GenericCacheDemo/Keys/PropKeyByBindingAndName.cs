using AllOverIt.Caching;
using AllOverIt.Reflection;

namespace GenericCacheDemo.Keys
{
    internal sealed class PropKeyByBindingAndName : GenericCacheKey<BindingOptions, string>
    {
        public PropKeyByBindingAndName(BindingOptions bindingOptions, string name)
            : base(bindingOptions, name)
        {
        }
    }
}