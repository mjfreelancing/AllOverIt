namespace AllOverIt.Validation
{
    public interface IValidationInvoker
    {
        void AssertValidation<TType>(TType instance)
            where TType : class;

        void AssertValidation<TType, TContext>(TType instance, TContext context)
            where TType : class;
    }
}