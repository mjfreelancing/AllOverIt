using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
    public static class ExpressionConstants
    {
        public static ConstantExpression Zero => Expression.Constant(0);
    }
}
