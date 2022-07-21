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

        public static Func<TType, object> CreateFieldGetter<TType>(string fieldName)
        {
            _ = fieldName.WhenNotNullOrEmpty(nameof(fieldName));

            var type = typeof(TType);
            var fieldInfo = ReflectionCache.GetFieldInfo(type.GetTypeInfo(), fieldName);

            if (fieldInfo == null)
            {
                throw new ReflectionException($"The field {fieldName} on type {type.GetFriendlyName()} does not exist.");
            }

            return CreateFieldGetter<TType>(fieldInfo);
        }
    }
}
