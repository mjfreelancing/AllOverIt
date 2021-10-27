using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of helper functions related to reflection.</summary>
    public static class ReflectionHelper
    {
        /// <summary>Gets the <see cref="PropertyInfo"/> (property metadata) for a given property on a <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The type to obtain the property metadata from.</typeparam>
        /// <param name="propertyName">The name of the property to obtain metadata for.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static PropertyInfo GetPropertyInfo<TType>(string propertyName)
        {
            return typeof(TType).GetPropertyInfo(propertyName);
        }

        /// <summary>Gets <see cref="PropertyInfo"/> (property metadata) for a given <typeparamref name="TType"/> and binding option.</summary>
        /// <typeparam name="TType">The type to obtain property metadata for.</typeparam>
        /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only property metadata of the declared type is returned.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static IEnumerable<PropertyInfo> GetPropertyInfo<TType>(BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
        {
            return typeof(TType).GetPropertyInfo(binding, declaredOnly);
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <typeparamref name="TType"/> and binding option.</summary>
        /// <typeparam name="TType">The type to obtain method metadata for.</typeparam>
        /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only method metadata of the declared type is returned.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static IEnumerable<MethodInfo> GetMethodInfo<TType>(BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
        {
            return typeof(TType).GetMethodInfo(binding, declaredOnly);
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <typeparamref name="TType"/> method with a given name and no arguments.</summary>
        /// <typeparam name="TType">The type to obtain method metadata for.</typeparam>
        /// <param name="name">The name of the method.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <typeparamref name="TType"/> with a given name and no arguments.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo<TType>(string name)
        {
            return GetMethodInfo<TType>(name, Type.EmptyTypes);
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <typeparamref name="TType"/> method with a given name and argument types.</summary>
        /// <typeparam name="TType">The type to obtain method metadata for.</typeparam>
        /// <param name="name">The name of the method.</param>
        /// <param name="types">The argument types expected on the method</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <typeparamref name="TType"/> with a given name and argument types.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo<TType>(string name, Type[] types)
        {
            return typeof(TType).GetMethodInfo(name, types);
        }

        /// <summary>Sets the value of a target property or field using its associated <see cref="MemberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> of the property or field to have its value set.</param>
        /// <param name="target">The target object being updated.</param>
        /// <param name="value">The value to assign to the property or field.</param>
        public static void SetMemberValue(MemberInfo memberInfo, object target, object value)
        {
            _ = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
            _ = target ?? throw new ArgumentNullException(nameof(target));

            switch (memberInfo)
            {
                case PropertyInfo property:
                    property.SetValue(target, value, null);
                    return;

                case FieldInfo field:
                    field.SetValue(target, value);
                    return;

                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo), $"Expected {nameof(memberInfo)} to be a property or field");
            }
        }

        /// <summary>Gets the value of a target property or field using its associated <see cref="MemberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> of the property or field being read.</param>
        /// <param name="target">The target object being read.</param>
        /// <returns>The value of the property or field referred to by <paramref name="memberInfo" />.</returns>
        public static object GetMemberValue(MemberInfo memberInfo, object target)
        {
            _ = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
            _ = target ?? throw new ArgumentNullException(nameof(target));

            return memberInfo switch
            {
                PropertyInfo property => property.GetValue(target),
                FieldInfo field => field.GetValue(target),
                _ => throw new ArgumentOutOfRangeException(nameof(memberInfo), $"Expected {nameof(memberInfo)} to be a property or field")
            };
        }

        /// <summary>Gets the property, field or method call return type associated with the <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> of the property, field or method call.</param>
        /// <returns>The property, field or method call return type.</returns>
        public static Type GetMemberType(MemberInfo memberInfo)
        {
            _ = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));

            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                FieldInfo fieldInfo => fieldInfo.FieldType,
                MethodInfo methodInfo => methodInfo.ReturnType,
                _ => throw new ArgumentOutOfRangeException(nameof(memberInfo))
            };
        }
    }
}
