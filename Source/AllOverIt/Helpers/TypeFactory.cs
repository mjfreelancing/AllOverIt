using AllOverIt.Expressions;
using AllOverIt.Reflection;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Helpers
{
    //TypeFactory<Test>.Create().Invoke();
    //TypeFactory<Test>.Create<int>().Invoke(3);
    //TypeFactory<Test>.Create<int, string>().Invoke(3, "4");

    //typeof(Test)
    //    .GetFactory()
    //    .Invoke();

    //typeof(Test)
    //    .GetFactory<int>()
    //    .Invoke(3);

    //typeof(Test)
    //    .GetFactory<int, string>()
    //    .Invoke(3, "4");



    //private class Test
    //{
    //    public Test()
    //    {

    //    }

    //    public Test(int a)
    //    {

    //    }

    //    public Test(int a, string s)
    //    {

    //    }
    //}



    // TODO: tests
    public static class TypeFactory<TCreate>
    {
        public static Func<TCreate> Create()
        {
            var newExpression = Expression.New(typeof(TCreate));

            return Expression
                .Lambda<Func<TCreate>>(newExpression)
                .Compile();
        }

        public static Func<TArg1, TCreate> Create<TArg1>()
        {
            var paramTypes = new[] { typeof(TArg1) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TCreate>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TCreate> Create<TArg1, TArg2>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TCreate>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TCreate> Create<TArg1, TArg2, TArg3>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TCreate>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TCreate> Create<TArg1, TArg2, TArg3, TArg4>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TCreate>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCreate>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TCreate>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TCreate>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TCreate>>(newExpression, parameters)
                .Compile();
        }
    }
}