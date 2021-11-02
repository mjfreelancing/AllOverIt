using AllOverIt.Assertion;
using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    public static class ValidationContextExtensions
    {
        // Can be called multiple times with different keys and associated data,
        // or just call it once with a complex object and use the default key.
        public static void SetContextData<TType, TContext>(this ValidationContext<TType> context, TContext data, string key = "data")
        {
            _ = context.WhenNotNull(nameof(context));
            _ = key.WhenNotNullOrEmpty(nameof(key));

            context.RootContextData[key] = data;
        }

        public static TContext GetContextData<TType, TContext>(this ValidationContext<TType> context, string key = "data")
        {
            _ = context.WhenNotNull(nameof(context));
            _ = key.WhenNotNullOrEmpty(nameof(key));

            return (TContext)context.RootContextData[key];
        }
    }
}