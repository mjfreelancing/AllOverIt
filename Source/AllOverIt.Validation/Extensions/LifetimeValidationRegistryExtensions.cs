using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ILifetimeValidationRegistry"/>.</summary>
    public static class LifetimeValidationRegistryExtensions
    {
        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry with a <see cref="ServiceLifetime.Transient"/> lifetime.</summary>
        /// <typeparam name="TRegistrar">The concrete registrar used to identify the assembly where the validators are located.</typeparam>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public static void AutoRegisterTransientValidators<TRegistrar>(this ILifetimeValidationRegistry validationRegistry,
            Func<Type, Type, bool>? predicate = default) where TRegistrar : LifetimeValidationRegistrarBase, new()
        {
            _ = validationRegistry.WhenNotNull(nameof(validationRegistry));

            AutoRegisterValidators<TRegistrar>(validationRegistry, ServiceLifetime.Transient, predicate);
        }

        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry with a <see cref="ServiceLifetime.Scoped"/> lifetime.</summary>
        /// <typeparam name="TRegistrar">The concrete registrar used to identify the assembly where the validators are located.</typeparam>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public static void AutoRegisterScopedValidators<TRegistrar>(this ILifetimeValidationRegistry validationRegistry,
            Func<Type, Type, bool>? predicate = default) where TRegistrar : LifetimeValidationRegistrarBase, new()
        {
            _ = validationRegistry.WhenNotNull(nameof(validationRegistry));

            AutoRegisterValidators<TRegistrar>(validationRegistry, ServiceLifetime.Scoped, predicate);
        }

        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry with a <see cref="ServiceLifetime.Singleton"/> lifetime.</summary>
        /// <typeparam name="TRegistrar">The concrete registrar used to identify the assembly where the validators are located.</typeparam>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public static void AutoRegisterSingletonValidators<TRegistrar>(this ILifetimeValidationRegistry validationRegistry,
            Func<Type, Type, bool>? predicate = default) where TRegistrar : LifetimeValidationRegistrarBase, new()
        {
            _ = validationRegistry.WhenNotNull(nameof(validationRegistry));

            AutoRegisterValidators<TRegistrar>(validationRegistry, ServiceLifetime.Singleton, predicate);
        }

        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry.</summary>
        /// <typeparam name="TRegistrar">The concrete registrar used to identify the assembly where the validators are located.</typeparam>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="lifetime">The lifetime of the validator.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public static void AutoRegisterValidators<TRegistrar>(this ILifetimeValidationRegistry validationRegistry, ServiceLifetime lifetime,
            Func<Type, Type, bool>? predicate = default) where TRegistrar : LifetimeValidationRegistrarBase, new()
        {
            _ = validationRegistry.WhenNotNull(nameof(validationRegistry));

            var registrar = new TRegistrar();
            registrar.AutoRegisterValidators(validationRegistry, lifetime, predicate);
        }
    }
}