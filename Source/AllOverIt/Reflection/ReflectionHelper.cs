using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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


        // =============
        // ALL WIP
        // =============


        // TODO: Tests
        public static Func<TType, TProp> GetCompiledPropertyGetter<TType, TProp>(string name)
        {
            var sourceType = typeof(TType);
            var instanceParam = Expression.Parameter(sourceType);
            var getMethod = sourceType.GetProperty(name).GetGetMethod();
            var getMethodCall = Expression.Call(instanceParam, getMethod);

            return Expression
                .Lambda<Func<TType, TProp>>(
                    Expression.Convert(getMethodCall, typeof(TProp)),
                    instanceParam)
                .Compile();
        }



        public static Func<object, object> GetCompiledPropertyGetter(Type sourceType, string name)
        {
            var propertyInfo = sourceType.GetProperty(name);

            return GetExpressionLambda(propertyInfo).Compile();
        }


        //public static Expression<Func<T, object>> GetExpressionLambda<T>(PropertyInfo propertyInfo)
        //{
        //    var instance = Expression.Parameter(typeof(T), "i");
        //    var property = typeof(T) != propertyInfo.DeclaringType
        //        ? Expression.Property(Expression.TypeAs(instance, propertyInfo.DeclaringType), propertyInfo)
        //        : Expression.Property(instance, propertyInfo);
        //    var convertProperty = Expression.TypeAs(property, typeof(object));
        //    return Expression.Lambda<Func<T, object>>(convertProperty, instance);
        //}


        //public static Expression<Func<T, P>> GetExpressionLambda<T, P>(PropertyInfo propertyInfo)
        //{
        //    var instance = Expression.Parameter(typeof(T), "i");
        //    var property = typeof(T) != propertyInfo.DeclaringType
        //        ? Expression.Property(Expression.TypeAs(instance, propertyInfo.DeclaringType), propertyInfo)
        //        : Expression.Property(instance, propertyInfo);
        //    var convertProperty = Expression.TypeAs(property, typeof(P));
        //    return Expression.Lambda<Func<T, P>>(convertProperty, instance);
        //}



        //public delegate object GetMemberDelegate(object instance);

        public static Expression<Func<object, object>> GetExpressionLambda(PropertyInfo propertyInfo)
        {
            var getMethodInfo = propertyInfo.GetGetMethod(nonPublic: true);

            if (getMethodInfo == null)
                return null;

            var oInstanceParam = Expression.Parameter(typeof(object), "oInstanceParam");
            var instanceParam = Expression.Convert(oInstanceParam, propertyInfo.ReflectedType); //propertyInfo.DeclaringType doesn't work on Proxy types

            var exprCallPropertyGetFn = Expression.Call(instanceParam, getMethodInfo);
            var oExprCallPropertyGetFn = Expression.Convert(exprCallPropertyGetFn, typeof(object));

            return Expression.Lambda<Func<object, object>>
            (
                oExprCallPropertyGetFn,
                oInstanceParam
            );
        }




        // TODO: Tests
        public static Action<TType, TProp> GetCompiledPropertySetter<TType, TProp>(string name)
        {
            var sourceType = typeof(TType);
            var propertyType = typeof(TProp);
            var instanceParam = Expression.Parameter(sourceType);
            var argumentParam = Expression.Parameter(propertyType);
            var propertyInfo = sourceType.GetProperty(name);

            if (propertyType != propertyInfo.PropertyType)
            {
                throw new InvalidOperationException($"The property {name} has a type of {propertyInfo.PropertyType.GetFriendlyName()} but expected it to be of type {propertyType.GetFriendlyName()}.");
            }

            return Expression
                .Lambda<Action<TType, TProp>>(
                    Expression.Call(
                        instanceParam,
                        propertyInfo.GetSetMethod(),
                        Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                    instanceParam,
                    argumentParam)
                .Compile();
        }

        // TODO: Write overloads that allow <object, object>
    }
}
