using System;
using System.Collections.Generic;

namespace AllOverIt.Helpers
{
    /// <summary>Provides static, general purpose, methods related to using Enums.</summary>
    public static class EnumHelper
    {
        /// <summary>Returns all possible enum values.</summary>
        /// <typeparam name="TType">The Enum type.</typeparam>
        /// <returns>All possible enum values.</returns>
        public static IReadOnlyCollection<TType> GetEnumValues<TType>()
          where TType : struct, Enum
        {
            return (TType[])Enum.GetValues(typeof(TType));
        }
    }
}