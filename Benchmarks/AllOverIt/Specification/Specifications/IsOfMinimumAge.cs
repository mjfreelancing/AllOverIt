using AllOverIt.Patterns.Specification;

namespace SpecificationBenchmark.Specifications
{
    internal sealed class IsOfMinimumAge : Specification<Person>
    {
        private readonly int _age;

        public IsOfMinimumAge(int age)
        {
            _age = age;
        }

        public override bool IsSatisfiedBy(Person candidate)
        {
            return candidate.Age >= _age;
        }
    }
}