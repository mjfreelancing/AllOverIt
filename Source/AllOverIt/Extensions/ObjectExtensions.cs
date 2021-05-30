using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Specifies the binding options to use when calculating the hash code of an object when using
        /// <see cref="CalculateHashCode{TType}(TType,System.Collections.Generic.IEnumerable{string},System.Collections.Generic.IEnumerable{string})"/>.
        /// </summary>
        public static BindingOptions DefaultHashCodeBindings { get; set; } = BindingOptions.Instance | BindingOptions.AllAccessor | BindingOptions.AllVisibility;

        /// <summary>Creates a dictionary containing property names and associated values.</summary>
        /// <param name="instance">The object instance to obtain property names and values from.</param>
        /// <param name="includeNulls">If <c>true</c> then <c>null</c> value properties will be returned, otherwise they will be omitted.</param>
        /// <param name="bindingOptions">Binding options that determine how property names are resolved.</param>
        /// <returns>Returns a dictionary containing property names and associated values.</returns>
        public static IDictionary<string, object> ToPropertyDictionary(this object instance, bool includeNulls = false, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var type = instance.GetType();
            var propertyInfo = type.GetPropertyInfo(bindingOptions, false);

            var propInfos = from prop in propertyInfo
                            where prop.CanRead
                            let value = prop.GetValue(instance)
                            where includeNulls || value != null
                            select new KeyValuePair<string, object>(prop.Name, value);

            return propInfos.ToDictionary(item => item.Key, item => item.Value);
        }

        /// <summary>
        /// Uses reflection to get the value of an object's property by name.
        /// </summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingFlags">.NET binding options that determine how property names are resolved.</param>
        /// <returns>The value of a property by name</returns>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding flags.</exception>
        public static TValue GetPropertyValue<TValue>(this object instance, string propertyName, BindingFlags bindingFlags)
        {
            var propertyInfo = GetPropertyInfo(instance, propertyName, bindingFlags)
                               ?? throw new MemberAccessException($"The property '{propertyName}' was not found");

            return (TValue)propertyInfo.GetValue(instance);
        }

        /// <summary>
        /// Uses reflection to get the value of an object's property by name.
        /// </summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingOptions">Binding options that determine how property names are resolved.</param>
        /// <returns>The value of a property by name</returns>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding options.</exception>
        public static TValue GetPropertyValue<TValue>(this object instance, string propertyName, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var propertyInfo = instance
              .GetType()
              .GetPropertyInfo(bindingOptions, false)
              .SingleOrDefault(item => item.Name == propertyName);

            _ = propertyInfo ?? throw new MemberAccessException($"The property '{propertyName}' was not found");

            return (TValue)propertyInfo?.GetValue(instance);
        }


        /// <summary>
        /// Uses reflection to set the value of an object's property by name.
        /// </summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingFlags">.NET binding options that determine how property names are resolved.</param>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding flags.</exception>
        public static void SetPropertyValue<TValue>(this object instance, string propertyName, TValue value, BindingFlags bindingFlags)
        {
            var propertyInfo = GetPropertyInfo(instance, propertyName, bindingFlags)
              ?? throw new MemberAccessException($"The property '{propertyName}' was not found");

            propertyInfo.SetValue(instance, value);
        }

        /// <summary>
        /// Uses reflection to set the value of an object's property by name.
        /// </summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to set on the property.</param>
        /// <param name="bindingOptions">Binding options that determine how property names are resolved.</param>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding options.</exception>
        public static void SetPropertyValue<TValue>(this object instance, string propertyName, TValue value, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var propertyInfo = instance
              .GetType()
              .GetPropertyInfo(bindingOptions, false)
              .SingleOrDefault(item => item.Name == propertyName);

            _ = propertyInfo ?? throw new MemberAccessException($"The property '{propertyName}' was not found");

            propertyInfo.SetValue(instance, value);
        }

        /// <summary>Determines if the specified object is an integral type (signed or unsigned).</summary>
        /// <param name="instance">The object instance to be compared to an integral type.</param>
        /// <returns>Returns <c>true</c> if the specified object is an integral type (signed or unsigned).</returns>
        public static bool IsIntegral(this object instance)
        {
            return instance is byte || instance is sbyte || instance is short || instance is ushort
                   || instance is int || instance is uint || instance is long || instance is ulong;
        }

        /// <summary>Converts the provided source <paramref name="instance"/> to a specified type.</summary>
        /// <typeparam name="TType">The type that <paramref name="instance"/> is to be converted to.</typeparam>
        /// <param name="instance">The object instance to be converted.</param>
        /// <param name="defaultValue">The default value to be returned when <paramref name="instance"/> is null.</param>
        /// <returns>Returns <paramref name="instance"/> converted to the specified <typeparam name="TType"></typeparam>.</returns>
        public static TType As<TType>(this object instance, TType defaultValue = default)
        {
            if (instance == null)
            {
                return defaultValue;
            }

            // return same value if no conversion is required or the destination is a class reference
            var isClassType = typeof(TType).IsClassType() && typeof(TType) != typeof(string);

            if (typeof(TType) == instance.GetType() || typeof(TType) == typeof(object) || isClassType)
            {
                return (TType)instance;
            }

            // convert from integral to bool (conversion from a string is handled further below)
            if (typeof(TType) == typeof(bool) && instance.IsIntegral())
            {
                var intValue = (int)Convert.ChangeType(instance, typeof(int));

                if (intValue < 0 || intValue > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(instance), $"Cannot convert integral '{intValue}' to a Boolean.");
                }

                // convert the integral to a boolean
                instance = (bool)Convert.ChangeType(intValue, typeof(bool));

                return (TType)instance;
            }

            // converting from Enum to byte, sbyte, short, ushort, int, uint, long, or ulong
            if (instance is Enum && typeof(TType).IsIntegralType())
            {
                // cater for when Enum has an underlying type other than 'int'
                instance = GetEnumAsUnderlyingValue(instance, instance.GetType());

                // now attempt to perform the converted value to the required type
                return (TType)Convert.ChangeType(instance, typeof(TType));
            }

            // converting from byte, sbyte, short, ushort, int, uint, long, or ulong to Enum
            if (typeof(TType).IsEnumType() && instance.IsIntegral())
            {
                // cater for when Enum has an underlying type other than 'int'
                instance = GetEnumAsUnderlyingValue(instance, typeof(TType));

                if (!Enum.IsDefined(typeof(TType), instance))
                {
                    throw new ArgumentOutOfRangeException(nameof(instance), $"Cannot cast '{instance}' to a '{typeof(TType)}' value.");
                }

                return (TType)instance;
            }

            if (typeof(TType) == typeof(bool) || instance is bool || typeof(TType) == typeof(char) || instance is char)
            {
                return (TType)Convert.ChangeType(instance, typeof(TType));
            }

            // all other cases
            return StringExtensions.As($"{instance}", defaultValue);
        }

        /// <summary>Converts the provided source <paramref name="instance"/> to a specified nullable type.</summary>
        /// <typeparam name="TType">The (nullable) type that <paramref name="instance"/> is to be converted to.</typeparam>
        /// <param name="instance">The object instance to be converted.</param>
        /// <param name="defaultValue">The default value to be returned when <paramref name="instance"/> is null.</param>
        /// <returns>Returns <paramref name="instance"/> converted to the specified <typeparam name="TType"></typeparam>.</returns>
        public static TType? AsNullable<TType>(this object instance, TType? defaultValue = null)
          where TType : struct
        {
            return instance == null
              ? defaultValue
              : ObjectExtensions.As<TType>(instance);
        }

        /// <summary>
        /// Uses reflection to find all instance properties and use them to calculate a hash code.
        /// </summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="instance">The instance having its hash code calculated.</param>
        /// <param name="includeProperties">The property names to include. If null, then all non-static properties are used.</param>
        /// <param name="excludeProperties">The property names to exclude. If null, then no properties are excluded.</param>
        /// <returns>The calculated hash code.</returns>
        /// <remarks>To ensure idempotency, the properties are ordered by their name before calculating the hash.</remarks>
        public static int CalculateHashCode<TType>(this TType instance, IEnumerable<string> includeProperties = null,
          IEnumerable<string> excludeProperties = null)
        {
            // includeProperties = null => include all
            // excludeProperties = null => exclude none

            var inclusions = includeProperties?.AsReadOnlyList();
            var exclusions = excludeProperties?.AsReadOnlyList();

            var objType = typeof(TType);

            // uses declaredOnly = false so base class properties are included
            // ordering by name to make the calculations predictable
            var properties = from property in objType.GetPropertyInfo(DefaultHashCodeBindings)
                             where (inclusions == null || inclusions.Contains(property.Name)) &&
                                   (exclusions == null || !exclusions.Contains(property.Name))
                             orderby property.Name
                             select property.GetValue(instance);

            return AggregateHashCode(properties);
        }

        /// <summary>
        /// Calculates the hash code based on explicitly specified properties, fields, or the return result from a method call.
        /// </summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="instance">The instance having its hash code calculated.</param>
        /// <param name="resolvers">One or more resolvers that provide the properties, fields, or method calls used to calculate the hash code.</param>
        /// <returns>The calculated hash code.</returns>
        public static int CalculateHashCode<TType>(this TType instance, params Func<TType, object>[] resolvers)
        {
            var properties = resolvers.Select(propertyResolver => propertyResolver.Invoke(instance));

            return AggregateHashCode(properties);
        }

        private static PropertyInfo GetPropertyInfo(object instance, string propertyName, BindingFlags bindingFlags)
        {
            var type = instance.GetType();

            while (type != null)
            {
                var propertyInfo = type.GetProperty(propertyName, bindingFlags);

                if (propertyInfo != null)
                {
                    return propertyInfo;
                }

                // move to the base class type
                type = type.BaseType;
            }

            return null;
        }

        private static int AggregateHashCode(IEnumerable<object> properties)
        {
            return properties.Aggregate(17, (current, property) => current * 23 + (property?.GetHashCode() ?? 0));
        }

        // gets the enum value as its underlying type (such as short)
        private static object GetEnumAsUnderlyingValue(object instance, Type enumType)
        {
            var underlyingType = Enum.GetUnderlyingType(enumType);
            return Convert.ChangeType(instance, underlyingType);
        }
    }
}