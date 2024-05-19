#nullable enable

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
#if !NET5_0_OR_GREATER
    [ExcludeFromCodeCoverage, DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
#endif

#if !NET7_0_OR_GREATER
    [ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute
    {
    }
#endif

#if !NET7_0_OR_GREATER
    [ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public string FeatureName { get; }
        public bool IsOptional { get; init; }
        public const string RefStructs = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);

        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }
    }
#endif
}

namespace System.Diagnostics.CodeAnalysis
{
#if !NET7_0_OR_GREATER
    [ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute
    {
    }
#endif

#if !NET5_0_OR_GREATER
    [ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(string member) => Members = [member];
        public MemberNotNullAttribute(params string[] members) => Members = members;
        public string[] Members { get; }
    }
#endif

#if !NET5_0_OR_GREATER
    [ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class MemberNotNullWhenAttribute : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            ReturnValue = returnValue;
            Members = [member];
        }

        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            ReturnValue = returnValue;
            Members = members;
        }

        public bool ReturnValue { get; }
        public string[] Members { get; }
    }
#endif
}

namespace System.Diagnostics
{
#if !NET7_0_OR_GREATER
    /// <summary>/// Exception thrown when the program executes an instruction that was thought to be unreachable.</summary>
    [ExcludeFromCodeCoverage]
    public sealed class UnreachableException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="UnreachableException"/> class with the default error message.</summary>
        public UnreachableException()
            : base("The program executed an instruction that was thought to be unreachable.")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="UnreachableException"/> class with a specified error message.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UnreachableException(string? message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="UnreachableException"/> class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UnreachableException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
#endif
}
