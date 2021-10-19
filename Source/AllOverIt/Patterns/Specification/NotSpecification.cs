namespace AllOverIt.Patterns.Specification
{
    // Implements a unary specification that negates the result of a specified specification.
    // TType is the candidate type to be tested.
    public sealed class NotSpecification<TType> : UnarySpecification<TType>
    {
        public NotSpecification(ISpecification<TType> specification)
            : base(specification, true)
        {
        }
    }
}