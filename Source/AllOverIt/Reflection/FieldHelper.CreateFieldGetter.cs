using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection.Exceptions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    public static class FieldHelper
    {
        private static readonly MethodInfo SetFieldMethodInfo = typeof(FieldHelper).GetMethod(
            nameof(SetField), BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

        public static Func<object, object> CreateFieldGetter(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var itemParam = Expression.Parameter(typeof(object), "item");
            var instanceParam = itemParam.CastOrConvertTo(fieldInfo.DeclaringType);

            var instanceField = Expression.Field(instanceParam, fieldInfo);
            var objectinstanceField = Expression.Convert(instanceField, typeof(object));

            return Expression
                .Lambda<Func<object, object>>(objectinstanceField, itemParam)
                .Compile();
        }

        public static Func<TType, object> CreateFieldGetter<TType>(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(TType), "item");

            var field = typeof(TType) != fieldInfo.DeclaringType
                ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                : Expression.Field(instance, fieldInfo);

            var convertField = Expression.TypeAs(field, typeof(object));

            return Expression.Lambda<Func<TType, object>>(convertField, instance).Compile();
        }

        //public static Func<TType, object> CreateFieldGetter<TType>(string fieldName)
        //{
        //    _ = fieldName.WhenNotNullOrEmpty(nameof(fieldName));

        //    var type = typeof(TType);
        //    var fieldInfo = ReflectionCache.GetFieldInfo(type.GetTypeInfo(), fieldName);

        //    if (fieldInfo == null)
        //    {
        //        throw new ReflectionException($"The field {fieldName} on type {type.GetFriendlyName()} does not exist.");
        //    }

        //    return CreateFieldGetter<TType>(fieldInfo);
        //}

        public static Action<object, object> CreateFieldSetter(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var declaringType = fieldInfo.DeclaringType;

            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var valueParameter = Expression.Parameter(typeof(object), "value");

            var sourceExpression = declaringType.IsValueType && !declaringType.IsNullableType()
                ? Expression.Unbox(sourceParameter, declaringType)
                : sourceParameter.CastOrConvertTo(declaringType);

            var fieldExpression = Expression.Field(sourceExpression, fieldInfo);

            var valueExpression = valueParameter.CastOrConvertTo(fieldExpression.Type);

            var genericSetFieldMethodInfo = SetFieldMethodInfo.MakeGenericMethod(fieldExpression.Type);

            var setFieldMethodCallExpression = Expression.Call(null, genericSetFieldMethodInfo, fieldExpression, valueExpression);

            var setterFn = Expression
                .Lambda<Action<object, object>>(setFieldMethodCallExpression, sourceParameter, valueParameter)
                .Compile();

            return setterFn;
        }

        public static Action<TType, object> CreateFieldSetter<TType>(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(TType), "item");
            var argument = Expression.Parameter(typeof(object), "arg");

            var field = typeof(TType) != fieldInfo.DeclaringType
                ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                : Expression.Field(instance, fieldInfo);

            var setterCall = Expression.Assign(
                field,
                Expression.Convert(argument, fieldInfo.FieldType));

            return Expression
                .Lambda<Action<TType, object>>(setterCall, instance, argument)
                .Compile();
        }

        public static Action<TType, object> CreateFieldSetterByRef<TType>(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(TType).MakeByRefType(), "item");
            var argument = Expression.Parameter(typeof(object), "arg");

            var field = typeof(TType) != fieldInfo.DeclaringType
                ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                : Expression.Field(instance, fieldInfo);

            var setterCall = Expression.Assign(
                field,
                Expression.Convert(argument, fieldInfo.FieldType));

            return Expression
                .Lambda<Action<TType, object>>(setterCall, instance, argument)
                .Compile();
        }

        internal static void SetField<TValue>(ref TValue field, TValue newValue) => field = newValue;
    }
}
