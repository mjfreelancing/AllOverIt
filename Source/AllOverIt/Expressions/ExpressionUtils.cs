using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
    /// <summary>A utility class that provides general purpose expression helpers.</summary>
    public static class ExpressionUtils
    {
        private sealed class ParameterHolder
        {
            public object? Value { get; }

            public ParameterHolder(object? value)
            {
                Value = value;
            }
        }

        /// <summary>Wraps a value in a placeholder object and returns the value as a property of that object,
        /// thus making it suitable for creating parameterized queryables.</summary>
        /// <typeparam name="TType">The type of the value.</typeparam>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>The value, via a proxy property, as an <see cref="Expression"/>.</returns>
        public static Expression CreateParameterizedValue<TType>(TType? value)
        {
            return CreateParameterizedValue(value, typeof(TType));
        }

        /// <summary>Wraps a value in a placeholder object and returns the value as a property of that object,
        /// thus making it suitable for creating parameterized queryables.</summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="valueType">The type of the value. If <see langword="null"/>, the object's type will be determined at runtime.</param>
        /// <returns>The value, via a proxy property, as an <see cref="Expression"/>.</returns>
        public static Expression CreateParameterizedValue(object? value, Type? valueType = default)
        {
            valueType ??= value?.GetType();

            Throw<ArgumentException>.WhenNull(valueType, "The value type must be provided when creating a parameterized value expression.");

            var parameterValue = new ParameterHolder(value);
            var constantParameter = Expression.Constant(parameterValue);
            var property = Expression.PropertyOrField(constantParameter, nameof(ParameterHolder.Value));

            return Expression.Convert(property, valueType);
        }

        /// <summary>Creates an enumerable of <see cref="ParameterExpression"/> for each of the provided types.</summary>
        /// <param name="parameterTypes">The enumerable of parameter types.</param>
        /// <returns>An enumerable of <see cref="ParameterExpression"/> for each of the provided types.</returns>
        public static IEnumerable<ParameterExpression> CreateParameterExpressions(IEnumerable<Type> parameterTypes)
        {
            _ = parameterTypes.WhenNotNullOrEmpty(nameof(parameterTypes));

            var id = 1;

            foreach (var paramType in parameterTypes)
            {
                yield return Expression.Parameter(paramType, $"t{id++}");
            }
        }

        /// <summary>Gets a <see cref="NewExpression"/> and the constructor parameters as <see cref="ParameterExpression"/>[] for
        /// a provided type and its' constructor parameter types.</summary>
        /// <param name="type">The type to obtain a <see cref="NewExpression"/> for.</param>
        /// <param name="paramTypes">The type's constructor argument types.</param>
        /// <returns>A <see cref="NewExpression"/> and the constructor parameters as <see cref="ParameterExpression"/>[] for
        /// a provided type and its' constructor parameter types.</returns>
        public static (NewExpression NewExpression, ParameterExpression[] ParameterExpressions) GetConstructorWithParameters(Type type, Type[] paramTypes)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = paramTypes.WhenNotNullOrEmpty(nameof(paramTypes));

            var ctor = type.GetConstructor(paramTypes);

            Throw<InvalidOperationException>.WhenNull(ctor, $"The type {type.GetFriendlyName()} does not have a suitable constructor.");

            var parameters = CreateParameterExpressions(paramTypes).ToArray();
            var newExpression = Expression.New(ctor, parameters);

            // 'parameters' is the same as newExpression.Arguments, but returning it as ParameterExpression[] makes it easier to consume
            return (newExpression, parameters);
        }

        /// <summary>Gets a <see cref="NewExpression"/> and the constructor parameters as <see cref="ParameterExpression"/>[] for
        /// a provided type and its' constructor parameter types. The returned parameter expressions are each of type <c>object</c>
        /// that are cast to the required type before being passed into the constructor.</summary>
        /// <param name="type">The type to obtain a <see cref="NewExpression"/> for.</param>
        /// <param name="paramTypes">The type's constructor argument types.</param>
        /// <returns>A <see cref="NewExpression"/> and the constructor parameters as <see cref="ParameterExpression"/>[] for
        /// a provided type and its' constructor parameter types.</returns>
        public static (NewExpression NewExpression, ParameterExpression[] ParameterExpressions) GetConstructorWithParametersAsObjects(Type type, Type[] paramTypes)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = paramTypes.WhenNotNullOrEmpty(nameof(paramTypes));

            var ctor = type.GetConstructor(paramTypes);

            Throw<InvalidOperationException>.WhenNull(ctor, $"The type {type.GetFriendlyName()} does not have a suitable constructor.");

            var objectParams = new List<ParameterExpression>();     // object arguments provided by the caller
            var ctorParams = new List<Expression>();                // each object cast to the required type so it can be passed to the constructor

            var id = 1;

            foreach (var paramType in paramTypes)
            {
                var objectParam = Expression.Parameter(CommonTypes.ObjectType, $"t{id++}");

                objectParams.Add(objectParam);

                ctorParams.Add(objectParam.CastOrConvertTo(paramType));
            }

            var newExpression = Expression.New(ctor, ctorParams);

            return (newExpression, objectParams.ToArray());
        }
    }
}
