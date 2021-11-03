using AllOverIt.Assertion;
using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ValidationContext{TType}"/>.</summary>
    public static class ValidationContextExtensions
    {
        private const string DefaultKeyName = "data";

        /// <summary>Sets additional data associated with the validation request.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TContext">The context data type.</typeparam>
        /// <param name="context">The validation context to set the additional data on.</param>
        /// <param name="data">The additional data to add to the validation context.</param>
        /// <param name="key">The key associated with the data. The default key is "data".</param>
        public static void SetContextData<TType, TContext>(this ValidationContext<TType> context, TContext data, string key = DefaultKeyName)
        {
            _ = context.WhenNotNull(nameof(context));
            _ = key.WhenNotNullOrEmpty(nameof(key));

            context.RootContextData[key] = data;
        }

        /// <summary>Gets additional data associated with the validation request.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TContext">The context data type.</typeparam>
        /// <param name="context">The validation context to get the additional data from.</param>
        /// <param name="key">The key associated with the data. The default key is "data".</param>
        /// <returns>The context data associated with the key on the validation context.</returns>
        public static TContext GetContextData<TType, TContext>(this ValidationContext<TType> context, string key = DefaultKeyName)
        {
            _ = context.WhenNotNull(nameof(context));
            _ = key.WhenNotNullOrEmpty(nameof(key));

            return (TContext)context.RootContextData[key];
        }
    }
}