namespace AllOverIt.Reflection
{
    /// <Summary>Specifies binding options that, when combined, provide the ability to filter reflection operations that
    /// target property and method information on a type.</Summary>
    [Flags]
    public enum BindingOptions : int
    {
#pragma warning disable IDE0079
#pragma warning disable CA1069      // Enums should not have duplicate values

        #region Scope

        /// <summary>Filter reflection operations to <i>static</i> scope.</summary>
        Static = 1 << 0,                    // 1

        /// <summary>Filter reflection operations to <i>instance</i> (non-static) scope.</summary>
        Instance = 1 << 1,                  // 2,

        #endregion

        #region Access

        /// <summary>Filter reflection operations to <i>abstract</i> access.</summary>
        Abstract = 1 << 2,                  // 4,

        /// <summary>Filter reflection operations to <i>virtual</i> access.</summary>
        Virtual = 1 << 3,                   // 8,

        /// <summary>Filter reflection operations to a <i>non-virtual</i> access.</summary>
        NonVirtual = 1 << 4,                // 16,

        #endregion

        #region Visibility

        /// <summary>Filter reflection operations to <i>internal</i> visibility.</summary>
        Internal = 1 << 5,                  // 32,

        /// <summary>Filter reflection operations to <i>private</i> visibility.</summary>
        Private = 1 << 6,                   // 64,

        /// <summary>Filter reflection operations to <i>protected</i> visibility.</summary>
        Protected = 1 << 7,                 // 128,

        /// <summary>Filter reflection operations to <i>public</i> visibility.</summary>
        Public = 1 << 8,                    // 256,

        #endregion

        #region Method

        /// <summary>Perform filtering against the property's <i>GetMethod</i>.</summary>
        GetMethod = 1 << 9,                 // 512,

        /// <summary>Perform filtering against the property's <i>SetMethod</i>.</summary>
        SetMethod = 1 << 10,                // 1024,

        #endregion

        #region DefaultOptions

        /// <summary>Filter reflection operations to <i>static</i> or <i>instance</i> scope.</summary>
        DefaultScope = Static | Instance,

        /// <summary>Filter reflection operations to <i>abstract</i>, <i>virtual</i>, or <i>non-virtual</i> access.</summary>
        DefaultAccessor = Abstract | Virtual | NonVirtual,

        /// <summary>Filter reflection operations to <i>public</i> or <i>protected</i> access.</summary>
        DefaultVisibility = Public,

        /// <summary>Filter reflection operators against the property <i>GetMethod</i> only.</summary>
        DefaultMethod = GetMethod,

        /// <summary>Filter reflection operations to use <see cref="DefaultScope"/> scope, <see cref="DefaultAccessor"/> access,
        /// <see cref="DefaultVisibility"/> visibility, and <see cref="DefaultMethod"/> method.</summary>
        Default = DefaultScope | DefaultAccessor | DefaultVisibility | DefaultMethod,

        #endregion

        #region AllOptions

        /// <summary>
        /// Filter reflection operations to <i>static</i> or <i>instance</i> scope.
        /// </summary>
        AllScope = Static | Instance,                   // Same as DefaultScope

        /// <summary>
        /// Filter reflection operations to <i>abstract</i>, <i>virtual</i>, or <i>non-virtual</i> access.
        /// </summary>
        AllAccessor = Abstract | Virtual | NonVirtual,  // Same asDefaultAccessor

        /// <summary>
        /// Filter reflection operations to <i>public</i>, <i>protected</i>, <i>private</i>, or <i>internal</i> access.
        /// </summary>
        AllVisibility = Public | Private | Protected | Internal,

        /// <summary>Filter reflection operations to <i>get</i> and <i>set</i> methods.</summary>
        AllMethod = GetMethod | SetMethod,

        /// <summary>
        /// Filter reflection operations to use <see cref="AllScope"/> scope, <see cref="AllAccessor"/> access,
        /// <see cref="AllVisibility"/> visibility, and <see cref="AllMethod"/> methods.
        /// </summary>
        All = AllScope | AllAccessor | AllVisibility | AllMethod

        #endregion

#pragma warning restore CA1069
#pragma warning restore IDE0079
    }
}