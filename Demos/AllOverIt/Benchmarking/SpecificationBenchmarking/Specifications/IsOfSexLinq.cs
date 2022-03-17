using AllOverIt.Patterns.Specification;

namespace SpecificationBenchmarking.Specifications
{
    internal sealed class IsOfSexLinq : LinqSpecification<Person>
    {
        public IsOfSexLinq(Sex sex)
            : base(() => person => person.Sex == sex)
        {
        }
    }
}