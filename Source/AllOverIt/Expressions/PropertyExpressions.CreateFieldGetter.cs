using AllOverIt.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    // TODO: Complete
    public static class ExpressionExtensions
    {
        public static Expression GetCastOrConvertExpression(this Expression expression, Type targetType)
        {
            if (targetType.IsAssignableFrom(expression.Type))
            {
                return expression;
            }

            return targetType.IsValueType && !targetType.IsNullableType()
                ? Expression.Convert(expression, targetType)
                : Expression.TypeAs(expression, targetType);
        }
    }



    public static partial class FieldExpressions
    {


        public static Func<object, object> CreateGetter(FieldInfo fieldInfo)
        {
            var itemParam = Expression.Parameter(typeof(object), "item");
            var instanceParam = itemParam.GetCastOrConvertExpression(fieldInfo.DeclaringType);

            var instanceField = Expression.Field(instanceParam, fieldInfo);
            var objectinstanceField = Expression.Convert(instanceField, typeof(object));

            return Expression
                .Lambda<Func<object, object>>(objectinstanceField, itemParam)
                .Compile();
        }

        

        public static Func<TType, object> CreateGetter<TType>(FieldInfo fieldInfo)
        {
            var instance = Expression.Parameter(typeof(TType), "item");
            
            var field = typeof(TType) != fieldInfo.DeclaringType
                ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                : Expression.Field(instance, fieldInfo);

            var convertField = Expression.TypeAs(field, typeof(object));
            
            return Expression.Lambda<Func<TType, object>>(convertField, instance).Compile();
        }





        //private static readonly MethodInfo setFieldMethod = typeof(ReflectionHelper).GetStaticMethod(nameof(SetField));
        //internal static void SetField<TValue>(ref TValue field, TValue newValue) => field = newValue;

        //public static SetMemberDelegate CreateSetter(FieldInfo fieldInfo)
        //{
        //    var declaringType = fieldInfo.DeclaringType;

        //    var sourceParameter = Expression.Parameter(typeof(object), "source");
        //    var valueParameter = Expression.Parameter(typeof(object), "value");

        //    var sourceExpression = declaringType.IsValueType && !declaringType.IsNullableType()
        //        ? Expression.Unbox(sourceParameter, declaringType)
        //        : GetCastOrConvertExpression(sourceParameter, declaringType);

        //    var fieldExpression = Expression.Field(sourceExpression, fieldInfo);

        //    var valueExpression = GetCastOrConvertExpression(valueParameter, fieldExpression.Type);

        //    var genericSetFieldMethodInfo = setFieldMethod.MakeGenericMethod(fieldExpression.Type);

        //    var setFieldMethodCallExpression = Expression.Call(
        //        null, genericSetFieldMethodInfo, fieldExpression, valueExpression);

        //    var setterFn = Expression.Lambda<SetMemberDelegate>(
        //        setFieldMethodCallExpression, sourceParameter, valueParameter).Compile();

        //    return setterFn;
        //}

        //public static SetMemberDelegate<T> CreateSetter<T>(FieldInfo fieldInfo)
        //{
        //    var instance = Expression.Parameter(typeof(T), "i");
        //    var argument = Expression.Parameter(typeof(object), "a");

        //    var field = typeof(T) != fieldInfo.DeclaringType
        //        ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
        //        : Expression.Field(instance, fieldInfo);

        //    var setterCall = Expression.Assign(
        //        field,
        //        Expression.Convert(argument, fieldInfo.FieldType));

        //    return Expression.Lambda<SetMemberDelegate<T>>
        //    (
        //        setterCall, instance, argument
        //    ).Compile();
        //}

        //public static SetMemberRefDelegate<T> CreateSetterRef<T>(FieldInfo fieldInfo)
        //{
        //    var instance = Expression.Parameter(typeof(T).MakeByRefType(), "i");
        //    var argument = Expression.Parameter(typeof(object), "a");

        //    var field = typeof(T) != fieldInfo.DeclaringType
        //        ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
        //        : Expression.Field(instance, fieldInfo);

        //    var setterCall = Expression.Assign(
        //        field,
        //        Expression.Convert(argument, fieldInfo.FieldType));

        //    return Expression.Lambda<SetMemberRefDelegate<T>>
        //    (
        //        setterCall, instance, argument
        //    ).Compile();
        //}




    }

    //#endif

}
