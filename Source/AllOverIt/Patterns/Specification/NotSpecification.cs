namespace AllOverIt.Patterns.Specification
{
    public sealed class NotSpecification<TType> : UnarySpecification<TType>
    {
        public NotSpecification(ISpecification<TType> specification)
            : base(specification, true)
        {
        }
    }
}