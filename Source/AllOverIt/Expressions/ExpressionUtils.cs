using AllOverIt.Assertion;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
    /// <summary>A utility class that provides general purpose expression helpers.</summary>
    public static class ExpressionUtils
    {
        private sealed class ParameterHolder
        {
            public object Value { get; }

            public ParameterHolder(object value)
            {
                Value = value;
            }
        }

        /// <summary>Wraps a value in a placeholder object and returns the value as a property of that object,
        /// thus making it suitable for creating parameterized queryables.</summary>
        /// <typeparam name="TType">The type of the value.</typeparam>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>The value, via a proxy property, as an <see cref="Expression"/>.</returns>
        public static Expression CreateParameterizedValue<TType>(TType value)
        {
            return CreateParameterizedValue(value, typeof(TType));
        }

        /// <summary>Wraps a value in a placeholder object and returns the value as a property of that object,
        /// thus making it suitable for creating parameterized queryables.</summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <param name="valueType">The type of the value.</param>
        /// <returns>The value, via a proxy property, as an <see cref="Expression"/>.</returns>
        public static Expression CreateParameterizedValue(object value, Type valueType = default)
        {
            valueType ??= value?.GetType();

            Throw<ArgumentException>.WhenNull(valueType, "The value type must be provided when creating a parameterized value expression.");

            var parameterValue = new ParameterHolder(value);
            var constantParameter = Expression.Constant(parameterValue);
            var property = Expression.PropertyOrField(constantParameter, nameof(ParameterHolder.Value));

            return Expression.Convert(property, valueType);
        }
    }
}
