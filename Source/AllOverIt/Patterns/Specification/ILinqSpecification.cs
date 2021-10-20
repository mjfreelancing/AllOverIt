using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    public interface ILinqSpecification<TType> : ISpecification<TType>
    {
        Expression<Func<TType, bool>> AsExpression();
    }
}