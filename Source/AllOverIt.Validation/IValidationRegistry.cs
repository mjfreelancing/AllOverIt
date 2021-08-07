namespace AllOverIt.Validation
{
    public interface IValidationRegistry
    {
        IValidationRegistry Register<TType, TValidator>() where TValidator : ValidatorBase<TType>, new();
    }
}