using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace LinqSpecificationDemo.Specifications
{
    internal sealed class IsOfMinimumAge : LinqSpecification<Person>
    {
        private readonly int _age;

        public IsOfMinimumAge(int age)
        {
            _age = age;
        }

        public override Expression<Func<Person, bool>> AsExpression()
        {
            return person => person.Age >= _age;
        }
    }
}