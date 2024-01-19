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
        /// <summary>A delegate type that allows a field value to be updated via a ref.</summary>
        /// <typeparam name="TType">The object type to set the field value on.</typeparam>
        /// <param name="instance">The ref instance to update the field value on.</param>
        /// <param name="value">The value to set on the field.</param>
        public delegate void SetFieldByRefDelegate<TType>(ref TType instance, object value);

        /// <summary>Creates a compiled expression as an <c>Action{object, object}</c> to set a field value
        /// based on a specified <see cref="FieldInfo"/> instance.</summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to build a field setter.</param>
        /// <returns>The compiled field setter.</returns>
        /// <remarks>This overload will only work with structs that are provided as object types. To set the
        /// value of a field on a strongly typed struct use <see cref="CreateFieldSetterByRef{TType}(FieldInfo)"/>.</remarks>
        public static Action<object, object> CreateFieldSetter(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var declaringType = fieldInfo.DeclaringType;

            var instanceExpression = Expression.Parameter(typeof(object), "item");

            var valueExpression = Expression.Parameter(typeof(object), "value");

            var castTargetExpression = declaringType.IsValueType
                ? Expression.Unbox(instanceExpression, declaringType)           // struct
                : Expression.Convert(instanceExpression, declaringType);        // class

            var castValueExpression = Expression.Convert(valueExpression, fieldInfo.FieldType);

            var fieldExpression = Expression.Field(castTargetExpression, fieldInfo);

            var assignExpression = Expression.Assign(fieldExpression, castValueExpression);

            return Expression
                .Lambda<Action<object, object>>(assignExpression, instanceExpression, valueExpression)
                .Compile();
        }

        /// <summary>Creates a compiled expression as an <c>Action{TType, object}</c> to set a field value
        /// based on a specified <see cref="FieldInfo"/> instance.</summary>
        /// <typeparam name="TType">The object type to set the field value on.</typeparam>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to build a field setter.</param>
        /// <returns>The compiled field setter.</returns>
        /// <remarks>This typed version will not work with strongly typed structs. To set the value of a field on a struct
        /// use either <see cref="CreateFieldSetter(FieldInfo)"/> or <see cref="CreateFieldSetterByRef{TType}(FieldInfo)"/>.</remarks>
        public static Action<TType, object> CreateFieldSetter<TType>(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(TType), "item");

            return CreateFieldSetter<TType, Action<TType, object>>(instance, fieldInfo);
        }

        /// <summary>Creates a compiled expression as an <c>Action{TType, object}</c> to set a field value based
        /// on a specified field name.</summary>
        /// <typeparam name="TType">The object type to set the field value on.</typeparam>
        /// <param name="fieldName">The name of the field to set the value on.</param>
        /// <returns>The compiled field setter.</returns>
        /// <remarks>This typed version will not work with strongly typed structs. To set the value of a field on a struct
        /// use either <see cref="CreateFieldSetter(FieldInfo)"/> or <see cref="CreateFieldSetterByRef{TType}(FieldInfo)"/>.</remarks>
        public static Action<TType, object> CreateFieldSetter<TType>(string fieldName)
        {
            _ = fieldName.WhenNotNullOrEmpty(nameof(fieldName));

            var type = typeof(TType);
            var fieldInfo = ReflectionCache.GetFieldInfo(type.GetTypeInfo(), fieldName);

            Throw<ReflectionException>.WhenNull(fieldInfo, $"The field {fieldName} on type {type.GetFriendlyName()} does not exist.");

            return CreateFieldSetter<TType>(fieldInfo);
        }

        /// <summary>Creates a compiled expression as a <see cref="SetFieldByRefDelegate{T}"/> to set a field value
        /// by reference based on a specified <see cref="FieldInfo"/> instance.</summary>
        /// <typeparam name="TType">The object type to set the field value on.</typeparam>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to build a field setter.</param>
        /// <returns>The compiled field setter.</returns>
        public static SetFieldByRefDelegate<TType> CreateFieldSetterByRef<TType>(FieldInfo fieldInfo)
        {
            _ = fieldInfo.WhenNotNull(nameof(fieldInfo));

            var instance = Expression.Parameter(typeof(TType).MakeByRefType(), "item");

            return CreateFieldSetter<TType, SetFieldByRefDelegate<TType>>(instance, fieldInfo);
        }

        /// <summary>Creates a compiled expression as an <see cref="SetFieldByRefDelegate{T}"/> to set a field value
        /// by reference based on a specified field name.</summary>
        /// <typeparam name="TType">The object type to set the field value on.</typeparam>
        /// <param name="fieldName">The name of the field to set the value on.</param>
        /// <returns>The compiled field setter.</returns>
        public static SetFieldByRefDelegate<TType> CreateFieldSetterByRef<TType>(string fieldName)
        {
            _ = fieldName.WhenNotNullOrEmpty(nameof(fieldName));

            var type = typeof(TType);
            var fieldInfo = ReflectionCache.GetFieldInfo(type.GetTypeInfo(), fieldName);

            Throw<ReflectionException>.WhenNull(fieldInfo, $"The field {fieldName} on type {type.GetFriendlyName()} does not exist.");

            return CreateFieldSetterByRef<TType>(fieldInfo);
        }

        private static TReturn CreateFieldSetter<TType, TReturn>(ParameterExpression instance, FieldInfo fieldInfo)
        {
            var argument = Expression.Parameter(typeof(object), "arg");

            var field = typeof(TType) != fieldInfo.DeclaringType
                ? Expression.Field(Expression.TypeAs(instance, fieldInfo.DeclaringType), fieldInfo)
                : Expression.Field(instance, fieldInfo);

            var setterCall = Expression.Assign(field, Expression.Convert(argument, fieldInfo.FieldType));

            return Expression
                .Lambda<TReturn>(setterCall, instance, argument)
                .Compile();
        }
    }
}
