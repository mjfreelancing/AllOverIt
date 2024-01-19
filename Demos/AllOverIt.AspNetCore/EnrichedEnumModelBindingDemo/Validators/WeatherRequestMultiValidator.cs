using AllOverIt.Validation;
using EnrichedEnumModelBindingDemo.Requests;
using FluentValidation;

namespace EnrichedEnumModelBindingDemo.Validators
{
    internal sealed class WeatherRequestMultiValidator : ValidatorBase<WeatherRequestMulti>
    {
        public WeatherRequestMultiValidator()
        {
            RuleFor(model => model.Periods).NotEmpty();
        }
    }
}