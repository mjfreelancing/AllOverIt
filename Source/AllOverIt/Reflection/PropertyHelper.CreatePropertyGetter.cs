using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection.Exceptions;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of property getter and setter helper functions related to <see cref="PropertyInfo"/>.</summary>
    public static partial class PropertyHelper
    {
        /// <summary>Creates a compiled expression as a <c>Func{object, object}</c> to get a property value based
        /// on a specified <see cref="PropertyInfo"/> instance.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property getter.</param>
        /// <returns>The compiled property getter.</returns>
        public static Func<object, object?> CreatePropertyGetter(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

            return CreatePropertyGetterExpressionLambda(propertyInfo).Compile();
        }

        /// <summary>Creates a compiled expression as a <c>Func{TType, object}</c> to get a property value based on a specified
        /// <see cref="PropertyInfo"/> instance.</summary>
        /// <typeparam name="TType">The object type to get the property value from.</typeparam>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property getter.</param>
        /// <returns>The compiled property getter.</returns>
        public static Func<TType, object?> CreatePropertyGetter<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

            return CreatePropertyGetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        /// <summary>Creates a compiled expression as a <c>Func{TType, object}</c> to get a property value based on a specified
        /// property name.</summary>
        /// <typeparam name="TType">The object type to get the property value from.</typeparam>
        /// <param name="propertyName">The name of the property to get the value from.</param>
        /// <returns>The compiled property getter.</returns>
        public static Func<TType, object?> CreatePropertyGetter<TType>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            var type = typeof(TType);
            var propertyInfo = ReflectionCache.GetPropertyInfo(type.GetTypeInfo(), propertyName);

            Throw<ReflectionException>.WhenNull(propertyInfo, $"The property {propertyName} on type {type.GetFriendlyName()} does not exist.");

            return CreatePropertyGetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        /// <summary>Gets an expression lambda that represents getting a property value from an object.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to build a property getter.</param>
        /// <returns>The expression lambda representing a property getter.</returns>
        public static Expression<Func<object, object?>> CreatePropertyGetterExpressionLambda(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

            AssertPropertyCanRead(propertyInfo);

            var getterMethodInfo = propertyInfo.GetGetMethod(true)!;

            // propertyInfo.DeclaringType should be ok but if there's problems consider propertyInfo.ReflectedType:
            //
            // MemberInfo m1 = typeof(Base).GetMethod("Method");
            // MemberInfo m2 = typeof(Derived).GetMethod("Method");
            // m1.DeclaringType => Base
            // m1.ReflectedType => Base
            // m2.DeclaringType => Base
            // m2.ReflectedType => Derived

            var itemParam = Expression.Parameter(typeof(object), "item");
            var instanceParam = Expression.Convert(itemParam, propertyInfo.DeclaringType!)!;
            var getterCall = Expression.Call(instanceParam, getterMethodInfo);
            var objectGetterCall = Expression.Convert(getterCall, typeof(object));

            return Expression.Lambda<Func<object, object?>>(objectGetterCall, itemParam);
        }

        private static Expression<Func<TType, object?>> CreatePropertyGetterExpressionLambda<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull();

            AssertPropertyCanRead(propertyInfo);

            var itemParam = Expression.Parameter(typeof(TType), "item");

            Expression declaringTypeItemParam = typeof(TType) != propertyInfo.DeclaringType
                ? Expression.TypeAs(itemParam, propertyInfo.DeclaringType!)
                : itemParam;

            var property = Expression.Property(declaringTypeItemParam, propertyInfo);

            var objectProperty = Expression.TypeAs(property, typeof(object));

            return Expression.Lambda<Func<TType, object?>>(objectProperty, itemParam);
        }

        private static void AssertPropertyCanRead(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                throw new ReflectionException($"The property {propertyInfo.Name} on type {propertyInfo.DeclaringType!.GetFriendlyName()} does not have a getter.");
            }
        }
    }
}
