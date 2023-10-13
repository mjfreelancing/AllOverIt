﻿using AllOverIt.Assertion;
using AllOverIt.Formatters.Objects;
using AllOverIt.Helpers;
using AllOverIt.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for object types.</summary>
    public static partial class ObjectExtensions
    {
        private static class ObjectConversionHelper
        {
            // instance, instance type, convertTo type, convertTo value
            private static readonly IReadOnlyCollection<Func<object, Type, Type, object>> AsConverters = new[]
            {
                AsSameTypeOrObject,
                AsFromIntegralToBool,
                AsFromEnumToIntegral,
                AsFromIntegralToEnum,
                AsFromConvertibleTypeToValueType,
                AsFromDerivedType,
                AsUsingInstanceTypeConverter
            };

            public static TType ConvertTo<TType>(object instance, TType defaultValue)
            {
                if (instance == null)
                {
                    return defaultValue;
                }

                var instanceType = instance.GetType();
                var convertToType = typeof(TType);

                var convertedValue = AsConverters
                    .Select(func => func.Invoke(instance, instanceType, convertToType))
                    .Where(result => result != null)
                    .FirstOrDefault();

                return convertedValue != null
                    ? (TType) convertedValue
                    : StringExtensions.As(instance.ToString(), defaultValue);
            }

            private static object AsSameTypeOrObject(object instance, Type instanceType, Type convertToType)
            {
                // return the same value if no conversion is required
                if (convertToType == instanceType || convertToType == CommonTypes.ObjectType)
                {
                    return instance;
                }

                return default;
            }

            private static object AsFromIntegralToBool(object instance, Type instanceType, Type convertToType)
            {
                // convert from integral to bool (conversion from a string is handled further below)
                if (instance.IsIntegral() && convertToType == CommonTypes.BoolType)
                {
                    var intValue = (int) Convert.ChangeType(instance, CommonTypes.IntType);

                    if (intValue is < 0 or > 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(instance), $"Cannot convert integral '{intValue}' to a Boolean.");
                    }

                    // convert the integral to a boolean
                    return (bool) Convert.ChangeType(intValue, CommonTypes.BoolType);
                }

                return default;
            }

            private static object AsFromEnumToIntegral(object instance, Type instanceType, Type convertToType)
            {
                // converting from Enum to byte, sbyte, short, ushort, int, uint, long, or ulong
                if (instance is Enum && convertToType.IsIntegralType())
                {
                    // cater for when Enum has an underlying type other than 'int'
                    var underlyingValue = GetEnumAsUnderlyingValue(instance, instanceType);

                    // now attempt to perform the converted value to the required type
                    return Convert.ChangeType(underlyingValue, convertToType);
                }

                return default;
            }

            private static object AsFromIntegralToEnum(object instance, Type instanceType, Type convertToType)
            {
                // Converting from byte, sbyte, short, ushort, int, uint, long, or ulong to Enum
                if (convertToType.IsEnumType() && instance.IsIntegral())
                {
                    // cater for when Enum has an underlying type other than 'int'
                    var underlyingValue = GetEnumAsUnderlyingValue(instance, convertToType);

                    if (!Enum.IsDefined(convertToType, underlyingValue))
                    {
                        throw new ArgumentOutOfRangeException(nameof(instance), $"Cannot cast '{instance}' to a '{convertToType.GetFriendlyName()}' value.");
                    }

                    return underlyingValue;
                }

                return default;
            }

            private static object AsFromConvertibleTypeToValueType(object instance, Type instanceType, Type convertToType)
            {
                if (instanceType != CommonTypes.StringType &&
                    instanceType.IsDerivedFrom(CommonTypes.IConvertibleType) &&
                    convertToType.IsValueType)
                {
                    return Convert.ChangeType(instance, convertToType);
                }

                return default;
            }

            private static object AsFromDerivedType(object instance, Type instanceType, Type convertToType)
            {
                if (convertToType.IsClassType() && convertToType != CommonTypes.StringType)
                {
                    // return the same value if the instance is a class inheriting `TType`
                    if (instanceType.IsDerivedFrom(convertToType))
                    {
                        return instance;
                    }

                    var typeConverter = TypeDescriptor.GetConverter(convertToType);

                    if (!typeConverter.IsValid(instance))
                    {
                        throw new InvalidCastException($"Unable to cast object of type '{instanceType.Name}' to type '{convertToType.Name}'.");
                    }

                    return typeConverter.ConvertFrom(instance);
                }

                return default;
            }

            private static object AsUsingInstanceTypeConverter(object instance, Type instanceType, Type convertToType)
            {
                if (instanceType.IsClassType() && instanceType != CommonTypes.StringType)
                {
                    var typeConverter = TypeDescriptor.GetConverter(instanceType);

                    if (typeConverter.CanConvertTo(convertToType))
                    {
                        return typeConverter.ConvertTo(instance, convertToType);
                    }
                }

                return default;
            }
        }

        /// <summary>Specifies the binding options to use when calculating the hash code of an object when using
        /// <see cref="CalculateHashCode{TType}(TType,IEnumerable{string},IEnumerable{string})"/>.</summary>
        public static BindingOptions DefaultHashCodeBindings { get; set; } = BindingOptions.Instance | BindingOptions.AllAccessor | BindingOptions.AllVisibility;

        /// <summary>Creates a dictionary containing property names and associated values.</summary>
        /// <param name="instance">The object instance to obtain property names and values from.</param>
        /// <param name="includeNulls">If true then null value properties will be included, otherwise they will be omitted.</param>
        /// <param name="bindingOptions">Binding options that determine how properties are resolved.</param>
        /// <returns>Returns a dictionary containing property names and associated values.</returns>
        public static IDictionary<string, object> ToPropertyDictionary(this object instance, bool includeNulls = false, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var type = instance.GetType();

            // Uses cached property info
            // Excludes any properties that don't have a getter
            var propertyInfo = type.GetPropertyInfo(bindingOptions, false);

            var propInfos = new Dictionary<string, object>();

            // More efficient than LINQ
            foreach (var propInfo in propertyInfo)
            {
                if (propInfo.IsIndexer())
                {
                    continue;
                }

                var value = propInfo.GetValue(instance);

                if (includeNulls || value != null)
                {
                    propInfos.Add(propInfo.Name, value);
                }
            }

            return propInfos;
        }

        /// <summary>Converts an object to an IDictionary{string, string} using a dot notation for nested members.</summary>
        /// <param name="instance">The instance to convert.</param>
        /// <param name="options">Options that determine how serialization of properties and their values are handled.</param>
        /// <param name="filter">An optional filter that can exclude properties by name or value, or format the values if a <see cref="IFormattableObjectPropertyFilter"/>.</param>
        /// <returns>Returns a dictionary containing property names and associated values (as strings). Nested members are named using dot notation.</returns>
        /// <remarks>
        /// <para>Collection type properties are named using a zero-based index notation.</para>
        /// <para>Dictionary type properties are named using the key values where possible. If the key is a class type then the class name is used along with
        /// a backtick and zero-based index suffix (to provide uniqueness).</para>
        /// </remarks>
        public static IDictionary<string, string> ToSerializedDictionary(this object instance, ObjectPropertySerializerOptions options = default, ObjectPropertyFilter filter = default)
        {
            var serializer = new ObjectPropertySerializer(options, filter);

            return serializer.SerializeToDictionary(instance);
        }

        /// <summary>Uses reflection to get the value of an object's property by name.</summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingFlags">.NET binding options that determine how property names are resolved.</param>
        /// <returns>The value of a property by name</returns>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding flags.</exception>
        public static TValue GetPropertyValue<TValue>(this object instance, string propertyName, BindingFlags bindingFlags)
        {
            var instanceType = instance.GetType();

            return (TValue) GetPropertyValue(instance, instanceType, propertyName, bindingFlags);
        }

        /// <summary>Uses reflection to get the value of an object's property by name.</summary>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="instanceType">The instance type. This overload avoids a call to get the instance type when it is already known.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingFlags">.NET binding options that determine how property names are resolved.</param>
        /// <returns>The value of a property by name</returns>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding flags.</exception>
        public static object GetPropertyValue(this object instance, Type instanceType, string propertyName, BindingFlags bindingFlags)
        {
            var propertyInfo = GetPropertyInfo(instanceType, propertyName, bindingFlags);

            _ = propertyInfo ?? throw new MemberAccessException($"The property '{propertyName}' was not found.");

            return propertyInfo.GetValue(instance);
        }

        /// <summary>Uses reflection to get the value of an object's property by name.</summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingOptions">Binding options that determine how property names are resolved.</param>
        /// <returns>The value of a property by name</returns>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding options.</exception>
        public static TValue GetPropertyValue<TValue>(this object instance, string propertyName, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var instanceType = instance.GetType();

            return (TValue) GetPropertyValue(instance, instanceType, propertyName, bindingOptions);
        }

        /// <summary>Uses reflection to get the value of an object's property by name.</summary>
        /// <param name="instance">The object to get the property value.</param>
        /// <param name="instanceType">The instance type. This overload avoids a call to get the instance type when it is already known.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="bindingOptions">Binding options that determine how property names are resolved.</param>
        /// <returns>The value of a property by name</returns>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding options.</exception>
        public static object GetPropertyValue(this object instance, Type instanceType, string propertyName, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var propertyInfo = instanceType
                .GetPropertyInfo(bindingOptions, false)
                .SingleOrDefault(item => item.Name == propertyName);

            _ = propertyInfo ?? throw new MemberAccessException($"The property '{propertyName}' was not found.");

            return propertyInfo.GetValue(instance);
        }

        /// <summary>Uses reflection to set the value of an object's property by name.</summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to set the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="bindingFlags">.NET binding options that determine how property names are resolved.</param>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding flags.</exception>
        public static void SetPropertyValue<TValue>(this object instance, string propertyName, TValue value, BindingFlags bindingFlags)
        {
            var instanceType = instance.GetType();

            SetPropertyValue(instance, instanceType, propertyName, value, bindingFlags);
        }

        /// <summary>Uses reflection to set the value of an object's property by name.</summary>
        /// <param name="instance">The object to set the property value.</param>
        /// <param name="instanceType">The instance type. This overload avoids a call to get the instance type when it is already known.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="bindingFlags">.NET binding options that determine how property names are resolved.</param>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding flags.</exception>
        public static void SetPropertyValue(this object instance, Type instanceType, string propertyName, object value, BindingFlags bindingFlags)
        {
            var propertyInfo = GetPropertyInfo(instanceType, propertyName, bindingFlags);

            _ = propertyInfo ?? throw new MemberAccessException($"The property '{propertyName}' was not found.");

            propertyInfo.SetValue(instance, value);
        }

        /// <summary>Uses reflection to set the value of an object's property by name.</summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        /// <param name="instance">The object to set the property value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to set on the property.</param>
        /// <param name="bindingOptions">Binding options that determine how property names are resolved.</param>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding options.</exception>
        public static void SetPropertyValue<TValue>(this object instance, string propertyName, TValue value, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var instanceType = instance.GetType();

            SetPropertyValue(instance, instanceType, propertyName, value, bindingOptions);
        }

        /// <summary>Uses reflection to set the value of an object's property by name.</summary>
        /// <param name="instance">The object to set the property value.</param>
        /// <param name="instanceType">The instance type. This overload avoids a call to get the instance type when it is already known.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to set on the property.</param>
        /// <param name="bindingOptions">Binding options that determine how property names are resolved.</param>
        /// <exception cref="MemberAccessException">When the property name cannot be found using the provided binding options.</exception>
        public static void SetPropertyValue(this object instance, Type instanceType, string propertyName, object value, BindingOptions bindingOptions = BindingOptions.Default)
        {
            var propertyInfo = instanceType
                .GetPropertyInfo(bindingOptions, false)
                .SingleOrDefault(item => item.Name == propertyName);

            _ = propertyInfo ?? throw new MemberAccessException($"The property '{propertyName}' was not found.");

            propertyInfo.SetValue(instance, value);
        }

        /// <summary>Determines if the specified object is an integral type (signed or unsigned).</summary>
        /// <param name="instance">The object instance to be compared to an integral type.</param>
        /// <returns>Returns <c>true</c> if the specified object is an integral type (signed or unsigned).</returns>
        public static bool IsIntegral(this object instance)
        {
            return instance is byte or sbyte or short or ushort or int or uint or long or ulong;
        }

        /// <summary>Converts the provided source <paramref name="instance"/> to a specified type.</summary>
        /// <typeparam name="TType">The type that <paramref name="instance"/> is to be converted to.</typeparam>
        /// <param name="instance">The object instance to be converted.</param>
        /// <param name="defaultValue">The default value to be returned when <paramref name="instance"/> is null.</param>
        /// <returns>Returns <paramref name="instance"/> converted to the specified <typeparamref name="TType"/>.</returns>
        public static TType As<TType>(this object instance, TType defaultValue = default)
        {
            return ObjectConversionHelper.ConvertTo(instance, defaultValue);
        }

        /// <summary>Converts the provided source <paramref name="instance"/> to a specified nullable type.</summary>
        /// <typeparam name="TType">The (nullable) type that <paramref name="instance"/> is to be converted to.</typeparam>
        /// <param name="instance">The object instance to be converted.</param>
        /// <param name="defaultValue">The default value to be returned when <paramref name="instance"/> is null.</param>
        /// <returns>Returns <paramref name="instance"/> converted to the specified <typeparamref name="TType"/>.</returns>
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

            return HashCodeHelper.CalculateHashCode(properties);
        }

        /// <summary>Calculates the hash code based on explicitly specified properties, fields, or the return result from a method call.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="instance">The instance having its hash code calculated.</param>
        /// <param name="resolvers">One or more resolvers that provide the properties, fields, or method calls used to calculate the hash code.</param>
        /// <returns>The calculated hash code.</returns>
        public static int CalculateHashCode<TType>(this TType instance, params Func<TType, object>[] resolvers)
        {
            var properties = resolvers.Select(resolver => resolver.Invoke(instance));

            return HashCodeHelper.CalculateHashCode(properties);
        }
        
        /// <summary>Determines if the provided object inherits from EnrichedEnum&lt;&gt;.</summary>
        /// <param name="object">The instance to be checked.</param>
        /// <returns><see langword="true" /> if the object inherits from EnrichedEnum&lt;>, otherwise <see langword="false" />.</returns>
        public static bool IsEnrichedEnum(this object @object)
        {
            return @object.GetType().IsEnrichedEnum();
        }

        /// <summary>Treats the provided object as an <see cref="IEnumerable{Object}"/> so it can be enumerated.</summary>
        /// <param name="object">The object to treat as an <see cref="IEnumerable{Object}"/> so it can be enumerated.</param>
        /// <returns>The object's elements.</returns>
        public static IEnumerable<object> GetObjectElements(this object @object)
        {
            var objectIterator = ((IEnumerable) @object).GetEnumerator();

            while (objectIterator.MoveNext())
            {
                yield return objectIterator.Current;
            }
        }

        private static PropertyInfo GetPropertyInfo(Type instanceType, string propertyName, BindingFlags bindingFlags)
        {
            while (instanceType != null)
            {
                var propertyInfo = instanceType.GetProperty(propertyName, bindingFlags);

                if (propertyInfo != null)
                {
                    return propertyInfo;
                }

                // move to the base class type
                instanceType = instanceType.BaseType;
            }

            return null;
        }

        // gets the enum value as its underlying type (such as short)
        private static object GetEnumAsUnderlyingValue(object instance, Type enumType)
        {
            var underlyingType = Enum.GetUnderlyingType(enumType);
            return Convert.ChangeType(instance, underlyingType);
        }
    }
}