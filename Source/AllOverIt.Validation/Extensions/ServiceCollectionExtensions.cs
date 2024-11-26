using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides extension methods for a <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds support for registering a <see cref="ValidationInvoker"/>, as a singleton <see cref="IValidationInvoker"/>. The registration
        /// of invokable validators can be configured as part of this call, or at a later time using the returned <see cref="IValidationRegistry"/>.</summary>
        /// <param name="services">The service collection to register the <see cref="IValidationInvoker"/>.</param>
        /// <param name="configure">An optional action to register invokable validators.</param>
        /// <returns>An <see cref="IValidationRegistry"/> that can be used to register additional validators.</returns>
        /// <remarks>All registered validators must have a default constructor, and will all be treated as a singleton. If the validators
        /// require a different lifetime, or if they require dependencies to be injected, then use
        /// <see cref="AddLifetimeValidationInvoker(IServiceCollection, Action{ILifetimeValidationRegistry})"/>.</remarks>
        public static IValidationRegistry AddValidationInvoker(this IServiceCollection services, Action<IValidationRegistry>? configure = default)
        {
            _ = services.WhenNotNull();

            var validationInvoker = new ValidationInvoker();

            configure?.Invoke(validationInvoker);

            services.AddSingleton<IValidationInvoker>(validationInvoker);

            // Allow additional registrations to be performed by the caller
            return validationInvoker;
        }

        /// <summary>Adds support for registering a <see cref="LifetimeValidationInvoker"/>, as a singleton <see cref="ILifetimeValidationInvoker"/>.
        /// The registration of invokable validators can be configured as part of this call, or at a later time using the returned
        /// <see cref="ILifetimeValidationRegistry"/>.</summary>
        /// <param name="services">The service collection to register the <see cref="ILifetimeValidationInvoker"/>.</param>
        /// <param name="configure">An optional action to register invokable validators.</param>
        /// <returns>An <see cref="ILifetimeValidationRegistry"/> that can be used to register additional validators.</returns>
        /// <remarks>If all registered validators have a default constructor and can be treated as a singleton, then a simpler alternative
        /// is to use <see cref="AddValidationInvoker(IServiceCollection, Action{IValidationRegistry})"/>.</remarks>
        public static ILifetimeValidationRegistry AddLifetimeValidationInvoker(this IServiceCollection services, Action<ILifetimeValidationRegistry>? configure = default)
        {
            _ = services.WhenNotNull();

            var validationInvoker = new LifetimeValidationInvoker(services);

            configure?.Invoke(validationInvoker);

            services.AddSingleton<ILifetimeValidationInvoker>(provider =>
            {
                var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

                validationInvoker.SetScopeFactory(scopeFactory);

                return validationInvoker;
            });

            // Allow additional registrations to be performed by the caller
            return validationInvoker;
        }
    }
}