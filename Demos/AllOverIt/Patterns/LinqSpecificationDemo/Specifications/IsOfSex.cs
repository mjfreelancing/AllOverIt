using AllOverIt.Patterns.Specification;

namespace LinqSpecificationDemo.Specifications
{
    internal sealed class IsOfSex : LinqSpecification<Person>
    {
        public IsOfSex(Sex sex)
            : base(() => person => person.Sex == sex)
        {
        }
    }
}