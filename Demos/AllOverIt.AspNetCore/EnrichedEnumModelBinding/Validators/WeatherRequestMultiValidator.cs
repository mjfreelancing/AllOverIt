using AllOverIt.Validation;
using EnrichedEnumModelBinding.Requests;
using FluentValidation;

namespace EnrichedEnumModelBinding.Validators
{
    internal sealed class WeatherRequestMultiValidator : ValidatorBase<WeatherRequestMulti>
    {
        public WeatherRequestMultiValidator()
        {
            RuleFor(model => model.Periods).NotEmpty();
        }
    }
}