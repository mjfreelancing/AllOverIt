using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;
using Microsoft.Extensions.Configuration;

public sealed class IsAddressValidator : ValidatorBase<Address>
{
    private readonly int _postcode;

    // Dependencies are supported when used with IServiceValidationInvoker
    public IsAddressValidator(IConfiguration configuration)
    {
        _ = configuration.WhenNotNull();    // Demonstrating this should be injected

        _postcode = configuration["Postcode"].As<int>();

        // methods starting with IsXXX are extension methods provided by AllOverIt.Validation
        RuleFor(address => address.Number).IsGreaterThan(0);
        RuleFor(address => address.Street).IsNotEmpty();
        RuleFor(address => address.Suburb).IsNotEmpty();
        RuleFor(address => address.Postcode).IsGreaterThan(_postcode);
    }
}
