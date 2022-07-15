using System;
using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
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

        // TODO: Tests

        public static Expression CreateParameterizedValue<TType>(TType value)
        {
            return CreateParameterizedValue(value, typeof(TType));
        }

        public static Expression CreateParameterizedValue(object value, Type valueType)
        {
            var parameterValue = new ParameterHolder(value);
            var constantParameter = Expression.Constant(parameterValue);
            var property = Expression.PropertyOrField(constantParameter, nameof(ParameterHolder.Value));

            return Expression.Convert(property, valueType);
        }
    }
}
