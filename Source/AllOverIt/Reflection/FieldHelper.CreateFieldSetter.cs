using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection.Exceptions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    public static partial class FieldHelper
    {
        private static readonly MethodInfo SetFieldMethodInfo = typeof(FieldHelper).GetMethod(nameof(SetField), BindingFlags.Static | BindingFlags.NonPublic);

        internal static void SetField<TValue>(ref TValue field, TValue newValue) => field = newValue;

        public static Action<object, object> CreateFieldSetter(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var declaringType = fieldInfo.DeclaringType;

            var sourceParameter = Expression.Parameter(typeof(object), "item");
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

        public static Action<TType, object> CreateFieldSetter<TType>(string fieldName)
        {
            _ = fieldName.WhenNotNullOrEmpty(nameof(fieldName));

            var type = typeof(TType);
            var fieldInfo = ReflectionCache.GetFieldInfo(type.GetTypeInfo(), fieldName);

            if (fieldInfo == null)
            {
                throw new ReflectionException($"The field {fieldName} on type {type.GetFriendlyName()} does not exist.");
            }

            return CreateFieldSetter<TType>(fieldInfo);
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
    }
}
