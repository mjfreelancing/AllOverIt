using AllOverIt.Patterns.Specification;

namespace LinqSpecificationDemo.Specifications
{
    internal sealed class IsOfMinimumAge : LinqSpecification<Person>
    {
        public IsOfMinimumAge(int age)
            : base(() => person => person.Age >= age)
        {
        }
    }
}