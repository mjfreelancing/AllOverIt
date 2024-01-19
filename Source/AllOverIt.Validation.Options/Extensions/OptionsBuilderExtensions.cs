using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AllOverIt.Validation.Options.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="OptionsBuilder{TOptions}"/>.</summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>Use FluentValidation to provide the rule validation.</summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="optionsBuilder">The options builder.</param>
        /// <returns>The <see cref="OptionsBuilder{TOptions}"/> so that configure calls can be chained in it.</returns>
        public static OptionsBuilder<TOptions> UseFluentValidation<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
        {
            optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(provider =>
            {
                var validator = provider.GetRequiredService<IValidator<TOptions>>();

                return new FluentValidationValidateOptions<TOptions>(optionsBuilder.Name, validator);
            });

            return optionsBuilder;
        }
    }
}