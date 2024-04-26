﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection.Exceptions;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    public static partial class PropertyHelper
    {
        /// <summary>Creates a compiled expression as an <c>Action{object, object}</c> to set a property value
        /// based on a specified <see cref="PropertyInfo"/> instance.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property setter.</param>
        /// <returns>The compiled property setter.</returns>
        /// <remarks>This overload will only work with structs that are provided as object types.</remarks>
        public static Action<object, object> CreatePropertySetter(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            AssertPropertyCanWrite(propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod(true);

            var declaringType = propertyInfo.ReflectedType;

            var instanceExpression = Expression.Parameter(typeof(object), "item");

            var castTargetExpression = declaringType.IsValueType
                ? Expression.Unbox(instanceExpression, declaringType)
                : Expression.Convert(instanceExpression, declaringType);

            var argumentExpression = Expression.Parameter(typeof(object), "arg");
            var valueParamExpression = Expression.Convert(argumentExpression, propertyInfo.PropertyType);

            var setterCall = Expression.Call(castTargetExpression, setterMethodInfo, valueParamExpression);

            return Expression
                .Lambda<Action<object, object>>(setterCall, instanceExpression, argumentExpression)
                .Compile();
        }

        /// <summary>Creates a compiled expression as an <c>Action{TType, object}</c> to set a property value based
        /// on a specified <see cref="PropertyInfo"/> instance.</summary>
        /// <typeparam name="TType">The object type to set the property value on.</typeparam>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property setter.</param>
        /// <returns>The compiled property setter.</returns>
        /// <remarks>This overload will not work with strongly typed structs. To set the value of a property on a struct
        /// use <see cref="CreatePropertySetter(PropertyInfo)"/>.</remarks>
        public static Action<TType, object> CreatePropertySetter<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            AssertPropertyCanWrite(propertyInfo);

            return CreatePropertySetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        /// <summary>Creates a compiled expression as an <c>Action{TType, object}</c> to set a property value based
        /// on a specified property name.</summary>
        /// <typeparam name="TType">The object type to set the property value on.</typeparam>
        /// <param name="propertyName">The name of the property to set the value on.</param>
        /// <returns>The compiled property setter.</returns>
        /// <remarks>This overload will not work with strongly typed structs. To set the value of a property on a struct
        /// use <see cref="CreatePropertySetter(PropertyInfo)"/>.</remarks>
        public static Action<TType, object> CreatePropertySetter<TType>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            var type = typeof(TType);
            var propertyInfo = ReflectionCache.GetPropertyInfo(type.GetTypeInfo(), propertyName);

            Throw<ReflectionException>.WhenNull(propertyInfo, $"The property {propertyName} on type {type.GetFriendlyName()} does not exist.");

            return CreatePropertySetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        private static Expression<Action<TType, object>> CreatePropertySetterExpressionLambda<TType>(PropertyInfo propertyInfo)
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
    }
}
