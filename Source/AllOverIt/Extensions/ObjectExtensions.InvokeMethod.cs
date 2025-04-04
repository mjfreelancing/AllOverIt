﻿using AllOverIt.Assertion;
using AllOverIt.Reflection;
using System.Reflection;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for object types.</summary>
    public static partial class ObjectExtensions
    {
        /// <summary>Uses reflection to invoke a method on an object.</summary>
        /// <param name="instance">The instance to invoke the method on.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        /// <param name="bindingOptions">The binding options to use to find the required method.</param>
        public static object? InvokeMethod(this object instance, string methodName, object?[]? parameters, BindingOptions bindingOptions = BindingOptions.Default)
        {
            _ = instance.WhenNotNull();
            _ = methodName.WhenNotNullOrEmpty();

            return InvokeMethod(instance, instance.GetType(), methodName, parameters, bindingOptions);
        }

        /// <summary>Uses reflection to invoke a method on an object.</summary>
        /// <param name="instance">The instance to invoke the method on. This can be <see langword="null"/> for static methods.</param>
        /// <param name="instanceType">The instance type.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        /// <param name="bindingOptions">The binding options to use to find the required method.</param>
        public static object? InvokeMethod(this object instance, Type instanceType, string methodName, object?[]? parameters, BindingOptions bindingOptions = BindingOptions.Default)
        {
            _ = instanceType.WhenNotNull();
            _ = methodName.WhenNotNullOrEmpty();

            var methodInfo = instanceType
                .GetMethodInfo(bindingOptions)
                .Single(method => method.Name == methodName);

            return InvokeMethod(instance, methodInfo, parameters);
        }

        /// <summary>Uses reflection to invoke a method on an object.</summary>
        /// <param name="instance">The instance to invoke the method on. This can be <see langword="null"/> for static methods.</param>
        /// <param name="instanceType">The instance type.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="types">The argument types on the method to match.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        public static object? InvokeMethod(this object instance, Type instanceType, string methodName, Type[] types, object?[]? parameters)
        {
            _ = instanceType.WhenNotNull();
            _ = methodName.WhenNotNullOrEmpty();

            var methodInfo = instanceType.GetMethodInfo(methodName, types);

#pragma warning disable IDE0270 // Use coalesce expression
            if (methodInfo is null)
            {
                throw new ArgumentException($"The {methodName} method was not found on type {instanceType.GetFriendlyName()}.", nameof(methodName));
            }
#pragma warning restore IDE0270 // Use coalesce expression

            return InvokeMethod(instance, methodInfo, parameters);
        }

        /// <summary>Uses reflection to invoke a method on an object.</summary>
        /// <param name="instance">The instance to invoke the method on. This can be <see langword="null"/> for static methods.</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> for the method to be invoked.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        public static object? InvokeMethod(this object instance, MethodInfo methodInfo, object?[]? parameters)
        {
            _ = methodInfo.WhenNotNull();

            return methodInfo.Invoke(instance, parameters);
        }
    }
}