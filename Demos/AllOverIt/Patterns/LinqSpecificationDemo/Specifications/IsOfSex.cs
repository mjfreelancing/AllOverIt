using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace LinqSpecificationDemo.Specifications
{
    internal sealed class IsOfSex : LinqSpecification<Person>
    {
        private readonly Sex _sex;

        public IsOfSex(Sex sex)
        {
            _sex = sex;
        }

        public override Expression<Func<Person, bool>> AsExpression()
        {
            return person => person.Sex == _sex;
        }
    }
}