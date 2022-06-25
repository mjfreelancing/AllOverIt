using AllOverIt.Assertion;
using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    public static partial class PropertyExpressions
    {
        // TODO: Comment and Tests
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
