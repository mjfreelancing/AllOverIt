using AllOverIt.Assertion;
using AllOverIt.Reflection;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Type"/> types.</summary>
    public static partial class TypeExtensions
    {
        /// <summary>Gets the <see cref="PropertyInfo"/> (property metadata) for a given public or protected property on a <see cref="Type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to obtain the property metadata from.</param>
        /// <param name="propertyName">The name of the property to obtain metadata for.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the provided <paramref name="type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            return TypeInfoExtensions.GetPropertyInfo(type.GetTypeInfo(), propertyName);
        }

        /// <summary>Gets <see cref="PropertyInfo"/> (property metadata) for all properties on a given <see cref="Type"/> satisfying a given binding option.</summary>
        /// <param name="type">The type to obtain property metadata for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If False, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If True, only property metadata of the declared type is returned.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a provided <see cref="Type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <paramref name="type"/>. Properties without a getter are excluded.</remarks>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(this Type type, BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false)
        {
            var predicate = BindingOptionsHelper.BuildPropertyOrMethodBindingPredicate(bindingOptions);
            var typeInfo = type.GetTypeInfo();

            // This implementation is better performing than using method/query LINQ queries

            var propInfos = new List<PropertyInfo>();

            foreach (var propInfo in typeInfo.GetPropertyInfo(declaredOnly))
            {
                var methodInfo = propInfo.GetMethod;

                // Ignore any properties without a getter
                if (methodInfo != null && predicate.Invoke(methodInfo))
                {
                    propInfos.Add(propInfo);
                }
            }

            return propInfos;
        }

        /// <summary>Gets the <see cref="FieldInfo"/> (field metadata) for a given public or protected field on a <see cref="Type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to obtain the field metadata from.</param>
        /// <param name="fieldName">The name of the field to obtain metadata for.</param>
        /// <returns>The field metadata, as <see cref="FieldInfo"/>, of a specified field on the provided <paramref name="type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first field found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static FieldInfo GetFieldInfo(this Type type, string fieldName)
        {
            return TypeInfoExtensions.GetFieldInfo(type.GetTypeInfo(), fieldName);
        }

        /// <summary>Gets <see cref="FieldInfo"/> (field metadata) for all properties on a given <see cref="Type"/> satisfying a given binding option.</summary>
        /// <param name="type">The type to obtain field metadata for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If False, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If True, only field metadata of the declared type is returned.</param>
        /// <returns>The field metadata, as <see cref="FieldInfo"/>, of a provided <see cref="Type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first field found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static IEnumerable<FieldInfo> GetFieldInfo(this Type type, BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false)
        {
            var predicate = BindingOptionsHelper.BuildFieldInfoBindingPredicate(bindingOptions);
            var typeInfo = type.GetTypeInfo();

            // This implementation is better performing than using method/query LINQ queries

            var fieldInfos = new List<FieldInfo>();

            foreach (var fieldInfo in typeInfo.GetFieldInfo(declaredOnly))
            {
                if (predicate.Invoke(fieldInfo))
                {
                    fieldInfos.Add(fieldInfo);
                }
            }

            return fieldInfos;
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <see cref="Type"/> and binding option.</summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If False, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If True, only method metadata of the declared type is returned.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a provided <see cref="Type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static IEnumerable<MethodInfo> GetMethodInfo(this Type type, BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false)
        {
            var predicate = BindingOptionsHelper.BuildPropertyOrMethodBindingPredicate(bindingOptions);
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

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <see cref="Type"/> method with a given name and no arguments.</summary>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="name">The name of the method.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a provided <see cref="Type"/> with a given name and no arguments.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo(this Type type, string name)
        {
            return GetMethodInfo(type, name, Type.EmptyTypes);
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <see cref="Type"/> method with a given name and argument types.</summary>
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

        /// <summary>Indicates if the <see cref="Type"/> represents an enumeration type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents an enumeration type, otherwise <see langword="False" />.</returns>
        public static bool IsEnumType(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        /// <summary>Indicates if the <see cref="Type"/> represents a class type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents a class type, otherwise <see langword="False" />.</returns>
        public static bool IsClassType(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        /// <summary>Indicates if the <see cref="Type"/> represents a value type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents a value type, otherwise <see langword="False" />.</returns>
        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        /// <summary>Indicates if the <see cref="Type"/> represents a primitive type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents a primitive type, otherwise <see langword="False" />.</returns>
        public static bool IsPrimitiveType(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }

        /// <summary>Indicates if the <see cref="Type"/> represents an integral type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents an integral type, otherwise <see langword="False" />.</returns>
        public static bool IsIntegralType(this Type type)
        {
            return new[]
            {
                CommonTypes.ByteType,
                CommonTypes.SByteType,
                CommonTypes.ShortType,
                CommonTypes.UShortType,
                CommonTypes.IntType,
                CommonTypes.UIntType,
                CommonTypes.LongType,
                CommonTypes.ULongType
            }.Contains(type);
        }

        /// <summary>Indicates if the <see cref="Type"/> represents a floating type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents a floating type, otherwise <see langword="False" />.</returns>
        public static bool IsFloatingType(this Type type)
        {
            return new[]
            {
                CommonTypes.FloatType,
                CommonTypes.DoubleType,
                CommonTypes.DecimalType
            }.Contains(type);
        }

        /// <summary>Indicates if the <see cref="Type"/> represents an enumerable type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <param name="includeString">Indicates if a string type should be considered as an enumerable (of char).</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents an enumerable type, otherwise <see langword="False" />.</returns>
        public static bool IsEnumerableType(this Type type, bool includeString = false)
        {
            return type == CommonTypes.StringType
              ? includeString
              : CommonTypes.IEnumerableType.IsAssignableFrom(type);
        }

        /// <summary>Gets the element type for any type assignable from <see cref="IEnumerable"/> (or an array). If the type is a generic
        /// it is expected to have a single generic argument type, such as <see cref="IEnumerable{TType}"/> or <see cref="IList{TType}"/>.</summary>
        /// <param name="type">The type to get the element type for.</param>
        /// <returns>The element type for any type assignable from <see cref="IEnumerable"/> (or an array).</returns>
        public static Type GetEnumerableElementType(this Type type)
        {
            Throw<InvalidOperationException>.When(!CommonTypes.IEnumerableType.IsAssignableFrom(type), $"{type.GetFriendlyName()} is not an {nameof(IEnumerable)}.");

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (!type.IsGenericEnumerableType())        // Cater for IEnumerable
            {
                return CommonTypes.ObjectType;
            }

            var genericsArguments = type.GetGenericArguments();

            Throw<InvalidOperationException>.When(genericsArguments.Length > 1, $"{type.GetFriendlyName()} is not an {nameof(IEnumerable)} with one generic argument.");

            return type.GetGenericArguments()[0];
        }

        /// <summary>Indicates if the <see cref="Type"/> represents a generic enumerable type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents a generic enumerable type, otherwise <see langword="False" />.</returns>
        public static bool IsGenericEnumerableType(this Type type)
        {
            return type.IsGenericType() && CommonTypes.IEnumerableType.IsAssignableFrom(type);
        }

        /// <summary>Indicates if the <see cref="Type"/> represents a generic type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents a generic type, otherwise <see langword="false" />.</returns>
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        /// <summary>Determines if a type is derived from another base type, including unbound generic types such as List&lt;>. Similar to
        /// IsSubClassOf(), this method does not support looking for inherited interfaces.</summary>
        /// <param name="type">The type to be tested.</param>
        /// <param name="fromType">The base type to compare against, including unbound generics, such as typeof(List&lt;>).</param>
        /// <returns><see langword="True" /> if <paramref name="type"/> inherits from <paramref name="fromType"/>, otherwise <see langword="False" />.</returns>
        /// <remarks>Use the <seealso cref="IsDerivedFrom"/> method when class and interface support is required.</remarks>
        public static bool IsSubClassOfRawGeneric(this Type type, Type fromType)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = fromType.WhenNotNull(nameof(fromType));

            while (type != null && type != CommonTypes.ObjectType)
            {
                if (fromType.IsRawGenericType(type))
                {
                    return true;
                }

                // Will be null when the type is an interface
                type = type.BaseType;
            }

            return false;
        }

        /// <summary>Determines if a type (or interface) inherits from another type (or interface), including open/unbound generics.</summary>
        /// <param name="type">The type to be tested.</param>
        /// <param name="fromType">The base type to compare against, including open/unbound generics, such as typeof(IList&lt;>).</param>
        /// <returns><see langword="True" /> if <paramref name="type"/> inherits from <paramref name="fromType"/>, otherwise <see langword="False" />.</returns>
        public static bool IsDerivedFrom(this Type type, Type fromType)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = fromType.WhenNotNull(nameof(fromType));

            // Tests any type/interface (including unbound such as IDerived2<,>) against another interface (such as IBase or IBase<int>)
            if (type.GetInterfaces().Any(item => item == fromType))
            {
                return true;
            }

            // Tests any type/interface (including unbound such as IDerived2<,>) against another unbound interface
            if (type.GetInterfaces().Any(fromType.IsRawGenericType))
            {
                return true;
            }

            return type.IsSubClassOfRawGeneric(fromType);
        }

        /// <summary>Indicates if the <see cref="Type"/> represents a generic nullable type.</summary>
        /// <param name="type">The type to compare.</param>
        /// <returns><see langword="True" /> if the <see cref="Type"/> represents a generic nullable type, otherwise <see langword="false" />.</returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType() && (type.GetGenericTypeDefinition() == CommonTypes.NullableGenericType);
        }

        /// <summary>Recursively looks for a base generic type definition of a specified type. As an example, given a <c>List&lt;string></c>, looking for
        /// <c>IEnumerable&lt;></c> will return a type of IEnumerable&lt;string>. All base classes and interfaces are searched.</summary>
        /// <param name="type">The source type to be searched.</param>
        /// <param name="genericTypeDefinition">The generic type definition to look for.</param>
        /// <returns>The resolved type if found, otherwise <see langword="null"/>.</returns>
        public static Type GetBaseGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            bool IsMatch(Type candidate)
            {
                return candidate.IsGenericType && candidate.GetGenericTypeDefinition() == genericTypeDefinition;
            }

            var genericIsInterface = genericTypeDefinition.IsInterface;

            if (genericIsInterface)
            {
                return type.GetInterfaces().SingleOrDefault(IsMatch);
            }

            if (IsMatch(type))
            {
                return type;
            }

            while (type.BaseType is not null)
            {
                var match = GetBaseGenericTypeDefinition(type.BaseType, genericTypeDefinition);

                if (match is not null)
                {
                    return match;
                }

                type = type.BaseType;
            }

            return null;
        }

        /// <summary>A utility method that returns a print-friendly name for a given type.</summary>
        /// <param name="type">The type to generate a print-friendly name for.</param>
        /// <returns>A print-friendly name for a given type.</returns>
        public static string GetFriendlyName(this Type type)
        {
            if (type.IsGenericType() && !type.IsNullableType())
            {
                var typeName = type.Name;

                // Some classes do not have the `, such as IDictionary<,>.KeyCollection
                var backtickIndex = typeName.IndexOf('`');

                if (backtickIndex != -1)
                {
                    typeName = typeName.Remove(backtickIndex);
                }

                var genericTypeNames = type.GetGenericArguments().Select(GetFriendlyName);
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(typeName);
                stringBuilder.Append('<');
                stringBuilder.Append(string.Join(", ", genericTypeNames));
                stringBuilder.Append('>');

                return stringBuilder.ToString();
            }

            return type.IsNullableType()
              ? $"{type.GetGenericArguments().Single().Name}?"
              : type.Name;
        }

        /// <summary>Determines if the provided type inherits from EnrichedEnum&lt;TEnum&gt;.</summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns><see langword="True" /> if the type inherits from EnrichedEnum&lt;>, otherwise <see langword="false" />.</returns>
        public static bool IsEnrichedEnum(this Type type)
        {
            return type.IsDerivedFrom(CommonTypes.EnrichedEnumGenericType);
        }

        /// <summary>Gets the method info for a static method.</summary>
        /// <param name="type">The type containing the static method.</param>
        /// <param name="methodName">The name of the static method.</param>
        /// <returns>The method info for a static method.</returns>
        public static MethodInfo GetStaticMethod(this Type type, string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>Gets the method info for an instance method.</summary>
        /// <param name="type">The type containing the instance method.</param>
        /// <param name="methodName">The name of the instance method.</param>
        /// <returns>The method info for an instance method.</returns>
        public static MethodInfo GetInstanceMethod(this Type type, string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>Creates a new list with elements of type <paramref name="type"/>.</summary>
        /// <param name="type">The new list's element type.</param>
        /// <returns>A new list instance.</returns>
        public static IList CreateListOf(this Type type)
        {
            var listType = CommonTypes.ListGenericType.MakeGenericType([type]);

            return (IList) Activator.CreateInstance(listType);
        }

        private static bool IsRawGenericType(this Type type, Type generic)
        {
            var toCompare = generic.IsGenericType
                ? generic.GetGenericTypeDefinition()
                : generic;

            return type == toCompare;
        }
    }
}