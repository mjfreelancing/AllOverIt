using AllOverIt.Patterns.Specification;

namespace SpecificationBenchmark.Specifications
{
    internal sealed class IsOfMinimumAgeLinq : LinqSpecification<Person>
    {
        public IsOfMinimumAgeLinq(int age)
            : base(() => person => person.Age >= age)
        {
        }
    }
}