using AllOverIt.Assertion;
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
        public static Action<object, object?> CreatePropertySetter(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

            AssertPropertyCanWrite(propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod(true)!;

            var declaringType = propertyInfo.ReflectedType;

            var instanceExpression = Expression.Parameter(typeof(object), "item");

            var castTargetExpression = declaringType!.IsValueType
                ? Expression.Unbox(instanceExpression, declaringType)
                : Expression.Convert(instanceExpression, declaringType);

            var argumentExpression = Expression.Parameter(typeof(object), "arg");
            var valueParamExpression = Expression.Convert(argumentExpression, propertyInfo.PropertyType);

            var setterCall = Expression.Call(castTargetExpression, setterMethodInfo, valueParamExpression);

            return Expression
                .Lambda<Action<object, object?>>(setterCall, instanceExpression, argumentExpression)
                .Compile();
        }

        /// <summary>Creates a compiled expression as an <c>Action{TType, object}</c> to set a property value based
        /// on a specified <see cref="PropertyInfo"/> instance.</summary>
        /// <typeparam name="TType">The object type to set the property value on.</typeparam>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property setter.</param>
        /// <returns>The compiled property setter.</returns>
        /// <remarks>This overload will not work with strongly typed structs. To set the value of a property on a struct
        /// use <see cref="CreatePropertySetter(PropertyInfo)"/>.</remarks>
        public static Action<TType, object?> CreatePropertySetter<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

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
        public static Action<TType, object?> CreatePropertySetter<TType>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            var type = typeof(TType);
            var propertyInfo = ReflectionCache.GetPropertyInfo(type.GetTypeInfo(), propertyName);

            Throw<ReflectionException>.WhenNull(propertyInfo, $"The property {propertyName} on type {type.GetFriendlyName()} does not exist.");

            return CreatePropertySetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        /// <summary>Creates a compiled expression as an <c>Action{TType, TProperty?}</c> to set a property value based
        /// on a specified property name.</summary>
        /// <typeparam name="TType">The object type to set the property value on.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The compiled property setter.</returns>
        public static Action<TType, TProperty?> CreatePropertySetter<TType, TProperty>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            var type = typeof(TType);
            var propertyInfo = ReflectionCache.GetPropertyInfo(type.GetTypeInfo(), propertyName);

            Throw<ReflectionException>.WhenNull(propertyInfo, $"The property {propertyName} on type {type.GetFriendlyName()} does not exist.");

            return CreatePropertySetterExpressionLambda<TType, TProperty>(propertyInfo).Compile();

        }

        /// <summary>Creates an expression as an <c>Action{TType, object}</c> to set a property value based
        /// on a specified <see cref="PropertyInfo"/> instance.</summary>
        /// <typeparam name="TType">The object type to set the property value on.</typeparam>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property setter.</param>
        /// <returns>The property setter expression.</returns>
        public static Expression<Action<TType, object?>> CreatePropertySetterExpressionLambda<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

            AssertPropertyCanWrite(propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod(true)!;

            // Create the parameters for the lambda expression
            var instanceParam = Expression.Parameter(typeof(TType), "item");
            var valueParam = Expression.Parameter(typeof(object), "arg");

            // Convert the instance to the declaring type if necessary
            var instanceExpr = typeof(TType) != propertyInfo.DeclaringType
                ? Expression.TypeAs(instanceParam, propertyInfo.DeclaringType!)
                : (Expression)instanceParam;

            // Convert the argument to the property type
            var valueExpr = Expression.Convert(valueParam, propertyInfo.PropertyType);

            // Create the setter method call
            var setterCall = Expression.Call(instanceExpr, setterMethodInfo, valueExpr);

            // Return the lambda expression
            return Expression.Lambda<Action<TType, object?>>(setterCall, instanceParam, valueParam);
        }

        /// <summary>Creates an expression as an <c>Action{TType, TProperty?}</c> to set a property value based
        /// on a specified <see cref="PropertyInfo"/> instance.</summary>
        /// <typeparam name="TType">The object type to set the property value on.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property setter.</param>
        /// <returns>The property setter expression.</returns>
        public static Expression<Action<TType, TProperty?>> CreatePropertySetterExpressionLambda<TType, TProperty>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

            AssertPropertyCanWrite(propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod(true)!;

            // Create the parameters for the lambda expression
            var instanceParam = Expression.Parameter(typeof(TType), "item");
            var valueParam = Expression.Parameter(typeof(TProperty), "value");

            // Create the call to the setter method
            var setterCall = Expression.Call(instanceParam, setterMethodInfo, valueParam);

            return Expression.Lambda<Action<TType, TProperty?>>(setterCall, instanceParam, valueParam);
        }

        private static void AssertPropertyCanWrite(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
            {
                throw new ReflectionException($"The property {propertyInfo.Name} on type {propertyInfo.DeclaringType!.GetFriendlyName()} does not have a setter.");
            }
        }
    }
}
