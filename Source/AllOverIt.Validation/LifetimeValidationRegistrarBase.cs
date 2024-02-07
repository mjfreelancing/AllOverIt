using AllOverIt.Assertion;
using AllOverIt.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AllOverIt.Validation
{
    /// <summary>Base class registrar for auto-discovering and registering validators that implement <see cref="ValidatorBase{TType}"/>
    /// in the same assembly as the concrete registrar.</summary>
    public abstract class LifetimeValidationRegistrarBase
    {
        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry with a <see cref="ServiceLifetime.Transient"/> lifetime.</summary>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public void AutoRegisterTransientValidators(ILifetimeValidationRegistry validationRegistry, Func<Type, Type, bool> predicate = default)
        {
            AutoRegisterValidators(validationRegistry, ServiceLifetime.Transient, predicate);
        }

        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry with a <see cref="ServiceLifetime.Scoped"/> lifetime.</summary>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public void AutoRegisterScopedValidators(ILifetimeValidationRegistry validationRegistry, Func<Type, Type, bool> predicate = default)
        {
            AutoRegisterValidators(validationRegistry, ServiceLifetime.Scoped, predicate);
        }

        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry with a <see cref="ServiceLifetime.Singleton"/> lifetime.</summary>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public void AutoRegisterSingletonValidators(ILifetimeValidationRegistry validationRegistry, Func<Type, Type, bool> predicate = default)
        {
            AutoRegisterValidators(validationRegistry, ServiceLifetime.Singleton, predicate);
        }

        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry with a provided lifetime.</summary>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="LifetimeValidationInvoker"/>.</param>
        /// <param name="lifetime">The lifetime for each registered validator.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public void AutoRegisterValidators(ILifetimeValidationRegistry validationRegistry, ServiceLifetime lifetime, Func<Type, Type, bool> predicate = default)
        {
            _ = validationRegistry.WhenNotNull(nameof(validationRegistry));

            var validatorTypes = GetType().Assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && type.IsDerivedFrom(typeof(ValidatorBase<>)));

            foreach (var validatorType in validatorTypes)
            {
                // Drill down until we find the abstract validator with the model type
                var modelType = ValidationTypeHelper.GetModelType(validatorType);

                var registerValidator = predicate?.Invoke(modelType, validatorType) ?? true;

                if (registerValidator)
                {
                    validationRegistry.Register(modelType, validatorType, lifetime);
                }
            }
        }
    }
}