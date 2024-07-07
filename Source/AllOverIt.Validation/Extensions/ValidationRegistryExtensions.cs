using AllOverIt.Assertion;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IValidationRegistry"/>.</summary>
    public static class ValidationRegistryExtensions
    {
        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry.</summary>
        /// <typeparam name="TRegistrar">The concrete registrar used to identify the assembly where the validators are located.</typeparam>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="ValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public static void AutoRegisterValidators<TRegistrar>(this IValidationRegistry validationRegistry, Func<Type, Type, bool>? predicate = default)
            where TRegistrar : ValidationRegistrarBase, new()
        {
            _ = validationRegistry.WhenNotNull(nameof(validationRegistry));

            var registrar = new TRegistrar();
            registrar.AutoRegisterValidators(validationRegistry, predicate);
        }
    }
}