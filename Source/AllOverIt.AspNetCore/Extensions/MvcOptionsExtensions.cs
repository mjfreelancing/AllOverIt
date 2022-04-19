using AllOverIt.AspNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace AllOverIt.AspNetCore.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="MvcOptions"/> types.</summary>
    public static class MvcOptionsExtensions
    {
        /// <summary>Adds support for <see cref="Patterns.Enumeration.EnrichedEnum{TEnum}" /> model binding.</summary>
        /// <param name="options">The <see cref="MvcOptions"/> instance.</param>
        /// <returns>The original <see cref="MvcOptions"/> instance provided to cater for a fluent syntax.</returns>
        public static MvcOptions AddEnrichedEnumModelBinder(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new EnrichedEnumModelBinderProvider());

            return options;
        }
    }
}
