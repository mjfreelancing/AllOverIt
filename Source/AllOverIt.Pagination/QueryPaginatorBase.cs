﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Pagination
{
    public abstract class QueryPaginatorBase
    {
        // base class mainly exists to keep statics out of the generic implementations
        private static readonly IDictionary<Type, MethodInfo> _comparisonMethods;

        internal static readonly ConstantExpression ConstantZero = Expression.Constant(0);

        static QueryPaginatorBase()
        {
            var comparableTypes = new[]
            {
                typeof(bool),
                typeof(string),
                typeof(Guid)
            };

            var registry = new Dictionary<Type, MethodInfo>();

            foreach (var type in comparableTypes)
            {
                var compareTo = type
                    .GetTypeInfo()
                    .GetMethod(nameof(IComparable.CompareTo), new[] { type });

                compareTo.CheckNotNull(nameof(compareTo), $"The type {type.GetFriendlyName()} does not provide a {nameof(IComparable.CompareTo)}() method.");

                registry.Add(type, compareTo);
            }

            _comparisonMethods = registry;
        }

        protected static bool TryGetComparisonMethodInfo(Type type, out MethodInfo methodInfo)
        {
            // Enum's are IComparable but we can't pre-register the types we don't know about - so register them as they arrive
            if (type.IsEnum && !_comparisonMethods.TryGetValue(type, out methodInfo))
            {
                methodInfo = type
                    .GetTypeInfo()
                    .GetMethod(nameof(Enum.CompareTo), new[] { type });

                _comparisonMethods.Add(type, methodInfo);

                return true;
            }

            return _comparisonMethods.TryGetValue(type, out methodInfo);
        }
    }
}