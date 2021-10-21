using AllOverIt.Patterns.Specification;

namespace SpecificationBenchmarking.Specifications
{
    internal sealed class IsOfSex : Specification<Person>
    {
        private readonly Sex _sex;

        public IsOfSex(Sex sex)
        {
            _sex = sex;
        }

        protected override bool DoIsSatisfiedBy(Person candidate)
        {
            return candidate.Sex == _sex;
        }
    }
}