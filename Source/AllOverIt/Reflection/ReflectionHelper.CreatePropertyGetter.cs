using AllOverIt.Assertion;
using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of helper functions related to reflection.</summary>
    public static partial class ReflectionHelper
    {
        public static Func<object, object> CreatePropertyGetter(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return CreatePropertyGetterExpressionLambda(propertyInfo).Compile();
        }

        public static Func<TType, object> CreatePropertyGetter<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            return CreatePropertyGetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        public static Func<TType, object> CreatePropertyGetter<TType>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            var type = typeof(TType);
            var propertyInfo = ReflectionCache.GetPropertyInfo(type.GetTypeInfo(), propertyName);

            if (propertyInfo == null)
            {
                throw new ReflectionException($"The property {propertyName} on type {type.GetFriendlyName()} does not exist.");
            }

            return CreatePropertyGetterExpressionLambda<TType>(propertyInfo).Compile();
        }

        public static Expression<Func<object, object>> CreatePropertyGetterExpressionLambda(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            AssertPropertyCanRead(propertyInfo);

            var getterMethodInfo = propertyInfo.GetGetMethod(true);

            // propertyInfo.DeclaringType should be ok but using ReflectedType, just in case:
            //
            // MemberInfo m1 = typeof(Base).GetMethod("Method");
            // MemberInfo m2 = typeof(Derived).GetMethod("Method");
            // m1.DeclaringType => Base
            // m1.ReflectedType => Base
            // m2.DeclaringType => Base
            // m2.ReflectedType => Derived
            
            var itemParam = Expression.Parameter(typeof(object), "item");
            var instanceParam = Expression.Convert(itemParam, propertyInfo.DeclaringType);
            var getterCall = Expression.Call(instanceParam, getterMethodInfo);
            var objectGetterCall = Expression.Convert(getterCall, typeof(object));

            return Expression.Lambda<Func<object, object>>(objectGetterCall, itemParam);
        }

        public static Expression<Func<TType, object>> CreatePropertyGetterExpressionLambda<TType>(PropertyInfo propertyInfo)
        {
            _ = propertyInfo.WhenNotNull(nameof(propertyInfo));

            AssertPropertyCanRead(propertyInfo);

            var itemParam = Expression.Parameter(typeof(TType), "item");

            Expression declaringTypeItemParam = typeof(TType) != propertyInfo.DeclaringType
                ? Expression.TypeAs(itemParam, propertyInfo.DeclaringType)
                : itemParam;

            var property = Expression.Property(declaringTypeItemParam, propertyInfo);

            var objectProperty = Expression.TypeAs(property, typeof(object));

            return Expression.Lambda<Func<TType, object>>(objectProperty, itemParam);
        }

        private static void AssertPropertyCanRead(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                throw new ReflectionException($"The property {propertyInfo.Name} on type {propertyInfo.DeclaringType.GetFriendlyName()} does not have a getter.");
            }
        }
    }
}
