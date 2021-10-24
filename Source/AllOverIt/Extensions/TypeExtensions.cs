using AllOverIt.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Type"/> types.</summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> (property metadata) for a given public or protected property on a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to obtain the property metadata from.</param>
        /// <param name="propertyName">The name of the property to obtain metadata for.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the provided <paramref name="type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            return TypeInfoExtensions.GetPropertyInfo(type.GetTypeInfo(), propertyName);
        }

        /// <summary>
        /// Gets <see cref="PropertyInfo"/> (property metadata) for all properties on a given <see cref="Type"/> satisfying a given binding option.
        /// </summary>
        /// <param name="type">The type to obtain property metadata for.</param>
        /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only property metadata of the declared type is returned.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a provided <see cref="Type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(this Type type, BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
        {
            var predicate = BindingOptionsHelper.BuildBindingPredicate(binding);
            var typeInfo = type.GetTypeInfo();

            return from propInfo in typeInfo.GetPropertyInfo(declaredOnly)
                   let methodInfo = propInfo.GetMethod
                   where predicate.Invoke(methodInfo)
                   select propInfo;
        }

        /// <summary>
        /// Gets <see cref="MethodInfo"/> (method metadata) for a given <see cref="Type"/> and binding option.
        /// </summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only method metadata of the declared type is returned.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a provided <see cref="Type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static IEnumerable<MethodInfo> GetMethodInfo(this Type type, BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
        {
            var predicate = BindingOptionsHelper.BuildBindingPredicate(binding);
            var currentType = type;

            while (currentType != null)
            {
                var typeInfo = currentType.GetTypeInfo();

                foreach (var method in typeInfo.DeclaredMethods)
                {
                    if (predicate.Invoke(method))
                    {
                        yield return method;
                    }
                }

                currentType = !declaredOnly
                  ? typeInfo.BaseType
                  : null;
            }
        }

        /// <summary>
        /// Gets <see cref="MethodInfo"/> (method metadata) for a given <see cref="Type"/> method with a given name and no arguments.
        /// </summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="name">The name of the method.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a provided <see cref="Type"/> with a given name and no arguments.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo(this Type type, string name)
        {
            return GetMethodInfo(type, name, Type.EmptyTypes);
        }

        /// <summary>
        /// Gets <see cref="MethodInfo"/> (method metadata) for a given <see cref="Type"/> method with a given name and argument types.
        /// </summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="types">The argument types expected on the method</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a provided <see cref="Type"/> with a given name and argument types.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo(this Type type, string name, Type[] types)
        {
            return type.GetMethod(
              name,
              BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
              null, types, null);
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents an enumeration type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents an enumeration type, otherwise false.</returns>
        public static bool IsEnumType(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents a class type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents a class type, otherwise false.</returns>
        public static bool IsClassType(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents a primitive type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents a primitive type, otherwise false.</returns>
        public static bool IsPrimitiveType(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents an integral type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents an integral type, otherwise false.</returns>
        public static bool IsIntegralType(this Type type)
        {
            return new[] {typeof (byte), typeof (sbyte), typeof (short), typeof (ushort),
                    typeof (int), typeof (uint), typeof (long), typeof (ulong)}.Contains(type);
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents a floating type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents a floating type, otherwise false.</returns>
        public static bool IsFloatingType(this Type type)
        {
            return new[] { typeof(float), typeof(double), typeof(decimal) }.Contains(type);
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents an enumerable type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <param name="includeString">Indicates if a string type should be considered as an enumerable (of char).</param>
        /// <returns>True if the <see cref="Type"/> represents an enumerable type, otherwise false.</returns>
        public static bool IsEnumerableType(this Type type, bool includeString = false)
        {
            return type == typeof(string)
              ? includeString
              : typeof(IEnumerable).IsAssignableFromType(type);
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents a generic enumerable type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents a generic enumerable type, otherwise false.</returns>
        public static bool IsGenericEnumerableType(this Type type)
        {
            return type.IsGenericType() && typeof(IEnumerable).IsAssignableFromType(type);
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents a generic type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents a generic type, otherwise false.</returns>
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        /// <summary>
        /// Gets an array of the generic type arguments for this type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> containing the generic type arguments.</param>
        /// <returns>An array of the generic type arguments for this type.</returns>
        public static IEnumerable<Type> GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        /// <summary>
        /// Indicates if another type can be assigned to the current type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="fromType">The type to check.</param>
        /// <returns>True if the <paramref name="fromType"/> can be assigned to the current <paramref name="type"/>, otherwise false.</returns>
        public static bool IsAssignableFromType(this Type type, Type fromType)
        {
            var fromTypeInfo = fromType.GetTypeInfo();

            return type.GetTypeInfo().IsAssignableFrom(fromTypeInfo);
        }

        /// <summary>
        /// Indicates if the <see cref="Type"/> represents a generic nullable type.
        /// </summary>
        /// <param name="type">The type to compare.</param>
        /// <returns>True if the <see cref="Type"/> represents a generic nullable type, otherwise false.</returns>
        public static bool IsGenericNullableType(this Type type)
        {
            return type.IsGenericType() && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// A utility method that returns a print-friendly name for a given type.
        /// </summary>
        /// <param name="type">The type to generate a print-friendly name for.</param>
        /// <returns>A print-friendly name for a given type.</returns>
        public static string GetFriendlyName(this Type type)
        {
            if (type.IsGenericType() && !type.IsGenericNullableType())
            {
                var typeName = type.Name;

                // Some classes do not have the `, such as IDictionary<,>.KeyCollection
                var backtickIndex = typeName.IndexOf('`');

                if (backtickIndex != -1)
                {
                    typeName = typeName.Remove(backtickIndex);
                }

                var genericTypeNames = from genericArgument in type.GetGenericArguments()
                                       select GetFriendlyName(genericArgument);

                var stringBuilder = new StringBuilder();

                stringBuilder.Append(typeName);
                stringBuilder.Append('<');
                stringBuilder.Append(string.Join(", ", genericTypeNames));
                stringBuilder.Append('>');

                return stringBuilder.ToString();
            }

            return type.IsGenericNullableType()
              ? $"{type.GetGenericArguments().Single().Name}?"
              : type.Name;
        }
    }
}