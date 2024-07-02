using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection.Exceptions;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of field getter and setter helper functions related to <see cref="FieldInfo"/>.</summary>
    public static partial class FieldHelper
    {
        /// <summary>Creates a compiled expression as a <c>Func{object, object}</c> to get a field value based
        /// on a specified <see cref="FieldInfo"/> instance.</summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to build a field getter.</param>
        /// <returns>The compiled field getter.</returns>
        public static Func<object, object> CreateFieldGetter(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var itemParam = Expression.Parameter(typeof(object), "item");
            var instanceParam = itemParam.CastOrConvertTo(fieldInfo.DeclaringType);

            var instanceField = Expression.Field(instanceParam, fieldInfo);
            var objectInstanceField = Expression.Convert(instanceField, typeof(object));

            return Expression
                .Lambda<Func<object, object>>(objectInstanceField, itemParam)
                .Compile();
        }

        /// <summary>Creates a compiled expression as a <c>Func{TType, object}</c> to get a field value based
        /// on a specified <see cref="FieldInfo"/> instance.</summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to build a field getter.</param>
        /// <returns>The compiled field getter.</returns>
        public static Func<TType, object> CreateFieldGetter<TType>(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(TType), "item");

            var field = typeof(TType) == fieldInfo.DeclaringType
                ? Expression.Field(instance, fieldInfo)
                : Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo);

            var convertField = Expression.TypeAs(field, typeof(object));

            return Expression.Lambda<Func<TType, object>>(convertField, instance).Compile();
        }

        /// <summary>Creates a compiled expression as a <c>Func{TType, object}</c> to get a field value based on a specified
        /// field name.</summary>
        /// <typeparam name="TType">The object type to get the field value from.</typeparam>
        /// <param name="fieldName">The name of the field to get the value from.</param>
        /// <returns>The compiled field getter.</returns>
        public static Func<TType, object> CreateFieldGetter<TType>(string fieldName)
        {
            _ = fieldName.WhenNotNullOrEmpty(nameof(fieldName));

            var type = typeof(TType);
            var fieldInfo = ReflectionCache.GetFieldInfo(type.GetTypeInfo(), fieldName);

            Throw<ReflectionException>.WhenNull(fieldInfo, $"The field {fieldName} on type {type.GetFriendlyName()} does not exist.");

            return CreateFieldGetter<TType>(fieldInfo);
        }
    }
}
