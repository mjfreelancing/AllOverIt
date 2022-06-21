﻿using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of helper functions related to reflection.</summary>
    public static partial class ReflectionHelper
    {
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
    }
}