#if !NET8_0_OR_GREATER

#nullable enable

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [ExcludeFromCodeCoverage, DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }

    [ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute
    {
    }

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

    namespace System.Diagnostics.CodeAnalysis
    {
        [ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
        internal sealed class SetsRequiredMembersAttribute : Attribute
        {
        }
    }
}

namespace System.Diagnostics
{
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS1591  // Missing XML comment for publicly visible type or member
    [ExcludeFromCodeCoverage]
    public sealed class UnreachableException : Exception
    {
        public UnreachableException()
            : base("The program executed an instruction that was thought to be unreachable.")
        {
        }

        public UnreachableException(string? message)
            : base(message)
        {
        }

        public UnreachableException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
#pragma warning restore CS1591
#pragma warning restore IDE0079 // Remove unnecessary suppression
}
#endif
