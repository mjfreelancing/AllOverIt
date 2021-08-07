using FluentValidation.Results;

namespace AllOverIt.Validation
{
    public interface IValidationInvoker
    {
        ValidationResult Validate<TType>(TType instance);
        ValidationResult Validate<TType, TContext>(TType instance, TContext context);

        void AssertValidation<TType>(TType instance);
        void AssertValidation<TType, TContext>(TType instance, TContext context);
    }
}