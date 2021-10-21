using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace SpecificationBenchmarking.Specifications
{
    internal sealed class IsOfSexLinq : LinqSpecification<Person>
    {
        private readonly Sex _sex;

        public IsOfSexLinq(Sex sex)
        {
            _sex = sex;
        }

        public override Expression<Func<Person, bool>> AsExpression()
        {
            return person => person.Sex == _sex;
        }
    }
}