//using System;
//using System.Linq.Expressions;

//namespace AllOverIt.Patterns.Specification
//{
//    /// <summary>An abstract base class for all concrete LINQ-based binary specifications.</summary>
//    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
//    public abstract class BinaryLinqSpecification<TType> : LinqSpecification<TType>
//    {
//        /// <summary>Constructor.</summary>
//        /// <param name="expressionResolver">A resolver that returns an expression that represents the binary specification.</param>
//        protected BinaryLinqSpecification(Func<Expression<Func<TType, bool>>> expressionResolver)
//            : base(expressionResolver)
//        {
//        }
//    }
//}