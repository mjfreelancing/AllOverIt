using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [ExcludeFromCodeCoverage, DebuggerNonUserCode]
    internal static class IsExternalInit
    {
    }
}
#endif
