namespace AllOverIt.Reflection.Extensions
{
    // A marker interface to ensure reflection keys are unique, such as differentiating PropertyInfo
    // from MethodInfo lookups when the input criteria are identical.
    internal interface ReflectionCacheKey<TType>
    {
    }
}