using AllOverIt.Patterns.Specification;

namespace SpecificationBenchmark.Specifications
{
    internal sealed class IsOfSexLinq : LinqSpecification<Person>
    {
        public IsOfSexLinq(Sex sex)
            : base(() => person => person.Sex == sex)
        {
        }
    }
}