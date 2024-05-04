using AllOverIt.Assertion;
using System.Reflection;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="MemberInfo"/> types.</summary>
    public static class MemberInfoExtensions
    {
        /// <summary>Sets the value of a target property or field using its associated <see cref="MemberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> of the property or field to have its value set.</param>
        /// <param name="target">The target object being updated.</param>
        /// <param name="value">The value to assign to the property or field.</param>
        public static void SetValue(this MemberInfo memberInfo, object target, object value)
        {
            _ = memberInfo.WhenNotNull(nameof(memberInfo));
            _ = target.WhenNotNull(nameof(target));

            switch (memberInfo)
            {
                case PropertyInfo property:
                    property.SetValue(target, value, null);
                    return;

                case FieldInfo field:
                    field.SetValue(target, value);
                    return;

                default:
                    throw new InvalidOperationException($"Expected {nameof(memberInfo)} to be a property or field.");
            }
        }

        /// <summary>Gets the value of a target property or field using its associated <see cref="MemberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> of the property or field being read.</param>
        /// <param name="target">The target object being read.</param>
        /// <returns>The value of the property or field referred to by <paramref name="memberInfo" />.</returns>
        public static object GetValue(this MemberInfo memberInfo, object target)
        {
            _ = memberInfo.WhenNotNull(nameof(memberInfo));
            _ = target.WhenNotNull(nameof(target));

            return memberInfo switch
            {
                PropertyInfo property => property.GetValue(target),
                FieldInfo field => field.GetValue(target),
                _ => throw new InvalidOperationException($"Expected {nameof(memberInfo)} to be a property or field.")
            };
        }

        /// <summary>Gets the property, field or method call return type associated with the <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> of the property, field or method call.</param>
        /// <returns>The property, field or method call return type.</returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            _ = memberInfo.WhenNotNull(nameof(memberInfo));

            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                FieldInfo fieldInfo => fieldInfo.FieldType,
                MethodInfo methodInfo => methodInfo.ReturnType,
                _ => throw new InvalidOperationException($"Expected {nameof(memberInfo)} to be a property, field, or method.")
            };
        }
    }
}