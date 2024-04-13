#if NETSTANDARD2_1

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
#endif
