using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;

namespace OptionsValidationDemo
{
    public sealed class AppOptionsValidator : ValidatorBase<AppOptions>
    {
        static AppOptionsValidator()
        {
            DisablePropertyNameSplitting();
        }

        public AppOptionsValidator()
        {
            RuleFor(model => model.MinLevel).IsGreaterThan(0);
            RuleFor(model => model.AppName).IsNotEmpty();
        }
    }
}