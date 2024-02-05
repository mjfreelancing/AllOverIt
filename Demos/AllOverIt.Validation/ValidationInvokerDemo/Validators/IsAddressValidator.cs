using AllOverIt.Assertion;
using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;
using Microsoft.Extensions.Configuration;

public sealed class IsAddressValidator : ValidatorBase<Address>
{
    // Dependencies are supported when used with IServiceValidationInvoker
    public IsAddressValidator(IConfiguration configuration)
    {
        _ = configuration.WhenNotNull();    // Demonstrating this should be injected

        // methods starting with IsXXX are extension methods provided by AllOverIt.Validation
        RuleFor(address => address.Number).IsGreaterThan(0);
        RuleFor(address => address.Street).IsNotEmpty();
        RuleFor(address => address.Suburb).IsNotEmpty();
    }
}
