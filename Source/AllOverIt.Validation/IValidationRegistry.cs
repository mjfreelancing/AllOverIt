namespace AllOverIt.Validation
{
    public interface IValidationRegistry
    {
        IValidationRegistry Register<TType, TValidator>()
            where TType : class
            where TValidator : ValidatorBase<TType>, new();
    }
}