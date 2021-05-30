using System;

namespace AllOverIt.Reflection
{
    /// <Summary>
    /// Specifies binding options that, when combined, provide the ability to filter reflection operations that
    /// target property and method information on a type.
    /// </Summary>
    [Flags]
#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
#pragma warning disable RCS1154 // Sort enum members.
    public enum BindingOptions
#pragma warning restore RCS1154 // Sort enum members.
#pragma warning restore RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
    {
        #region Scope

        /// <summary>
        /// Filter reflection operations to <i>static</i> scope.
        /// </summary>
        Static = 1,

        /// <summary>
        /// Filter reflection operations to <i>instance</i> (non-static) scope.
        /// </summary>
        Instance = 2,

        #endregion

        #region Access

        /// <summary>
        /// Filter reflection operations to <i>abstract</i> access.
        /// </summary>
        Abstract = 4,

        /// <summary>
        /// Filter reflection operations to <i>virtual</i> access.
        /// </summary>
        Virtual = 8,

        /// <summary>
        /// Filter reflection operations to a <i>non-virtual</i> access.
        /// </summary>
        NonVirtual = 16,

        #endregion

        #region Visibility

        /// <summary>
        /// Filter reflection operations to <i>internal</i> visibility.
        /// </summary>
        Internal = 32,

        /// <summary>
        /// Filter reflection operations to <i>private</i> visibility.
        /// </summary>
        Private = 64,

        /// <summary>
        /// Filter reflection operations to <i>protected</i> visibility.
        /// </summary>
        Protected = 128,

        /// <summary>
        /// Filter reflection operations to <i>public</i> visibility.
        /// </summary>
        Public = 256,

        #endregion

        #region DefaultOptions

        /// <summary>
        /// Filter reflection operations to <i>static</i> or <i>instance</i> scope.
        /// </summary>
        DefaultScope = Static | Instance,

        /// <summary>
        /// Filter reflection operations to <i>abstract</i>, <i>virtual</i>, or <i>non-virtual</i> access.
        /// </summary>
        DefaultAccessor = Abstract | Virtual | NonVirtual,

        /// <summary>
        /// Filter reflection operations to <i>public</i> or <i>protected</i> access.
        /// </summary>
        DefaultVisibility = Public | Protected,

        /// <summary>
        /// Filter reflection operations to use <see cref="DefaultScope"/> scope, <see cref="DefaultAccessor"/> access,
        /// and <see cref="DefaultVisibility"/> visibility.
        /// </summary>
        Default = DefaultScope | DefaultAccessor | DefaultVisibility,

        #endregion

        #region AllOptions

        /// <summary>
        /// Filter reflection operations to <i>static</i> or <i>instance</i> scope.
        /// </summary>
        AllScope = DefaultScope,

        /// <summary>
        /// Filter reflection operations to <i>abstract</i>, <i>virtual</i>, or <i>non-virtual</i> access.
        /// </summary>
        AllAccessor = DefaultAccessor,

        /// <summary>
        /// Filter reflection operations to <i>public</i>, <i>protected</i>, <i>private</i>, or <i>internal</i> access.
        /// </summary>
        AllVisibility = DefaultVisibility | Private | Internal,

        /// <summary>
        /// Filter reflection operations to use <see cref="AllScope"/> scope, <see cref="AllAccessor"/> access,
        /// and <see cref="AllVisibility"/> visibility.
        /// </summary>
        All = AllScope | AllAccessor | AllVisibility

        #endregion
    }
}