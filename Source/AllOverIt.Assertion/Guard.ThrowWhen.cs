using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Assertion
{
    public static partial class Guard
    {
        public sealed class ThrowWhen
        {
            private static readonly ThrowWhen None = new(false);
            private readonly bool _predicate;

            private ThrowWhen(bool predicate)
            {
                _predicate = predicate;
            }

            public static ThrowWhen Create(bool predicate)
            {
                return predicate
                    ? new ThrowWhen(true)
                    : None;
            }

            public void Throw<TException>(string errorMessage = default) where TException : Exception
            {
                if (_predicate)
                {
                    ThrowException<TException>(errorMessage);
                }
            }
        }

        public static ThrowWhen WhenNull<TType>(this TType @object)
            where TType : class
        {
            return ThrowWhen.Create(@object is null);
        }

        public static ThrowWhen WhenNotNull<TType>(this TType @object)
            where TType : class
        {
            return ThrowWhen.Create(@object is not null);
        }

        public static ThrowWhen WhenNullOrEmpty(this string @object)
        {
            return ThrowWhen.Create(string.IsNullOrWhiteSpace(@object));
        }

        public static ThrowWhen WhenNotNullOrEmpty(this string @object)
        {
            return ThrowWhen.Create(!string.IsNullOrWhiteSpace(@object));
        }

        public static ThrowWhen WhenNullOrEmpty<TType>(this IEnumerable<TType> @object)
        {
            return ThrowWhen.Create(@object is null || !@object.Any());
        }

        public static ThrowWhen WhenNotNullOrEmpty<TType>(this IEnumerable<TType> @object)
        {
            return ThrowWhen.Create(@object is not null && @object.Any());
        }

        private static void ThrowException<TException>(string errorMessage) where TException : Exception
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw (Exception) Activator.CreateInstance(typeof(TException));
            }

            throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { errorMessage });
        }
    }
}