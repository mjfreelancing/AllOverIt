using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace SpecificationBenchmarking.Specifications
{
    internal sealed class IsOfMinimumAgeLinq : LinqSpecification<Person>
    {
        private readonly int _age;

        public IsOfMinimumAgeLinq(int age)
        {
            _age = age;
        }

        public override Expression<Func<Person, bool>> AsExpression()
        {
            return person => person.Age >= _age;
        }
    }
}