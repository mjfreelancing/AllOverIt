using AllOverIt.Assertion;
using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of helper functions related to reflection.</summary>
    public static partial class PropertyExpressions
    {
        public static Action<object, object> CreatePropertySetter(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            AssertPropertyCanWrite(propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod(true);

            var declaringType = propertyInfo.ReflectedType;

            var instance = Expression.Parameter(typeof(object), "item");

            var instanceParam = declaringType.IsValueType && !declaringType.IsNullableType()
                ? Expression.Unbox(instance, declaringType)
                : Expression.Convert(instance, declaringType);

            var argument = Expression.Parameter(typeof(object), "arg");
            var valueParam = Expression.Convert(argument, propertyInfo.PropertyType);

            var setterCall = Expression.Call(instanceParam, setterMethodInfo, valueParam);

            return Expression.Lambda<Action<object, object>>(setterCall, instance, argument).Compile();
        }

        public static Action<TType, object> CreatePropertySetter<TType>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            var type = typeof(TType);
            var propertyInfo = ReflectionCache.GetPropertyInfo(type.GetTypeInfo(), propertyName);

            if (propertyInfo == null)
            {
                throw new ReflectionException($"The property {propertyName} on type {type.GetFriendlyName()} does not exist.");
            }

            return CreatePropertySetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        public static Action<TType, object> CreatePropertySetter<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            AssertPropertyCanWrite(propertyInfo);

            return CreatePropertySetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        public static Expression<Action<TType, object>> CreatePropertySetterExpressionLambda<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            AssertPropertyCanWrite(propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod(true);

            var instance = Expression.Parameter(typeof(TType), "item");

            var instanceType = typeof(TType) != propertyInfo.DeclaringType
                ? (Expression) Expression.TypeAs(instance, propertyInfo.DeclaringType)
                : instance;

            var argument = Expression.Parameter(typeof(object), "arg");
            var valueParam = Expression.Convert(argument, propertyInfo.PropertyType);

            var setterCall = Expression.Call(instanceType, setterMethodInfo, valueParam);

            return Expression.Lambda<Action<TType, object>>(setterCall, instance, argument);
        }

        private static void AssertPropertyCanWrite(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
            {
                throw new ReflectionException($"The property {propertyInfo.Name} on type {propertyInfo.DeclaringType.GetFriendlyName()} does not have a setter.");
            }
        }





        //public static Func<object, object> CreateGetter(FieldInfo fieldInfo)
        //{
        //    var fieldDeclaringType = fieldInfo.DeclaringType;

        //    var oInstanceParam = Expression.Parameter(typeof(object), "source");
        //    var instanceParam = GetCastOrConvertExpression(oInstanceParam, fieldDeclaringType);

        //    var exprCallFieldGetFn = Expression.Field(instanceParam, fieldInfo);
        //    var oExprCallFieldGetFn = Expression.Convert(exprCallFieldGetFn, typeof(object));

        //    var fieldGetterFn = Expression.Lambda<Func<object, object>>
        //        (
        //            oExprCallFieldGetFn,
        //            oInstanceParam
        //        )
        //        .Compile();

        //    return fieldGetterFn;
        //}

        //private static Expression GetCastOrConvertExpression(Expression expression, Type targetType)
        //{
        //    Expression result;
        //    var expressionType = expression.Type;

        //    if (targetType.IsAssignableFrom(expressionType))
        //    {
        //        result = expression;
        //    }
        //    else
        //    {
        //        // Check if we can use the as operator for casting or if we must use the convert method
        //        if (targetType.IsValueType && !targetType.IsNullableType())
        //        {
        //            result = Expression.Convert(expression, targetType);
        //        }
        //        else
        //        {
        //            result = Expression.TypeAs(expression, targetType);
        //        }
        //    }

        //    return result;
        //}

        //public static GetMemberDelegate<T> CreateGetter<T>(FieldInfo fieldInfo)
        //{
        //    var instance = Expression.Parameter(typeof(T), "i");
        //    var field = typeof(T) != fieldInfo.DeclaringType
        //        ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
        //        : Expression.Field(instance, fieldInfo);
        //    var convertField = Expression.TypeAs(field, typeof(object));
        //    return Expression.Lambda<GetMemberDelegate<T>>(convertField, instance).Compile();
        //}

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
