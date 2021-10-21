using AllOverIt.Patterns.Specification;

namespace SpecificationBenchmarking.Specifications
{
    internal sealed class IsOfMinimumAge : Specification<Person>
    {
        private readonly int _age;

        public IsOfMinimumAge(int age)
        {
            _age = age;
        }

        protected override bool DoIsSatisfiedBy(Person candidate)
        {
            return candidate.Age >= _age;
        }
    }
}