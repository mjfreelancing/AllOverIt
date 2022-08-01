using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
    /// <summary>Contains Expression constants.</summary>
    public static class ExpressionConstants
    {
        /// <summary>A constant expression for the value zero.</summary>
        public static ConstantExpression Zero => Expression.Constant(0);
    }
}
