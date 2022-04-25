using AllOverIt.Assertion;
using AllOverIt.Extensions;
using FluentValidation;
using System;
using System.Linq;

namespace AllOverIt.Validation
{
    /// <summary>Base class registrar for auto-discovering and registering validators that implement <see cref="ValidatorBase{TType}"/>
    /// in the same assembly as the concrete registrar.</summary>
    public abstract class ValidationRegistrarBase
    {
        /// <summary>Auto-discover all validators that implement <see cref="ValidatorBase{TType}"/> in the same assembly as the concrete
        /// registrar and register them with the provided registry.</summary>
        /// <param name="validationRegistry">The registry to be populated with all discovered validators. This would normally be an
        /// instance of a <see cref="ValidationInvoker"/>.</param>
        /// <param name="predicate">An optional predicate to filter discovered validators.</param>
        public void AutoRegisterValidators(IValidationRegistry validationRegistry, Func<Type, Type, bool> predicate = default)
        {
            _ = validationRegistry.WhenNotNull(nameof(validationRegistry));

            var validatorTypes = GetType().Assembly
                   .GetTypes()
                   .Where(type => !type.IsAbstract && type.IsDerivedFrom(typeof(ValidatorBase<>)));

            foreach (var validatorType in validatorTypes)
            {
                var modelType = validatorType.BaseType!.GenericTypeArguments[0];

                var registerValidator = predicate?.Invoke(modelType, validatorType) ?? true;

                if (registerValidator)
                {
                    validationRegistry.Register(modelType, validatorType);
                }
            }
        }
    }
}