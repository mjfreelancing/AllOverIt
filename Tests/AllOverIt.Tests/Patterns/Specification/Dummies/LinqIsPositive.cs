using System;
using System.Linq.Expressions;
using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal sealed class LinqIsPositive : LinqSpecification<int>
    {
        public override Expression<Func<int, bool>> AsExpression()
        {
            return candidate => candidate > 0;
        }
    }
}